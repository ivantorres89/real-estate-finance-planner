import { RiskProfile, BonusCategory } from '../models';
import type {
  SaleDataDto,
  PurchaseDataDto,
  DebtCapacityDataDto,
  BankOfferDto,
  BonusDto,
  StrategyParametersDto,
  CreateScenarioRequest,
} from '../models';

export function defaultSaleData(): SaleDataDto {
  return {
    salePrice: 0,
    saleRelatedCosts: 0,
    outstandingMortgageDebt: 0,
    currentCashBalance: 0,
    municipalCapitalGainsTax: 0,
    extraordinaryCosts: 0,
  };
}

export function defaultPurchaseData(): PurchaseDataDto {
  return {
    purchasePrice: 0,
    deedPrice: 0,
    financeablePercentage: 80,
    notaryCosts: 0,
    administrativeCosts: 0,
    appraisalCosts: 0,
    agencyCosts: 0,
    otherCosts: 0,
    applyReducedItp: false,
    isMainResidence: true,
    buyerAge: 30,
  };
}

export function defaultDebtCapacity(): DebtCapacityDataDto {
  return {
    monthlyNetSalary: 0,
    monthlyOutstandingLoanPayments: 0,
    maxDebtRatioPercentage: 35,
  };
}

export function defaultBonus(): BonusDto {
  return {
    name: '',
    category: BonusCategory.Other,
    tinReductionPercentage: 0,
    monthlyCost: 0,
    yearlyCost: 0,
    oneTimeCost: 0,
    isMandatory: false,
    isAccepted: false,
  };
}

export function defaultBankOffer(): BankOfferDto {
  return {
    bankName: '',
    baseTinPercentage: 0,
    bonuses: [],
    mortgageTermsYears: [15, 20, 25, 30],
  };
}

export function defaultStrategyParameters(): StrategyParametersDto {
  return {
    expectedAnnualReturnConservative: 3,
    expectedAnnualReturnBase: 6,
    expectedAnnualReturnOptimistic: 9,
    useNetReturns: true,
    analysisHorizonYears: 20,
    minimumLiquidityCushion: 10000,
    riskProfile: RiskProfile.Balanced,
    additionalCapitalToPreserve: 0,
  };
}

export function defaultScenarioRequest(): CreateScenarioRequest {
  return {
    name: '',
    sale: defaultSaleData(),
    purchase: defaultPurchaseData(),
    debtCapacity: defaultDebtCapacity(),
    banks: [],
    strategyParameters: defaultStrategyParameters(),
  };
}
