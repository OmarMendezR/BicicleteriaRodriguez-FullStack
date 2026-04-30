import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'br-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);

  loginData = {
    email: '',
    password: ''
  };

  onLogin() {
    this.authService.login(this.loginData).subscribe({
      next: (res) => {
        // Al usar el pipe(tap) en el servicio, para este punto 
        // bici_token, bici_rol y bici_user ya están guardados.
        console.log('Sesión iniciada para:', res.nombre);
        this.router.navigate(['/home']); 
      },
      error: (err) => {
        alert('Error al iniciar sesión. Revisa tus credenciales.');
        console.error(err);
      }
    });
  }
}