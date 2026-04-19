import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Proveedor } from '../../models/proveedor.model';

@Injectable({ providedIn: 'root' })
export class ProveedorService {
  private readonly API_URL = 'https://localhost:7046/api/Proveedores'; 

  constructor(private http: HttpClient) { }

  getAll(): Observable<Proveedor[]> {
    return this.http.get<Proveedor[]>(this.API_URL);
  }
}