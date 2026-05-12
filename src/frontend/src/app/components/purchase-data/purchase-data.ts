import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { PurchaseDataDto } from '../../models';

@Component({
  selector: 'app-purchase-data',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatCheckboxModule,
  ],
  templateUrl: './purchase-data.html',
  styleUrl: './purchase-data.scss',
})
export class PurchaseDataComponent {
  @Input() purchase!: PurchaseDataDto;

  totalPurchasePrice(): number {
    return (Number(this.purchase?.officialPurchasePriceA) || 0)
      + (Number(this.purchase?.unofficialPurchasePriceB) || 0);
  }

  appraisalCapsMortgage(): boolean {
    const a = Number(this.purchase?.officialPurchasePriceA) || 0;
    const appraisal = Number(this.purchase?.appraisalValue) || 0;
    return appraisal > 0 && appraisal < a;
  }
}
