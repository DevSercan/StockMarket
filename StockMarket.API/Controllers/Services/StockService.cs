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
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
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
                    } else {
                        var newStock = new Stock
                        {
                            Name = name,
                            Price = price,
                            Quantity = 1000,
                            IsActive = true
                        };
                        await _stockRepository.Create(newStock);
                    }
                }
            }

        }
    }
}
