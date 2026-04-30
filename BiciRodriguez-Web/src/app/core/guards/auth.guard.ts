import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Verificamos si existe el token que definiste en tu servicio
  if (authService.getToken()) {
    return true;
  }

  // Si no hay token, lo mandamos al login
  router.navigate(['/login']);
  return false;
};