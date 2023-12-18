using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockMarket.API.Controllers.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(List<Claim> claims)
        {
            _logger.LogInformation("'GenerateToken' method executed.");
            try
            {
                var secretKey = _configuration["JwtSettings:Key"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("Token generated successfully.");

                return generatedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token generation.");
                throw;
            }
        }
    }
}
