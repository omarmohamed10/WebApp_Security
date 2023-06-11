using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if(credential.UserName == "Admin" && credential.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,"admim"),
                    new Claim(ClaimTypes.Email,"admin@Company.com"),
                    new Claim("Department" , "HR"),
                    new Claim("Admin","true"),
                    new Claim("EmploymentDate" , "2021-05-01"),
                    new Claim("Manager","true")
                };
                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                return Ok(new
                {
                    access_token = CreateToken(claims , expiresAt),
                    expires_at = expiresAt
                });
            }
            ModelState.AddModelError("Unauthorized", "You 're not authorized to access the endpoint.");
            return Unauthorized(ModelState);
            
        }
        private string CreateToken(IEnumerable<Claim> claims , DateTime expiresAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecrectKey"));

            var jwt = new JwtSecurityToken(

                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(

                  new SymmetricSecurityKey(secretKey),
                  SecurityAlgorithms.HmacSha256Signature)

                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
