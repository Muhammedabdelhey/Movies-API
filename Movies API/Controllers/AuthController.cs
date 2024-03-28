using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Movies_Core_Layer.Dtos;
using Movies_Core_Layer.Interfaces;
using Movies_Core_Layer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies_Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IBaseRepository<User> _userRepository, JwtOptions _jwtOptions) : ControllerBase
    {
        //private readonly IBaseRepository<User> _userRepository;
        //private readonly JwtOptions _jwtOptions;
        //public AuthController(IBaseRepository<User> userRepository, JwtOptions jwtOptions)
        //{
        //    _userRepository = userRepository;
        //    _jwtOptions = jwtOptions;
        //}
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            User user = new User { Name = userDto.Name, Email = userDto.Email, Password = userDto.Password };
            var result = await _userRepository.AddAsync(user);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( ModelState.Values);
            }
            var result = await _userRepository.GetByAsync(user => user.Email.Equals(loginDto.Email) && user.Password == loginDto.Password);
            if (result.Count()==0)
            {
                return BadRequest("Email or Password Not Correct");
            }
            var token = creaeteToken(result.First());
            return Ok(token);
        }

        private string creaeteToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
                    SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new (ClaimTypes.Email, user.Email)
                }) 
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Convert token to string
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken;
        }

    }
}
