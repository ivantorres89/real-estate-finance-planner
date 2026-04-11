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
            NetSaleLiquidity = result.NetSaleLiquidity,
            RealAvailableCash = result.RealAvailableCash,
        });
    }

    [HttpPost("purchase-costs")]
    [ProducesResponseType(typeof(PurchaseCostResultDto), StatusCodes.Status200OK)]
    public IActionResult CalculatePurchaseCosts([FromBody] PurchaseCostRequest request)
    {
        var result = _analysisService.CalculatePurchaseCosts(
            request.Purchase.ToEntity(), request.RealAvailableCash);
        return Ok(new PurchaseCostResultDto
        {
            ItpAmount = result.ItpAmount,
            MaxMortgageAmount = result.MaxMortgageAmount,
            EntryPayment = result.EntryPayment,
            TotalCashNeeded = result.TotalCashNeeded,
            RemainingLiquidity = result.RemainingLiquidity,
            IsViable = result.IsViable,
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
            request.RealAvailableCash,
            request.PurchasePrice,
            request.MaxMortgageAmount,
            request.TotalPurchaseCostsExcludingEntry,
            request.MortgageTin,
            request.TermYears,
            request.MonthlyNetSalary,
            request.MonthlyOutstandingLoanPayments,
            request.TotalAcceptedBonusCost,
            request.StrategyParameters.ToEntity());
        return Ok(result.ToDto());
    }
}
