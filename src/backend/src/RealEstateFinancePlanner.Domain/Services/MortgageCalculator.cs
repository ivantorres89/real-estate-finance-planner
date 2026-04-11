using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Domain.Services;

/// <summary>
/// Calculates fixed-rate mortgage payments using the French amortization system.
/// All monetary calculations use decimal to avoid floating-point precision issues.
/// </summary>
public static class MortgageCalculator
{
    /// <summary>
    /// Calculates the fixed monthly payment and totals for a French amortization mortgage.
    /// Formula: M = P * [r * (1 + r)^n] / [(1 + r)^n - 1]
    /// where P = principal, r = monthly rate, n = total payments.
    /// </summary>
    public static MortgageCalculationResult Calculate(
        decimal principal,
        decimal annualTinPercentage,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments)
    {
        if (principal <= 0)
            throw new ArgumentException("Principal must be positive.", nameof(principal));
        if (annualTinPercentage < 0)
            throw new ArgumentException("TIN cannot be negative.", nameof(annualTinPercentage));
        if (termYears <= 0)
            throw new ArgumentException("Term must be positive.", nameof(termYears));

        int totalPayments = termYears * 12;
        decimal monthlyPayment;
        decimal totalPaid;

        if (annualTinPercentage == 0m)
        {
            monthlyPayment = principal / totalPayments;
            totalPaid = principal;
        }
        else
        {
            decimal monthlyRate = annualTinPercentage / 100m / 12m;

            // (1 + r)^n calculated with decimal math
            decimal compoundFactor = DecimalPow(1m + monthlyRate, totalPayments);

            // M = P * [r * (1+r)^n] / [(1+r)^n - 1]
            monthlyPayment = principal * (monthlyRate * compoundFactor) / (compoundFactor - 1m);
            monthlyPayment = Math.Round(monthlyPayment, 2);

            totalPaid = monthlyPayment * totalPayments;
        }

        decimal totalInterest = totalPaid - principal;

        decimal resultingDebtRatio = monthlyNetSalary > 0
            ? (monthlyPayment + monthlyOutstandingLoanPayments) / monthlyNetSalary * 100m
            : 0m;

        decimal remainingFreeCash = monthlyNetSalary - monthlyPayment - monthlyOutstandingLoanPayments;

        return new MortgageCalculationResult
        {
            Principal = principal,
            TinPercentage = annualTinPercentage,
            TermYears = termYears,
            MonthlyPayment = monthlyPayment,
            TotalPaid = totalPaid,
            TotalInterest = totalInterest,
            ResultingDebtRatio = Math.Round(resultingDebtRatio, 2),
            RemainingMonthlyFreeCash = remainingFreeCash
        };
    }

    /// <summary>
    /// Calculates decimal exponentiation for integer exponents.
    /// Uses iterative multiplication to maintain decimal precision.
    /// </summary>
    internal static decimal DecimalPow(decimal baseValue, int exponent)
    {
        if (exponent < 0)
            throw new ArgumentException("Exponent must be non-negative.", nameof(exponent));

        if (exponent == 0) return 1m;

        decimal result = 1m;
        decimal current = baseValue;
        int exp = exponent;

        while (exp > 0)
        {
            if ((exp & 1) == 1)
                result *= current;
            current *= current;
            exp >>= 1;
        }

        return result;
    }
}
