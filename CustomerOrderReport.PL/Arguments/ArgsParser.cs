using System.Globalization;
using CustomerOrderReport.PL.Exceptions;

namespace CustomerOrderReport.PL.Arguments;

public class ArgsParser : IArgsParser
{
    public ReportRequest Parse(string[] args)
    {
        if (args.Length == 0)
            throw new InvalidArgumentException("Не переданы аргументы.");

        var (mode, idArgs) = IsClientId(args[0])
            ? (ReportModes.Default, args)
            : (args[0], args[1..]);

        if (!ReportModes.All.Contains(mode))
            throw new InvalidArgumentException(
                $"Неизвестный режим \"{mode}\". Допустимые режимы: {string.Join(", ", ReportModes.All)}.");

        if (idArgs.Length == 0)
            throw new InvalidArgumentException("Не переданы id клиентов.");

        var invalidIds = idArgs.Where(arg => !IsClientId(arg)).ToList();
        if (invalidIds.Count > 0)
            throw new InvalidArgumentException(
                $"Некорректные id клиентов: {string.Join(", ", invalidIds)}. Id должен быть целым числом.");

        var clientIds = idArgs
            .Select(arg => long.Parse(arg, CultureInfo.InvariantCulture))
            .Distinct()
            .ToList();

        return new ReportRequest(mode, clientIds);
    }

    private static bool IsClientId(string arg) =>
        long.TryParse(arg, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
}
