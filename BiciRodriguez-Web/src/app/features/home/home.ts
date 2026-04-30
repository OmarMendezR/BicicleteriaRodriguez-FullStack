import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrls: ['./home.scss']
})
export class HomeComponent implements OnInit {
  private authService = inject(AuthService);
  
  rol: string = '';
  nombreUsuario: string = '';

  ngOnInit(): void {
    const rawRole = this.authService.getRol(); // Esto leerá 'Administrador'
    
    // Normalizamos para que tus *ngIf="rol === 'Admin'" sigan funcionando
    if (rawRole === 'Administrador') {
      this.rol = 'Admin';
    } else if (rawRole === 'Mecanico') {
      this.rol = 'Mecanico';
    } else {
      this.rol = rawRole;
    }

    this.nombreUsuario = localStorage.getItem('bici_user') || 'Usuario';
  }
}