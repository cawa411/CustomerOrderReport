namespace CustomerOrderReport.PL.Arguments;

public static class ReportModes
{
    public const string Console = "console";
    public const string File = "file";
    public const string Default = Console;

    public static readonly List<string> All = [Console, File];
}
