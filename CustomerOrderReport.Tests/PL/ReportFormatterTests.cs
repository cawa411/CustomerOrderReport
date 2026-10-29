using System.Globalization;
using CustomerOrderReport.BLL.Models;
using CustomerOrderReport.PL.Writers;

namespace CustomerOrderReport.Tests.PL;

public class ReportFormatterTests
{
    private readonly ReportFormatter _formatter = new();

    [Fact]
    public void Format_FirstLine_IsHeader()
    {
        var lines = _formatter.Format([new ClientReport(1010, 499.96m, "Чипсеки")]).ToList();

        Assert.Equal("client_id | orders_sum | favorite_item_name", lines[0]);
        Assert.Equal(2, lines.Count);
    }

    [Fact]
    public void Format_RussianCulture_UsesDotAndTwoDecimals()
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
        try
        {
            var lines = _formatter.Format(
            [
                new ClientReport(1010, 499.96m, "Чипсеки"),
                new ClientReport(1020, 660m, "Сырок со сгущенкой"),
            ]).ToList();

            Assert.Equal("1010 | 499.96 | Чипсеки", lines[1]);
            Assert.Equal("1020 | 660.00 | Сырок со сгущенкой", lines[2]);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
