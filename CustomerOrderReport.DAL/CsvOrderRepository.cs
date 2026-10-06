using System.Globalization;
using CustomerOrderReport.DAL.Models;
using CustomerOrderReport.DAL.Options;

namespace CustomerOrderReport.DAL;

public class CsvOrderRepository: IOrderRepository
{
    private readonly IReadOnlyDictionary<long, List<Order>> _orders;

    public CsvOrderRepository(OrdersFileOptions fileOptions)
    {
        _orders = GetAll(fileOptions)
            .GroupBy(order => order.ClientId)
            .ToDictionary(group => group.Key, group => group.ToList());
    }

    public List<Order> GetByClientId(long clientId) => _orders.TryGetValue(clientId, out var orders) ? orders : [];

    private List<Order> GetAll(OrdersFileOptions fileOptions)
    {
        List<Order> result = new ();
        using (StreamReader reader = new StreamReader(fileOptions.TablePath))
        {
            reader.ReadLine();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var temp = line.Split(",");
                if (!OrderItemValidation(temp))
                    throw new Exception("bebebe");
                Order orderItem = CreateOrderItem(temp[0], temp[1], temp[2], temp[3], temp[4]);
                result.Add(orderItem);
            }
        }

        return result;
    }

    private Order CreateOrderItem(string clientId, string orderId, string itemName, string itemQuantity, string itemPrice)
    {
        return new Order(long.Parse(clientId), long.Parse(orderId), itemName, 
            int.Parse(itemQuantity), decimal.Parse(itemPrice, CultureInfo.InvariantCulture));
    }
    
    private static bool OrderItemValidation(string[] fields)
    {
        var culture = CultureInfo.InvariantCulture;

        return fields.Length == 5
               && long.TryParse(fields[0], NumberStyles.Integer, culture, out _)
               && long.TryParse(fields[1], NumberStyles.Integer, culture, out _)
               && !string.IsNullOrWhiteSpace(fields[2])
               && int.TryParse(fields[3], NumberStyles.Integer, culture, out var quantity) && quantity > 0
               && decimal.TryParse(fields[4], NumberStyles.Number, culture, out var price) && price >= 0 && price.Scale <= 2;
    }
}