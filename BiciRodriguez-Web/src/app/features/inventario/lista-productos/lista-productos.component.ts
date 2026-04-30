import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductoService } from '../../../core/services/producto.service';
import { CategoriaService } from '../../../core/services/categoria.service';
import { ProveedorService } from '../../../core/services/proveedor.service';
import { Producto, inicializarProducto } from '../../../models/producto.model';
import { Categoria } from '../../../models/categoria.model';
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
    this.cargarDatosIniciales();
  }

  /**
   * Carga todas las dependencias necesarias para el componente
   */
  private cargarDatosIniciales(): void {
    this.cargarProductos();
    this.cargarCategorias();
    this.cargarProveedores();
  }

  cargarProductos(): void {
    this.productoService.getAll().subscribe({
      next: (data) => (this.productos = data),
      error: (err) => this.manejarError('Error al cargar productos', err)
    });
  }

  cargarCategorias(): void {
    this.categoriaService.getAll().subscribe({
      next: (data) => (this.categorias = data),
      error: (err) => this.manejarError('Error al cargar categorías', err)
    });
  }

  cargarProveedores(): void {
    this.proveedorService.getAll().subscribe({
      next: (data) => (this.proveedores = data),
      error: (err) => this.manejarError('Error al cargar proveedores', err)
    });
  }

  /**
   * Procesa la persistencia del producto (Creación o Actualización)
   */
  onGuardar(): void {
    // Aplicamos Inmutabilidad para no afectar el formulario durante el envío
    const payload = this.prepararPayload(this.productoSeleccionado);

    if (payload.productoId === 0) {
      this.crearProducto(payload);
    } else {
      this.actualizarProducto(payload);
    }
  }

  /**
   * Limpia el objeto antes de enviarlo al servidor para evitar conflictos de integridad
   */
  private prepararPayload(producto: Producto): Producto {
    const copia = { ...producto };

    // Corrección del conflicto de Foreign Key:
    // Si el proveedorId es 0 o inexistente, se envía como null para que el backend 
    // (con el DTO actualizado a int?) lo procese correctamente.
    if (!copia.proveedorId || copia.proveedorId === 0) {
      copia.proveedorId = 0;
    }

    return copia;
  }

  private crearProducto(producto: Producto): void {
    this.productoService.create(producto).subscribe({
      next: () => this.finalizarOperacion('Producto creado con éxito'),
      error: (err) => this.manejarError('Error al crear producto', err)
    });
  }

  private actualizarProducto(producto: Producto): void {
    this.productoService.update(producto.productoId, producto).subscribe({
      next: () => this.finalizarOperacion('Producto actualizado con éxito'),
      error: (err) => this.manejarError('Error al actualizar producto', err)
    });
  }

  onEliminar(id: number): void {
    if (confirm('¿Desea desactivar este producto del inventario?')) {
      this.productoService.delete(id).subscribe({
        next: () => this.cargarProductos(),
        error: (err) => this.manejarError('Error al eliminar producto', err)
      });
    }
  }

  prepararEdicion(producto: Producto): void {
    // Usamos spread operator para evitar vinculación bidireccional inmediata en la lista
    this.productoSeleccionado = { ...producto };
  }

  prepararNuevo(): void {
    this.productoSeleccionado = inicializarProducto();
  }

  private finalizarOperacion(mensaje: string): void {
    alert(mensaje);
    this.refrescar();
  }

  private refrescar(): void {
    this.cargarProductos();
    this.productoSeleccionado = inicializarProducto();
  }

  private manejarError(contexto: string, err: any): void {
    console.error(`${contexto}:`, err);
    const mensajeServidor = err.error?.mensaje || 'Error inesperado en el servidor';
    alert(`${contexto}: ${mensajeServidor}`);
  }
}