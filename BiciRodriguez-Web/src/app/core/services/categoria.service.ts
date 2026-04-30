import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Categoria } from '../../models/categoria.model';

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private readonly API_URL = 'http://localhost:5062/api/Categorias'; // Verifica que este sea el endpoint en tu Swagger

  constructor(private http: HttpClient) { }

  getAll(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(this.API_URL);
  }
}