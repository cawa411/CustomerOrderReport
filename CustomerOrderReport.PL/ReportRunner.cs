using CustomerOrderReport.BLL;
using CustomerOrderReport.BLL.Exceptions;
using CustomerOrderReport.BLL.Models;
using CustomerOrderReport.PL.Writers;

namespace CustomerOrderReport.PL;

public class ReportRunner(IReportService reportService) : IReportRunner
{
    public bool Run(List<long> clientIds, IWriter writer)
    {
        var reports = new List<ClientReport>();
        var allFound = true;

        foreach (var clientId in clientIds)
        {
            try
            {
                reports.Add(reportService.GetClientReport(clientId));
            }
            catch (ClientNotFoundException e)
            {
                Console.Error.WriteLine($"Ошибка: {e.Message}");
                allFound = false;
            }
        }

        if (reports.Count > 0)
            writer.Write(reports);

        return allFound;
    }
}
