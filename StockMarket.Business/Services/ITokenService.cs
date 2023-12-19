using StockMarket.Entities;
using System.Security.Claims;

namespace StockMarket.API.Controllers.Services
{
    public interface ITokenService
    {
        string GenerateToken(List<Claim> claims);
        Task<List<Claim>> GenerateClaims(User user);
    }
}
