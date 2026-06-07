# Arquitectura do sistema

Este projeto segue uma arquitectura em camadas para API ASP.NET Core com EF Core.

## Estado real

Resumo do estado consolidado baseado em implementacao e documentacao.

## Estrutura principal

- Controllers (`Controllers/`): definem rotas HTTP e retornam `IActionResult`.
- Data (`Data/`): `ContextoBDNzola` com mapeamentos, relacionamentos, indices e constraints.
- Models (`Models/`): entidades de dominio persistidas no SQL Server.
- DTOs (`DTOs/`): contratos de entrada e saida da API.
- Mappers (`Mappers/`): conversao entre Models e DTOs.
- Interfaces (`Interfaces/`): contratos dos repositorios.
- Repository (`Repository/`): implementacao de acesso a dados.
- Services (`Services/`): servicos transversais (ex.: JWT).

## Fluxo padrao

1. Controller recebe request.
2. Controller delega para repositorio.
3. Repositorio usa `NzolaDBContext` para leitura/escrita.
4. Mapper converte Model/DTO na entrada e saida.

## Responsabilidades por camada

### Controllers

- Orquestracao de endpoint.
- Validacao de fluxo (ex.: existencia de recurso relacionado).

### Repositories

- CRUD assincrono.
- Regras de persistencia e consultas agregadas (isso deve acontecer no Services acho eu, certo?).

### DbContext

- Conversao de enums para string.
- Relacionamentos e regras de integridade.
- Indices unicos para campos de identidade (telefone, email, etc.).

### DTOs e Mappers

- Contrato externo desacoplado de entidades.
- Conversao bidirecional para evitar exposicao direta de Models.

## Atualizacao recente: campos de imagem

As entidades abaixo passaram a conter binario de imagem:

- `Utilizador.FotoPerfil`
- `Utilizador.ImagemCobertura`

Padrao adotado na API:

- DTOs de POST/PUT/GET tambem incluem os campos de imagem.
- Transporte em JSON como base64 (serializacao padrao de `byte[]`).
- Persistencia no SQL Server em `varbinary(max)` (binario), nao em texto base64.
- Mappers copiam imagem em ambos sentidos.
- Repositories preservam imagem atual quando PUT nao envia nova imagem.
- O payload esperado para bind direto e base64 puro (sem prefixo `data:image/...;base64,`).

Observacao:

- Ainda nao ha validacao centralizada de tamanho/tipo de imagem por assinatura binaria.

## Relacoes principais

- Utilizador 1:N Comentario
- Utilizador 1:N Publicacao
- Utilizador 1:N Seguidor
- Publicacao 1:N ConteudoPublicacao
- Utilizador 1:N Notificacao
