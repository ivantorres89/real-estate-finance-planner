using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Domain.Services;

public static class DebtCapacityCalculator
{
    public static DebtCapacityResult Calculate(DebtCapacityData debtData)
    {
        ArgumentNullException.ThrowIfNull(debtData);

        if (debtData.MonthlyNetSalary < 0)
            throw new ArgumentException("Monthly net salary cannot be negative.", nameof(debtData));
        if (debtData.MaxDebtRatioPercentage < 0 || debtData.MaxDebtRatioPercentage > 100)
            throw new ArgumentException("Debt ratio must be between 0 and 100.", nameof(debtData));

        decimal maxGrossPayment = debtData.MonthlyNetSalary * (debtData.MaxDebtRatioPercentage / 100m);
        decimal maxMonthlyPaymentCapacity = maxGrossPayment - debtData.MonthlyOutstandingLoanPayments;

        return new DebtCapacityResult
        {
            MaxMonthlyPaymentCapacity = Math.Max(0m, maxMonthlyPaymentCapacity)
        };
    }
}
