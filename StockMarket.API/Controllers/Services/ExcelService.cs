using OfficeOpenXml;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IStockService _stockService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<ExcelService> _logger;
        public ExcelService(IUserRepository userRepository, IStockRepository stockRepository, IStockService stockService, ITransactionRepository transactionRepository, ILogger<ExcelService> logger)
        {
            _userRepository = userRepository;
            _stockRepository = stockRepository;
            _stockService = stockService;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        static string GetCurrentDateTimeAsString()
        {
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyyMMdd_HHmmss");
            return formattedDateTime;
        }

        private string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public async Task<bool> ExportStocksToExcel()
        {
            try
            {
                await _stockService.FetchStockData();
                var stocks = await _stockRepository.GetAll();
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Stocks");
                    worksheet.Cells["A1"].Value = "Name";
                    worksheet.Cells["B1"].Value = "Price";
                    worksheet.Cells["C1"].Value = "Quantity";
                    worksheet.Cells["D1"].Value = "IsActive";

                    int rowIndex = 2;
                    foreach (var stock in stocks)
                    {
                        worksheet.Cells[$"A{rowIndex}"].Value = stock.Name;
                        worksheet.Cells[$"B{rowIndex}"].Value = stock.Price;
                        worksheet.Cells[$"C{rowIndex}"].Value = stock.Quantity;
                        worksheet.Cells[$"D{rowIndex}"].Value = stock.IsActive;
                        rowIndex++;
                    }

                    string currentDateTimeString = GetCurrentDateTimeAsString();
                    FileInfo excelFile = new FileInfo($"Stocks_{currentDateTimeString}.xlsx");
                    package.SaveAs(excelFile);
                    _logger.LogInformation($"Stock data exported to Excel: {excelFile.FullName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting stocks to Excel. Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ExportTransactionsToExcel(int userId)
        {
            try
            {
                var existingUser = await _userRepository.Get(userId);
                if (existingUser == null)
                {
                    _logger.LogWarning($"Invalid User Id: {userId}");
                    return false;
                }
                var transactions = await _transactionRepository.GetByUserId(userId);
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Transactions");
                    worksheet.Cells["A1"].Value = "UserId";
                    worksheet.Cells["B1"].Value = "StockId";
                    worksheet.Cells["C1"].Value = "Type";
                    worksheet.Cells["D1"].Value = "Date";
                    worksheet.Cells["E1"].Value = "Quantity";
                    worksheet.Cells["F1"].Value = "Price";
                    worksheet.Cells["G1"].Value = "Commission";

                    int rowIndex = 2;
                    foreach (var transaction in transactions)
                    {
                        worksheet.Cells[$"A{rowIndex}"].Value = transaction.UserId;
                        worksheet.Cells[$"B{rowIndex}"].Value = transaction.StockId;
                        worksheet.Cells[$"C{rowIndex}"].Value = transaction.Type;
                        worksheet.Cells[$"D{rowIndex}"].Value = FormatDateTime(transaction.Date);
                        worksheet.Cells[$"E{rowIndex}"].Value = transaction.Quantity;
                        worksheet.Cells[$"F{rowIndex}"].Value = transaction.Price;
                        worksheet.Cells[$"G{rowIndex}"].Value = transaction.Commission;
                        rowIndex++;
                    }

                    string currentDateTimeString = GetCurrentDateTimeAsString();
                    FileInfo excelFile = new FileInfo($"Transactions_UserId{userId}_{currentDateTimeString}.xlsx");
                    package.SaveAs(excelFile);
                    _logger.LogInformation($"Transaction data exported to Excel: {excelFile.FullName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting transactions to Excel. Error: {ex.Message}");
                return false;
            }
        }
    }
}
