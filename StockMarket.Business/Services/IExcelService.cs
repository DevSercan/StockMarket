namespace StockMarket.API.Controllers.Services
{
    public interface IExcelService
    {
        Task<bool> ExportStocksToExcel();
        Task<bool> ExportTransactionsToExcel(int userId);
    }
}
