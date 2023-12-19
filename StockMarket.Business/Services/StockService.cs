using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;
using AngleSharp;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;

namespace StockMarket.API.Controllers.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IPriceHistoryRepository _priceHistoryRepository;
        private readonly ILogger<StockService> _logger;
        public StockService(IStockRepository stockRepository, IPriceHistoryRepository priceHistoryRepository, ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _priceHistoryRepository = priceHistoryRepository;
            _logger = logger;
        }

        public async Task FetchStockData()
        {
            _logger.LogInformation("'FetchStockData' method executed.");
            try
            {
                string url = "https://bigpara.hurriyet.com.tr/borsa/canli-borsa/";
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync(url);

                    var config = Configuration.Default;
                    var context = BrowsingContext.New(config);
                    var parser = context.GetService<IHtmlParser>();
                    var document = parser.ParseDocument(response);

                    var stocks = document.QuerySelectorAll("ul.live-stock-item");
                    foreach (var stock in stocks)
                    {
                        string name = stock.GetAttribute("data-symbol").ToString();
                        decimal price = Convert.ToDecimal(stock.QuerySelector("li.node-c").TextContent);

                        var check = await _stockRepository.GetByName(name);
                        if (check != null)
                        {
                            await _stockRepository.UpdatePriceByName(name, price);
                            await UpdateStockHistory(check, price);
                        }
                        else
                        {
                            var newStock = new Stock
                            {
                                Name = name,
                                Price = price,
                                Quantity = 10000,
                                IsActive = true
                            };
                            await _stockRepository.Create(newStock);
                            await UpdateStockHistory(newStock, price);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during stock data fetch.");
                throw;
            }

        }
        public async Task UpdateStockHistory(Stock stock, decimal price)
        {
            _logger.LogInformation("'UpdateStockHistory' method executed.");
            try
            {
                await _stockRepository.UpdatePriceByName(stock.Name, price);

                var history = await _priceHistoryRepository.GetByStockId(stock.Id);

                if (history == null)
                {
                    await _priceHistoryRepository.Create(stock.Id, price);
                }
                else
                {
                    if (history.Price != price)
                    {
                        await _priceHistoryRepository.Create(stock.Id, price);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during stock history update.");
                throw; // You might want to handle the exception accordingly based on your application requirements.
            }
        }
    }
}
