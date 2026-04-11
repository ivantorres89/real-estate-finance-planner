namespace RealEstateFinancePlanner.Domain.Enums;

public enum BonusCategory
{
    HomeInsurance = 0,
    LifeInsurance = 1,
    AlarmPackage = 2,
    Payroll = 3,
    Card = 4,
    PensionPlan = 5,
    Other = 6
}

public enum RiskProfile
{
    Conservative = 0,
    Balanced = 1,
    Aggressive = 2
}

public enum StrategyRecommendation
{
    AmortizeFaster = 0,
    MaintainCapital = 1
}

public enum BankRecommendationLevel
{
    Recommended = 0,
    Questionable = 1,
    NotWorthIt = 2
}
