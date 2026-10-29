using CustomerOrderReport.BLL;
using CustomerOrderReport.DAL;
using CustomerOrderReport.DAL.Options;

namespace CustomerOrderReport.Tests;

// Проверка на настоящем orders_log.csv из задания: DAL и BLL вместе, без моков
public class OrdersLogAcceptanceTests
{
    private readonly ReportService _service = new(new CsvOrderRepository(new OrdersFileOptions
    {
        TablePath = Path.Combine(AppContext.BaseDirectory, "orders_log.csv")
    }));

    [Theory]
    [InlineData(1010, "5412.15", "Чипсеки")]
    [InlineData(1020, "9809.56", "Сыр")]
    [InlineData(1022, "21195.98", "Дошик")]
    [InlineData(1034, "508", "Хлеб")]
    public void GetClientReport_RealOrdersLog_MatchesManualCalculation(long clientId, string expectedSum, string expectedItem)
    {
        var report = _service.GetClientReport(clientId);

        Assert.Equal(decimal.Parse(expectedSum, System.Globalization.CultureInfo.InvariantCulture), report.OrderSum);
        Assert.Equal(expectedItem, report.FavoriteItemName);
    }
}
