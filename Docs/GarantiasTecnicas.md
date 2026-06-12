# Garantias Técnicas da API Nzola

Este documento resume as garantias implementadas no backend, com foco em restrições da base de dados, Fluent API, anotações de dados, migrations, Entity Framework Core, JWT/tokenização, serviço de email e upload de ficheiros com `FormFile`.

## 1. Garantias na Base de Dados

As garantias de integridade do sistema estão concentradas nos modelos, no `DbContext` e nas migrations geradas pelo Entity Framework Core.

### 1.1 Anotações de Dados

**Exemplo prático:**

```csharp
[Table("tb_Utilizadores")]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(NomeUtilizador), IsUnique = true)]
public class Utilizador
{
    [Key]
    public int Id { get; set; }

    [Required]
    [EnumDataType(typeof(Genero))]
    [Column(TypeName = "nvarchar(10)")]
    public Genero Genero { get; set; }

    [Required]
    [MaxLength(50)]
    public string NomeUtilizador { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    [MinLength(6)]
    public string PalavraPasse { get; set; } = string.Empty;
}
```

Nesta classe:
- `[Index(..., IsUnique = true)]` garante que não há duplicados em `Email` e `NomeUtilizador`.
- `[Required]` força valores obrigatórios.
- `[MaxLength]` e `[MinLength]` limitam o tamanho.
- `[EmailAddress]` valida o formato do email.

As entidades usam Data Annotations para declarar regras próximas do modelo:

- `[Key]` define a chave primária.
- `[Required]` garante campos obrigatórios.
- `[MaxLength]` e `[MinLength]` limitam o tamanho dos valores.
- `[EmailAddress]` valida o formato do email.
- `[EnumDataType]` valida enums na camada de modelo.
- `[ForeignKey]` liga propriedades de navegação às chaves estrangeiras.
- `[Index(..., IsUnique = true)]` cria índices únicos diretamente no modelo.
- `[Table(...)]` fixa o nome físico da tabela.

Exemplos relevantes:

- `Utilizador` exige `Genero`, `NomeUtilizador` e `Email`.
- `Utilizador` impõe unicidade em `Email` e `NomeUtilizador`.
- `Baze` impõe unicidade combinada em `PublicacaoId` e `UtilizadorId`.
- `Publicacao` e `FicheiroConteudo` usam relações de navegação para ligar publicações aos seus ficheiros.

### 1.2 Fluent API

Algumas garantias são reforçadas no `OnModelCreating` do `ContextoBDNzola`.

**Exemplo prático:**

```csharp
public class ContextoBDNzola : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Impedir cascata circular na relação Comentário -> Publicação
        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.Publicacao)
            .WithMany(p => p.Comentarios)
            .HasForeignKey(c => c.PublicacaoId)
            .OnDelete(DeleteBehavior.Restrict); // Não apagar comments quando apagar publicação

        // Chave composta em Baze para garantir 1 baze por utilizador por publicação
        modelBuilder.Entity<Baze>().HasKey(b => new { b.UtilizadorId, b.PublicacaoId });

        // Relação com cascade delete para ficheiros
        modelBuilder
            .Entity<Publicacao>()
            .HasMany(p => p.Ficheiros)
            .WithOne(f => f.Publicacao)
            .HasForeignKey(f => f.PublicacaoId)
            .OnDelete(DeleteBehavior.Cascade); // Apagar ficheiros quando apagar publicação

        // Garantir que Género só aceita dois valores
        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.Property(u => u.Genero)
                .HasConversion<string>()
                .HasColumnType("nvarchar(10)");

            entity.HasCheckConstraint(
                "CK_Utilizadores_Genero",
                "Genero IN ('Masculino','Feminino')"
            );
        });
    }
}
```

Neste exemplo:
- `DeleteBehavior.Restrict` impede que a exclusão de uma publicação apague comentários.
- `HasKey()` com dois campos cria uma chave composta.
- `HasCheckConstraint()` aplica uma regra diretamente na base de dados.

#### Relações e comportamento de eliminação

O projeto evita ciclos de cascata no SQL Server usando `DeleteBehavior.Restrict` em relações críticas:

- `Comentario -> Publicacao`
- `Baze -> Publicacao`
- `Publicacao -> Utilizador`
- `Baze -> Utilizador`
- `Seguidor -> UtilizadorSeguidor`
- `Seguidor -> UtilizadorSeguido`

Isto reduz o risco de exclusões em cascata indesejadas e resolve conflitos de múltiplos caminhos de cascade.

#### Chaves e unicidade

A Fluent API também reforça garantias de consistência:

- `Baze` usa chave composta em `(UtilizadorId, PublicacaoId)`.
- `Utilizador.Email` tem índice único.
- `Utilizador.NomeUtilizador` tem índice único.

#### Conversão e validação de enum

O enum `Genero` é persistido como string:

- `.HasConversion<string>()`
- `.HasColumnType("nvarchar(10)")`

Além disso, existe uma check constraint:

- `CK_Utilizadores_Genero`
- Regra SQL: `Genero IN ('Masculino','Feminino')`

### 1.3 Regras de negócio refletidas no modelo

Algumas garantias funcionais foram transformadas em restrições de persistência:

- um utilizador não pode ter dois registos iguais de `Baze` na mesma publicação;
- o email e o nome de utilizador não podem repetir;
- o género é limitado aos valores permitidos;
- a eliminação de dados ligados é controlada para evitar apagamentos acidentais em cadeia.

## 2. O que são Migrations

Migrations são versões do esquema da base de dados geradas pelo Entity Framework Core a partir da evolução do modelo.

Elas servem para:

- manter o esquema sincronizado com as classes `Model` e com o `DbContext`;
- aplicar alterações incrementais sem recriar a base de dados;
- permitir histórico e reversão com métodos `Up` e `Down`.

Em termos práticos, cada migration contém:

- operações para criar, alterar ou remover tabelas, colunas, índices e foreign keys;
- a lógica inversa para desfazer a alteração;
- um ficheiro `.Designer.cs` com a representação detalhada do modelo naquele ponto no tempo.

## 3. Como as Migrations foram Implementadas

O projeto tem migrations que mostram a evolução da arquitetura dos dados.

### 3.1 `AddRestricoesNomeUtilizadorEGenero`

Esta migration introduz garantias importantes na tabela de utilizadores:

**Código da migration:**

```csharp
public partial class AddRestricoesNomeUtilizadorEGenero : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Alterar tipo de Genero para nvarchar(10)
        migrationBuilder.AlterColumn<string>(
            name: "Genero",
            table: "tb_Utilizadores",
            type: "nvarchar(10)",
            nullable: false);

        // Adicionar coluna NomeUtilizador único
        migrationBuilder.AddColumn<string>(
            name: "NomeUtilizador",
            table: "tb_Utilizadores",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false);

        // Criar índice único em NomeUtilizador
        migrationBuilder.CreateIndex(
            name: "IX_tb_Utilizadores_NomeUtilizador",
            table: "tb_Utilizadores",
            column: "NomeUtilizador",
            unique: true);

        // Adicionar restrição de verificação para Género
        migrationBuilder.AddCheckConstraint(
            name: "CK_Utilizadores_Genero",
            table: "tb_Utilizadores",
            sql: "Genero IN ('Masculino','Feminino')");
    }
}
```

Esta migration:
- altera `Genero` para `nvarchar(10)`;
- adiciona a coluna `NomeUtilizador` com tamanho máximo de 50;
- cria índice único em `NomeUtilizador`;
- adiciona a check constraint `CK_Utilizadores_Genero`.

### 3.2 `SimplificarArquiteturaConteudo`

Esta migration refatorou a arquitetura do conteúdo:

**Alterações realizadas:**

```csharp
public partial class SimplificarArquiteturaConteudo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Remover o índice antigo e criar novo
        migrationBuilder.DropIndex(
            name: "IX_tb_ConteudosPublicacao_PublicacaoId",
            table: "tb_ConteudosPublicacao");

        // Remover coluna Ordem
        migrationBuilder.DropColumn(
            name: "Ordem",
            table: "tb_ConteudosPublicacao");

        // Renomear coluna Conteudo para Texto
        migrationBuilder.RenameColumn(
            name: "Conteudo",
            table: "tb_ConteudosPublicacao",
            newName: "Texto");

        // Adicionar coluna DataCriacao
        migrationBuilder.AddColumn<DateTime>(
            name: "DataCriacao",
            table: "tb_ConteudosPublicacao",
            type: "datetime2",
            nullable: false);

        // Criar tabela de ficheiros
        migrationBuilder.CreateTable(
            name: "tb_FicheirosConteudo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PublicacaoId = table.Column<int>(type: "int", nullable: false),
                CaminhoFicheiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TipoMime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tb_FicheirosConteudo", x => x.Id);
                table.ForeignKey(
                    name: "FK_FicheirosConteudo_Publicacoes",
                    column: x => x.PublicacaoId,
                    principalTable: "tb_Publicacoes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }
}
```

Esta etapa mostra uma transição para um modelo mais simples e mais adequado a conteúdos com ficheiros.

### 3.3 `PublicacaoRefactorado`

Esta migration conclui a refatoração do conteúdo:

**Simplificação final:**

```csharp
public partial class PublicacaoRefactorado : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Remover a foreign key antiga de FicheirosConteudo
        migrationBuilder.DropForeignKey(
            name: "FK_tb_FicheirosConteudo_tb_ConteudosPublicacao_ConteudoPublicacaoId",
            table: "tb_FicheirosConteudo");

        // Remover a tabela inteira de ConteudosPublicacao
        migrationBuilder.DropTable(
            name: "tb_ConteudosPublicacao");

        // Renomear a coluna ConteudoPublicacaoId para PublicacaoId
        migrationBuilder.RenameColumn(
            name: "ConteudoPublicacaoId",
            table: "tb_FicheirosConteudo",
            newName: "PublicacaoId");

        // Adicionar Texto diretamente à tabela de Publicações
        migrationBuilder.AddColumn<string>(
            name: "Texto",
            table: "tb_Publicacoes",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        // Recriar a foreign key com nova coluna
        migrationBuilder.AddForeignKey(
            name: "FK_tb_FicheirosConteudo_tb_Publicacoes_PublicacaoId",
            table: "tb_FicheirosConteudo",
            column: "PublicacaoId",
            principalTable: "tb_Publicacoes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
```

O efeito final é que a publicação passa a concentrar o conteúdo textual, enquanto os ficheiros ficam numa entidade separada. Isto simplifica queries e manutenção.

### 3.4 Snapshot do modelo

O ficheiro `ContextoBDNzolaModelSnapshot.cs` é importante porque guarda o estado atual do modelo para o EF Core comparar alterações futuras.

É esse snapshot que permite ao EF Core perceber:

- chaves primárias;
- índices únicos;
- relações;
- delete behaviors;
- constraints aplicadas.

## 4. Entity Framework Core

O Entity Framework Core é a camada de mapeamento objeto-relacional usada no backend.

No projeto ele é usado para:

- mapear classes para tabelas;
- configurar relacionamentos via Data Annotations e Fluent API;
- criar migrations;
- executar queries via `DbSet<T>`;
- aplicar regras de persistência sem SQL manual na maior parte do código.

### 4.1 `DbContext`

A classe `ContextoBDNzola` expõe os conjuntos de entidades:

- `Publicacoes`
- `FicheirosConteudo`
- `Comentarios`
- `Bazes`
- `Notificacoes`
- `Utilizadores`
- `Seguidores`

Ela centraliza toda a configuração relacional do domínio.

### 4.2 Relações principais

- `Utilizador` tem muitas `Publicacao`
- `Publicacao` tem muitos `FicheiroConteudo`
- `Publicacao` tem muitos `Comentario`
- `Publicacao` tem muitos `Baze`
- `Utilizador` participa na relação de seguidores como seguidor e seguido

## 5. JWT e Tokenização

A autenticação usa JSON Web Tokens através de `TokenService`.

### 5.1 Como o token é criado

O `TokenService`:

**Código do serviço:**

```csharp
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _chave;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!)
        );
    }

    public string CriarToken(Utilizador utilizador)
    {
        var claims = new List<Claim>
        {
            new Claim("id", utilizador.Id.ToString()),
            new Claim("email", utilizador.Email),
            new Claim("name", utilizador.NomeUtilizador)
        };

        var credenciais = new SigningCredentials(
            _chave,
            SecurityAlgorithms.HmacSha512Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7), // Validade de 7 dias
            SigningCredentials = credenciais,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
```

**Configuração em `appsettings.json`:**

```json
{
  "Jwt": {
    "SigningKey": "minha-chave-secreta-muito-segura-com-minimo-32-caracteres",
    "Issuer": "NzolaAPI",
    "Audience": "NzolaClients"
  }
}
```

No projeto, a configuração real está em `JWT`, com `SigningKey`, `Issuer` e `Audience`.

**Uso no controller:**

```csharp
[HttpPost("login")]
public IActionResult Login([FromBody] LoginDto loginDto)
{
    var utilizador = _contexto.Utilizadores
        .FirstOrDefault(u => u.Email == loginDto.Email);
    
    if (utilizador == null || !VerifyPassword(loginDto.Senha, utilizador.PalavraPasse))
        return Unauthorized("Email ou senha inválidos");
    
    var tokenGerado = _tokenService.CriarToken(utilizador);
    
    return Ok(new { token = tokenGerado });
}
```

### 5.2 Uso na aplicação

O serviço é registado por injeção de dependência em `Program.cs` e é usado pelo fluxo de autenticação.

Na prática, o login valida o utilizador e devolve um token que pode ser usado para chamadas autenticadas na API.

### 5.3 Garantias associadas

- o token é assinado com chave simétrica;
- a validade é limitada;
- a configuração externa permite separar ambiente de desenvolvimento e produção;
- a emissão do token fica encapsulada num serviço dedicado.

## 6. Serviço de Email

O projeto inclui um serviço de email assíncrono baseado em `MailKit` e `MimeKit`.

### 6.1 Implementação

O `EmailService` constrói mensagens `MimeMessage`, define remetente, destinatário, assunto e corpo HTML, e envia através de `SmtpClient`.

**Código do serviço:**

```csharp
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _emailSettings.SmtpServer,
            _emailSettings.Port,
            SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(
            _emailSettings.SenderEmail,
            _emailSettings.SenderPassword
        );
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
```

**Configuração em `appsettings.json`:**

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "seu-email@gmail.com",
    "SenderPassword": "sua-senha-app"
    },
    "SmtpSettings": {
        "Server": "smtp.gmail.com",
        "Port": 587,
        "NomeEmissor": "Nzola Network",
        "EmailEmissor": "seu-email@gmail.com",
        "Password": "sua-senha-app"
  }
}
```

**Registo em `Program.cs`:**

```csharp
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddScoped<IEmailService, EmailService>();
```

**Uso no serviço de utilizador:**

```csharp
await _emailService.SendEmailAsync(
    utilizador.Email,
    "Bem-vindo à Nzola!",
    $"<h1>Olá {utilizador.NomeUtilizador}</h1><p>A tua conta foi criada com sucesso.</p>"
);
```

### 6.2 Configuração

O serviço lê configurações de email a partir de `EmailSettings` e também de secções `SmtpSettings`.

Na prática, `SendEmailAsync` usa `EmailSettings` e `EnviarEmailConfirmacaoAsync` usa `SmtpSettings`.

**Classe de configuração:**

```csharp
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderPassword { get; set; } = string.Empty;
}
```

O `Program.cs` regista:

```csharp
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddScoped<IEmailService, EmailService>();
```

**Integração com serviço de utilizador:**

```csharp
public class UtilizadorService : IUtilizadorService
{
    private readonly IEmailService _emailService;

    public async Task<UtilizadorDto> RegistarAsync(CriarUtilizadorRequestDto registoDto)
    {
        var utilizador = new Utilizador { ... };
        _contexto.Utilizadores.Add(utilizador);
        await _contexto.SaveChangesAsync();

        // Enviar email de confirmação após registo
        await _emailService.SendEmailAsync(
            utilizador.Email,
            "Bem-vindo à Nzola",
            $"<h1>Olá {utilizador.NomeUtilizador}!</h1><p>Registo efetuado com sucesso.</p>"
        );

        return UtilizadorMappers.ToDto(utilizador);
    }
}
```

### 6.3 Exemplos de uso

O serviço de email é utilizado em vários cenários:

**Notificação de novo comentário:**

```csharp
public class ComentarioService : IComentarioService
{
    private readonly IEmailService _emailService;
    private readonly ContextoBDNzola _contexto;

    public async Task<ComentarioDto> CriarAsync(
        int utilizadorId,
        int publicacaoId,
        CriarComentarioRequestDto comentarioDto)
    {
        var comentario = new Comentario
        {
            UtilizadorId = utilizadorId,
            PublicacaoId = publicacaoId,
            Texto = comentarioDto.Texto,
            DataCriacao = DateTime.UtcNow
        };

        _contexto.Comentarios.Add(comentario);
        await _contexto.SaveChangesAsync();

        // Obter o autor da publicação
        var publicacao = await _contexto.Publicacoes
            .Include(p => p.Utilizador)
            .FirstOrDefaultAsync(p => p.Id == publicacaoId);

        if (publicacao?.Utilizador != null)
        {
            // Enviar email ao autor
            await _emailService.SendEmailAsync(
                publicacao.Utilizador.Email,
                "Novo comentário na sua publicação",
                $"<p>Você recebeu um novo comentário de <strong>{comentario.Utilizador.NomeUtilizador}</strong>:</p>" +
                $"<p>{System.Net.WebUtility.HtmlEncode(comentarioDto.Texto)}</p>"
            );
        }

        return ComentarioMappers.ToDto(comentario);
    }
}
```

### 6.4 Garantias associadas

- envio assíncrono;
- conexão SMTP com `StartTls`;
- autenticação explícita antes do envio;
- `DisconnectAsync(true)` para fechar a sessão corretamente;
- corpo HTML para mensagens formatadas.

## 7. FormFile e Garantias de Upload

O upload de ficheiros é tratado como parte do modelo de publicações.

### 7.1 Estrutura de dados

A DTO `CriarPublicacaoRequestDto` expõe:

**Modelo DTO:**

```csharp
public class CriarPublicacaoRequestDto
{
    [Required]
    public string Texto { get; set; } = string.Empty;
    
    public List<IFormFile>? Ficheiros { get; set; }
}
```

A entidade `FicheiroConteudo` guarda os metadados persistidos:

```csharp
[Table("tb_FicheirosConteudo")]
public class FicheiroConteudo
{
    [Key]
    public int Id { get; set; }
    
    public int PublicacaoId { get; set; }
    
    [ForeignKey("PublicacaoId")]
    public Publicacao Publicacao { get; set; }
    
    public string CaminhoFicheiro { get; set; }   // ex: "/uploads/uuid.jpg"
    public string TipoMime { get; set; }           // ex: "image/jpeg"
    public long TamanhoBytes { get; set; }         // Tamanho do ficheiro
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
}
```

### 7.2 Garantias de upload

O backend limita e documenta o suporte a uploads maiores:

**Configuração em `Program.cs`:**

```csharp
// Permitir uploads até 200 MB
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 209_715_200; // 200 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 209_715_200; // 200 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
```

**Endpoint do controller:**

```csharp
[HttpPost("{utilizadorId}")]
[Consumes("multipart/form-data")]
[RequestSizeLimit(209715200)]
[RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
public async Task<IActionResult> PublicarConteudo(
    [FromRoute] int utilizadorId,
    [FromForm] CriarPublicacaoRequestDto publicacaoDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var publicacao = await _publicacaoService.CriarAsync(
        utilizadorId,
        publicacaoDto
    );
    
    return CreatedAtAction(nameof(SelecionarPublicacao), 
        new { id = publicacao?.Id }, publicacao);
}
```

### 7.3 Suporte no Swagger

O projeto inclui filtros próprios para o Swagger:

**Registar filtros em `Program.cs`:**

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Nzola Network API", 
        Version = "v1" 
    });
    
    // Filtros para suporte a upload de ficheiros
    options.OperationFilter<FormFileOperationFilter>();
    options.SchemaFilter<FormFileSchemaFilter>();
    options.MapType<IFormFile>(() => new OpenApiSchema 
    { 
        Type = "string", 
        Format = "binary" 
    });
});
```

**Estrutura do filtro:**

```csharp
public class FormFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParameter = context.ApiDescription.ParameterDescriptions
            .FirstOrDefault(p =>
                p.Source?.Id?.Equals("Form", StringComparison.OrdinalIgnoreCase) == true
                && ContainsFormFile(p.Type)
            );

        if (formParameter?.Type == null)
            return;

        // Gera schema e processa ficheiros
        var schema = context.SchemaGenerator.GenerateSchema(
            formParameter.Type, 
            context.SchemaRepository
        );

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType { Schema = schema }
            }
        };
    }
}
```

Esses filtros transformam `IFormFile` em `string/binary` no OpenAPI e tratam também estruturas aninhadas. Isso garante que a documentação de upload de ficheiros fique utilizável no Swagger UI.

## 8. Resumo das Garantias Mais Importantes

- Email e nome de utilizador são únicos.
- O género é restrito a valores válidos.
- Relações críticas usam `Restrict` para evitar cascatas perigosas.
- `Baze` não permite duplicados por utilizador e publicação.
- Publicações suportam ficheiros com upload controlado.
- JWT usa assinatura simétrica e expiração.
- Email é enviado por SMTP com autenticação.
- O esquema evolui por migrations versionadas.

## 9. Conclusão

A arquitetura combina Data Annotations, Fluent API e migrations para assegurar integridade de dados, enquanto JWT, email e `FormFile` são tratados por serviços especializados. O resultado é um backend com regras de persistência explícitas, evolução controlada do esquema e mecanismos transversais separados do domínio principal.
