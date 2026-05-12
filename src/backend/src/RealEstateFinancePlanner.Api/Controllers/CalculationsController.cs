using Microsoft.AspNetCore.Mvc;
using RealEstateFinancePlanner.Api.Dtos;
using RealEstateFinancePlanner.Api.Mapping;
using RealEstateFinancePlanner.Application.Interfaces;

namespace RealEstateFinancePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculationsController : ControllerBase
{
    private readonly IAnalysisService _analysisService;

    public CalculationsController(IAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    [HttpPost("sale-liquidity")]
    [ProducesResponseType(typeof(SaleLiquidityResultDto), StatusCodes.Status200OK)]
    public IActionResult CalculateSaleLiquidity([FromBody] SaleLiquidityRequest request)
    {
        var result = _analysisService.CalculateSaleLiquidity(request.Sale.ToEntity());
        return Ok(new SaleLiquidityResultDto
        {
            OfficialSalePriceA = result.OfficialSalePriceA,
            UnofficialSalePriceB = result.UnofficialSalePriceB,
            TotalSalePrice = result.TotalSalePrice,
            SaleRelatedCosts = result.SaleRelatedCosts,
            OutstandingMortgageDebt = result.OutstandingMortgageDebt,
            MunicipalCapitalGainsTax = result.MunicipalCapitalGainsTax,
            ExtraordinaryCosts = result.ExtraordinaryCosts,
            TotalSaleDeductionsA = result.TotalSaleDeductionsA,
            NetSaleLiquidityA = result.NetSaleLiquidityA,
            NetSaleLiquidityB = result.NetSaleLiquidityB,
            CurrentCashBalance = result.CurrentCashBalance,
            RealAvailableCashA = result.RealAvailableCashA,
            RealAvailableCashB = result.RealAvailableCashB,
            TotalRealAvailableCash = result.TotalRealAvailableCash,
            IsSaleViable = result.IsSaleViable,
        });
    }

    [HttpPost("purchase-costs")]
    [ProducesResponseType(typeof(PurchaseCostResultDto), StatusCodes.Status200OK)]
    public IActionResult CalculatePurchaseCosts([FromBody] PurchaseCostRequest request)
    {
        var result = _analysisService.CalculatePurchaseCosts(
            request.Purchase.ToEntity(),
            request.RealAvailableCashA,
            request.RealAvailableCashB);
        return Ok(new PurchaseCostResultDto
        {
            OfficialPurchasePriceA = result.OfficialPurchasePriceA,
            UnofficialPurchasePriceB = result.UnofficialPurchasePriceB,
            TotalPurchasePrice = result.TotalPurchasePrice,
            AppraisalValue = result.AppraisalValue,
            EffectiveAppraisalValue = result.EffectiveAppraisalValue,
            MortgageBaseValue = result.MortgageBaseValue,
            FinanceablePercentage = result.FinanceablePercentage,
            ItpAmount = result.ItpAmount,
            MaxMortgageAmount = result.MaxMortgageAmount,
            EntryPaymentA = result.EntryPaymentA,
            EntryPaymentB = result.EntryPaymentB,
            NotaryCosts = result.NotaryCosts,
            AdministrativeCosts = result.AdministrativeCosts,
            AppraisalCosts = result.AppraisalCosts,
            AgencyCosts = result.AgencyCosts,
            AgencyCostsB = result.AgencyCostsB,
            OtherCosts = result.OtherCosts,
            OtherCostsB = result.OtherCostsB,
            RenovationCostsB = result.RenovationCostsB,
            TotalCashNeededA = result.TotalCashNeededA,
            TotalCashNeededB = result.TotalCashNeededB,
            RemainingLiquidityA = result.RemainingLiquidityA,
            RemainingLiquidityB = result.RemainingLiquidityB,
            IdleCashB = result.IdleCashB,
            IsViable = result.IsViable,
            BalancingAdvice = result.BalancingAdvice,
        });
    }

    [HttpPost("debt-capacity")]
    [ProducesResponseType(typeof(DebtCapacityResultDto), StatusCodes.Status200OK)]
    public IActionResult CalculateDebtCapacity([FromBody] DebtCapacityRequest request)
    {
        var result = _analysisService.CalculateDebtCapacity(request.DebtCapacity.ToEntity());
        return Ok(new DebtCapacityResultDto
        {
            MaxMonthlyPaymentCapacity = result.MaxMonthlyPaymentCapacity,
        });
    }

    [HttpPost("mortgage")]
    [ProducesResponseType(typeof(MortgageResultDto), StatusCodes.Status200OK)]
    public IActionResult CalculateMortgage([FromBody] MortgageRequest request)
    {
        var result = _analysisService.CalculateMortgage(
            request.Principal, request.AnnualTinPercentage, request.TermYears,
            request.MonthlyNetSalary, request.MonthlyOutstandingLoanPayments);
        return Ok(result.ToDto());
    }

    [HttpPost("bank-offer")]
    [ProducesResponseType(typeof(BankOfferResultDto), StatusCodes.Status200OK)]
    public IActionResult EvaluateBankOffer([FromBody] BankOfferEvaluationRequest request)
    {
        var result = _analysisService.EvaluateBankOffer(
            request.BankOffer.ToEntity(),
            request.MortgagePrincipal,
            request.ReferenceTermYears,
            request.MonthlyNetSalary,
            request.MonthlyOutstandingLoanPayments);
        return Ok(result.ToDto());
    }

    [HttpPost("strategy-comparison")]
    [ProducesResponseType(typeof(StrategyComparisonResultDto), StatusCodes.Status200OK)]
    public IActionResult CompareStrategies([FromBody] StrategyComparisonRequest request)
    {
        var result = _analysisService.CompareStrategies(
            request.RealAvailableCashA,
            request.OfficialPurchasePriceA,
            request.MaxMortgageAmount,
            request.TotalPurchaseCostsExcludingEntryA,
            request.MortgageTin,
            request.TermYears,
            request.MonthlyNetSalary,
            request.MonthlyOutstandingLoanPayments,
            request.TotalAcceptedBonusCost,
            request.StrategyParameters.ToEntity());
        return Ok(result.ToDto());
    }
}
