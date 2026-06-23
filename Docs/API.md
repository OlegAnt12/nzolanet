# API NzolaWebAPI — Documentação

Resumo da API e endpoints expostos pelo projeto NzolaWebAPI.

## Visão Geral
- Base route: `/api/{controller}` onde aplicável.
- Formatos: JSON para a maioria dos endpoints; `multipart/form-data` para upload de publicações.

## Controladores e Endpoints

**AutenticacoesController**: [NzolaWebAPI/Controllers/AutenticacoesController.cs](NzolaWebAPI/Controllers/AutenticacoesController.cs)
- Base route: `api/autenticacoes`
- POST `registar` — Registar novo utilizador
  - Request: `CriarUtilizadorRequestDto` (body)
  - Response: `UtilizadorDto` / `IActionResult`
- POST `login` — Autenticar e obter token
  - Request: `LoginDto` (body)
  - Response: `UtilizadorDto` + token (via `ITokenService`)

**BazesController**: [NzolaWebAPI/Controllers/BazesController.cs](NzolaWebAPI/Controllers/BazesController.cs)
- Base route: `api/bazes`
- GET `/{id}` — Obter uma baze por id
  - Params: `id` (route)
  - Response: `BazeDto`
- GET `/publicacao/{id}` — Bazes de uma publicação
  - Params: `id` (route)
  - Response: `IEnumerable<BazeDto>`
- POST `/{publicacaoId}/{utilizadorId}` — Dar/Remover baze
  - Request: `DarBazeRequestDto` (body)
  - Response: `BazeDto`

**ComentariosController**: [NzolaWebAPI/Controllers/ComentariosController.cs](NzolaWebAPI/Controllers/ComentariosController.cs)
- Base route: `api/comentarios`
- GET `/publicacao/{Id}` — Listar comentários de publicação
  - Params: `Id` (route)
  - Response: `IEnumerable<ComentarioDto>`
- GET `/{id}` — Obter comentário
  - Params: `id` (route)
- POST `/{publicacaoId}/{utilizadorId}` — Adicionar comentário
  - Request: `AdicionarComentarioRequestDto` (body)
  - Response: `ComentarioDto`
- PUT `/{id}` — Editar comentário
  - Request: `EditarComentarioRequestDto` (body)
- DELETE `/{id}` — Excluir comentário

**EmailController**: [NzolaWebAPI/Controllers/EmailController.cs](NzolaWebAPI/Controllers/EmailController.cs)
- Base route: `api/email`
- POST `/send-test` — Enviar email de teste
  - Params: `toEmail` (query/implicit)
  - Service: `IEmailService`

**NotificacaoController**: [NzolaWebAPI/Controllers/NotificacaoController.cs](NzolaWebAPI/Controllers/NotificacaoController.cs)
- Base route: `api/notificacao`
- GET `/` — Listar notificações
- GET `/{id}` — Obter notificação
- POST `/` — Criar notificação (`CriarNotificacaoDto`)
- PUT `/` — Marcar como lida (espera `id`)
- DELETE `/` — Apagar (espera `id`)

Observação: alguns métodos PUT/DELETE usam parâmetros de rota sem atributo explícito `[FromRoute]` — considerar adicionar para clareza.

**PublicacoesController**: [NzolaWebAPI/Controllers/PublicacoesController.cs](NzolaWebAPI/Controllers/PublicacoesController.cs)
- Base route: `api/publicacoes`
- GET `/` — Listar publicações
  - Response: `IEnumerable<PublicacaoDto>`
- GET `/{id}` — Selecionar publicação (feed)
- POST `/{utilizadorId}` — Publicar conteúdo (form-data)
  - Consumes: `multipart/form-data`
  - Request: `CriarPublicacaoRequestDto` (form)
  - Limits: até 200 MB (RequestSizeLimit)
- PUT `/{Id}` — Atualizar publicação (`ActualizarPublicacaoRequestDto`)
- DELETE `/{Id}` — Remover publicação

**SeguidorController**: [NzolaWebAPI/Controllers/SeguidorController.cs](NzolaWebAPI/Controllers/SeguidorController.cs)
- Base route: `api/seguidor`
- GET `/` — Listar seguidores
- GET `/{id}` — Selecionar seguidor
- POST `/` — Criar seguidor (`CriarSeguidorDto`)
- DELETE `/` — Apagar (espera `id`)

**UtilizadoresController**: [NzolaWebAPI/Controllers/UtilizadoresController.cs](NzolaWebAPI/Controllers/UtilizadoresController.cs)
- Base route: `api/utilizadores`
- GET `/` — Listar utilizadores
- GET `/{id}` — Selecionar utilizador
- POST `/` — Criar utilizador (`CriarUtilizadorRequestDto`)
- DELETE `/{id}` — Apagar utilizador

## DTOs Principais (resumo)
- `CriarUtilizadorRequestDto`, `LoginDto`, `UtilizadorDto`
- `BazeDto`, `DarBazeRequestDto`
- `ComentarioDto`, `AdicionarComentarioRequestDto`, `EditarComentarioRequestDto`
- `NotificacaoDto`, `CriarNotificacaoDto`
- `PublicacaoDto`, `PublicacaoFeedDto`, `CriarPublicacaoRequestDto`, `ActualizarPublicacaoRequestDto`
- `SeguidorDto`, `CriarSeguidorDto`

## Observações e Recomendações
- Adicionar `[FromRoute]` em parâmetros de rota para evitar ambiguidades.
- Unificar respostas: preferir `ActionResult<T>` com tipos concretos para permitir documentação OpenAPI automática.
- Expor Swagger/OpenAPI: adicionar `Swashbuckle.AspNetCore` e mapear DTOs para melhorar consumíveis.
- Incluir exemplos de requisição/resposta e códigos de status para cada endpoint (próximo passo).

## Próximos Passos
1. Gerar especificação OpenAPI/Swagger a partir dos controllers.
2. Adicionar exemplos de request/response para os endpoints críticos (autenticação, publicações, comentários).
3. Publicar `Docs/API.md` e, opcionalmente, converter para HTML/Swagger UI.

---
Gerado automaticamente a partir do inventário de controllers em NzolaWebAPI.
