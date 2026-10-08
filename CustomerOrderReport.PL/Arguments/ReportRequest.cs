namespace CustomerOrderReport.PL.Arguments;

public record ReportRequest(string Mode, List<long> ClientIds);
