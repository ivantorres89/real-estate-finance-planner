import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'eur' })
export class EurPipe implements PipeTransform {
  transform(value: number | null | undefined): string {
    if (value == null) return '-';
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  }
}
