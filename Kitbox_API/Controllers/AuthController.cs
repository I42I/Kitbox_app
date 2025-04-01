using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kitbox_API.Configuration;
using Kitbox_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Kitbox_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(AuthRequestDto request)
        {
            // Ici, vous implémenteriez la vérification des identifiants contre votre système d'authentification
            // Pour cet exemple, vérifions simplement un nom d'utilisateur et mot de passe codés en dur
            
            // Note: Dans une implémentation réelle, vous utiliseriez:
            // - ASP.NET Core Identity
            // - Stockage sécurisé des mots de passe hachés
            // - Base de données des utilisateurs
            
            // Exemple simple pour la démonstration
            if (request.Username == "admin" && request.Password == "Password123!")
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60"))
                });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            // Récupérer la clé depuis les variables d'environnement ou KeyVault
            var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("La clé JWT n'a pas été configurée");
                
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenValidationDto tokenValidation)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"];
                
                tokenHandler.ValidateToken(tokenValidation.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return Ok(new { isValid = true });
            }
            catch
            {
                return Ok(new { isValid = false });
            }
        }
    }
}