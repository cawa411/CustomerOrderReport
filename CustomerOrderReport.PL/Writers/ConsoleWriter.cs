using CustomerOrderReport.BLL.Models;

namespace CustomerOrderReport.PL.Writers;

public class ConsoleWriter(ReportFormatter formatter) : IWriter
{
    public void Write(List<ClientReport> reports)
    {
        foreach (var line in formatter.Format(reports))
        {
            Console.WriteLine(line);
        }
    }
}
