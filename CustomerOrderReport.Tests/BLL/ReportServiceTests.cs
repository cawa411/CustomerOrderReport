using CustomerOrderReport.BLL;
using CustomerOrderReport.BLL.Exceptions;
using CustomerOrderReport.DAL;
using CustomerOrderReport.DAL.Models;
using Moq;

namespace CustomerOrderReport.Tests.BLL;

public class ReportServiceTests
{
    private static ReportService CreateService(long clientId, List<Order> orders)
    {
        var repository = new Mock<IOrderRepository>();
        repository.Setup(r => r.GetByClientId(It.IsAny<long>())).Returns([]);
        repository.Setup(r => r.GetByClientId(clientId)).Returns(orders);
        return new ReportService(repository.Object);
    }

    [Fact]
    public void GetClientReport_ExampleFromTask_ReturnsSumAndFavoriteItem()
    {
        var service = CreateService(1010,
        [
            new Order(1010, 1, "Огурчики маринованные", 1, 100m),
            new Order(1010, 1, "Чипсеки", 4, 99.99m),
        ]);

        var report = service.GetClientReport(1010);

        Assert.Equal(1010, report.ClientId);
        Assert.Equal(499.96m, report.OrderSum);
        Assert.Equal("Чипсеки", report.FavoriteItemName);
    }

    [Fact]
    public void GetClientReport_FavoriteItem_IsChosenByQuantityNotByRowCount()
    {
        // Хлеб встречается в трёх строках по 1 штуке, вода — в одной строке на 10 штук
        var service = CreateService(1,
        [
            new Order(1, 1, "Хлеб", 1, 50m),
            new Order(1, 2, "Хлеб", 1, 50m),
            new Order(1, 3, "Хлеб", 1, 50m),
            new Order(1, 4, "Вода", 10, 30m),
        ]);

        var report = service.GetClientReport(1);

        Assert.Equal("Вода", report.FavoriteItemName);
    }

    [Fact]
    public void GetClientReport_SameItemInDifferentOrders_QuantitiesAreSummed()
    {
        // Сок: 3 + 2 = 5 штук, это больше, чем 4 штуки чая
        var service = CreateService(1,
        [
            new Order(1, 1, "Сок", 3, 80m),
            new Order(1, 2, "Чай", 4, 60m),
            new Order(1, 3, "Сок", 2, 80m),
        ]);

        var report = service.GetClientReport(1);

        Assert.Equal("Сок", report.FavoriteItemName);
        Assert.Equal(640m, report.OrderSum);
    }

    [Fact]
    public void GetClientReport_TieInQuantity_ReturnsOneOfTiedItems()
    {
        var service = CreateService(1,
        [
            new Order(1, 1, "Сырок", 10, 33m),
            new Order(1, 2, "Огурчики", 10, 100m),
            new Order(1, 3, "Дошик", 6, 22.5m),
        ]);

        var report = service.GetClientReport(1);

        Assert.Contains(report.FavoriteItemName, new[] { "Сырок", "Огурчики" });
    }

    [Fact]
    public void GetClientReport_ClientWithoutOrders_ThrowsClientNotFoundException()
    {
        var service = CreateService(1010, [new Order(1010, 1, "Чипсеки", 1, 99.99m)]);

        var exception = Assert.Throws<ClientNotFoundException>(() => service.GetClientReport(9999));

        Assert.Equal(9999, exception.ClientId);
    }
}
