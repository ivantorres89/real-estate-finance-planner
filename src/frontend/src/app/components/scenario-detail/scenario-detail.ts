import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ScenarioService } from '../../services/scenario.service';
import {
  CreateScenarioRequest,
  ScenarioResponse,
  AnalysisResultResponse,
  BankOfferDto,
} from '../../models';
import { defaultScenarioRequest, defaultBankOffer } from '../../shared/defaults';
import { SaleDataComponent } from '../sale-data/sale-data';
import { PurchaseDataComponent } from '../purchase-data/purchase-data';
import { BankOfferComponent } from '../bank-offer/bank-offer';
import { StrategyComparisonComponent } from '../strategy-comparison/strategy-comparison';
import { AnalysisResultsComponent } from '../analysis-results/analysis-results';

@Component({
  selector: 'app-scenario-detail',
  imports: [
    CommonModule,
    FormsModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    SaleDataComponent,
    PurchaseDataComponent,
    BankOfferComponent,
    StrategyComparisonComponent,
    AnalysisResultsComponent,
  ],
  templateUrl: './scenario-detail.html',
  styleUrl: './scenario-detail.scss',
})
export class ScenarioDetailComponent implements OnInit {
  scenarioId: string | null = null;
  isNew = true;
  loading = true;
  saving = false;
  analyzing = false;

  model: CreateScenarioRequest = defaultScenarioRequest();
  analysisResult: AnalysisResultResponse | null = null;

  // i18n labels
  scenarioNamePlaceholder = $localize`:@@scenarioNamePlaceholder:Scenario name`;
  tabSaleLiquidityLabel = $localize`:@@tabSaleLiquidity:Sale & Liquidity`;
  tabPurchaseLabel = $localize`:@@tabPurchase:Purchase`;
  tabStrategyLabel = $localize`:@@tabStrategy:Strategy`;
  tabResultsLabel = $localize`:@@tabResults:Results`;

  get saveButtonText(): string {
    return this.saving ? $localize`:@@saving:Saving...` : $localize`:@@save:Save`;
  }

  get analyzeButtonText(): string {
    return this.analyzing ? $localize`:@@analyzing:Analyzing...` : $localize`:@@runAnalysis:Run Analysis`;
  }

  bankDefaultLabel(index: number): string {
    return $localize`:@@bankPrefix:Bank` + ' ' + (index + 1);
  }

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly location: Location,
    private readonly scenarioService: ScenarioService,
    private readonly snackBar: MatSnackBar,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.scenarioId = id;
      this.isNew = false;
      this.scenarioService.getById(id).subscribe({
        next: (scenario) => {
          this.loadFromResponse(scenario);
          this.cdr.markForCheck();
        },
        error: () => {
          this.snackBar.open($localize`:@@scenarioNotFound:Scenario not found`, $localize`:@@snackClose:Close`, { duration: 3000 });
          this.router.navigate(['/']);
        },
      });
    } else {
      this.loading = false;
    }
  }

  private loadFromResponse(scenario: ScenarioResponse): void {
    this.model = {
      name: scenario.name,
      sale: { ...scenario.sale },
      purchase: { ...scenario.purchase },
      debtCapacity: { ...scenario.debtCapacity },
      banks: scenario.banks.map((b) => ({
        ...b,
        bonuses: b.bonuses.map((bn) => ({ ...bn })),
        mortgageTermsYears: [...b.mortgageTermsYears],
      })),
      strategyParameters: { ...scenario.strategyParameters },
    };
    this.analysisResult = scenario.lastResult ?? null;
    this.loading = false;
  }

  save(): void {
    this.saving = true;
    const obs = this.isNew
      ? this.scenarioService.create(this.model)
      : this.scenarioService.update(this.scenarioId!, this.model);

    obs.subscribe({
      next: (response) => {
        this.saving = false;
        if (this.isNew) {
          this.scenarioId = response.id;
          this.isNew = false;
          this.location.replaceState(`/scenarios/${response.id}`);
        }
        this.snackBar.open($localize`:@@scenarioSaved:Scenario saved`, $localize`:@@snackClose:Close`, { duration: 2000 });
        this.cdr.markForCheck();
      },
      error: () => {
        this.saving = false;
        this.snackBar.open($localize`:@@errorSavingScenario:Error saving scenario`, $localize`:@@snackClose:Close`, { duration: 3000 });
        this.cdr.markForCheck();
      },
    });
  }

  runAnalysis(): void {
    if (!this.scenarioId) return;
    this.analyzing = true;
    this.scenarioService.runAnalysis(this.scenarioId).subscribe({
      next: (result) => {
        this.analysisResult = result;
        this.analyzing = false;
        this.snackBar.open($localize`:@@analysisComplete:Analysis complete`, $localize`:@@snackClose:Close`, { duration: 2000 });
        this.cdr.markForCheck();
      },
      error: () => {
        this.analyzing = false;
        this.snackBar.open($localize`:@@errorRunningAnalysis:Error running analysis`, $localize`:@@snackClose:Close`, { duration: 3000 });
        this.cdr.markForCheck();
      },
    });
  }

  addBank(): void {
    this.model.banks.push(defaultBankOffer());
  }

  removeBank(index: number): void {
    this.model.banks.splice(index, 1);
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  trackByIndex(index: number): number {
    return index;
  }
}
