using System.Globalization;
using System.Text;
using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Domain.Services;

public static class PurchaseCostCalculator
{
    private const decimal ReducedItpRate = 0.03m;
    private const decimal StandardItpRate = 0.06m;
    private const decimal IdleCashBWarningThreshold = 5_000m;

    private static readonly CultureInfo MoneyCulture = CultureInfo.GetCultureInfo("es-ES");

    public static PurchaseCostResult Calculate(
        PurchaseData purchase,
        decimal realAvailableCashA,
        decimal realAvailableCashB)
    {
        ArgumentNullException.ThrowIfNull(purchase);

        if (purchase.OfficialPurchasePriceA <= 0)
            throw new ArgumentException("Official purchase price A must be positive.", nameof(purchase));
        if (purchase.UnofficialPurchasePriceB < 0)
            throw new ArgumentException("Unofficial purchase price B cannot be negative.", nameof(purchase));
        if (purchase.AppraisalValue < 0)
            throw new ArgumentException("Appraisal value cannot be negative.", nameof(purchase));
        if (purchase.RenovationCostsB < 0)
            throw new ArgumentException("Renovation costs B cannot be negative.", nameof(purchase));
        if (purchase.AgencyCostsB < 0)
            throw new ArgumentException("Agency costs B cannot be negative.", nameof(purchase));
        if (purchase.OtherCostsB < 0)
            throw new ArgumentException("Other costs B cannot be negative.", nameof(purchase));
        if (purchase.FinanceablePercentage < 0 || purchase.FinanceablePercentage > 100)
            throw new ArgumentException("Financeable percentage must be between 0 and 100.", nameof(purchase));

        decimal totalPurchasePrice = purchase.OfficialPurchasePriceA + purchase.UnofficialPurchasePriceB;

        decimal effectiveAppraisalValue = purchase.AppraisalValue > 0m
            ? purchase.AppraisalValue
            : purchase.OfficialPurchasePriceA;

        decimal mortgageBaseValue = Math.Min(purchase.OfficialPurchasePriceA, effectiveAppraisalValue);

        decimal financeableRatio = purchase.FinanceablePercentage / 100m;
        decimal maxMortgageAmount = mortgageBaseValue * financeableRatio;

        decimal itpRate = purchase.ApplyReducedItp ? ReducedItpRate : StandardItpRate;
        decimal itpAmount = purchase.OfficialPurchasePriceA * itpRate;

        decimal entryPaymentA = Math.Max(0m, purchase.OfficialPurchasePriceA - maxMortgageAmount);
        decimal entryPaymentB = purchase.UnofficialPurchasePriceB;

        decimal totalCashNeededA =
            entryPaymentA
            + itpAmount
            + purchase.NotaryCosts
            + purchase.AdministrativeCosts
            + purchase.AppraisalCosts
            + purchase.AgencyCosts
            + purchase.OtherCosts;

        decimal totalCashNeededB =
            entryPaymentB
            + purchase.RenovationCostsB
            + purchase.AgencyCostsB
            + purchase.OtherCostsB;

        decimal remainingLiquidityA = realAvailableCashA - totalCashNeededA;
        decimal remainingLiquidityB = realAvailableCashB - totalCashNeededB;
        decimal idleCashB = Math.Max(0m, remainingLiquidityB);

        bool isViable = remainingLiquidityA >= 0m && remainingLiquidityB >= 0m;

        string balancingAdvice = BuildBalancingAdvice(
            realAvailableCashA,
            remainingLiquidityA,
            remainingLiquidityB,
            idleCashB,
            purchase.OfficialPurchasePriceA,
            effectiveAppraisalValue);

        return new PurchaseCostResult
        {
            OfficialPurchasePriceA = purchase.OfficialPurchasePriceA,
            UnofficialPurchasePriceB = purchase.UnofficialPurchasePriceB,
            TotalPurchasePrice = totalPurchasePrice,
            AppraisalValue = purchase.AppraisalValue,
            EffectiveAppraisalValue = effectiveAppraisalValue,
            MortgageBaseValue = mortgageBaseValue,
            FinanceablePercentage = purchase.FinanceablePercentage,
            ItpAmount = itpAmount,
            MaxMortgageAmount = maxMortgageAmount,
            EntryPaymentA = entryPaymentA,
            EntryPaymentB = entryPaymentB,
            NotaryCosts = purchase.NotaryCosts,
            AdministrativeCosts = purchase.AdministrativeCosts,
            AppraisalCosts = purchase.AppraisalCosts,
            AgencyCosts = purchase.AgencyCosts,
            AgencyCostsB = purchase.AgencyCostsB,
            OtherCosts = purchase.OtherCosts,
            OtherCostsB = purchase.OtherCostsB,
            RenovationCostsB = purchase.RenovationCostsB,
            TotalCashNeededA = totalCashNeededA,
            TotalCashNeededB = totalCashNeededB,
            RemainingLiquidityA = remainingLiquidityA,
            RemainingLiquidityB = remainingLiquidityB,
            IdleCashB = idleCashB,
            IsViable = isViable,
            BalancingAdvice = balancingAdvice,
        };
    }

    private static string BuildBalancingAdvice(
        decimal realAvailableCashA,
        decimal remainingLiquidityA,
        decimal remainingLiquidityB,
        decimal idleCashB,
        decimal officialPurchasePriceA,
        decimal effectiveAppraisalValue)
    {
        var lines = new List<string>();

        if (realAvailableCashA < 0m)
        {
            decimal missing = Math.Abs(realAvailableCashA);
            lines.Add($"• La parte A de la venta no cubre la deuda y los gastos oficiales. Falta {FormatEur(missing)}. Opciones: subir el precio en escritura con el comprador, aportar {FormatEur(missing)} de cash propio, o amortizar parcialmente la deuda antes de vender.");
        }

        if (remainingLiquidityA < 0m)
        {
            decimal missing = Math.Abs(remainingLiquidityA);
            lines.Add($"• La parte A no cubre la entrada + ITP + gastos de compra. Faltan {FormatEur(missing)}. Opciones: reducir el precio de escritura A si la tasación lo permite (atención al LTV), aportar cash adicional, o renegociar A/B con el vendedor.");
        }

        if (remainingLiquidityB < 0m)
        {
            decimal missing = Math.Abs(remainingLiquidityB);
            lines.Add($"• La parte B no cubre el sobreprecio en efectivo al vendedor + reformas. Faltan {FormatEur(missing)}. Opciones: aumentar el B de la venta, reducir el B de la compra negociando con el vendedor, o ajustar reformas.");
        }

        if (idleCashB > IdleCashBWarningThreshold)
        {
            lines.Add($"• Sobran {FormatEur(idleCashB)} en B ocioso que no podrás ingresar limpios. Considera aumentar las reformas pagables en efectivo o renegociar el A/B (subir A, bajar B) con el vendedor de la compra para reducir tu B ocioso.");
        }

        if (officialPurchasePriceA > effectiveAppraisalValue)
        {
            lines.Add($"• La hipoteca está topada por la tasación: el banco financiará el porcentaje sobre la tasación ({FormatEur(effectiveAppraisalValue)}), no sobre la escritura ({FormatEur(officialPurchasePriceA)}). Considera tasar más alto o reducir A.");
        }

        if (lines.Count == 0)
        {
            lines.Add("• Operación cuadrada. Sin déficit en A, sin déficit en B, sin B ocioso.");
        }

        return string.Join("\n", lines);
    }

    private static string FormatEur(decimal amount)
    {
        return string.Format(MoneyCulture, "{0:N2} €", amount);
    }
}
