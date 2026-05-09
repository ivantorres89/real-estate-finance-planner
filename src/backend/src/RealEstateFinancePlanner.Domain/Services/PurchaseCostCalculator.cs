using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Domain.Services;

public static class PurchaseCostCalculator
{
    private const decimal ReducedItpRate = 0.03m;
    private const decimal StandardItpRate = 0.06m;

    public static PurchaseCostResult Calculate(PurchaseData purchase, decimal realAvailableCash)
    {
        ArgumentNullException.ThrowIfNull(purchase);

        if (purchase.PurchasePrice <= 0)
            throw new ArgumentException("Purchase price must be positive.", nameof(purchase));
        if (purchase.DeedPrice <= 0)
            throw new ArgumentException("Deed price must be positive.", nameof(purchase));
        if (purchase.FinanceablePercentage < 0 || purchase.FinanceablePercentage > 100)
            throw new ArgumentException("Financeable percentage must be between 0 and 100.", nameof(purchase));

        decimal itpRate = purchase.ApplyReducedItp ? ReducedItpRate : StandardItpRate;
        decimal itpAmount = purchase.DeedPrice * itpRate;

        decimal financeableRatio = purchase.FinanceablePercentage / 100m;
        decimal maxMortgageAmount = purchase.DeedPrice * financeableRatio;

        decimal entryPayment = Math.Max(0m, purchase.PurchasePrice - maxMortgageAmount);

        decimal totalCashNeeded =
            entryPayment
            + itpAmount
            + purchase.NotaryCosts
            + purchase.AdministrativeCosts
            + purchase.AppraisalCosts
            + purchase.AgencyCosts
            + purchase.OtherCosts;

        decimal remainingLiquidity = realAvailableCash - totalCashNeeded;

        return new PurchaseCostResult
        {
            ItpAmount = itpAmount,
            MaxMortgageAmount = maxMortgageAmount,
            EntryPayment = entryPayment,
            NotaryCosts = purchase.NotaryCosts,
            AdministrativeCosts = purchase.AdministrativeCosts,
            AppraisalCosts = purchase.AppraisalCosts,
            AgencyCosts = purchase.AgencyCosts,
            OtherCosts = purchase.OtherCosts,
            TotalCashNeeded = totalCashNeeded,
            RemainingLiquidity = remainingLiquidity,
            IsViable = remainingLiquidity >= 0
        };
    }
}
