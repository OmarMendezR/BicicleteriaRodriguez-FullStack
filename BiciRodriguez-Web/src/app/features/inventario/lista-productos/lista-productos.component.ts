import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductoService } from '../../../core/services/producto.service';
import { CategoriaService } from '../../../core/services/categoria.service';
import { Producto, inicializarProducto } from '../../../models/producto.model';
import { Categoria } from '../../../models/categoria.model';
import { ProveedorService } from '../../../core/services/proveedor.service';
import { Proveedor } from '../../../models/proveedor.model';

@Component({
  selector: 'br-lista-productos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lista-productos.component.html'
})
export class ListaProductosComponent implements OnInit {
  public productos: Producto[] = [];
  public categorias: Categoria[] = [];
  public proveedores: Proveedor[] = [];
  public productoSeleccionado: Producto = inicializarProducto();

  constructor(
    private productoService: ProductoService,
    private categoriaService: CategoriaService,
    private proveedorService: ProveedorService
  ) {}

  ngOnInit(): void {
    this.cargarProductos();
    this.cargarCategorias();
    this.cargarProveedores();
  }

  cargarProductos(): void {
    this.productoService.getAll().subscribe({
      next: (data) => this.productos = data,
      error: (err) => console.error('Error al cargar productos:', err)
    });
  }

  cargarCategorias(): void {
    this.categoriaService.getAll().subscribe({
      next: (data) => this.categorias = data,
      error: (err) => console.error('Error al cargar categorías:', err)
    });
  }

  cargarProveedores(): void {
    this.proveedorService.getAll().subscribe({
      next: (data) => this.proveedores = data,
      error: (err) => console.error('Error al cargar proveedores:', err)
    });
  }

onGuardar(): void {
  // En lugar de guardar el observable en una variable, ejecutamos la lógica directamente
  if (this.productoSeleccionado.productoId === 0) {
    // Lógica para Crear
    this.productoService.create(this.productoSeleccionado).subscribe({
      next: () => {
        alert('Producto creado con éxito');
        this.refrescar();
      },
      error: (err: any) => this.manejarError(err)
    });
  } else {
    // Lógica para Actualizar
    this.productoService.update(this.productoSeleccionado.productoId, this.productoSeleccionado).subscribe({
      next: () => {
        alert('Producto actualizado con éxito');
        this.refrescar();
      },
      error: (err: any) => this.manejarError(err)
    });
  }
}

// Creamos un método aparte para manejar el error y evitar el error 7006 (any implícito)
private manejarError(err: any): void {
  console.error('Error en la operación:', err);
  const msg = err.error?.mensaje || 'Error inesperado en el servidor';
  alert(msg);
}

  onEliminar(id: number): void {
    if (confirm('¿Desea desactivar este producto del inventario?')) {
      this.productoService.delete(id).subscribe(() => this.cargarProductos());
    }
  }

  private refrescar(): void {
    this.cargarProductos();
    this.productoSeleccionado = inicializarProducto();
  }

  prepararEdicion(p: Producto): void { this.productoSeleccionado = { ...p }; }
  prepararNuevo(): void { this.productoSeleccionado = inicializarProducto(); }
}