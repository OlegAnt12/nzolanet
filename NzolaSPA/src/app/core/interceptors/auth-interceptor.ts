import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { RefreshTokenService } from '../../services/refresh-token/refresh-token.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);
  const isBrowser = isPlatformBrowser(platformId);

  if (!isBrowser) {
    return next(req);
  }

  const router = inject(Router);
  const refreshTokenService = inject(RefreshTokenService);

  const token = localStorage.getItem('token');
  const refreshToken = localStorage.getItem('refreshToken');

  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && refreshToken && !req.url.includes('refresh')) {
        return refreshTokenService.refresh(refreshToken).pipe(
          switchMap((res) => {
            if (isBrowser) {
              localStorage.setItem('token', res.token);
              localStorage.setItem('refreshToken', res.refreshToken);
            }
            const retryReq = req.clone({
              setHeaders: { Authorization: `Bearer ${res.token}` },
            });
            return next(retryReq);
          }),
          catchError(() => {
            if (isBrowser) {
              localStorage.clear();
            }
            router.navigate(['/home/login']);
            return throwError(() => error);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
