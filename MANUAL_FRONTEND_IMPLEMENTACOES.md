# Manual de Implementação Frontend

Este documento descreve a implementação frontend para os requisitos de login, registo, recuperação de senha, listagem de publicações, perfil, contas públicas/privadas, pesquisa e tratamento de comentários ofensivos.

---

## 1. Login

### 1.1 O que já existe
- `src/app/services/auth/auth.ts` contém métodos `login` e `register`.
- `src/app/modules/home/pages/login.component/login.component.ts` já monta um formulário de login.

### 1.2 Implementação recomendada
1. Manter `AuthService.login(dados)` apontando para a API.
2. Armazenar o token JWT no `localStorage` ou `sessionStorage`.
3. Criar um `AuthInterceptor` que anexe o token em todas as requisições `Api`.
4. Criar um serviço de estado de autenticação para verificar se o utilizador está logado.

### 1.3 Passo a passo
- `src/app/core/guards/auth/auth-guard.ts`: usar para proteger rotas privadas.
- `src/app/services/auth/auth.ts`: adicionar método `setToken(token: string)` e `getToken()`.
- `src/app/core/interceptors/auth.interceptor.ts`: criar interceptor HTTP.

### 1.4 Código sugerido
```ts
// auth.service.ts
login(dados: any): Observable<any> {
  return this.http.post(`${this.apiUrl}/auth/login`, dados);
}

setToken(token: string) {
  localStorage.setItem('nzola_token', token);
}

getToken() {
  return localStorage.getItem('nzola_token');
}
```

```ts
// auth.interceptor.ts
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '../../services/auth/auth';

@Injectable({ providedIn: 'root' })
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = this.authService.getToken();
    if (!token) return next.handle(req);

    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next.handle(cloned);
  }
}
```

```ts
// login.component.ts
this.authService.login(this.loginForm.value).subscribe((res: any) => {
  if (res?.token) {
    this.authService.setToken(res.token);
    // navegar para o feed
  }
});
```

---

## 2. Registo

### 2.1 O que já existe
- `auth.ts` possui `register(dados)`.
- `src/app/modules/auth/register/register.ts` já monta o formulário de registo.

### 2.2 Ajustes recomendados
1. Adicionar validação de senha/confirmar senha.
2. Enviar `Dados de registo` para a API backend.
3. Exibir mensagens de erro (email já existe, campo inválido, etc.).
4. Após registo bem-sucedido, redirecionar para login.

### 2.3 Código sugerido
```ts
this.registerForm = this.fb.group({
  nome: ['', Validators.required],
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, Validators.minLength(6)]],
  confirmPassword: ['', Validators.required],
});
```

```ts
onRegister() {
  if (this.registerForm.valid && this.passwordsMatch()) {
    this.authService.register(this.registerForm.value).subscribe({
      next: () => { /* navegar para login */ },
      error: err => { /* mostrar mensagem */ },
    });
  }
}
```

---

## 3. Esqueceu a senha / recuperação por email

### 3.1 Ponto importante
O frontend precisa de dois fluxos:
- pedido de recuperação (`email`)
- reset de senha com código/token recebido por email

### 3.2 O que criar
- `src/app/modules/auth/forgot-password/forgot-password.ts`
- `src/app/modules/auth/reset-password/reset-password.ts`
- Adicionar rotas com `auth.routes.ts`.

### 3.3 Métodos no `AuthService`
```ts
forgotPassword(email: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/auth/esqueci-senha`, { email });
}

resetPassword(dados: any): Observable<any> {
  return this.http.post(`${this.apiUrl}/auth/reset-senha`, dados);
}
```

### 3.4 Formulários
- `ForgotPassword`: campo `email`, botão `Enviar código`.
- `ResetPassword`: campos `email`, `codigo`, `novaSenha`, `confirmarSenha`.

### 3.5 UX
1. Usuário insere email e envia.
2. Backend envia código para email.
3. Usuário usa código na página de reset.
4. Se sucesso, redireciona para login.

---

## 4. Listar publicações recentes

### 4.1 O que já existe
- `PublicacaoService.obterRecentes()` retorna `PublicacaoDto[]`.
- `CartaoPublicacao` exibe lista de publicações.

### 4.2 Implementação recomendada
1. Criar um componente de feed ou usar `feed-module` existente.
2. No `ngOnInit`, chamar `publicacaoService.obterRecentes()`.
3. Mostrar loading e tratar erro.

### 4.3 Código sugerido
```ts
ngOnInit() {
  this.publicacaoService.obterRecentes().subscribe({
    next: posts => { this.listaPublicacoes = posts; },
    error: err => { console.error(err); },
  });
}
```

---

## 5. Mostrar perfil

### 5.1 O que já existe
- `src/app/modules/feed/pages/perfil.component/perfil.component.ts`
- `CartaoPerfil` já existe como componente reutilizável.

### 5.2 Implementação recomendada
1. Criar `PerfilService` ou estender `Api` para buscar perfil por ID/username.
2. Exibir dados de perfil em `PerfilComponent`.
3. Exibir as publicações do perfil, usando `CartaoPublicacao`.

### 5.3 Serviço sugerido
```ts
export class PerfilService {
  private endpoint = 'utilizadores';
  constructor(private api: Api) {}

  obterPerfilPorId(id: number) {
    return this.api.get<PerfilDto>(`${this.endpoint}/perfil/${id}`);
  }

  buscarPerfilPorUsername(username: string) {
    return this.api.get<PerfilDto>(`${this.endpoint}/perfil/usuario/${username}`);
  }
}
```

---

## 6. Publicações de contas públicas

### 6.1 Como implementar
1. Criar rota `/perfil/:usuarioId` ou `/usuario/:username`.
2. No componente de perfil, carregar perfil público e lista de publicações com requisição separada.
3. Se o perfil for público, mostrar as publicações diretamente.

### 6.2 Serviço sugerido
```ts
obterPublicacoesDoUsuario(usuarioId: number) {
  return this.api.get<PublicacaoDto[]>(`publicacoes/usuario/${usuarioId}`);
}
```

### 6.3 UI
- Mostrar botões `Seguir` / `Deixar de seguir`.
- Se perfil público, renderizar lista completa.

---

## 7. Contas privadas - restringir acesso às publicações apenas para seguidores

### 7.1 Ponto chave
O backend deve devolver informação de visibilidade e do relacionamento de seguimento.

### 7.2 Fluxo frontend
1. Buscar perfil e sua propriedade `privado`.
2. Buscar se o utilizador logado já segue esse perfil.
3. Se `privado === true` e `não segue`, mostrar apenas `informações do perfil` e mensagem `Conteúdo privado`.
4. Se segue, mostrar publicações.

### 7.3 Exemplo de lógica
```ts
if (perfil.privado && !perfil.segueAtualUsuario) {
  this.mensagemPrivado = 'Este perfil é privado. Siga para ver as publicações.';
  return;
}
this.carregarPublicacoesDoPerfil();
```

### 7.4 Serviços envolvidos
- `SeguidorService` para verificar se já segue e para iniciar seguimento.
- `PerfilService` para carregar estado de privacidade.

---

## 8. Pesquisa com filtro

### 8.1 Onde colocar
- Barra de pesquisa no feed ou painel principal.
- Componente de pesquisa no topo da página.

### 8.2 Campos de filtro
- palavras-chave
- tipo de publicação
- autor / username
- data

### 8.3 Requisição sugerida
```ts
pesquisarPublicacoes(term: string, tipo?: string) {
  return this.api.get<PublicacaoDto[]>(
    `publicacoes/pesquisar?termo=${encodeURIComponent(term)}${tipo ? `&tipo=${tipo}` : ''}`
  );
}
```

### 8.4 UI
- Input de texto
- Select de filtro de tipo
- Botão de buscar
- Mostra resultados em `CartaoPublicacao`

---

## 9. Tratamento de comentários ofensivos

### 9.1 Opção frontend
- Filtrar termos ofensivos antes de enviar.
- Exibir aviso ao utilizador ao tentar publicar comentário ofensivo.
- Permitir marcar um comentário como ofensivo e reportar ao backend.

### 9.2 Implementação recomendada
1. No componente de comentário, criar lista de palavras proibidas.
2. Antes de enviar, validar o texto do comentário.
3. Se houver palavra ofensiva, bloquear e mostrar mensagem.
4. Criar botão `Reportar` em comentários visíveis.

### 9.3 Exemplo de validação
```ts
const termosOfensivos = ['ofensa1', 'ofensa2', 'insulto'];
const texto = comentario.trim().toLowerCase();
const contemOfensivo = termosOfensivos.some(termo => texto.includes(termo));
if (contemOfensivo) {
  this.mensagemErro = 'Comentário contém palavras ofensivas.';
  return;
}
```

### 9.4 Reportar comentário
```ts
reportarComentario(comentarioId: number) {
  return this.api.post(`comentarios/${comentarioId}/reportar`, {});
}
```

---

## 10. Recomendações gerais de frontend

- Centralizar requisições HTTP no `Api` service.
- Usar `AuthInterceptor` para enviar token JWT automaticamente.
- Usar rotas `auth.routes.ts`/`feed-module.ts` para separar autenticação e feed.
- Manter DTOs em `src/app/dtos` e services em `src/app/services`.
- Criar componentes reutilizáveis em `src/app/shared/componentes`.

---

## 11. Arquivo sugerido para o manual
- `MANUAL_FRONTEND_IMPLEMENTACOES.md`

Este documento já descreve a implementação por requisito e é compatível com a arquitetura atual do projeto.
