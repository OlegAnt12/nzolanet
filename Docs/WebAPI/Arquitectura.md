# Arquitectura do sistema

Este projeto segue uma arquitectura em camadas para API ASP.NET Core com EF Core.

## Estrutura principal

- Controllers (`Controllers/`): definem rotas HTTP e retornam `IActionResult`.
- Data (`Data/`): `SysDBContext` com mapeamentos, relacionamentos, indices e constraints.
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
