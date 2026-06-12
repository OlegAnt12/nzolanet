import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginDtos, LoginResponseDto } from '../../dtos/utilizador/auth/login/login.dtos';
import { RegistoRequestDto } from '../../dtos/utilizador/auth/registo/registo-request.dto';
import { Api } from '../api/api';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private endpoint = 'Autenticacoes'; 

  constructor(private api: Api, private router: Router) {}
  
  login(dados: LoginDtos): Observable<LoginResponseDto> {
    return this.api.post<LoginResponseDto>(`${this.endpoint}/login`, dados);
  }

  registar(dados: RegistoRequestDto): Observable<any> {
    return this.api.post(`${this.endpoint}/registo`, dados);
  }

  logout(): void {
    // 1. Remove os dados específicos da sessão guardados no login
    localStorage.removeItem('token');
    localStorage.removeItem('utilizadorId');
    localStorage.removeItem('nomeUtilizador');

    // Dica: Se quiseres apagar absolutamente tudo o que está guardado, podes usar:
    // localStorage.clear();

    // 2. Redireciona o utilizador imediatamente para o ecrã de login
    this.router.navigate(['/home/login']);
  }

  // Método auxiliar para verificar se o utilizador está autenticado (útil para os Guards de rotas)
  isAutenticado(): boolean {
    return !!localStorage.getItem('token'); // Retorna true se houver um token
  }
}
