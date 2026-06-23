# Nzolanet

Projeto de rede social para comunicação, partilha de informação e interação entre utilizadores.
O sistema contempla autenticação e gestão de utilizadores, publicação de conteúdo, comentários, bazes, feed e notificações.

## Visão Geral

- Backend: **.NET 8 Web API**
- Frontend: **Angular 21 (SPA / SSR)**
- Base de dados: **SQL Server**
- Solução: `NZOLANET.sln`

## Estrutura do Repositório

- `NzolaWebAPI/`
  - Projeto backend em C# com Web API e Entity Framework Core
- `NzolaSPA/`
  - Aplicação frontend em Angular, com suporte a Server-Side Rendering (SSR)
- `Docs/`
  - Documentação de arquitetura e requisitos funcionais
- `README.md`
  - Este documento de visão geral do projeto

## Backend: NzolaWebAPI

A API é organizada com separação de responsabilidades e camadas clássicas:

- `Controllers/`
  - Exposição dos endpoints HTTP para cada recurso do sistema
- `Services/`
  - Lógica de domínio e orquestração de regras de negócio
- `Repositories/`
  - Acesso aos dados e consultas ao contexto EF
- `Data/`
  - Configuração do contexto de banco de dados (`ContextoBDNzola`)
- `Models/`
  - Entidades do domínio que representam tabelas no banco de dados
- `Dtos/`
  - Objetos de transferência de dados para requests e responses
- `Mappers/`
  - Conversão entre `Models` e `Dtos`
- `Interfaces/`
  - Contratos para serviços e repositórios
- `Configurations/`
  - Configurações especiais, como envio de e-mail
- `Migrations/`
  - Histórico de migrações do Entity Framework para manter o esquema do banco de dados

### Recursos principais do backend

Controladores implementados:

- `BazesController` — gestão de "bazes"
- `ComentariosController` — comentários em publicações
- `ConteudosPublicacaoController` — conteúdos associados a publicações
- `EmailController` — envio de mensagens / notificações por email
- `NotificacaoController` — notificações internas do sistema
- `PublicacoesController` — publicação de posts e atualizações
- `SeguidorController` — seguir e deixar de seguir utilizadores
- `UtilizadoresController` — gestão de utilizadores e perfis

### Configuração principal

- `Program.cs` define:
  - criação do `WebApplication`
  - injeção de dependências
  - uso de `DbContext` com SQL Server
  - registro de `Swagger` para documentação da API
  - configuração de serviço de e-mail

## Frontend: NzolaSPA

A aplicação Angular está estruturada como um SPA com rotas carregadas via lazy loading.

- `src/app/`
  - pasta principal da aplicação Angular
- `src/app/modules/`
  - módulos funcionais carregados por rota
  - `home/`, `feed/`, `admin/`, `naosituado/`
- `src/app/core/`
  - serviços centrais, guardas, configurações globais e infraestrutura de aplicação
- `src/app/shared/`
  - componentes e funcionalidades reutilizáveis
- `src/app/dtos/`
  - definições de tipos e interfaces para dados trocados com a API
- `src/app/services/`
  - serviços responsáveis pela comunicação com o backend e lógica de UI

### Rotas principais

O arquivo `src/app/app.routes.ts` define as rotas:

- `/home` — módulo da página inicial
- `/feed` — módulo do feed de publicações
- `/admin` — módulo administrativo
- `**` — rota para página de não encontrado

### Dependências relevantes

- `@angular/*` — framework Angular 21
- `@angular/ssr` — suporte a renderização no servidor
- `@fortawesome/*` — ícones FontAwesome
- `express` — servidor Node para SSR
- `rxjs` — reatividade
- `vitest` — testes unitários

## Como Executar o Projeto

### Backend

1. Abrir a solução `NZOLANET.sln`
2. Restaurar pacotes e construir o projeto:
   - `dotnet restore`
   - `dotnet build`
3. Executar a API:
   - `dotnet run --project NzolaWebAPI\NzolaWebAPI.csproj`
4. Verificar o Swagger em modo de desenvolvimento (se ativo):
   - `https://localhost:<porta>/swagger`

### Frontend

1. Abrir `NzolaSPA` no terminal
2. Instalar dependências:
   - `npm install`
3. Iniciar o servidor de desenvolvimento:
   - `npm start`
4. Acessar a aplicação no navegador:
   - `http://localhost:4200`

### SSR (quando disponível)

- `npm run serve:ssr:NzolaSPA`

## Observações

- O backend usa SQL Server como provedor de dados via Entity Framework Core.
- A arquitetura está preparada para evolução com camadas claras e separação entre API e UI.
- O frontend utiliza lazy loading para os módulos principais, melhorando o desempenho inicial.

## Equipa Técnica

- Eduarda Malungo
- Femiel Pedro
- Holeg António
- Paulo Afonso

