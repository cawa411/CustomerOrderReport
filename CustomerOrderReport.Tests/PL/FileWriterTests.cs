using CustomerOrderReport.BLL.Models;
using CustomerOrderReport.PL.Writers;

namespace CustomerOrderReport.Tests.PL;

public class FileWriterTests : IDisposable
{
    private readonly string _reportsDirectory = Path.Combine(AppContext.BaseDirectory, "reports");
    private readonly HashSet<string> _filesBefore;

    public FileWriterTests()
    {
        _filesBefore = Directory.Exists(_reportsDirectory)
            ? Directory.GetFiles(_reportsDirectory).ToHashSet()
            : [];
    }

    public void Dispose()
    {
        foreach (var file in NewFiles())
            File.Delete(file);
    }

    private List<string> NewFiles() =>
        Directory.Exists(_reportsDirectory)
            ? Directory.GetFiles(_reportsDirectory).Where(f => !_filesBefore.Contains(f)).ToList()
            : [];

    [Fact]
    public void Write_TwoRequests_CreateTwoDifferentFiles()
    {
        var writer = new FileWriter(new ReportFormatter());
        List<ClientReport> reports = [new ClientReport(1010, 499.96m, "Чипсеки")];

        writer.Write(reports);
        writer.Write(reports);

        Assert.Equal(2, NewFiles().Count);
    }

    [Fact]
    public void Write_FileContainsFormattedReport()
    {
        var formatter = new ReportFormatter();
        var writer = new FileWriter(formatter);
        List<ClientReport> reports = [new ClientReport(1010, 499.96m, "Чипсеки")];

        writer.Write(reports);

        var file = Assert.Single(NewFiles());
        Assert.Equal(formatter.Format(reports), File.ReadAllLines(file));
    }
}
