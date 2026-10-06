using System.Globalization;
using CustomerOrderReport.BLL;
using CustomerOrderReport.DAL;
using CustomerOrderReport.DAL.Options;
using Microsoft.Extensions.DependencyInjection;

const string ordersFileName = "order_log.csv";

var services = new ServiceCollection();

services.AddSingleton<IOrderRepository, CsvOrderRepository>();
services.AddSingleton<IReportService, ReportService>();
services.AddSingleton(new OrdersFileOptions()
{
    TablePath = Path.Combine(AppContext.BaseDirectory, ordersFileName)
});

using var provider = services.BuildServiceProvider();
var reportService = provider.GetRequiredService<IReportService>();



var idArgs = args.Length > 0 && args[0] is "console" or "file" ? args[1..] : args;

Console.WriteLine("client_id | orders_sum | favorite_item_name");
foreach (var arg in idArgs)
{
    if (!long.TryParse(arg, out var clientId))
    {
        Console.Error.WriteLine($"Некорректный id клиента: {arg}");
        continue;
    }

    var report = reportService.GetClientReport(clientId);
    Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
        $"{report.ClientId} | {report.OrderSum:F2} | {report.FavoriteItemName}"));
}
