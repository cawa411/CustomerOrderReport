using CustomerOrderReport.BLL.Exceptions;
using CustomerOrderReport.BLL.Models;
using CustomerOrderReport.DAL;
using CustomerOrderReport.DAL.Models;

namespace CustomerOrderReport.BLL;

public class ReportService(IOrderRepository repository): IReportService
{
    public ClientReport GetClientReport(long clientId)
    {
        var orders = repository.GetByClientId(clientId);
        if (orders.Count == 0) throw new ClientNotFoundException(clientId);
        return new ClientReport(clientId, CalculateOrderSum(orders), FindFavoriteItem(orders));
    }

    private decimal CalculateOrderSum(List<Order> orders) => orders.Sum(o => o.Price *  o.Quantity);
    
    private string FindFavoriteItem(List<Order> orders) => orders
        .GroupBy(o => o.OrderName)
        .MaxBy(g => g.Sum(o => o.Quantity))!
        .Key;
}