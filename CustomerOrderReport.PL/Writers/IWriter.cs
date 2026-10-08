using CustomerOrderReport.BLL.Models;

namespace CustomerOrderReport.PL.Writers;

public interface IWriter
{
    void Write(List<ClientReport> reports);
}
