import { loadTranslations } from '@angular/localize';
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

const locale = localStorage.getItem('locale') ?? 'en';
document.documentElement.lang = locale;

if (locale === 'es') {
  import('./locale/messages.es').then(({ translations }) => {
    loadTranslations(translations);
    bootstrap();
  });
} else {
  bootstrap();
}

function bootstrap(): void {
  bootstrapApplication(App, appConfig)
    .catch((err) => console.error(err));
}
