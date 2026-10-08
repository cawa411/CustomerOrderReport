namespace CustomerOrderReport.DAL.Exceptions;

public class OrdersFileNotFoundException(string filePath) : OrdersFileException($"Файл заказов не найден: {filePath}")
{
    public string FilePath { get; } = filePath;
}
