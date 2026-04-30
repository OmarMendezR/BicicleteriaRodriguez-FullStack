export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  role: string;   // .NET enviará 'role'
  nombre: string; // .NET enviará 'nombre'
}