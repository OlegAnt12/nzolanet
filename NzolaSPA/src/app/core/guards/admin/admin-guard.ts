import { CanMatchFn, Router, UrlTree } from '@angular/router';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export const adminGuard: CanMatchFn = (): boolean | UrlTree => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  if (!isPlatformBrowser(platformId)) {
    return router.parseUrl('/home/login');
  }

  const stored = localStorage.getItem('utilizadorLogado');
  if (!stored) {
    return router.parseUrl('/home/login');
  }

  try {
    const utilizador = JSON.parse(stored);
    if (Number(utilizador?.nivelAcesso) === 1 || utilizador?.nivelAcesso === 'Admin') {
      return true;
    }
  } catch {
    // segue para login abaixo
  }

  return router.parseUrl('/feed');
};
