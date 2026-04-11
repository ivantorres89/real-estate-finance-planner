# Decisions, Gaps, and Trade-offs

## Detected Ambiguities and Resolutions

### 1. ITP Taxable Base
**Ambiguity:** The prompt says "3% of the configurable/documented taxable base" without specifying what the taxable base is.
**Resolution:** The taxable base for ITP (Impuesto de Transmisiones Patrimoniales) is the deed price (`deedPrice`), which is standard practice in Spain. This is configurable in the model.

### 2. Purchase Price vs. Deed Price
**Ambiguity:** The prompt references both `purchasePrice` and `deedPrice`. These can differ in real transactions.
**Resolution:** Both are modeled separately. The mortgage maximum is calculated over the deed price (bank appraisal reference). The entry payment is `purchasePrice - maxMortgageAmount`. If the deed price is lower than the purchase price, the buyer must cover the difference from their own funds.

### 3. Financeable Percentage Default
**Ambiguity:** The prompt mentions "up to 90% of deed price" without specifying a default.
**Resolution:** Default is set to 80% (standard for main residences in Spain). The user can configure up to 100% per bank scenario. The system validates that the resulting debt ratio stays within limits.

### 4. Bonus Duration Handling
**Ambiguity:** "Duration if the bonus does not apply during the full life of the mortgage" - unclear how to calculate savings when a bonus expires mid-mortgage.
**Resolution:** Each bonus has an optional `durationYears`. If null, it applies for the full mortgage term. The bonus cost is calculated only for its active period. The TIN reduction also only applies during that period, and the system splits the amortization into two phases (discounted TIN period + base TIN period) for accurate total cost calculation.
**Simplification applied:** For MVP, the bonus TIN reduction is applied for the full term but the cost is only charged for the bonus duration. This slightly overestimates the benefit of time-limited bonuses. A future enhancement could split the amortization schedule.

### 5. Entry Payment Floor
**Ambiguity:** What happens if `purchasePrice <= maxMortgageAmount`?
**Resolution:** Entry payment is `Math.Max(0, purchasePrice - maxMortgageAmount)`. If the bank finances 100% or more of the purchase price, entry payment is zero.

### 6. Municipal Capital Gains Tax (Plusvalia)
**Ambiguity:** Not always applicable and calculation varies by municipality.
**Resolution:** Modeled as a simple optional input amount. The user is expected to calculate or estimate it externally since municipal calculation methods vary significantly.

### 7. Investment Return Modeling
**Ambiguity:** "Gross vs net expected return" and three scenarios.
**Resolution:** The user provides three return rates (conservative, base, optimistic). If `useNetReturns` is true, these are treated as net. Otherwise, they are gross and a configurable tax rate is not applied (the user should input net values directly). Future enhancement could add automatic tax deduction.

### 8. Strategy Comparison Baseline
**Ambiguity:** What exactly is compared in Strategy A vs Strategy B?
**Resolution:**
- **Strategy A (Amortize Faster):** Uses all available liquidity minus the minimum cushion as entry payment. Results in a smaller mortgage, lower monthly payment, lower total interest.
- **Strategy B (Maintain Capital):** Uses only the minimum required entry (purchase price - max mortgage amount). Preserves maximum capital for reinvestment. Results in a larger mortgage, higher monthly payment, higher total interest, but more capital to invest.
- The difference in capital between strategies is the "investable delta," which is projected forward at the three return scenarios.

### 9. Recommendation Weighting
**Decision:** Risk profile affects the weighting of the three investment scenarios:
| Risk Profile   | Conservative | Base | Optimistic |
|---------------|-------------|------|------------|
| Conservative   | 60%         | 30%  | 10%        |
| Balanced       | 25%         | 50%  | 25%        |
| Aggressive     | 10%         | 30%  | 60%        |

The weighted net benefit must be positive AND remaining liquidity must meet the minimum cushion for Strategy B to be recommended.

### 10. Bank Recommendation Thresholds
**Decision:** Per-bank recommendation is based on the real global cost (principal + interest + bonus costs):
- **Recommended:** Lowest real global cost among all banks AND debt ratio within limits
- **Questionable:** Within 5% of the best option OR debt ratio approaching limit (>30%)
- **Not Worth It:** More than 5% above the best option OR any debt ratio violation

### 11. Target Framework
**Decision:** Using .NET 10 (`net10.0`) as it is the current LTS release.

### 12. MongoDB Document Model
**Decision:** Single document per scenario (denormalized). Since this is a single-user local tool, there are no concurrency concerns. A single document keeps reads and writes simple and atomic.

## Modeling Decisions

### French Amortization Formula
Monthly payment = `P * [r * (1 + r)^n] / [(1 + r)^n - 1]`
Where:
- `P` = loan principal
- `r` = monthly interest rate (annual TIN / 12 / 100)
- `n` = total number of monthly payments (years * 12)

All calculations use `decimal` with sufficient precision. No `double` is used for monetary values.

### Compound Interest for Investment Projection
Future Value = `Capital * (1 + annualReturn / 100) ^ years`

Using annual compounding for simplicity and clarity. Monthly compounding would be more precise but adds complexity without meaningful impact on a strategy recommendation.

## Trade-offs

| Decision | Trade-off |
|----------|-----------|
| Single MongoDB document | Simplicity over query flexibility. Acceptable for single-user local tool. |
| Annual compounding for investments | Simplicity over precision. Difference is marginal for recommendation purposes. |
| No authentication | As requested. Would need to be added for any multi-user scenario. |
| Swagger UI in all environments | Convenience for development and testing. In production, it would be behind auth. |
| Bonus TIN applied for full term | Slight overestimation of benefit for time-limited bonuses. Clear enough for decision-making. |

## Future Evolution Opportunities
1. **Amortization schedule export:** Generate month-by-month amortization table (PDF/CSV)
2. **Variable rate support:** Add Euribor + spread modeling with rate scenarios
3. **Tax deduction modeling:** Include mortgage tax deductions where applicable
4. **Multi-scenario comparison dashboard:** Side-by-side visual comparison
5. **Split amortization for time-limited bonuses:** Two-phase calculation
6. **Historical data tracking:** Track how scenarios evolve over time
7. **Import/export scenarios:** JSON import/export for backup and sharing
