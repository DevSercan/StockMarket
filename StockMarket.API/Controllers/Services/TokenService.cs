using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockMarket.API.Controllers.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, IRoleRepository roleRepository, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _roleRepository = roleRepository;
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

                var tokenLifetimeInMinutes = _configuration.GetValue<int>("JwtSettings:TokenLifetimeInMinutes");
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(tokenLifetimeInMinutes),
                    signingCredentials: creds
                );

                var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("Token generated successfully.");

                return generatedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token generation: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<List<Claim>> GenerateClaims(User user)
        {
            _logger.LogInformation("Generating claims for user {UserId} with role {RoleId}.", user.Id, user.RoleId);
            try
            {
                var role = await _roleRepository.Get(user.RoleId);
                if (role == null)
                {
                    _logger.LogWarning("Role not found for user {UserId}.", user.Id);
                    throw new InvalidOperationException($"Role not found for user {user.Id}.");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, role.Name)
                };

                _logger.LogInformation("Claims generated successfully for user {UserId}.", user.Id);
                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during claim generation: {ErrorMessage}", ex.Message);
                throw;
            }
        }

    }
}
