using CustomerOrderReport.DAL.Models;
using CustomerOrderReport.DAL.Options;

namespace CustomerOrderReport.DAL;

public class CsvOrderRepository(OrdersFileOptions fileOptions) : IOrderRepository
{
    public List<Order> GetByClientId(long clientId)
    {
        return new List<Order>();
    }
}