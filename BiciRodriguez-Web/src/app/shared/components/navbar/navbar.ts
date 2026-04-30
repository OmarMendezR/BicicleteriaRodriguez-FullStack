import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.scss']
})
export class NavbarComponent implements OnInit {
  rol: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    // Al cargar el componente, obtenemos el rol real
    this.rol = this.authService.getRol();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}