using CustomerOrderReport.BLL;
using CustomerOrderReport.DAL;
using CustomerOrderReport.DAL.Exceptions;
using CustomerOrderReport.DAL.Options;
using CustomerOrderReport.PL;
using CustomerOrderReport.PL.Arguments;
using CustomerOrderReport.PL.Exceptions;
using CustomerOrderReport.PL.Writers;
using Microsoft.Extensions.DependencyInjection;

const string ordersFileName = "order_log.csv";
const string usage = "Использование: CustomerOrderReport.PL [console|file] <client_id> [client_id ...]";

var services = new ServiceCollection();

services.AddSingleton<IOrderRepository, CsvOrderRepository>();
services.AddSingleton<IReportService, ReportService>();
services.AddSingleton(new OrdersFileOptions()
{
    TablePath = Path.Combine(AppContext.BaseDirectory, ordersFileName)
});
services.AddSingleton<IArgsParser, ArgsParser>();
services.AddSingleton<IReportRunner, ReportRunner>();
services.AddSingleton<ReportFormatter>();
services.AddKeyedSingleton<IWriter, ConsoleWriter>(ReportModes.Console);
services.AddKeyedSingleton<IWriter, FileWriter>(ReportModes.File);

using var provider = services.BuildServiceProvider();

try
{
    var request = provider.GetRequiredService<IArgsParser>().Parse(args);
    var writer = provider.GetRequiredKeyedService<IWriter>(request.Mode);
    var runner = provider.GetRequiredService<IReportRunner>();

    return runner.Run(request.ClientIds, writer) ? 0 : 1;
}
catch (InvalidArgumentException e)
{
    Console.Error.WriteLine($"Ошибка: {e.Message}");
    Console.Error.WriteLine(usage);
    return 1;
}
catch (OrdersFileException e)
{
    Console.Error.WriteLine($"Ошибка чтения заказов: {e.Message}");
    return 1;
}
