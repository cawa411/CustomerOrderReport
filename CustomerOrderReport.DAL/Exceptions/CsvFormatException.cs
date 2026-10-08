namespace CustomerOrderReport.DAL.Exceptions;

public class CsvFormatException(int lineNumber, string line) : OrdersFileException(
    $"Строка {lineNumber}: неверный формат \"{line}\". " +
    "Ожидается: client_id(long),order_id(long),item_name,item_quantity(int > 0),item_price(до 2 знаков)")
{
    public int LineNumber { get; } = lineNumber;
}
