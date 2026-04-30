import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginDto, AuthResponse } from '../../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly URL = 'http://localhost:5062/api/auth';

  login(dto: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.URL}/login`, dto).pipe(
      tap(res => {
        localStorage.setItem('bici_token', res.token);
        localStorage.setItem('bici_rol', res.role);   // Guardamos "Administrador"
        localStorage.setItem('bici_user', res.nombre); // Guardamos "Omar" (por ejemplo)
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem('bici_token');
  }

  // NUEVO: Método para obtener el rol fácilmente
  getRol(): string {
    return localStorage.getItem('bici_rol') || '';
  }

  logout() {
    localStorage.removeItem('bici_token');
    localStorage.removeItem('bici_rol');
    localStorage.removeItem('bici_user');
  }
}