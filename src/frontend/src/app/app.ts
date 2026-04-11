import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LanguageToggleComponent } from './components/language-toggle/language-toggle';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LanguageToggleComponent],
  template: `
    <div class="lang-toggle-wrapper">
      <app-language-toggle />
    </div>
    <router-outlet />
  `,
  styles: `
    .lang-toggle-wrapper {
      position: fixed;
      top: 8px;
      right: 16px;
      z-index: 1000;
    }
  `,
})
export class App {}
