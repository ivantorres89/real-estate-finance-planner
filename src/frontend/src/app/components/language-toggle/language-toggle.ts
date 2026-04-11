import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-language-toggle',
  imports: [MatButtonModule, MatIconModule, MatMenuModule],
  template: `
    <button mat-icon-button [matMenuTriggerFor]="langMenu" aria-label="Change language">
      <mat-icon>language</mat-icon>
    </button>
    <mat-menu #langMenu="matMenu">
      <button mat-menu-item (click)="setLocale('en')" [class.active-lang]="currentLocale === 'en'">
        English
      </button>
      <button mat-menu-item (click)="setLocale('es')" [class.active-lang]="currentLocale === 'es'">
        Espa&#241;ol
      </button>
    </mat-menu>
  `,
  styles: `
    :host { display: inline-flex; }
    .active-lang { font-weight: 500; background: var(--mat-sys-surface-variant); }
  `,
})
export class LanguageToggleComponent {
  currentLocale = localStorage.getItem('locale') ?? 'en';

  setLocale(locale: string): void {
    if (locale === this.currentLocale) return;
    localStorage.setItem('locale', locale);
    location.reload();
  }
}
