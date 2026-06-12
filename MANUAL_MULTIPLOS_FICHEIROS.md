# Manual: Implementar Múltiplos Ficheiros por Conteúdo

## Objetivo
Permitir que cada `ConteúdoPublicação` tenha **vários ficheiros** (múltiplas imagens/vídeos), em vez de apenas um.

---

## 📋 Arquitetura Final

```
PublicacãoRequest
  ├── Conteúdos (Array)
      ├── Conteúdo 1
      │   ├── Texto: "descrição"
      │   ├── Ficheiros: [foto1.jpg, foto2.jpg, vídeo.mp4]
      │   └── TipoConteudo: Misto
      │
      ├── Conteúdo 2
      │   ├── Texto: "outro comentário"
      │   ├── Ficheiros: [imagem.png]
      │   └── TipoConteudo: Imagem
```

---

## ✅ Passo 1: Alterar DTOs

### 1.1 - ItemConteudoRequestDto.cs
**Mudar `Ficheiro` (singular) para `Ficheiros` (array)**

```csharp
using Microsoft.AspNetCore.Http;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class ItemConteudoRequestDto
    {
        public int PublicacaoId { get; set; }
        public string Texto { get; set; }
        
        // ANTES: public IFormFile? Ficheiro { get; set; }
        // DEPOIS:
        public List<IFormFile>? Ficheiros { get; set; }
        
        public int Ordem { get; set; }
        public TipoConteudo TipoConteudo { get; set; }
    }
}
```

---

## ✅ Passo 2: Alterar Modelos

### 2.1 - ConteudoPublicacao.cs
**Adicionar relacionamento para múltiplos ficheiros**

```csharp
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    public class ConteudoPublicacao
    {
        public int Id { get; set; }
        public int PublicacaoId { get; set; }
        public string Texto { get; set; }
        
        // NOVO: Coleção de ficheiros
        public List<FicheiroConteudo> Ficheiros { get; set; } = new();
        
        public int Ordem { get; set; }
        public TipoConteudo TipoConteudo { get; set; }
        
        // Foreign Key
        public Publicacao Publicacao { get; set; }
    }
}
```

### 2.2 - Novo Model: FicheiroConteudo.cs
**Criar modelo para armazenar ficheiros**

```csharp
namespace NzolaWebAPI.Models
{
    public class FicheiroConteudo
    {
        public int Id { get; set; }
        public int ConteudoPublicacaoId { get; set; }
        public string CaminhoFicheiro { get; set; }  // Caminho relativo (ex: "/uploads/guid.jpg")
        public string TipoMime { get; set; }         // Tipo MIME (ex: "image/jpeg")
        public long TamanhoBytes { get; set; }       // Tamanho do ficheiro
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;
        
        // Foreign Key
        public ConteudoPublicacao ConteudoPublicacao { get; set; }
    }
}
```

---

## ✅ Passo 3: Alterar o Contexto (DbContext)

### 3.1 - ContextoBDNzola.cs
**Adicionar DbSet para o novo modelo**

```csharp
public DbSet<ConteudoPublicacao> ConteudoPublicacoes { get; set; }

// NOVO:
public DbSet<FicheiroConteudo> FicheirosConteudo { get; set; }
```

---

## ✅ Passo 4: Criar Migração

```bash
cd NzolaWebAPI

# Criar migração
dotnet ef migrations add AddMultiplosFicheirosConteudo

# Aplicar migração
dotnet ef database update
```

**Gerados automaticamente:**
- Tabela `FicheirosConteudo`
- Foreign Key para `ConteudoPublicacao`

---

## ✅ Passo 5: Alterar Mappers

### 5.1 - ConteudoPublicacaoMappers.cs
**Atualizar método de mapeamento**

```csharp
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public class ConteudoPublicacaoMappers
    {
        // ANTES:
        // public static ConteudoPublicacao ParaConteudoPublicacaoDeItemConteudoRequestDto(
        //     ItemConteudoRequestDto conteudoDto, int publicacaoId, string conteudoResolvido)

        // DEPOIS:
        public static ConteudoPublicacao ParaConteudoPublicacaoDeItemConteudoRequestDto(
            ItemConteudoRequestDto conteudoDto, 
            int publicacaoId, 
            List<FicheiroConteudo> ficheirosResolvidos)
        {
            var conteudo = new ConteudoPublicacao
            {
                PublicacaoId = publicacaoId,
                Texto = conteudoDto.Texto,
                Ordem = conteudoDto.Ordem,
                TipoConteudo = conteudoDto.TipoConteudo,
                Ficheiros = ficheirosResolvidos ?? new()
            };

            return conteudo;
        }

        // NOVO: Converter array de IFormFile em FicheiroConteudo
        public static List<FicheiroConteudo> ParaFicheirosConteudo(
            List<IFormFile> ficheiros, 
            string caminhoUpload)
        {
            var ficheirosConteudo = new List<FicheiroConteudo>();

            if (ficheiros == null || ficheiros.Count == 0)
                return ficheirosConteudo;

            foreach (var ficheiro in ficheiros)
            {
                ficheirosConteudo.Add(new FicheiroConteudo
                {
                    CaminhoFicheiro = caminhoUpload,
                    TipoMime = ficheiro.ContentType,
                    TamanhoBytes = ficheiro.Length
                });
            }

            return ficheirosConteudo;
        }
    }
}
```

---

## ✅ Passo 6: Alterar Services

### 6.1 - ConteudoPublicacaoService.cs
**Atualizar para processar múltiplos ficheiros**

```csharp
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Repositories;

namespace NzolaWebAPI.Services
{
    public class ConteudoPublicacaoService : IConteudoPublicacaoService
    {
        private readonly IConteudoPublicacaoRepository _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadsPath;

        public ConteudoPublicacaoService(
            IConteudoPublicacaoRepository repository,
            IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        public async Task AdicionarListaAsync(
            List<ItemConteudoRequestDto> conteudosDtos, 
            int publicacaoId)
        {
            var ficheirosUpload = new Dictionary<string, List<string>>(); // Para rollback
            var conteudosAdicionar = new List<ConteudoPublicacao>();

            try
            {
                // Criar diretório se não existir
                if (!Directory.Exists(_uploadsPath))
                    Directory.CreateDirectory(_uploadsPath);

                // Processar cada conteúdo
                foreach (var conteudoDto in conteudosDtos)
                {
                    // 1. Processar ficheiros
                    var ficheirosResolvidos = new List<FicheiroConteudo>();

                    if (conteudoDto.Ficheiros != null && conteudoDto.Ficheiros.Count > 0)
                    {
                        var caminhosMapeados = new List<string>();

                        foreach (var ficheiro in conteudoDto.Ficheiros)
                        {
                            if (ficheiro.Length > 0)
                            {
                                var caminhoSalvo = await SalvarFicheiroNoDiscoAsync(ficheiro);
                                caminhosMapeados.Add(caminhoSalvo);

                                // Mapear para FicheiroConteudo
                                ficheirosResolvidos.Add(new FicheiroConteudo
                                {
                                    CaminhoFicheiro = caminhoSalvo,
                                    TipoMime = ficheiro.ContentType,
                                    TamanhoBytes = ficheiro.Length
                                });
                            }
                        }

                        // Guardar para rollback
                        ficheirosUpload[conteudoDto.GetHashCode().ToString()] = caminhosMapeados;
                    }

                    // 2. Mapear DTO para entidade
                    var conteudoResolvido = ConteudoPublicacaoMappers
                        .ParaConteudoPublicacaoDeItemConteudoRequestDto(
                            conteudoDto, 
                            publicacaoId, 
                            ficheirosResolvidos);

                    conteudosAdicionar.Add(conteudoResolvido);
                }

                // 3. Guardar todos os conteúdos
                await _repository.AdicionarMultiplosAsync(conteudosAdicionar);
            }
            catch (Exception ex)
            {
                // Rollback: Apagar ficheiros carregados
                foreach (var caminhos in ficheirosUpload.Values)
                {
                    foreach (var caminho in caminhos)
                    {
                        var caminhoCompleto = Path.Combine(_webHostEnvironment.WebRootPath, 
                            caminho.TrimStart('/'));
                        if (File.Exists(caminhoCompleto))
                            File.Delete(caminhoCompleto);
                    }
                }

                throw new Exception($"Erro ao adicionar conteúdos: {ex.Message}", ex);
            }
        }

        // MÉTODO AUXILIAR: Salvar ficheiro no disco
        private async Task<string> SalvarFicheiroNoDiscoAsync(IFormFile ficheiro)
        {
            var nomeGuid = Guid.NewGuid().ToString();
            var extensao = Path.GetExtension(ficheiro.FileName);
            var nomeArquivo = $"{nomeGuid}{extensao}";
            var caminhoCompleto = Path.Combine(_uploadsPath, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await ficheiro.CopyToAsync(stream);
            }

            // Retornar caminho relativo
            return $"/uploads/{nomeArquivo}";
        }
    }
}
```

### 6.2 - PublicacaoService.cs
**Atualizar CriarAsync para usar novos ficheiros**

```csharp
public async Task CriarAsync(int utilizadorId, CriarPublicacaoRequestDto publicacaoDto)
{
    var ficheirosUpload = new Dictionary<int, List<string>>();

    try
    {
        // Preparar diretório
        if (!Directory.Exists(_uploadsPath))
            Directory.CreateDirectory(_uploadsPath);

        var publicacao = PublicacaoMappers
            .ParaPublicacaoDePublicacaoDto(publicacaoDto, utilizadorId);

        // Processar cada conteúdo
        for (int i = 0; i < publicacaoDto.Conteudos.Count; i++)
        {
            var conteudoDto = publicacaoDto.Conteudos[i];
            var ficheirosResolvidos = new List<FicheiroConteudo>();

            if (conteudoDto.Ficheiros != null && conteudoDto.Ficheiros.Count > 0)
            {
                var caminhosMapeados = new List<string>();

                foreach (var ficheiro in conteudoDto.Ficheiros)
                {
                    if (ficheiro.Length > 0)
                    {
                        var caminhoSalvo = await SalvarFicheiroNoDiscoAsync(ficheiro);
                        caminhosMapeados.Add(caminhoSalvo);

                        ficheirosResolvidos.Add(new FicheiroConteudo
                        {
                            CaminhoFicheiro = caminhoSalvo,
                            TipoMime = ficheiro.ContentType,
                            TamanhoBytes = ficheiro.Length
                        });
                    }
                }

                ficheirosUpload[i] = caminhosMapeados;
            }

            var conteudoResolvido = ConteudoPublicacaoMappers
                .ParaConteudoPublicacaoDeItemConteudoRequestDto(
                    conteudoDto, 
                    0, // publicacaoId será atribuído depois
                    ficheirosResolvidos);

            publicacao.Conteudos.Add(conteudoResolvido);
        }

        // Guardar no repositório
        await _repository.CriarAsync(publicacao);
    }
    catch (Exception ex)
    {
        // Rollback
        foreach (var caminhos in ficheirosUpload.Values)
        {
            foreach (var caminho in caminhos)
            {
                var caminhoCompleto = Path.Combine(_webHostEnvironment.WebRootPath, 
                    caminho.TrimStart('/'));
                if (File.Exists(caminhoCompleto))
                    File.Delete(caminhoCompleto);
            }
        }

        throw;
    }
}

// MÉTODO AUXILIAR
private async Task<string> SalvarFicheiroNoDiscoAsync(IFormFile ficheiro)
{
    var nomeGuid = Guid.NewGuid().ToString();
    var extensao = Path.GetExtension(ficheiro.FileName);
    var nomeArquivo = $"{nomeGuid}{extensao}";
    var caminhoCompleto = Path.Combine(_uploadsPath, nomeArquivo);

    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
    {
        await ficheiro.CopyToAsync(stream);
    }

    return $"/uploads/{nomeArquivo}";
}
```

---

## ✅ Passo 7: Criar DTOs de Response

### 7.1 - Novo: FicheiroConteudoResponseDto.cs

```csharp
namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class FicheiroConteudoResponseDto
    {
        public int Id { get; set; }
        public string CaminhoFicheiro { get; set; }
        public string TipoMime { get; set; }
        public long TamanhoBytes { get; set; }
        public DateTime DataUpload { get; set; }
    }
}
```

### 7.2 - Atualizar: ItemConteudoResponseDto.cs

```csharp
namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class ItemConteudoResponseDto
    {
        public int Id { get; set; }
        public int PublicacaoId { get; set; }
        public string Texto { get; set; }
        
        // NOVO: Array de ficheiros
        public List<FicheiroConteudoResponseDto> Ficheiros { get; set; }
        
        public int Ordem { get; set; }
        public int TipoConteudo { get; set; }
    }
}
```

---

## ✅ Passo 8: Criar Mapper para Response

### 8.1 - Adicionar em ConteudoPublicacaoMappers.cs

```csharp
public static ItemConteudoResponseDto ParaItemConteudoResponseDto(ConteudoPublicacao conteudo)
{
    return new ItemConteudoResponseDto
    {
        Id = conteudo.Id,
        PublicacaoId = conteudo.PublicacaoId,
        Texto = conteudo.Texto,
        Ficheiros = conteudo.Ficheiros
            .Select(f => new FicheiroConteudoResponseDto
            {
                Id = f.Id,
                CaminhoFicheiro = f.CaminhoFicheiro,
                TipoMime = f.TipoMime,
                TamanhoBytes = f.TamanhoBytes,
                DataUpload = f.DataUpload
            })
            .ToList(),
        Ordem = conteudo.Ordem,
        TipoConteudo = (int)conteudo.TipoConteudo
    };
}
```

---

## ✅ Passo 9: Exemplo de Uso no Swagger

### Request POST `/api/publicacoes/{utilizadorId}`

```json
{
  "conteudos": [
    {
      "texto": "Primeira foto da viagem",
      "ficheiros": [ficheiro1.jpg, ficheiro2.jpg],
      "ordem": 1,
      "tipoConteudo": 1
    },
    {
      "texto": "Vídeo do momento",
      "ficheiros": [video.mp4],
      "ordem": 2,
      "tipoConteudo": 2
    }
  ]
}
```

### Response 

```json
{
  "id": 1,
  "autorId": 5,
  "conteudos": [
    {
      "id": 1,
      "publicacaoId": 1,
      "texto": "Primeira foto da viagem",
      "ficheiros": [
        {
          "id": 1,
          "caminhoFicheiro": "/uploads/a1b2c3d4.jpg",
          "tipoMime": "image/jpeg",
          "tamanhoBytes": 1024000,
          "dataUpload": "2026-06-11T10:30:00Z"
        },
        {
          "id": 2,
          "caminhoFicheiro": "/uploads/e5f6g7h8.jpg",
          "tipoMime": "image/jpeg",
          "tamanhoBytes": 2048000,
          "dataUpload": "2026-06-11T10:30:00Z"
        }
      ],
      "ordem": 1,
      "tipoConteudo": 1
    }
  ]
}
```

---

## 📝 Checklist de Implementação

- [ ] Alterar `ItemConteudoRequestDto` (Ficheiros como array)
- [ ] Alterar `ConteudoPublicacao.cs` (Adicionar coleção)
- [ ] Criar `FicheiroConteudo.cs` (Novo modelo)
- [ ] Adicionar DbSet ao `ContextoBDNzola`
- [ ] Criar migração: `dotnet ef migrations add AddMultiplosFicheirosConteudo`
- [ ] Aplicar migração: `dotnet ef database update`
- [ ] Atualizar `ConteudoPublicacaoMappers`
- [ ] Atualizar `PublicacaoService.CriarAsync`
- [ ] Atualizar `ConteudoPublicacaoService.AdicionarListaAsync`
- [ ] Criar `FicheiroConteudoResponseDto`
- [ ] Atualizar `ItemConteudoResponseDto`
- [ ] Testar no Swagger

---

## 🚀 Próximos Passos

1. Executar as alterações acima
2. Compilar: `dotnet build`
3. Executar migrations: `dotnet ef database update`
4. Testar no Swagger: Carregar múltiplas imagens num único conteúdo
5. Verificar os ficheiros em `wwwroot/uploads/`

**Dúvidas?** Peça ajuda para algum passo específico!
