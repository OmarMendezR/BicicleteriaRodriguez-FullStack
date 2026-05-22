export interface Producto {
  productoId: number;     
  nombre: string;
  precioCompra: number;
  precioVenta: number;
  stock: number;          // Mapea a StockActual en la DB
  stockMinimo: number;    
  categoriaId: number | null;  // Permite null para productos sin categoría
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
  categoriaId: null,
  proveedorId: 0,
  activo: true
});