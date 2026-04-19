import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ListaProductosComponent } from './features/inventario/lista-productos/lista-productos.component';

@Component({
  selector: 'br-root',
  imports: [RouterOutlet, ListaProductosComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('BiciRodriguez-Web');
}
