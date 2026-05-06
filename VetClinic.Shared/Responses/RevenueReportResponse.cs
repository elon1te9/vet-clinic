namespace VetClinic.Shared.Responses;

public class RevenueReportResponse
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PaidInvoicesCount { get; set; }
}
