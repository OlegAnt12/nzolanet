# Documentacao dos Endpoints (API)

URL Base: `http://<host>/api`

Os endpoints abaixo refletem as rotas atualmente implementadas nos controllers.

## Estado de confirmacao funcional (08-06-2026)

Classificacao usada neste documento:

- Confirmado: implementado e alinhado com o estado real consolidado.
- Parcial: implementacao existe, mas com dependencia externa ou ambiguidade de fluxo.
- Pendente: necessario validar com equipa responsavel antes de considerar concluido.

Resumo por dominio:

- CRUD principal (Utilizador, Publicacao, ConteudoPublicacao, Baze, Comentario, Seguidor, Notificacaao): Confirmado.
- Relatorios: Pendentes.
- Backup no backend: Pendente (rotas e enforcement).
- Cache administrativo (`/api/cache/*`): Confirmado.
- Autenticacao backend (JWT, renovacao e aplicacao transversal de autorizacao): Parcial/Pendente de validacao documental e de fluxo ponta a ponta.
_______________________

## Notas gerais

- DTOs com `byte[]` (imagem) devem ser enviados/recebidos como string base64 em JSON.
- `POST /api/Publicacoes` cria Conteudos automaticamente. 
- `POST /api/Bazes` remove ou adiciona baze numa publicacao dependendo do estado actual por utilizador (inclui a soma ou subtração da quantidade de bazes).
- Notificacoes sao geradas em eventos relevantes (login, registo, etc.). - por alterar...

## Imagens (`byte[]`) no contrato e armazenamento

- A API usa `byte[]` nos DTOs e models para imagens de `Utilizador`.
- Usa `string` nos DTOs e models para imagens de `ConteudoPublicacao`.
- Em JSON, a conversao entre `byte[]` e base64 e automatica pelo serializer do ASP.NET Core:
  - Entrada: base64 no JSON -> `byte[]` no backend.
  - Saida: `byte[]` no backend -> base64 no JSON.
- No SQL Server, os campos sao persistidos como `varbinary(max)` (binario), nao como texto base64.
- Enviar apenas o base64 puro no JSON. Exemplo: `"imagemProduto": "iVBORw0KGgo..."`.
- Evite enviar prefixo Data URL (`data:image/png;base64,`) porque esse formato pode falhar no bind direto para `byte[]`.
 
Nota de seguranca:

- Atualmente nao existe validacao centralizada de tamanho maximo/tipo de imagem por assinatura binaria (magic bytes).
- Recomenda-se adicionar validacao para reduzir payload excessivo e risco de ficheiros invalidos.

