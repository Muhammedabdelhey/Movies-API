using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies_Core_Layer.Interfaces;
using Movies_Core_Layer.Models;
using Movies_Data_Access_Layer.EF;
using Movies_Data_Access_Layer.EF.Repositories;
using Movies_With_Reopsitory_Pattren.Authentication;
using Movies_With_Reopsitory_Pattren.Filters;
using Movies_With_Reopsitory_Pattren.Middlewares;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

#region  Configration
// to add JSON File for Configuration  
//builder.Configuration.AddJsonFile("filename.json");

//############## to fetch Configuration data using Options Pattern ####################
//recommended one this allow to inject IOptionsInteface,IOptionsSnapshot,IOptionsMonitor
builder.Services.Configure<AttachmentsOptions>(builder.Configuration.GetSection("Attachments"));

//sec way to regstier model with config
//var attachmentsOptions=builder.Configuration.GetSection("Attachments").Get<AttachmentsOptions>();
//builder.Services.AddSingleton(attachmentsOptions);

//thired way 
//var attachmentsOptions = new AttachmentsOptions();
//builder.Configuration.GetSection("Attachments").Bind(attachmentsOptions);
//builder.Services.AddSingleton("attachmentsOptions");
#endregion

#region regstier DbContext
// Add services(AddDbContext) to the Dependency Injection container.
// scoped per request
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = builder.Configuration.GetConnectionString("CompanyConnection");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString)
);

#endregion

#region Dependency Injection
// interface binding 

// create instance for each class in each request 
builder.Services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// create instance for each request 
// builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// create instance once after the first call during the program's lifecycle
// builder.Services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
#endregion

#region regstier ActionFiliter
//builder.Services.AddControllers();
// #########to regstier filter to all controllers (global action filter all apis will pass to it)##########
// Note : to regstier Filter for spacifc controller or Action(method) check genreController
// this way active dependency injection for filter 
builder.Services.AddControllers(options => options.Filters.Add<LogActivityFilter>());

//to regstier more than one filter 
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<LogActivityFilter>();
//    options.Filters.Add<LogActivityFilter>();
//    options.Filters.Add<LogActivityFilter>();
//});
#endregion

#region BasicAuthentication
// Basic Authentication
// add your Authentication scheme 
// first parm  AuthenticationSchemeOptions is standerd options , sec your auth handelr (Logic) you will find this class in Presentation Layer in Authentication folder
// you can add many scheme , to detrmaine what default one pass scheme name 
//builder.Services.AddAuthentication("Basic")
//    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
#endregion

#region JWTBearerToken Authentication
//regstier jwt data in appSettings
// make model for jwt options
var JwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(JwtOptions);
// first parm AuthenticationScheme ,sec options of creating token 
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{   // save token on request
    options.SaveToken = true;
    // this options show how our system Validate Token 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //issuer and audience can by array ValidIssuers and VaildAudiences
        ValidateIssuer = true,
        ValidIssuer = JwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtOptions.Audience,
        ValidateIssuerSigningKey = true,
        //we craete signing key should converted to bytes here 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey))
    };
});
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();

app.UseAuthorization();

#region Middleware
//############ you can regstier by Using => take logic inline ###############################

//app.Use(async (context, next) =>
//{
//    // Do work that can write to the Response.
//    await next.Invoke();
//    // Do logging or other work that doesn't write to the Response.
//});

//############### UseMiddleware => take class #############################################

//app.UseMiddleware<ProfilingMiddleware>();

//###############   run => Middleware without next(terminal Middlewere) #####################

//app.Run(async context =>
//{
//    await context.Response.WriteAsync("Hello from 2nd delegate.");
//});

//############### Map => make middlewere for spacifc Paths ,can write middlewere inline or in class)###########

//static void HandleMultiSeg(IApplicationBuilder app)
//{
//    app.Run(async context =>
//    {
//        await context.Response.WriteAsync("Map Test 1");
//    });
//}

//app.Map("/map1/seg1", HandleMultiSeg);

//app.Map("/level1", level1App => {
//    level1App.Map("/level2a", level2AApp => {
//        // "/level1/level2a" processing
//    });
//    level1App.Map("/level2b", level2BApp => {
//        // "/level1/level2b" processing
//    });
//});
//###########################################################################################
// place middleware based on what will do the order of middleware can effect of his behavior

//if you put  rate limit MW before swagger this will add swagger requests to counter and the behavior will not correct
app.UseMiddleware<ProfilingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
#endregion
app.MapControllers();

app.Run();
