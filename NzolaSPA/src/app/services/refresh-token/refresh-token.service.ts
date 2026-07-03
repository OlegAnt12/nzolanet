import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class RefreshTokenService {
  private baseUrl = 'http://localhost:5043/api';

  constructor(private http: HttpClient) {}

  refresh(refreshToken: string): Observable<{ token: string; refreshToken: string }> {
    return this.http.post<{ token: string; refreshToken: string }>(
      `${this.baseUrl}/Autenticacoes/refresh`,
      { refreshToken }
    );
  }
}
