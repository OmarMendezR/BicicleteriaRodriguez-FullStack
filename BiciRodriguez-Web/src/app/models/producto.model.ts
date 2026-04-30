export interface Producto {
  productoId: number;     // Automático (Identity)
  nombre: string;
  precioCompra: number;
  precioVenta: number;
  stock: number;          // Mapea a StockActual en la DB
  stockMinimo: number;    // Nuevo
  categoriaId: number;
  proveedorId: number;
  nombreCategoria?: string;
  activo: boolean;
}

export const inicializarProducto = (): Producto => ({
  productoId: 0,
  nombre: '',
  precioCompra: 0,
  precioVenta: 0,
  stock: 0,
  stockMinimo: 5,         // Siguiendo el default de tu SQL
  categoriaId: 0,
  proveedorId: 0,
  activo: true
});