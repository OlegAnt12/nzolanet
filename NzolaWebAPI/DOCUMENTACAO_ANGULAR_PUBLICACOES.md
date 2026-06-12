# Documentação: Implementação em Angular - Publicações

## 1. Visão Geral

Este documento descreve como implementar gerenciamento de publicações na aplicação Angular, incluindo:
- Criar publicações com ficheiros
- Listar publicações recentes
- Editar publicações
- Remover publicações
- Gerir estado com RxJS

---

## 2. Estrutura do Projeto Angular

```
src/
  app/
    models/
      publicacao.model.ts
      conteudo-publicacao.model.ts
      ficheiro-conteudo.model.ts
    services/
      publicacao.service.ts
      ficheiro.service.ts
    components/
      publicacoes-lista/
        publicacoes-lista.component.ts
        publicacoes-lista.component.html
        publicacoes-lista.component.css
      criar-publicacao/
        criar-publicacao.component.ts
        criar-publicacao.component.html
        criar-publicacao.component.css
      editar-publicacao/
        editar-publicacao.component.ts
        editar-publicacao.component.html
        editar-publicacao.component.css
```

---

## 3. Modelos TypeScript

### 3.1 publicacao.model.ts

```typescript
export enum TipoConteudo {
  Texto = 'Texto',
  Imagem = 'Imagem',
  Video = 'Video',
  Documento = 'Documento'
}

export enum EstadoPublicacao {
  Ativa = 'Ativa',
  Inativa = 'Inativa',
  Arquivada = 'Arquivada'
}

export interface FicheiroConteudo {
  id: number;
  nomeOriginal: string;
  caminhoArmazenado: string;
  tipoMime: string;
  tamanhoBytes: number;
}

export interface ConteudoPublicacao {
  id: number;
  texto: string;
  tipoConteudo: TipoConteudo;
  ficheirosConteudo: FicheiroConteudo[];
}

export interface Utilizador {
  id: number;
  nomeCompleto: string;
  nomeUtilizador: string;
  fotoPerfil?: string;
}

export interface Publicacao {
  id: number;
  utilizadorId: number;
  utilizador?: Utilizador;
  conteudoPublicacao: ConteudoPublicacao;
  dataCriacao: Date;
  dataModificacao?: Date;
  estadoPublicacao: EstadoPublicacao;
}

export interface CriarPublicacaoRequest {
  utilizadorId: number;
  conteudo: {
    texto: string;
    tipoConteudo: TipoConteudo;
    ficheiros?: File[];
  };
}

export interface AtualizarPublicacaoRequest {
  texto: string;
  tipoConteudo: TipoConteudo;
}

export interface PaginacaoResponse<T> {
  items: T[];
  totalItens: number;
  totalPaginas: number;
  paginaAtual: number;
}
```

---

## 4. Serviço da API

### 4.1 publicacao.service.ts

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import {
  Publicacao,
  CriarPublicacaoRequest,
  AtualizarPublicacaoRequest,
  PaginacaoResponse
} from '../models/publicacao.model';

@Injectable({
  providedIn: 'root'
})
export class PublicacaoService {
  private apiUrl = 'http://localhost:5001/api/publicacoes';
  
  // BehaviorSubjects para gerenciar estado
  private publicacoesSubject = new BehaviorSubject<Publicacao[]>([]);
  public publicacoes$ = this.publicacoesSubject.asObservable();
  
  private publicacaoSelecionadaSubject = new BehaviorSubject<Publicacao | null>(null);
  public publicacaoSelecionada$ = this.publicacaoSelecionadaSubject.asObservable();
  
  private carregandoSubject = new BehaviorSubject<boolean>(false);
  public carregando$ = this.carregandoSubject.asObservable();
  
  private erroSubject = new BehaviorSubject<string | null>(null);
  public erro$ = this.erroSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Criar nova publicação com ficheiros
   */
  criarPublicacao(utilizadorId: number, request: CriarPublicacaoRequest): Observable<Publicacao> {
    this.carregandoSubject.next(true);
    this.erroSubject.next(null);

    const formData = new FormData();
    
    // Adicionar campos do conteúdo
    if (request.conteudo.texto) {
      formData.append('Texto', request.conteudo.texto);
    }
    formData.append('TipoConteudo', request.conteudo.tipoConteudo);
    
    // Adicionar ficheiros
    if (request.conteudo.ficheiros && request.conteudo.ficheiros.length > 0) {
      request.conteudo.ficheiros.forEach(file => {
        formData.append('Ficheiros', file, file.name);
      });
    }

    return this.http.post<Publicacao>(
      `${this.apiUrl}/${utilizadorId}`,
      formData
    ).pipe(
      tap(publicacao => {
        this.atualizarListaPublicacoes([publicacao, ...this.publicacoesSubject.value]);
        this.carregandoSubject.next(false);
      }),
      tap(publicacao => console.log('Publicação criada:', publicacao))
    );
  }

  /**
   * Obter publicações recentes com paginação
   */
  obterPublicacoesRecentes(pagina: number = 1, tamanho: number = 20): Observable<PaginacaoResponse<Publicacao>> {
    this.carregandoSubject.next(true);
    this.erroSubject.next(null);

    let params = new HttpParams()
      .set('pagina', pagina.toString())
      .set('tamanho', tamanho.toString());

    return this.http.get<PaginacaoResponse<Publicacao>>(
      `${this.apiUrl}/recentes`,
      { params }
    ).pipe(
      tap(response => {
        this.atualizarListaPublicacoes(response.items);
        this.carregandoSubject.next(false);
      }),
      tap(response => console.log('Publicações carregadas:', response))
    );
  }

  /**
   * Obter publicação por ID
   */
  obterPublicacao(id: number): Observable<Publicacao> {
    this.carregandoSubject.next(true);
    this.erroSubject.next(null);

    return this.http.get<Publicacao>(`${this.apiUrl}/${id}`).pipe(
      tap(publicacao => {
        this.publicacaoSelecionadaSubject.next(publicacao);
        this.carregandoSubject.next(false);
      })
    );
  }

  /**
   * Atualizar publicação
   */
  atualizarPublicacao(id: number, request: AtualizarPublicacaoRequest): Observable<Publicacao> {
    this.carregandoSubject.next(true);
    this.erroSubject.next(null);

    return this.http.put<Publicacao>(
      `${this.apiUrl}/${id}`,
      request
    ).pipe(
      tap(publicacaoAtualizada => {
        const publicacoes = this.publicacoesSubject.value.map(p =>
          p.id === id ? publicacaoAtualizada : p
        );
        this.atualizarListaPublicacoes(publicacoes);
        this.publicacaoSelecionadaSubject.next(publicacaoAtualizada);
        this.carregandoSubject.next(false);
      }),
      tap(publicacao => console.log('Publicação atualizada:', publicacao))
    );
  }

  /**
   * Remover publicação
   */
  removerPublicacao(id: number): Observable<void> {
    this.carregandoSubject.next(true);
    this.erroSubject.next(null);

    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        const publicacoes = this.publicacoesSubject.value.filter(p => p.id !== id);
        this.atualizarListaPublicacoes(publicacoes);
        if (this.publicacaoSelecionadaSubject.value?.id === id) {
          this.publicacaoSelecionadaSubject.next(null);
        }
        this.carregandoSubject.next(false);
      }),
      tap(() => console.log('Publicação removida'))
    );
  }

  /**
   * Obter publicações do utilizador
   */
  obterPublicacoesUtilizador(utilizadorId: number, pagina: number = 1): Observable<PaginacaoResponse<Publicacao>> {
    let params = new HttpParams()
      .set('utilizadorId', utilizadorId.toString())
      .set('pagina', pagina.toString());

    return this.http.get<PaginacaoResponse<Publicacao>>(
      `${this.apiUrl}/utilizador/${utilizadorId}`,
      { params }
    );
  }

  /**
   * Atualizar lista de publicações no estado
   */
  private atualizarListaPublicacoes(publicacoes: Publicacao[]): void {
    this.publicacoesSubject.next(publicacoes);
  }

  /**
   * Definir erro
   */
  setErro(mensagem: string): void {
    this.erroSubject.next(mensagem);
  }

  /**
   * Limpar erro
   */
  limparErro(): void {
    this.erroSubject.next(null);
  }
}
```

---

## 5. Componentes

### 5.1 Listar Publicações (publicacoes-lista.component.ts)

```typescript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PublicacaoService } from '../../services/publicacao.service';
import { Publicacao } from '../../models/publicacao.model';

@Component({
  selector: 'app-publicacoes-lista',
  templateUrl: './publicacoes-lista.component.html',
  styleUrls: ['./publicacoes-lista.component.css']
})
export class PublicacoesListaComponent implements OnInit, OnDestroy {
  publicacoes: Publicacao[] = [];
  carregando = false;
  erro: string | null = null;
  paginaAtual = 1;
  
  private destroy$ = new Subject<void>();

  constructor(private publicacaoService: PublicacaoService) {}

  ngOnInit(): void {
    this.carregarPublicacoes();
    
    // Subscrever ao estado
    this.publicacaoService.publicacoes$
      .pipe(takeUntil(this.destroy$))
      .subscribe(publicacoes => {
        this.publicacoes = publicacoes;
      });

    this.publicacaoService.carregando$
      .pipe(takeUntil(this.destroy$))
      .subscribe(carregando => {
        this.carregando = carregando;
      });

    this.publicacaoService.erro$
      .pipe(takeUntil(this.destroy$))
      .subscribe(erro => {
        this.erro = erro;
      });
  }

  carregarPublicacoes(): void {
    this.publicacaoService.obterPublicacoesRecentes(this.paginaAtual, 20)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          console.log('Publicações carregadas', response);
        },
        error: (erro) => {
          this.publicacaoService.setErro('Erro ao carregar publicações: ' + erro.message);
        }
      });
  }

  removerPublicacao(id: number): void {
    if (confirm('Tem certeza que deseja remover esta publicação?')) {
      this.publicacaoService.removerPublicacao(id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            console.log('Publicação removida com sucesso');
          },
          error: (erro) => {
            this.publicacaoService.setErro('Erro ao remover publicação: ' + erro.message);
          }
        });
    }
  }

  proximaPagina(): void {
    this.paginaAtual++;
    this.carregarPublicacoes();
  }

  paginaAnterior(): void {
    if (this.paginaAtual > 1) {
      this.paginaAtual--;
      this.carregarPublicacoes();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

### 5.2 Template Listar Publicações (publicacoes-lista.component.html)

```html
<div class="publicacoes-container">
  <!-- Mensagem de Erro -->
  <div *ngIf="erro" class="alert alert-danger">
    {{ erro }}
  </div>

  <!-- Carregando -->
  <div *ngIf="carregando" class="text-center">
    <div class="spinner-border" role="status">
      <span class="sr-only">Carregando...</span>
    </div>
  </div>

  <!-- Lista de Publicações -->
  <div *ngIf="!carregando && publicacoes.length > 0" class="publicacoes-lista">
    <div *ngFor="let pub of publicacoes" class="publicacao-card">
      <!-- Header -->
      <div class="publicacao-header">
        <img [src]="pub.utilizador?.fotoPerfil || 'assets/default-avatar.png'"
             alt="{{ pub.utilizador?.nomeUtilizador }}"
             class="avatar">
        <div class="info-utilizador">
          <h5>{{ pub.utilizador?.nomeCompleto }}</h5>
          <p class="username">@{{ pub.utilizador?.nomeUtilizador }}</p>
          <p class="data">{{ pub.dataCriacao | date: 'dd/MM/yyyy HH:mm' }}</p>
        </div>
        <div class="acoes">
          <button class="btn btn-sm btn-secondary" [routerLink]="['/publicacoes', pub.id, 'editar']">
            Editar
          </button>
          <button class="btn btn-sm btn-danger" (click)="removerPublicacao(pub.id)">
            Remover
          </button>
        </div>
      </div>

      <!-- Conteúdo -->
      <div class="publicacao-conteudo">
        <p *ngIf="pub.conteudoPublicacao?.texto">{{ pub.conteudoPublicacao.texto }}</p>

        <!-- Ficheiros -->
        <div *ngIf="pub.conteudoPublicacao?.ficheirosConteudo.length > 0" class="ficheiros">
          <ng-container *ngFor="let ficheiro of pub.conteudoPublicacao.ficheirosConteudo">
            <!-- Imagens -->
            <img *ngIf="ficheiro.tipoMime.startsWith('image/')"
                 [src]="ficheiro.caminhoArmazenado"
                 alt="{{ ficheiro.nomeOriginal }}"
                 class="ficheiro-imagem">

            <!-- Vídeos -->
            <video *ngIf="ficheiro.tipoMime.startsWith('video/')"
                   [src]="ficheiro.caminhoArmazenado"
                   controls
                   class="ficheiro-video">
            </video>

            <!-- Documentos -->
            <a *ngIf="ficheiro.tipoMime === 'application/pdf'"
               [href]="ficheiro.caminhoArmazenado"
               target="_blank"
               class="btn btn-link">
              📄 {{ ficheiro.nomeOriginal }}
            </a>
          </ng-container>
        </div>
      </div>

      <!-- Tipo de Conteúdo -->
      <div class="publicacao-tipo">
        <span class="badge" [ngClass]="{
          'badge-info': pub.conteudoPublicacao?.tipoConteudo === 'Texto',
          'badge-warning': pub.conteudoPublicacao?.tipoConteudo === 'Imagem',
          'badge-danger': pub.conteudoPublicacao?.tipoConteudo === 'Video',
          'badge-secondary': pub.conteudoPublicacao?.tipoConteudo === 'Documento'
        }">
          {{ pub.conteudoPublicacao?.tipoConteudo }}
        </span>
      </div>
    </div>
  </div>

  <!-- Lista Vazia -->
  <div *ngIf="!carregando && publicacoes.length === 0" class="text-center mt-5">
    <p class="text-muted">Nenhuma publicação encontrada</p>
  </div>

  <!-- Paginação -->
  <nav *ngIf="publicacoes.length > 0" aria-label="Paginação">
    <ul class="pagination justify-content-center">
      <li class="page-item" [class.disabled]="paginaAtual === 1">
        <button class="page-link" (click)="paginaAnterior()" [disabled]="paginaAtual === 1">
          Anterior
        </button>
      </li>
      <li class="page-item active">
        <span class="page-link">{{ paginaAtual }}</span>
      </li>
      <li class="page-item">
        <button class="page-link" (click)="proximaPagina()">
          Próxima
        </button>
      </li>
    </ul>
  </nav>
</div>
```

### 5.3 Criar Publicação (criar-publicacao.component.ts)

```typescript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PublicacaoService } from '../../services/publicacao.service';
import { TipoConteudo, CriarPublicacaoRequest } from '../../models/publicacao.model';

@Component({
  selector: 'app-criar-publicacao',
  templateUrl: './criar-publicacao.component.html',
  styleUrls: ['./criar-publicacao.component.css']
})
export class CriarPublicacaoComponent implements OnInit, OnDestroy {
  tiposConteudo = Object.values(TipoConteudo);
  
  texto = '';
  tipoConteudo: TipoConteudo = TipoConteudo.Texto;
  ficheiros: File[] = [];
  
  carregando = false;
  erro: string | null = null;
  
  private destroy$ = new Subject<void>();

  // Simular utilizador logado (em produção, obter do AuthService)
  utilizadorId = 1;

  constructor(
    private publicacaoService: PublicacaoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.publicacaoService.carregando$
      .pipe(takeUntil(this.destroy$))
      .subscribe(carregando => {
        this.carregando = carregando;
      });

    this.publicacaoService.erro$
      .pipe(takeUntil(this.destroy$))
      .subscribe(erro => {
        this.erro = erro;
      });
  }

  onFicheirosSelected(event: any): void {
    const files = event.target.files;
    if (files) {
      this.ficheiros = Array.from(files);
      console.log('Ficheiros selecionados:', this.ficheiros.length);
    }
  }

  removerFicheiro(index: number): void {
    this.ficheiros.splice(index, 1);
  }

  validarFormulario(): boolean {
    if (!this.texto.trim() && this.ficheiros.length === 0) {
      this.publicacaoService.setErro('Adicione texto ou ficheiros');
      return false;
    }

    if (this.texto.length > 5000) {
      this.publicacaoService.setErro('Texto não pode exceder 5000 caracteres');
      return false;
    }

    if (this.ficheiros.length > 10) {
      this.publicacaoService.setErro('Máximo de 10 ficheiros permitidos');
      return false;
    }

    // Validar tamanho individual
    const MAX_SIZE = 10 * 1024 * 1024; // 10 MB
    if (this.ficheiros.some(f => f.size > MAX_SIZE)) {
      this.publicacaoService.setErro('Ficheiros não podem exceder 10 MB');
      return false;
    }

    return true;
  }

  criar(): void {
    if (!this.validarFormulario()) {
      return;
    }

    const request: CriarPublicacaoRequest = {
      utilizadorId: this.utilizadorId,
      conteudo: {
        texto: this.texto,
        tipoConteudo: this.tipoConteudo,
        ficheiros: this.ficheiros.length > 0 ? this.ficheiros : undefined
      }
    };

    this.publicacaoService.criarPublicacao(this.utilizadorId, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (publicacao) => {
          console.log('Publicação criada com sucesso:', publicacao);
          this.router.navigate(['/publicacoes']);
        },
        error: (erro) => {
          this.publicacaoService.setErro('Erro ao criar publicação: ' + erro.error?.message || erro.message);
        }
      });
  }

  cancelar(): void {
    this.router.navigate(['/publicacoes']);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

### 5.4 Template Criar Publicação (criar-publicacao.component.html)

```html
<div class="criar-publicacao-container">
  <h2>Nova Publicação</h2>

  <!-- Mensagem de Erro -->
  <div *ngIf="erro" class="alert alert-danger alert-dismissible fade show">
    {{ erro }}
    <button type="button" class="btn-close" (click)="publicacaoService.limparErro()"></button>
  </div>

  <form (ngSubmit)="criar()">
    <!-- Texto -->
    <div class="mb-3">
      <label for="texto" class="form-label">Texto</label>
      <textarea class="form-control"
                id="texto"
                [(ngModel)]="texto"
                name="texto"
                placeholder="O que está em sua mente?"
                rows="5"
                maxlength="5000"></textarea>
      <small class="form-text text-muted">{{ texto.length }}/5000 caracteres</small>
    </div>

    <!-- Tipo de Conteúdo -->
    <div class="mb-3">
      <label for="tipoConteudo" class="form-label">Tipo de Conteúdo</label>
      <select class="form-control"
              id="tipoConteudo"
              [(ngModel)]="tipoConteudo"
              name="tipoConteudo">
        <option *ngFor="let tipo of tiposConteudo" [value]="tipo">
          {{ tipo }}
        </option>
      </select>
    </div>

    <!-- Ficheiros -->
    <div class="mb-3">
      <label for="ficheiros" class="form-label">Adicionar Ficheiros (Imagens/Vídeos/Documentos)</label>
      <div class="input-group">
        <input type="file"
               class="form-control"
               id="ficheiros"
               #fileInput
               (change)="onFicheirosSelected($event)"
               multiple
               accept="image/*,video/*,.pdf">
      </div>
      <small class="form-text text-muted">Máx 10 ficheiros, 10 MB cada</small>

      <!-- Ficheiros Selecionados -->
      <div *ngIf="ficheiros.length > 0" class="mt-3">
        <h5>Ficheiros Selecionados:</h5>
        <ul class="list-group">
          <li *ngFor="let ficheiro of ficheiros; let i = index" class="list-group-item d-flex justify-content-between">
            <span>{{ ficheiro.name }} ({{ (ficheiro.size / 1024 / 1024).toFixed(2) }} MB)</span>
            <button type="button" class="btn btn-sm btn-danger" (click)="removerFicheiro(i)">
              Remover
            </button>
          </li>
        </ul>
      </div>
    </div>

    <!-- Botões -->
    <div class="d-flex gap-2">
      <button type="submit" class="btn btn-primary" [disabled]="carregando">
        <span *ngIf="carregando" class="spinner-border spinner-border-sm me-2"></span>
        {{ carregando ? 'Publicando...' : 'Publicar' }}
      </button>
      <button type="button" class="btn btn-secondary" (click)="cancelar()">
        Cancelar
      </button>
    </div>
  </form>
</div>
```

### 5.5 Editar Publicação (criar-publicacao.component.ts com adaptações)

```typescript
// editar-publicacao.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PublicacaoService } from '../../services/publicacao.service';
import { Publicacao, AtualizarPublicacaoRequest, TipoConteudo } from '../../models/publicacao.model';

@Component({
  selector: 'app-editar-publicacao',
  templateUrl: './editar-publicacao.component.html',
  styleUrls: ['./editar-publicacao.component.css']
})
export class EditarPublicacaoComponent implements OnInit, OnDestroy {
  tiposConteudo = Object.values(TipoConteudo);
  
  publicacao: Publicacao | null = null;
  texto = '';
  tipoConteudo: TipoConteudo = TipoConteudo.Texto;
  
  carregando = false;
  erro: string | null = null;
  
  private destroy$ = new Subject<void>();
  private publicacaoId: number = 0;

  constructor(
    private publicacaoService: PublicacaoService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.publicacaoId = parseInt(params['id'], 10);
        this.carregarPublicacao();
      });
  }

  carregarPublicacao(): void {
    this.publicacaoService.obterPublicacao(this.publicacaoId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (publicacao) => {
          this.publicacao = publicacao;
          this.texto = publicacao.conteudoPublicacao?.texto || '';
          this.tipoConteudo = publicacao.conteudoPublicacao?.tipoConteudo || TipoConteudo.Texto;
        },
        error: (erro) => {
          this.erro = 'Erro ao carregar publicação: ' + erro.message;
        }
      });
  }

  atualizar(): void {
    if (!this.texto.trim()) {
      this.erro = 'Adicione texto à publicação';
      return;
    }

    const request: AtualizarPublicacaoRequest = {
      texto: this.texto,
      tipoConteudo: this.tipoConteudo
    };

    this.publicacaoService.atualizarPublicacao(this.publicacaoId, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.router.navigate(['/publicacoes']);
        },
        error: (erro) => {
          this.erro = 'Erro ao atualizar publicação: ' + erro.message;
        }
      });
  }

  cancelar(): void {
    this.router.navigate(['/publicacoes']);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

---

## 6. Módulo Angular

### app.module.ts

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { PublicacoesListaComponent } from './components/publicacoes-lista/publicacoes-lista.component';
import { CriarPublicacaoComponent } from './components/criar-publicacao/criar-publicacao.component';
import { EditarPublicacaoComponent } from './components/editar-publicacao/editar-publicacao.component';

@NgModule({
  declarations: [
    AppComponent,
    PublicacoesListaComponent,
    CriarPublicacaoComponent,
    EditarPublicacaoComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

---

## 7. Roteamento

### app-routing.module.ts

```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PublicacoesListaComponent } from './components/publicacoes-lista/publicacoes-lista.component';
import { CriarPublicacaoComponent } from './components/criar-publicacao/criar-publicacao.component';
import { EditarPublicacaoComponent } from './components/editar-publicacao/editar-publicacao.component';

const routes: Routes = [
  { path: 'publicacoes', component: PublicacoesListaComponent },
  { path: 'publicacoes/criar', component: CriarPublicacaoComponent },
  { path: 'publicacoes/:id/editar', component: EditarPublicacaoComponent },
  { path: '', redirectTo: '/publicacoes', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

---

## 8. CSS Estilos

### publicacoes-lista.component.css

```css
.publicacoes-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
}

.publicacoes-lista {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.publicacao-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 15px;
  background-color: #fff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.3s ease;
}

.publicacao-card:hover {
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.15);
}

.publicacao-header {
  display: flex;
  gap: 15px;
  margin-bottom: 15px;
  align-items: flex-start;
}

.avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  object-fit: cover;
}

.info-utilizador {
  flex: 1;
}

.info-utilizador h5 {
  margin: 0;
  font-weight: bold;
}

.username {
  margin: 3px 0;
  color: #666;
  font-size: 0.9rem;
}

.data {
  margin: 3px 0;
  color: #999;
  font-size: 0.8rem;
}

.acoes {
  display: flex;
  gap: 5px;
}

.publicacao-conteudo {
  margin-bottom: 15px;
}

.publicacao-conteudo p {
  margin: 0 0 15px 0;
  line-height: 1.5;
}

.ficheiros {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 10px;
}

.ficheiro-imagem,
.ficheiro-video {
  width: 100%;
  border-radius: 8px;
  max-height: 300px;
  object-fit: cover;
}

.publicacao-tipo {
  text-align: right;
}

.badge {
  padding: 5px 10px;
}
```

---

## 9. Integração com Bootstrap (opcional)

```html
<!-- index.html -->
<!doctype html>
<html lang="pt-PT">
<head>
  <meta charset="utf-8">
  <title>NzolaNet - Rede Social</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <!-- Bootstrap -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body>
  <app-root></app-root>
  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

---

## 10. Checklist de Implementação

- [ ] Criar modelos TypeScript (publicacao.model.ts)
- [ ] Criar serviço da API (publicacao.service.ts)
- [ ] Criar componente listar publicações
- [ ] Criar componente criar publicação
- [ ] Criar componente editar publicação
- [ ] Configurar roteamento (app-routing.module.ts)
- [ ] Adicionar Bootstrap CSS
- [ ] Testar criar publicação com ficheiros
- [ ] Testar listar publicações
- [ ] Testar editar publicação
- [ ] Testar remover publicação
- [ ] Tratamento de erros e validação

