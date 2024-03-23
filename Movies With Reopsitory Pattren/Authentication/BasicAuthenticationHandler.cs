using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Movies_With_Reopsitory_Pattren.Authentication
{
    // should extend AuthenticationHandler<AuthenticationSchemeOptions> this also take auth options like AuthenticationSchemeOptions 
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //first check Authorization Header exists
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.NoResult());
            // store Authorization header
            var AuthHeader = Request.Headers["Authorization"].ToString();
            // check this have basic scheme 
            if (!AuthHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.NoResult());
            // get Creadntials only (it will be in base64 encodeing )
            var encodedCreadntials = AuthHeader.Substring("Basic ".Length).Trim();
            //var encodedCreadntials = AuthHeader["Basic ".Length..];
            // decode creadntials Convert.FromBase64String(encodedCreadntials); this will return array of byte
            // to convert it to string use Encoding.UTF8.GetString()
            var decodedCreadnials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreadntials));
            //username and password will splited using : "user1:pass123"
            //to get user name userNameAndPassword[0] , to get password userNameAndPassword[1] ,
            var userNameAndPassword = decodedCreadnials.Split(':');
            //next step compare userNameAndPassword from DataBase
            // currently we make it static 
            if (!userNameAndPassword[0].Equals("admin") || userNameAndPassword[1].Equals("password"))
                return Task.FromResult(AuthenticateResult.Fail("Invalid Creadntials"));
            // if code reach here user name and password correct and user Authenticated
            //first we create an identity of type ClaimsIdentity , Claims have user data you create for each indo one claim , sec parm scheme type
            var identity = new ClaimsIdentity(new Claim[]
            {
                // when u add claim add claim type(key) and value , if u wanna add user id as Claim use ClaimTypes.NameIdentifier , never put pass in calims  
                new Claim(ClaimTypes.Name,userNameAndPassword[0]),
            }, "Basic");
            //sec create principle , principle can have many identity 
            var principle = new ClaimsPrincipal(identity);
            // finaly create ticket first parm priciable , sec scheme 
            var ticket =  new AuthenticationTicket(principle, "Basic");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
