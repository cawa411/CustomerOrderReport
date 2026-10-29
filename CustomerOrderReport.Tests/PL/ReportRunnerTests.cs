using CustomerOrderReport.BLL;
using CustomerOrderReport.BLL.Exceptions;
using CustomerOrderReport.BLL.Models;
using CustomerOrderReport.PL;
using CustomerOrderReport.PL.Writers;
using Moq;

namespace CustomerOrderReport.Tests.PL;

public class ReportRunnerTests
{
    private readonly Mock<IReportService> _reportService = new();
    private readonly Mock<IWriter> _writer = new();

    public ReportRunnerTests()
    {
        _reportService.Setup(s => s.GetClientReport(1010)).Returns(new ClientReport(1010, 499.96m, "Чипсеки"));
        _reportService.Setup(s => s.GetClientReport(1020)).Returns(new ClientReport(1020, 660m, "Сырок со сгущенкой"));
        _reportService.Setup(s => s.GetClientReport(9999)).Throws(new ClientNotFoundException(9999));
    }

    [Fact]
    public void Run_AllClientsFound_WritesAllReportsAndReturnsTrue()
    {
        var runner = new ReportRunner(_reportService.Object);

        var result = runner.Run([1010, 1020], _writer.Object);

        Assert.True(result);
        _writer.Verify(w => w.Write(It.Is<List<ClientReport>>(r =>
            r.Count == 2 && r[0].ClientId == 1010 && r[1].ClientId == 1020)), Times.Once);
    }

    [Fact]
    public void Run_SomeClientsNotFound_WritesFoundOnesAndReturnsFalse()
    {
        var runner = new ReportRunner(_reportService.Object);

        var result = runner.Run([1010, 9999, 1020], _writer.Object);

        Assert.False(result);
        _writer.Verify(w => w.Write(It.Is<List<ClientReport>>(r =>
            r.Count == 2 && r.All(x => x.ClientId != 9999))), Times.Once);
    }

    [Fact]
    public void Run_NoClientsFound_DoesNotCallWriter()
    {
        var runner = new ReportRunner(_reportService.Object);

        var result = runner.Run([9999], _writer.Object);

        Assert.False(result);
        _writer.Verify(w => w.Write(It.IsAny<List<ClientReport>>()), Times.Never);
    }
}
