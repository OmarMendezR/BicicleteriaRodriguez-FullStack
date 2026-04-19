import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Producto } from '../../models/producto.model';

@Injectable({ providedIn: 'root' })
export class ProductoService {
  private readonly API_URL = 'https://localhost:7046/api/Productos';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Producto[]> {
    return this.http.get<Producto[]>(this.API_URL);
  }

  create(producto: Producto): Observable<Producto> {
    return this.http.post<Producto>(this.API_URL, producto);
  }

  update(id: number, producto: Producto): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}`, producto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}