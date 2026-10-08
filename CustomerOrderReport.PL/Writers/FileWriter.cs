using CustomerOrderReport.BLL.Models;

namespace CustomerOrderReport.PL.Writers;

public class FileWriter(ReportFormatter formatter) : IWriter
{
    private const string ReportsDirectoryName = "reports";

    public void Write(List<ClientReport> reports)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, ReportsDirectoryName);
        Directory.CreateDirectory(directory);
        
        //TODO придумать как собирать норм имя
        var fileName = $"report_{Guid.NewGuid().ToString()}.txt";
        var path = Path.Combine(directory, fileName);

        File.WriteAllLines(path, formatter.Format(reports));
        Console.WriteLine($"Отчёт сохранён: {path}");
    }
}
