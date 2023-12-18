using StockMarket.Entities;

namespace StockMarket.API.Controllers.Services
{
    public interface IStockService
    {
        Task FetchStockData();
        Task UpdateStockHistory(Stock stock, decimal price);
    }
}
