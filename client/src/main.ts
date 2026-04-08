import { bootstrapApplication } from '@angular/platform-browser';
import { appConfigDefault } from './app/app.config';
import { App } from './app/app';
import { registerLocaleData } from '@angular/common';
import localeEn from '@angular/common/locales/en';

const config = appConfigDefault;

registerLocaleData(localeEn);

bootstrapApplication(App, config)
  .catch((err) => console.error(err));
