namespace RealEstateFinancePlanner.Domain.Entities;

public class DebtCapacityData
{
    public decimal MonthlyNetSalary { get; set; }
    public decimal MonthlyOutstandingLoanPayments { get; set; }
    public decimal MaxDebtRatioPercentage { get; set; } = 35m;
}
