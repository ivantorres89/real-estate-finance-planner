import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { StrategyParametersDto, RiskProfile } from '../../models';

@Component({
  selector: 'app-strategy-comparison',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatSelectModule,
    MatCheckboxModule,
  ],
  templateUrl: './strategy-comparison.html',
  styleUrl: './strategy-comparison.scss',
})
export class StrategyComparisonComponent {
  @Input() params!: StrategyParametersDto;

  readonly riskProfiles = Object.values(RiskProfile);
}
