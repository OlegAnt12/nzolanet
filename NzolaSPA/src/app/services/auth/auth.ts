import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginDtos, LoginResponseDto } from '../../dtos/utilizador/auth/login/login.dtos';
import { RegistoRequestDto } from '../../dtos/utilizador/auth/registo/registo-request.dto';
import { Api } from '../api/api';
import { Router } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private endpoint = 'Autenticacoes';
  private isBrowser: boolean;

  constructor(private api: Api, private router: Router, @Inject(PLATFORM_ID) platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  login(dados: LoginDtos): Observable<LoginResponseDto> {
    return this.api.post<LoginResponseDto>(`${this.endpoint}/login`, dados);
  }

  registar(dados: RegistoRequestDto): Observable<any> {
    return this.api.post(`${this.endpoint}/registo`, dados);
  }

  logout(): void {
    const refreshToken = this.isBrowser ? localStorage.getItem('refreshToken') : null;
    if (refreshToken) {
      this.api.post(`${this.endpoint}/logout`, { refreshToken }).subscribe();
    }
    if (this.isBrowser) {
      localStorage.clear();
    }
    this.router.navigate(['/home/login']);
  }

  isAutenticado(): boolean {
    return this.isBrowser ? !!localStorage.getItem('token') : false;
  }

  esqueciPassword(email: string): Observable<any> {
    return this.api.post(`${this.endpoint}/esqueci-password`, { email });
  }

  redefinirPassword(token: string, novaPalavraPasse: string): Observable<any> {
    return this.api.post(`${this.endpoint}/redefinir-password`, { token, novaPalavraPasse });
  }
}
