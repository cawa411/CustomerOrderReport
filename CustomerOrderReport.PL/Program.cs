using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
// TODO: зарегистрировать слои после добавления ссылок PL → BLL → DAL
using var provider = services.BuildServiceProvider();

var idArgs = args.Length > 0 && args[0] is "console" or "file" ? args[1..] : args;

Console.WriteLine("client_id | orders_sum | favorite_item_name");
foreach (var arg in idArgs)
{
    if (!long.TryParse(arg, out var clientId))
    {
        Console.Error.WriteLine($"Некорректный id клиента: {arg}");
        continue;
    }
    
    Console.WriteLine($"{clientId} | 0.00 | заглушка");
}
