import { ErrorHandler, inject, Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { NGXLogger } from 'ngx-logger';
import Swal from 'sweetalert2';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private logger = inject(NGXLogger);

  handleError(error: any): void {
    const err = error.rejection || error;

    this.logger.error('Global error handler:', err);

    let title = 'Chyba aplikace';
    let message = 'Došlo k neočekávané chybě.';

    if (err instanceof HttpErrorResponse) {
      if (err.status === 401) {
        title = 'Neplatný klíč';
        message = 'Vaše přihlášení není platné (401).';
      } else if (err.status >= 500) {
        title = 'Chyba serveru';
        message = 'Server je momentálně nedostupný (500).';
      } else {
        message = `Chyba komunikace: ${err.statusText} (${err.status})`;
      }
    } else {
      message = err.message || message;
    }

    Swal.fire({
      title: title,
      text: message,
      icon: 'error',
      confirmButtonText: 'Zkusit znovu (Reload)',
      allowOutsideClick: false 
    }).then(() => {
      window.location.reload();
    });
  }
}