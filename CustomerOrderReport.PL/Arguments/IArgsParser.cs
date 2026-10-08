namespace CustomerOrderReport.PL.Arguments;

public interface IArgsParser
{
    ReportRequest Parse(string[] args);
}
