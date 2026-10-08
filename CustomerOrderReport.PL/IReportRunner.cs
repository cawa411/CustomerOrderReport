using CustomerOrderReport.PL.Writers;

namespace CustomerOrderReport.PL;

public interface IReportRunner
{
    bool Run(List<long> clientIds, IWriter writer);
}
