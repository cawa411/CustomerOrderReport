using System.Globalization;
using CustomerOrderReport.BLL.Models;

namespace CustomerOrderReport.PL.Writers;

public class ReportFormatter
{
    public IEnumerable<string> Format(List<ClientReport> reports)
    {
        yield return "client_id | orders_sum | favorite_item_name";
        foreach (var report in reports)
        {
            yield return string.Create(CultureInfo.InvariantCulture,
                $"{report.ClientId} | {report.OrderSum:F2} | {report.FavoriteItemName}");
        }
    }
}