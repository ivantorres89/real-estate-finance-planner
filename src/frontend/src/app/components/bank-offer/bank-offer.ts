import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { BankOfferDto, BonusDto, BonusCategory } from '../../models';
import { defaultBonus } from '../../shared/defaults';

@Component({
  selector: 'app-bank-offer',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatSelectModule,
    MatChipsModule,
    MatDividerModule,
  ],
  templateUrl: './bank-offer.html',
  styleUrl: './bank-offer.scss',
})
export class BankOfferComponent {
  @Input() bank!: BankOfferDto;

  readonly bonusCategories = Object.values(BonusCategory);
  termsInput = '';

  addBonus(): void {
    this.bank.bonuses.push(defaultBonus());
  }

  removeBonus(index: number): void {
    this.bank.bonuses.splice(index, 1);
  }

  get termsDisplay(): string {
    return this.bank.mortgageTermsYears.join(', ');
  }

  updateTerms(value: string): void {
    const parsed = value
      .split(',')
      .map((t) => parseInt(t.trim(), 10))
      .filter((n) => !isNaN(n) && n > 0 && n <= 50);
    if (parsed.length > 0) {
      this.bank.mortgageTermsYears = [...new Set(parsed)].sort((a, b) => a - b);
    }
  }

  trackByIndex(index: number): number {
    return index;
  }
}
