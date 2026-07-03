# Guia de Diagnóstico: UI do Perfil em Branco ou Lenta

## Sintomas
- A página de perfil abre sem conteúdo.
- A UI fica presa em carregamento.
- A navegação para o perfil funciona, mas o ecrã não mostra dados.
- O carregamento parece mais lento do que o feed normal.

## Causa Principal
O perfil dependia de dois fatores que, em conjunto, podiam bloquear a renderização:

1. Acesso direto a `localStorage` em caminhos executados durante SSR/hidratação.
2. O template chamava funções de transformação de imagem várias vezes por ciclo de change detection.

Quando o componente tentava ler `localStorage` no servidor, a renderização podia falhar. Mesmo quando a página carregava, chamadas repetidas como `base64Image(...)` no template aumentavam o custo do render.

## Como Identificar
Verifica estes sinais:

- O componente usa `localStorage` sem `isPlatformBrowser()`.
- A rota do perfil é renderizada no servidor em `app.routes.server.ts`.
- O template do perfil usa `*ngIf="dadosCarregados"` sem um estado alternativo de loading/erro.
- O template chama funções diretamente para converter imagens Base64.

## Solução Aplicada

### 1. Proteger o acesso ao browser
Todos os acessos a `localStorage` foram condicionados com `isPlatformBrowser()`.

Ficheiros afetados:
- `src/app/core/interceptors/auth-interceptor.ts`
- `src/app/services/auth/auth.ts`
- `src/app/modules/home/pages/login.component/login.component.ts`
- `src/app/modules/home/pages/registo.component/registo.component.ts`
- `src/app/modules/feed/pages/feed-principal.component/feed-principal.component.ts`
- `src/app/modules/feed/pages/perfil.component/perfil.component.ts`

### 2. Garantir renderização do perfil
O perfil passou a ter um estado explícito de loading/erro em vez de ficar silenciosamente vazio.

Ficheiros afetados:
- `src/app/modules/feed/pages/perfil.component/perfil.component.ts`
- `src/app/modules/feed/pages/perfil.component/perfil.component.html`
- `src/app/app.routes.server.ts`

### 3. Reduzir trabalho no render
As transformações de imagem Base64 passaram a usar o pipe puro `base64Image` em vez de chamadas diretas a funções no template.

Ficheiros afetados:
- `src/app/core/pipes/base64-image.pipe.ts`
- `src/app/modules/feed/components/mini-perfil/mini-perfil.ts`
- `src/app/modules/feed/components/mini-perfil/mini-perfil.html`
- `src/app/modules/feed/components/lista-notificacoes/lista-notificacoes.ts`
- `src/app/modules/feed/components/lista-notificacoes/lista-notificacoes.html`
- `src/app/modules/feed/pages/feed-principal.component/feed-principal.component.ts`
- `src/app/modules/feed/pages/feed-principal.component/feed-principal.component.html`
- `src/app/modules/feed/pages/pesquisa/pesquisa.component.ts`
- `src/app/modules/feed/pages/pesquisa/pesquisa.component.html`
- `src/app/modules/feed/pages/perfil.component/perfil.component.ts`
- `src/app/modules/feed/pages/perfil.component/perfil.component.html`

## Como Proceder Se Voltar a Acontecer

1. Confirmar se a rota está a correr em SSR ou no browser.
2. Procurar qualquer `localStorage` ou `window` usado fora de `isPlatformBrowser()`.
3. Procurar funções chamadas diretamente no template, sobretudo em `img [src]`, `*ngFor` e `*ngIf`.
4. Substituir lógica repetida por pipe puro ou por valor já preparado no componente.
5. Garantir que o template tem um estado visível de loading e um fallback de erro.

## Regra Prática
Se a página depende de dados do browser, o componente deve:

- verificar a plataforma antes de ler/escrever no storage;
- mostrar loading enquanto os dados não chegam;
- evitar funções pesadas chamadas diretamente no HTML.
