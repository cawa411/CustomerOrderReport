using CustomerOrderReport.DAL.Models;
using CustomerOrderReport.DAL.Options;

namespace CustomerOrderReport.DAL;

public interface IOrderRepository
{
    List<Order> GetByClientId(long clientId);
}