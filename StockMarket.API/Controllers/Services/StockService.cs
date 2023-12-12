using StockMarket.DataAccess.Repositories;

namespace StockMarket.API.Controllers.Services
{
    public class StockService
    {
        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }
    }
}
