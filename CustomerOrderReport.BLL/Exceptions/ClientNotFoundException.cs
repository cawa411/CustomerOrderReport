namespace CustomerOrderReport.BLL.Exceptions;

public class ClientNotFoundException(long clientId) : Exception($"Клиент {clientId} не найден в заказах.")
{
    public long ClientId { get; } = clientId;
}
