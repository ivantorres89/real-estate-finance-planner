import { BonusCategory, RiskProfile } from './enums';

// ---- Shared DTOs ----

export interface SaleDataDto {
  salePrice: number;
  saleRelatedCosts: number;
  outstandingMortgageDebt: number;
  currentCashBalance: number;
  municipalCapitalGainsTax: number;
  extraordinaryCosts: number;
}

export interface PurchaseDataDto {
  purchasePrice: number;
  deedPrice: number;
  financeablePercentage: number;
  notaryCosts: number;
  administrativeCosts: number;
  appraisalCosts: number;
  agencyCosts: number;
  otherCosts: number;
  applyReducedItp: boolean;
  isMainResidence: boolean;
  buyerAge: number;
}

export interface DebtCapacityDataDto {
  monthlyNetSalary: number;
  monthlyOutstandingLoanPayments: number;
  maxDebtRatioPercentage: number;
}

export interface BonusDto {
  id?: string;
  name: string;
  category: BonusCategory;
  tinReductionPercentage: number;
  monthlyCost: number;
  yearlyCost: number;
  oneTimeCost: number;
  isMandatory: boolean;
  isAccepted: boolean;
  durationYears?: number;
  notes?: string;
}

export interface BankOfferDto {
  id?: string;
  bankName: string;
  baseTinPercentage: number;
  bonuses: BonusDto[];
  mortgageTermsYears: number[];
}

export interface StrategyParametersDto {
  expectedAnnualReturnConservative: number;
  expectedAnnualReturnBase: number;
  expectedAnnualReturnOptimistic: number;
  useNetReturns: boolean;
  grossExpectedReturn?: number;
  netExpectedReturn?: number;
  analysisHorizonYears: number;
  minimumLiquidityCushion: number;
  riskProfile: RiskProfile;
  additionalCapitalToPreserve: number;
}

// ---- Request DTOs ----

export interface CreateScenarioRequest {
  name: string;
  sale: SaleDataDto;
  purchase: PurchaseDataDto;
  debtCapacity: DebtCapacityDataDto;
  banks: BankOfferDto[];
  strategyParameters: StrategyParametersDto;
}

export type UpdateScenarioRequest = CreateScenarioRequest;

export interface SaleLiquidityRequest {
  sale: SaleDataDto;
}

export interface PurchaseCostRequest {
  purchase: PurchaseDataDto;
  realAvailableCash: number;
}

export interface DebtCapacityRequest {
  debtCapacity: DebtCapacityDataDto;
}

export interface MortgageRequest {
  principal: number;
  annualTinPercentage: number;
  termYears: number;
  monthlyNetSalary: number;
  monthlyOutstandingLoanPayments: number;
}

export interface BankOfferEvaluationRequest {
  bankOffer: BankOfferDto;
  mortgagePrincipal: number;
  referenceTermYears: number;
  monthlyNetSalary: number;
  monthlyOutstandingLoanPayments: number;
}

export interface StrategyComparisonRequest {
  realAvailableCash: number;
  purchasePrice: number;
  maxMortgageAmount: number;
  totalPurchaseCostsExcludingEntry: number;
  mortgageTin: number;
  termYears: number;
  monthlyNetSalary: number;
  monthlyOutstandingLoanPayments: number;
  totalAcceptedBonusCost: number;
  strategyParameters: StrategyParametersDto;
}

// ---- Response DTOs ----

export interface ScenarioResponse {
  id: string;
  name: string;
  sale: SaleDataDto;
  purchase: PurchaseDataDto;
  debtCapacity: DebtCapacityDataDto;
  banks: BankOfferDto[];
  strategyParameters: StrategyParametersDto;
  lastResult?: AnalysisResultResponse;
  createdAt: string;
  updatedAt: string;
}

export interface ScenarioListItem {
  id: string;
  name: string;
  createdAt: string;
  updatedAt: string;
  hasResults: boolean;
}

export interface AnalysisResultResponse {
  saleLiquidity: SaleLiquidityResultDto;
  purchaseCosts: PurchaseCostResultDto;
  debtCapacity: DebtCapacityResultDto;
  bankResults: BankOfferResultDto[];
  strategyComparison?: StrategyComparisonResultDto;
  calculatedAt: string;
}

export interface SaleLiquidityResultDto {
  salePrice: number;
  saleRelatedCosts: number;
  outstandingMortgageDebt: number;
  municipalCapitalGainsTax: number;
  extraordinaryCosts: number;
  totalSaleDeductions: number;
  netSaleLiquidity: number;
  currentCashBalance: number;
  realAvailableCash: number;
}

export interface PurchaseCostResultDto {
  purchasePrice: number;
  deedPrice: number;
  financeablePercentage: number;
  itpAmount: number;
  maxMortgageAmount: number;
  entryPayment: number;
  notaryCosts: number;
  administrativeCosts: number;
  appraisalCosts: number;
  agencyCosts: number;
  otherCosts: number;
  totalCashNeeded: number;
  remainingLiquidity: number;
  isViable: boolean;
}

export interface DebtCapacityResultDto {
  maxMonthlyPaymentCapacity: number;
}

export interface MortgageResultDto {
  principal: number;
  tinPercentage: number;
  termYears: number;
  monthlyPayment: number;
  totalPaid: number;
  totalInterest: number;
  resultingDebtRatio: number;
  remainingMonthlyFreeCash: number;
}

export interface BonusEvaluationDto {
  bonusId: string;
  bonusName: string;
  interestSavings: number;
  totalBonusCost: number;
  netGainOrLoss: number;
  isWorthIt: boolean;
}

export interface BankOfferResultDto {
  bankId: string;
  bankName: string;
  baseTin: number;
  maxTheoreticalDiscountedTin: number;
  finalRealTin: number;
  mortgagesByTerm: MortgageResultDto[];
  totalAcceptedBonusCost: number;
  bonusEvaluations: BonusEvaluationDto[];
  realGlobalCost: number;
  recommendation: string;
}

export interface StrategyResultDto {
  entryPayment: number;
  mortgagePrincipal: number;
  monthlyPayment: number;
  totalInterest: number;
  totalBonusCost: number;
  remainingLiquidity: number;
  debtRatio: number;
  totalFinancialCost: number;
  futureValueConservative?: number;
  futureValueBase?: number;
  futureValueOptimistic?: number;
}

export interface StrategyComparisonResultDto {
  amortizeFaster: StrategyResultDto;
  maintainCapital: StrategyResultDto;
  netDifferenceConservative: number;
  netDifferenceBase: number;
  netDifferenceOptimistic: number;
  weightedNetDifference: number;
  recommendation: string;
  explanation: string;
}
