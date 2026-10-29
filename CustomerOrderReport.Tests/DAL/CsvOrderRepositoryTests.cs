using System.Globalization;
using CustomerOrderReport.DAL;
using CustomerOrderReport.DAL.Exceptions;
using CustomerOrderReport.DAL.Options;

namespace CustomerOrderReport.Tests.DAL;

public class CsvOrderRepositoryTests : IDisposable
{
    private const string Header = "client_id,order_id,item_name,item_quantity,item_price";

    private readonly List<string> _tempFiles = [];

    public void Dispose()
    {
        foreach (var file in _tempFiles)
            File.Delete(file);
    }

    private CsvOrderRepository CreateRepository(params string[] lines)
    {
        var path = Path.GetTempFileName();
        _tempFiles.Add(path);
        File.WriteAllLines(path, [Header, .. lines]);
        return new CsvOrderRepository(new OrdersFileOptions { TablePath = path });
    }

    [Fact]
    public void GetByClientId_ValidFile_ReturnsOnlyOrdersOfThatClient()
    {
        var repository = CreateRepository(
            "1010,1,Огурчики маринованные,1,100",
            "1010,1,Чипсеки,4,99.99",
            "1020,2,Сырок со сгущенкой,20,33");

        var orders = repository.GetByClientId(1010);

        Assert.Equal(2, orders.Count);
        Assert.All(orders, o => Assert.Equal(1010, o.ClientId));
        Assert.Equal("Чипсеки", orders[1].OrderName);
        Assert.Equal(4, orders[1].Quantity);
        Assert.Equal(99.99m, orders[1].Price);
    }

    [Fact]
    public void GetByClientId_RussianCulture_ParsesPriceWithDot()
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
        try
        {
            var repository = CreateRepository("1010,1,Чипсеки,4,99.99");

            Assert.Equal(99.99m, repository.GetByClientId(1010)[0].Price);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void GetByClientId_IdLargerThanInt_IsParsedAsLong()
    {
        var repository = CreateRepository("99999999999,1,Сок,1,50");

        Assert.Single(repository.GetByClientId(99999999999));
    }

    [Fact]
    public void GetByClientId_SpacesAroundItemName_AreTrimmed()
    {
        var repository = CreateRepository(
            "1010,1,Чипсеки,4,99.99",
            "1010,2, Чипсеки ,1,99.99");

        Assert.All(repository.GetByClientId(1010), o => Assert.Equal("Чипсеки", o.OrderName));
    }

    [Fact]
    public void GetByClientId_UnknownClient_ReturnsEmptyList()
    {
        var repository = CreateRepository("1010,1,Чипсеки,4,99.99");

        Assert.Empty(repository.GetByClientId(9999));
    }

    [Fact]
    public void Constructor_EmptyLines_AreSkipped()
    {
        var repository = CreateRepository("1010,1,Чипсеки,4,99.99", "", "   ");

        Assert.Single(repository.GetByClientId(1010));
    }

    [Theory]
    [InlineData("1010,1,Чипсеки,4,99,99")]   // цена с запятой — лишнее поле
    [InlineData("1010,1,Чипсеки,4")]         // не хватает поля
    [InlineData("abc,1,Чипсеки,4,99.99")]    // id не число
    [InlineData("1010,1,,4,99.99")]          // пустое название
    [InlineData("1010,1,Чипсеки,0,99.99")]   // количество 0
    [InlineData("1010,1,Чипсеки,4.5,99.99")] // дробное количество
    [InlineData("1010,1,Чипсеки,4,-1")]      // отрицательная цена
    [InlineData("1010,1,Чипсеки,4,99.999")]  // больше 2 знаков
    public void Constructor_BrokenLine_ThrowsCsvFormatExceptionWithLineNumber(string brokenLine)
    {
        var exception = Assert.Throws<CsvFormatException>(() =>
            CreateRepository("1010,1,Огурчики маринованные,1,100", brokenLine));

        Assert.Equal(3, exception.LineNumber);
    }

    [Fact]
    public void Constructor_MissingFile_ThrowsOrdersFileNotFoundException()
    {
        var path = Path.Combine(Path.GetTempPath(), $"no-such-file-{Guid.NewGuid()}.csv");

        var exception = Assert.Throws<OrdersFileNotFoundException>(() =>
            new CsvOrderRepository(new OrdersFileOptions { TablePath = path }));

        Assert.Equal(path, exception.FilePath);
    }
}
