namespace RealEstateFinancePlanner.Domain.Entities;

public class MortgageCalculationResult
{
    public decimal Principal { get; set; }
    public decimal TinPercentage { get; set; }
    public int TermYears { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal ResultingDebtRatio { get; set; }
    public decimal RemainingMonthlyFreeCash { get; set; }
}
