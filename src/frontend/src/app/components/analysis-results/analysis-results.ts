import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatChipsModule } from '@angular/material/chips';
import {
  AnalysisResultResponse,
  BankOfferResultDto,
  MortgageResultDto,
  BonusEvaluationDto,
} from '../../models';
import { EurPipe } from '../../shared/eur.pipe';

@Component({
  selector: 'app-analysis-results',
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatDividerModule,
    MatExpansionModule,
    MatChipsModule,
    EurPipe,
  ],
  templateUrl: './analysis-results.html',
  styleUrl: './analysis-results.scss',
})
export class AnalysisResultsComponent {
  @Input() result!: AnalysisResultResponse;

  mortgageCols = ['termYears', 'tinPercentage', 'monthlyPayment', 'totalPaid', 'totalInterest', 'debtRatio', 'freeCash'];
  bonusCols = ['bonusName', 'interestSavings', 'totalCost', 'netGain', 'worthIt'];
}
