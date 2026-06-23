import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Api {
  private baseUrl = 'http://localhost:5043/api';

  constructor(private http: HttpClient) {}

  // GET
  get<T>(endpoint: string): Observable<T> {
    endpoint = endpoint.replace(/^\/+|\/+$/g, '');
    
    const headers = new HttpHeaders({
      'accept': '*/*'
    });
    
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`);
  }

  // GET BY ID
  getById<T>(endpoint: string, id: number | string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${endpoint}/${id}`);
  }

  // POST
  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, data);
  }

  // PUT
  put<T>(endpoint: string, id: number | string, data: any): Observable<T> {
    return this.http.put<T>(
      `${this.baseUrl}/${endpoint}/${id}`,
      data
    );
  }

  // DELETE
  delete<T>(endpoint: string, id: number | string): Observable<T> {
    return this.http.delete<T>(
      `${this.baseUrl}/${endpoint}/${id}`
    );
  }
}
