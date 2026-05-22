import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html'
})
export class Register {
  private authService = inject(AuthService);
  private router = inject(Router);

  regData = {
    nombreCompleto: '',
    email: '',
    password: '',
    rolID: 2 // Por defecto Mecánico
  };

  mensajeError = '';

  onRegister() {
    this.authService.register(this.regData).subscribe({
      next: () => {
        alert('¡Usuario registrado con éxito! Ahora puedes iniciar sesión.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.mensajeError = err.error?.mensaje || 'Error al registrar usuario';
      }
    });
  }
}