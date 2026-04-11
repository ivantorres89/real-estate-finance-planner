// MongoDB seed data for development
db = db.getSiblingDB('real_estate_planner');

db.scenarios.insertOne({
  Name: "Example - Madrid Apartment",
  Sale: {
    SalePrice: NumberDecimal("320000"),
    SaleRelatedCosts: NumberDecimal("3500"),
    OutstandingMortgageDebt: NumberDecimal("95000"),
    CurrentCashBalance: NumberDecimal("45000"),
    MunicipalCapitalGainsTax: NumberDecimal("1800"),
    ExtraordinaryCosts: NumberDecimal("500")
  },
  Purchase: {
    PurchasePrice: NumberDecimal("280000"),
    DeedPrice: NumberDecimal("280000"),
    FinanceablePercentage: NumberDecimal("80"),
    NotaryCosts: NumberDecimal("900"),
    AdministrativeCosts: NumberDecimal("400"),
    AppraisalCosts: NumberDecimal("350"),
    AgencyCosts: NumberDecimal("0"),
    OtherCosts: NumberDecimal("200"),
    ApplyReducedItp: false,
    IsMainResidence: true,
    BuyerAge: 35
  },
  DebtCapacity: {
    MonthlyNetSalary: NumberDecimal("3200"),
    MonthlyOutstandingLoanPayments: NumberDecimal("0"),
    MaxDebtRatioPercentage: NumberDecimal("35")
  },
  Banks: [
    {
      BankName: "Banco Santander",
      BaseTinPercentage: NumberDecimal("2.90"),
      Bonuses: [
        {
          Name: "Payroll direct deposit",
          Category: 3,
          TinReductionPercentage: NumberDecimal("0.50"),
          MonthlyCost: NumberDecimal("0"),
          YearlyCost: NumberDecimal("0"),
          OneTimeCost: NumberDecimal("0"),
          IsMandatory: false,
          IsAccepted: true,
          Notes: "Minimum 600 EUR/month"
        },
        {
          Name: "Home Insurance",
          Category: 0,
          TinReductionPercentage: NumberDecimal("0.30"),
          MonthlyCost: NumberDecimal("35"),
          YearlyCost: NumberDecimal("0"),
          OneTimeCost: NumberDecimal("0"),
          IsMandatory: false,
          IsAccepted: true,
          DurationYears: 25
        },
        {
          Name: "Life Insurance",
          Category: 1,
          TinReductionPercentage: NumberDecimal("0.20"),
          MonthlyCost: NumberDecimal("28"),
          YearlyCost: NumberDecimal("0"),
          OneTimeCost: NumberDecimal("0"),
          IsMandatory: false,
          IsAccepted: false,
          DurationYears: 25
        }
      ],
      MortgageTermsYears: [15, 20, 25, 30]
    },
    {
      BankName: "CaixaBank",
      BaseTinPercentage: NumberDecimal("3.10"),
      Bonuses: [
        {
          Name: "Payroll direct deposit",
          Category: 3,
          TinReductionPercentage: NumberDecimal("0.60"),
          MonthlyCost: NumberDecimal("0"),
          YearlyCost: NumberDecimal("0"),
          OneTimeCost: NumberDecimal("0"),
          IsMandatory: false,
          IsAccepted: true
        },
        {
          Name: "Home + Life Insurance Package",
          Category: 0,
          TinReductionPercentage: NumberDecimal("0.40"),
          MonthlyCost: NumberDecimal("55"),
          YearlyCost: NumberDecimal("0"),
          OneTimeCost: NumberDecimal("0"),
          IsMandatory: false,
          IsAccepted: true,
          DurationYears: 25
        }
      ],
      MortgageTermsYears: [20, 25, 30]
    }
  ],
  StrategyParameters: {
    ExpectedAnnualReturnConservative: NumberDecimal("3"),
    ExpectedAnnualReturnBase: NumberDecimal("6"),
    ExpectedAnnualReturnOptimistic: NumberDecimal("9"),
    UseNetReturns: true,
    AnalysisHorizonYears: 20,
    MinimumLiquidityCushion: NumberDecimal("10000"),
    RiskProfile: 1,
    AdditionalCapitalToPreserve: NumberDecimal("0")
  },
  CreatedAt: new Date(),
  UpdatedAt: new Date()
});

print("Seed data inserted successfully.");
