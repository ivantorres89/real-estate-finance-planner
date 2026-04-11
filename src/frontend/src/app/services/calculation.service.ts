import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  SaleLiquidityRequest,
  SaleLiquidityResultDto,
  PurchaseCostRequest,
  PurchaseCostResultDto,
  DebtCapacityRequest,
  DebtCapacityResultDto,
  MortgageRequest,
  MortgageResultDto,
  BankOfferEvaluationRequest,
  BankOfferResultDto,
  StrategyComparisonRequest,
  StrategyComparisonResultDto,
} from '../models';

@Injectable({ providedIn: 'root' })
export class CalculationService {
  private readonly baseUrl = '/api/calculations';

  constructor(private readonly http: HttpClient) {}

  calculateSaleLiquidity(request: SaleLiquidityRequest): Observable<SaleLiquidityResultDto> {
    return this.http.post<SaleLiquidityResultDto>(`${this.baseUrl}/sale-liquidity`, request);
  }

  calculatePurchaseCosts(request: PurchaseCostRequest): Observable<PurchaseCostResultDto> {
    return this.http.post<PurchaseCostResultDto>(`${this.baseUrl}/purchase-costs`, request);
  }

  calculateDebtCapacity(request: DebtCapacityRequest): Observable<DebtCapacityResultDto> {
    return this.http.post<DebtCapacityResultDto>(`${this.baseUrl}/debt-capacity`, request);
  }

  calculateMortgage(request: MortgageRequest): Observable<MortgageResultDto> {
    return this.http.post<MortgageResultDto>(`${this.baseUrl}/mortgage`, request);
  }

  evaluateBankOffer(request: BankOfferEvaluationRequest): Observable<BankOfferResultDto> {
    return this.http.post<BankOfferResultDto>(`${this.baseUrl}/bank-offer`, request);
  }

  compareStrategies(request: StrategyComparisonRequest): Observable<StrategyComparisonResultDto> {
    return this.http.post<StrategyComparisonResultDto>(`${this.baseUrl}/strategy-comparison`, request);
  }
}
