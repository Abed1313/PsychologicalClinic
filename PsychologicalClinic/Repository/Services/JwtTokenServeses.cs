using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PsychologicalClinic.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PsychologicalClinic.Repository.Services
{
    public class JwtTokenServeses
    {
        private IConfiguration _configuration;
        private readonly SignInManager<Characters> _signInManager;

        public JwtTokenServeses(IConfiguration configuration, SignInManager<Characters> signInManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }


        public static TokenValidationParameters ValidateToken(IConfiguration configuration)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(configuration),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
        private static SecurityKey GetSecurityKey(IConfiguration configuration)
        {
            var secretKey = configuration["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Jwt Secret key is missing or not configured properly.");
            }

            var secretBytes = Encoding.UTF8.GetBytes(secretKey);
            return new SymmetricSecurityKey(secretBytes);
        }


        public async Task<string> GenerateToken(Characters user, TimeSpan expiryDate)
        {
            var userPrincliple = await _signInManager.CreateUserPrincipalAsync(user);
            if (userPrincliple == null)
            {
                return null;
            }

            var signInKey = GetSecurityKey(_configuration);

            var token = new JwtSecurityToken
                (
                expires: DateTime.UtcNow + expiryDate,
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256),
                claims: userPrincliple.Claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

