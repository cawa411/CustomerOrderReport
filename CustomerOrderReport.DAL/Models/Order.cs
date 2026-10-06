namespace CustomerOrderReport.DAL.Models;

public record Order(long ClientId, long OrderId, string OrderName, int Quantity, decimal Price);