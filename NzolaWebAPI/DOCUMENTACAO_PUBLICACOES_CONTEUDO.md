# Documentação: Publicações e Conteúdo com Ficheiros/Imagens/Vídeos

## 1. Visão Geral

Este documento descreve como o sistema NzolaWebAPI manuseia publicações com conteúdo multimídia (imagens, vídeos, ficheiros). O sistema suporta dois formatos:

- **File Input (Recomendado)**: Upload de ficheiros através de `multipart/form-data`
- **String Format (Base64)**: Envio de ficheiros codificados como string Base64

---

## 2. Modelos de Dados

### 2.1 Estrutura do Banco de Dados

```
Publicacao (1:1) → ConteudoPublicacao (1:N) → FicheiroConteudo
```

- **Publicacao**: Entidade principal com metadados (DataCriacao, EstadoPublicacao, etc.)
- **ConteudoPublicacao**: Conteúdo efetivo (texto, tipo, ficheiros relacionados)
- **FicheiroConteudo**: Ficheiros individuais (nome, caminho, tipo MIME, tamanho)

### 2.2 DTOs de Requisição

#### CriarPublicacaoRequestDto
```csharp
public class CriarPublicacaoRequestDto
{
    public int UtilizadorId { get; set; }
    public ItemConteudoRequestDto Conteudo { get; set; }
}
```

#### ItemConteudoRequestDto
```csharp
public class ItemConteudoRequestDto
{
    public string Texto { get; set; }                    // Texto opcional da publicação
    public List<IFormFile>? Ficheiros { get; set; }     // Ficheiros (imagens/vídeos/PDFs)
    public TipoConteudo TipoConteudo { get; set; }      // Enum: Texto, Imagem, Video, Documento
}
```

#### Enum TipoConteudo
```csharp
public enum TipoConteudo
{
    Texto = 0,
    Imagem = 1,
    Video = 2,
    Documento = 3
}
```

---

## 3. Endpoints da API

### 3.1 Criar Publicação com Ficheiros (File Input)

**Endpoint**: `POST /api/publicacoes/{utilizadorId}`

**Content-Type**: `multipart/form-data`

**Parâmetros Path**:
- `utilizadorId`: ID do utilizador que cria a publicação

**Campos Form**:
```
Texto (string, opcional)             - Conteúdo textual
Ficheiros (file[], opcional)         - Array de ficheiros (máx 10 MB cada)
TipoConteudo (string, obrigatório)  - Valor: "Texto", "Imagem", "Video", "Documento"
```

**Exemplo cURL**:
```bash
curl -X POST http://localhost:5001/api/publicacoes/1 \
  -F "Texto=Eis meu novo post!" \
  -F "Ficheiros=@imagem1.jpg" \
  -F "Ficheiros=@imagem2.jpg" \
  -F "TipoConteudo=Imagem"
```

**Resposta Sucesso** (201 Created):
```json
{
  "id": 1,
  "utilizadorId": 1,
  "conteudoPublicacao": {
    "id": 1,
    "texto": "Eis meu novo post!",
    "tipoConteudo": "Imagem",
    "ficheirosConteudo": [
      {
        "id": 1,
        "nomeOriginal": "imagem1.jpg",
        "caminhoArmazenado": "/uploads/2026-06-11/imagem1_abc123.jpg",
        "tipoMime": "image/jpeg",
        "tamanhoBytes": 245632
      },
      {
        "id": 2,
        "nomeOriginal": "imagem2.jpg",
        "caminhoArmazenado": "/uploads/2026-06-11/imagem2_def456.jpg",
        "tipoMime": "image/jpeg",
        "tamanhoBytes": 312145
      }
    ]
  },
  "dataCriacao": "2026-06-11T14:30:00Z",
  "estadoPublicacao": "Ativa"
}
```

### 3.2 Criar Publicação com Base64 (String Format)

**Para clientes que não suportam multipart/form-data**

**DTO Alternativo**: `CriarPublicacaoBase64RequestDto`

```csharp
public class CriarPublicacaoBase64RequestDto
{
    public int UtilizadorId { get; set; }
    public string Texto { get; set; }
    public List<FicheiroBase64Dto>? Ficheiros { get; set; }
    public string TipoConteudo { get; set; }
}

public class FicheiroBase64Dto
{
    public string NomeOriginal { get; set; }
    public string ConteudoBase64 { get; set; }  // Base64 encoded
    public string TipoMime { get; set; }         // Exemplo: "image/jpeg"
}
```

**Endpoint**: `POST /api/publicacoes/criar-base64/{utilizadorId}`

**Content-Type**: `application/json`

**Exemplo JSON**:
```json
{
  "utilizadorId": 1,
  "texto": "Eis meu novo post!",
  "tipoConteudo": "Imagem",
  "ficheiros": [
    {
      "nomeOriginal": "imagem1.jpg",
      "conteudoBase64": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+P+/HgAFhAJ/wlseKgAAAABJRU5ErkJggg==",
      "tipoMime": "image/jpeg"
    }
  ]
}
```

---

## 4. Validações e Restrições

### 4.1 Validações no Backend

| Campo | Validação | Erro |
|-------|-----------|------|
| UtilizadorId | Must exist | 404 Not Found |
| Texto | Max 5000 chars | 400 Bad Request |
| Ficheiros | Max 10 files | 400 Bad Request |
| Ficheiro Size | Max 10 MB each | 413 Payload Too Large |
| TipoConteudo | Must be valid enum | 400 Bad Request |
| Ficheiro MIME | Whitelist allowed types | 415 Unsupported Media Type |

### 4.2 Tipos MIME Permitidos

```csharp
private static readonly string[] ALLOWED_MIME_TYPES = 
{
    "image/jpeg", "image/png", "image/gif", "image/webp",  // Imagens
    "video/mp4", "video/webm",                              // Vídeos
    "application/pdf",                                       // PDFs
    "application/msword",                                    // DOC
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document" // DOCX
};
```

---

## 5. Armazenamento de Ficheiros

### 5.1 Estrutura de Pastas

```
wwwroot/
  uploads/
    2026-06-11/
      imagem1_abc123.jpg
      video1_def456.mp4
    2026-06-10/
      documento1_ghi789.pdf
```

### 5.2 Estratégia de Armazenamento

1. **Nome Único**: `[nomeOriginal]_[guid].extensão`
2. **Pasta por Data**: `uploads/YYYY-MM-DD/`
3. **Referência BD**: Caminho relativo armazenado em `FicheiroConteudo.CaminhoArmazenado`

### 5.3 Exemplo de Implementação (Service)

```csharp
public async Task<string> ArmazenarFicheiroAsync(IFormFile file)
{
    var extensao = Path.GetExtension(file.FileName);
    var nomeUnico = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extensao}";
    var pastaData = $"uploads/{DateTime.Now:yyyy-MM-dd}";
    var caminhoAbsoluto = Path.Combine("wwwroot", pastaData);
    
    if (!Directory.Exists(caminhoAbsoluto))
        Directory.CreateDirectory(caminhoAbsoluto);
    
    var caminhoCompleto = Path.Combine(caminhoAbsoluto, nomeUnico);
    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    
    return $"/{pastaData}/{nomeUnico}";
}
```

---

## 6. Endpoints Adicionais

### 6.1 Obter Publicação por ID

**Endpoint**: `GET /api/publicacoes/{id}`

**Resposta**:
```json
{
  "id": 1,
  "utilizadorId": 1,
  "utilizador": {
    "id": 1,
    "nomeCompleto": "João Silva",
    "nomeUtilizador": "joao.silva",
    "fotoPerfil": "/uploads/avatares/joao_profile.jpg"
  },
  "conteudoPublicacao": {
    "id": 1,
    "texto": "Eis meu novo post!",
    "tipoConteudo": "Imagem",
    "ficheirosConteudo": [
      {
        "id": 1,
        "nomeOriginal": "imagem1.jpg",
        "caminhoArmazenado": "/uploads/2026-06-11/imagem1_abc123.jpg",
        "tipoMime": "image/jpeg",
        "tamanhoBytes": 245632
      }
    ]
  },
  "dataCriacao": "2026-06-11T14:30:00Z",
  "estadoPublicacao": "Ativa"
}
```

### 6.2 Listar Publicações Recentes

**Endpoint**: `GET /api/publicacoes/recentes?pagina=1&tamanho=20`

**Query Parameters**:
- `pagina`: Número da página (padrão: 1)
- `tamanho`: Itens por página (padrão: 20, máx: 100)

**Resposta**:
```json
{
  "items": [
    {
      "id": 5,
      "utilizadorId": 2,
      "utilizador": {
        "nomeUtilizador": "maria.santos",
        "fotoPerfil": "/uploads/avatares/maria_profile.jpg"
      },
      "conteudoPublicacao": {
        "texto": "Novo vídeo disponível!",
        "tipoConteudo": "Video"
      },
      "dataCriacao": "2026-06-11T15:00:00Z"
    },
    {
      "id": 4,
      "utilizadorId": 1,
      "utilizador": {
        "nomeUtilizador": "joao.silva",
        "fotoPerfil": "/uploads/avatares/joao_profile.jpg"
      },
      "conteudoPublicacao": {
        "texto": "Eis meu novo post!",
        "tipoConteudo": "Imagem"
      },
      "dataCriacao": "2026-06-11T14:30:00Z"
    }
  ],
  "totalItens": 145,
  "totalPaginas": 8,
  "paginaAtual": 1
}
```

### 6.3 Atualizar Publicação

**Endpoint**: `PUT /api/publicacoes/{id}`

**Body**:
```json
{
  "texto": "Texto atualizado",
  "tipoConteudo": "Imagem"
}
```

### 6.4 Remover Publicação

**Endpoint**: `DELETE /api/publicacoes/{id}`

**Nota**: Remove publicação e seus ficheiros associados

---

## 7. Tratamento de Erros

### Códigos HTTP Comuns

| Código | Cenário | Mensagem |
|--------|---------|---------|
| 200 | Sucesso | OK |
| 201 | Criado | Created |
| 400 | Validação falhou | "Ficheiro muito grande" |
| 401 | Não autenticado | "Unauthorized" |
| 403 | Sem permissão | "Apenas o autor pode editar" |
| 404 | Não encontrado | "Publicação não existe" |
| 413 | Ficheiro grande demais | "Payload Too Large" |
| 415 | Tipo MIME não permitido | "Unsupported Media Type" |
| 500 | Erro servidor | "Internal Server Error" |

---

## 8. Exemplo Prático: Criar Publicação com 3 Imagens

### Backend (C# - Service)

```csharp
public async Task<PublicacaoDto> CriarPublicacaoComFicheirosAsync(
    int utilizadorId, 
    CriarPublicacaoRequestDto request)
{
    // Validar utilizador
    var utilizador = await _context.Utilizadores.FindAsync(utilizadorId);
    if (utilizador == null)
        throw new Exception("Utilizador não encontrado");
    
    // Criar publicação
    var publicacao = new Publicacao
    {
        UtilizadorId = utilizadorId,
        DataCriacao = DateTime.UtcNow,
        EstadoPublicacao = "Ativa"
    };
    
    // Criar conteúdo
    var conteudo = new ConteudoPublicacao
    {
        Texto = request.Conteudo.Texto,
        TipoConteudo = request.Conteudo.TipoConteudo,
        Publicacao = publicacao
    };
    
    // Processar ficheiros
    if (request.Conteudo.Ficheiros?.Count > 0)
    {
        foreach (var file in request.Conteudo.Ficheiros)
        {
            // Validar
            if (file.Length > 10 * 1024 * 1024) // 10 MB
                throw new Exception("Ficheiro muito grande");
            
            if (!IsAllowedMimeType(file.ContentType))
                throw new Exception("Tipo de ficheiro não permitido");
            
            // Armazenar
            var caminho = await _ficheiroService.ArmazenarFicheiroAsync(file);
            
            // Adicionar à BD
            conteudo.FicheirosConteudo.Add(new FicheiroConteudo
            {
                NomeOriginal = file.FileName,
                CaminhoArmazenado = caminho,
                TipoMime = file.ContentType,
                TamanhoBytes = file.Length
            });
        }
    }
    
    _context.Publicacoes.Add(publicacao);
    await _context.SaveChangesAsync();
    
    return _mapper.Map<PublicacaoDto>(publicacao);
}
```

---

## 9. Melhorias Futuras

- [ ] Compressão automática de imagens
- [ ] Geração de thumbnails
- [ ] Streaming de vídeos progressivo
- [ ] Cache de ficheiros frequentes
- [ ] Análise de segurança (antivírus)
- [ ] Integração com CDN (Azure Blob Storage, AWS S3)

