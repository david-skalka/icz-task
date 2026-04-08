import { APP_INITIALIZER, ApplicationConfig, ErrorHandler, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, Router } from '@angular/router';
import { routes } from './app.routes';
import { ApiModule, Configuration } from './api-client';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';

import { GlobalErrorHandler } from './global-error-handler';
import AccountService from './services/account.service';

import { LOGIN_BEHAVIOR, LoginBehavior } from './pages/login/login.page';
import { LOCALE_ID } from '@angular/core';

export function createBaseAppConfig(basePath: string): ApplicationConfig {
  return {
    providers: [
      { provide: ErrorHandler, useClass: GlobalErrorHandler },
      provideBrowserGlobalErrorListeners(),
      provideZoneChangeDetection({ eventCoalescing: true }),
      provideRouter(routes),
      { provide: LOCALE_ID, useValue: 'en' },
      provideHttpClient(),
      importProvidersFrom(
        ApiModule.forRoot(() => new Configuration({
          basePath,
          credentials: {
            Bearer: () => localStorage.getItem('jwt') || '',
          },
          withCredentials: true
        }))
      ),
      {
        provide: APP_INITIALIZER,
        useFactory: (accountService: AccountService, router: Router) => async () => {
          try {
            await accountService.identity();
          } catch (error) {
            if (error instanceof HttpErrorResponse && error.status === 401) {
              router.navigate(['/login']);
            } else {
              throw error;
            }
          }
        },
        deps: [AccountService, Router],
        multi: true
      },
      importProvidersFrom(
        LoggerModule.forRoot(
          {
            serverLoggingUrl: undefined,
            level: NgxLoggerLevel.TRACE,
            serverLogLevel: NgxLoggerLevel.OFF,
          }
          
        )
      ),
      {
        provide: AccountService,
        useFactory: () => {
          const retD = new AccountService();
          return retD;
        },
        deps: []
      }, { provide: LOGIN_BEHAVIOR, useValue: {
        autoLogins: []
      } as LoginBehavior }
    ]
  };
}


export const appConfigDefault = createBaseAppConfig('');

