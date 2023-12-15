using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;
using System.Xml;
using AngleSharp;
using System.Net.Http;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using Azure;
using System.Diagnostics;
using System.Globalization;

namespace StockMarket.API.Controllers.Services
{
    public class StockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IPriceHistoryRepository _priceHistoryRepository;
        public StockService(IStockRepository stockRepository, IPriceHistoryRepository priceHistoryRepository)
        {
            _stockRepository = stockRepository;
            _priceHistoryRepository = priceHistoryRepository;
        }

        public async Task FetchStockData()
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
                    } else {
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
        private async Task UpdateStockHistory(Stock stock, decimal price)
        {
            await _stockRepository.UpdatePriceByName(stock.Name, price);
            var history = await _priceHistoryRepository.GetByStockId(stock.Id);
            if (history == null)
            {
                await _priceHistoryRepository.Create(stock.Id, price);
            } else
            {
                if (history.Price != price)
                {
                    await _priceHistoryRepository.Create(stock.Id, price);
                }
            }
        }
    }
}
