import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginDtos, LoginResponseDto } from '../../dtos/utilizador/auth/login/login.dtos';
import { RegistoRequestDto } from '../../dtos/utilizador/auth/registo/registo-request.dto';
import { Api } from '../api/api';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private endpoint = 'autenticacoes'; 

  constructor(private api: Api) {}

  
  login(dados: LoginDtos): Observable<LoginResponseDto> {
    return this.api.post<LoginResponseDto>(`${this.endpoint}/login`, dados);
  }

  registar(dados: RegistoRequestDto): Observable<any> {
    return this.api.post(`${this.endpoint}/registo`, dados);
  }
}
