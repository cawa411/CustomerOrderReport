using CustomerOrderReport.BLL.Models;

namespace CustomerOrderReport.BLL;

public interface IReportService
{
    public ClientReport GetClientReport(long clientId);
}