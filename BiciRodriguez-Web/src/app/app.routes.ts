import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { ListaProductosComponent } from './features/inventario/lista-productos/lista-productos.component';
import { HomeComponent } from './features/home/home'; 
import { authGuard } from './core/guards/auth.guard';
import { Register } from './features/auth/register/register';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { 
    path: 'home', 
    component: HomeComponent, 
    canActivate: [authGuard] 
  },
  { 
    path: 'inventario', 
    component: ListaProductosComponent, 
    canActivate: [authGuard] 
  },
  // Alias para mantener compatibilidad con redirecciones previas
  { path: 'productos', redirectTo: '/inventario' } 
];