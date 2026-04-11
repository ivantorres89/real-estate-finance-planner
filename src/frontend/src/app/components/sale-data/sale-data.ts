import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { SaleDataDto, DebtCapacityDataDto } from '../../models';

@Component({
  selector: 'app-sale-data',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
  ],
  templateUrl: './sale-data.html',
  styleUrl: './sale-data.scss',
})
export class SaleDataComponent {
  @Input() sale!: SaleDataDto;
  @Input() debtCapacity!: DebtCapacityDataDto;
}
