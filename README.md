# Real Estate Finance Planner

> [Leer en Espanol](README.es.md)

A comprehensive local tool for **real estate financial decision-making**. Model the full cycle: property sale, liquidity calculation, new property purchase, mortgage comparison across banks (with bonus evaluation), and strategic recommendation (amortize faster vs. maintain capital for investment).

## Architecture

```
src/
  backend/                         .NET 10 Web API (Hexagonal Architecture)
    src/
      RealEstateFinancePlanner.Domain/       Core domain: entities, value objects, services
      RealEstateFinancePlanner.Application/  Use cases, interfaces
      RealEstateFinancePlanner.Infrastructure/ MongoDB persistence, DI
      RealEstateFinancePlanner.Api/          Controllers, DTOs, mapping
    tests/
      RealEstateFinancePlanner.Tests.Unit/        48 unit tests
      RealEstateFinancePlanner.Tests.Integration/ Integration tests with Testcontainers
  frontend/                        Angular 21 + Angular Material
  docker/                          MongoDB seed data
docker-compose.yml                 3-service orchestration
```

### Hexagonal Architecture

- **Domain** - Zero external dependencies. Contains entities, value objects (`Money`, `Percentage`), enums, and domain services (calculators, evaluators, recommendation engine).
- **Application** - Orchestrates domain services. Defines repository interfaces. No framework dependencies.
- **Infrastructure** - MongoDB persistence with `Decimal128` serializer for financial precision. Dependency injection registration.
- **API** - ASP.NET Core controllers, request/response DTOs with validation, Scalar/OpenAPI.

## Tech Stack

| Layer      | Technology                                  |
|------------|---------------------------------------------|
| Backend    | .NET 10, ASP.NET Core Web API               |
| Frontend   | Angular 21, Angular Material, SCSS          |
| Database   | MongoDB 7 (Decimal128 for money precision)  |
| Testing    | xUnit, FluentAssertions, Testcontainers     |
| Containers | Docker, Docker Compose                      |

## Quick Start

### Docker Compose (recommended)

```bash
docker compose up --build
```

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5000
- **API Docs (Scalar)**: http://localhost:5000/scalar/v1
- **MongoDB**: localhost:27017

### Local Development

**Prerequisites**: .NET 10 SDK, Node.js 24+, MongoDB running on localhost:27017

```bash
# Backend
cd src/backend
dotnet build
dotnet run --project src/RealEstateFinancePlanner.Api

# Frontend (in separate terminal)
cd src/frontend
npm install
npx ng serve
```

The Angular dev server proxies `/api/*` requests to `http://localhost:5000`.

## API Endpoints

### Scenarios

| Method | Endpoint                       | Description              |
|--------|--------------------------------|--------------------------|
| GET    | `/api/scenarios`               | List all scenarios       |
| GET    | `/api/scenarios/{id}`          | Get scenario by ID       |
| POST   | `/api/scenarios`               | Create new scenario      |
| PUT    | `/api/scenarios/{id}`          | Update scenario          |
| DELETE | `/api/scenarios/{id}`          | Delete scenario          |
| POST   | `/api/scenarios/{id}/analyze`  | Run full analysis        |

### Calculations (standalone)

| Method | Endpoint                              | Description                |
|--------|---------------------------------------|----------------------------|
| POST   | `/api/calculations/sale-liquidity`    | Calculate sale liquidity   |
| POST   | `/api/calculations/purchase-costs`    | Calculate purchase costs   |
| POST   | `/api/calculations/debt-capacity`     | Calculate debt capacity    |
| POST   | `/api/calculations/mortgage`          | Calculate single mortgage  |
| POST   | `/api/calculations/bank-offer`        | Evaluate bank offer        |
| POST   | `/api/calculations/strategy-comparison` | Compare strategies       |

## Financial Formulas

### French Amortization

$$M = P \times \frac{r(1+r)^n}{(1+r)^n - 1}$$

Where: P = principal, r = monthly interest rate (TIN/12/100), n = total months.

### ITP (Transfer Tax)

- Standard: **6%** of official purchase price A
- Reduced (large family / disability): **3%** of official purchase price A

### Dual A/B Accounting

Sale and purchase are split into two accountings:

- **Side A (official)** — what gets written on the deed. The seller's outstanding mortgage cancellation, sale-related costs, ITP, notary, gestoria, appraisal, agency, other purchase costs, and the mortgage entry payment all draw from A.
- **Side B (cash)** — off-the-books money. Cannot cancel a mortgage, cannot count toward the new mortgage's LTV, and cannot be deposited into a bank account without a paper trail. Suitable for: off-the-books premium paid to the new seller, and B-side renovations.

```
totalSaleDeductionsA = saleRelatedCosts + outstandingMortgageDebt + capitalGainsTax + extraordinaryCosts
netSaleLiquidityA    = officialSalePriceA - totalSaleDeductionsA
netSaleLiquidityB    = unofficialSalePriceB
realAvailableCashA   = currentCashBalance + netSaleLiquidityA
realAvailableCashB   = netSaleLiquidityB

effectiveAppraisal   = appraisalValue > 0 ? appraisalValue : officialPurchasePriceA
mortgageBaseValue    = min(officialPurchasePriceA, effectiveAppraisal)
maxMortgage          = mortgageBaseValue × financeablePercentage/100
itp                  = officialPurchasePriceA × itpRate
entryPaymentA        = max(0, officialPurchasePriceA - maxMortgage)
entryPaymentB        = unofficialPurchasePriceB
totalCashNeededA     = entryPaymentA + itp + notary + admin + appraisal + agency + other
totalCashNeededB     = entryPaymentB + renovationCostsB
remainingLiquidityA  = realAvailableCashA - totalCashNeededA
remainingLiquidityB  = realAvailableCashB - totalCashNeededB
idleCashB            = max(0, remainingLiquidityB)
```

The operation is **viable** when `realAvailableCashA >= 0`, `remainingLiquidityA >= 0` and `remainingLiquidityB >= 0`. The calculator emits a multi-line `BalancingAdvice` that flags shortfalls on A or B, idle B cash above 5,000 EUR, and appraisal-capped mortgages.

### Strategy Comparison

Two strategies are compared:
- **Strategy A (Amortize Faster)**: Use all available cash beyond the liquidity cushion as entry payment, minimizing mortgage principal.
- **Strategy B (Maintain Capital)**: Use the standard 80% financing, invest the remaining liquidity.

Future value of invested capital is projected under three scenarios (conservative / base / optimistic) using compound interest. A **risk-weighted net difference** determines the recommendation, using profile-specific weights:

| Profile      | Conservative | Base | Optimistic |
|-------------|-------------|------|------------|
| Conservative | 60%         | 30%  | 10%        |
| Balanced     | 25%         | 50%  | 25%        |
| Aggressive   | 10%         | 30%  | 60%        |

## Testing

```bash
# Unit tests (48 tests)
cd src/backend
dotnet test tests/RealEstateFinancePlanner.Tests.Unit

# Integration tests (requires Docker)
dotnet test tests/RealEstateFinancePlanner.Tests.Integration

# E2E tests (requires Docker stack running)
cd tests/e2e
npm install
npx playwright install chromium
npx playwright test
```

## Project Decisions

See [DECISIONS_AND_GAPS.md](DECISIONS_AND_GAPS.md) for documented ambiguity resolutions, modeling trade-offs, and future evolution notes.

## License

MIT
Complex finance decission orchestrator on real state
