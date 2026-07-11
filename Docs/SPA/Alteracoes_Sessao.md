# Alterações da Sessão — Julho 2026

## 1. Correção: Ciclo Infinito nos Mappers (StackOverflowException)

**Problema:** `SeguidorFeedDto` usava `AutorPublicacaoDto`, que por sua vez mapeava (`ToAutorPublicacaoDto`) chamava `ToSeguidorFeedDto`, criando um ciclo infinito.

**Solução:** Alterou-se `SeguidorFeedDto.Seguidor` de `AutorPublicacaoDto` para `UtilizadorSimplificadoDto`. O mapper `ToSeguidorFeedDto` passou a chamar `ToUtilizadorSimplificadoDto` em vez de `ToAutorPublicacaoDto`.

**Ficheiros alterados:**
- `NzolaWebAPI/Dtos/Seguidor/SeguidorFeedDto.cs` — tipo do campo `Seguidor` alterado
- `NzolaWebAPI/Mappers/SeguidorMappers.cs` — `ToSeguidorFeedDto` ajustado

---

## 2. Criação de Notificações no Frontend

**Decisão:** As notificações são criadas **apenas no frontend** chamando o endpoint `POST /api/Notificacoes` do backend, para evitar duplicação de lógica.

**Comportamento:**
- **Ao dar baze:** Cria notificação do tipo `Baze (0)` para o autor da publicação, se não for o próprio
- **Ao comentar:** Cria notificação do tipo `Comentario (1)` para o autor da publicação, se não for o próprio
- **Ao seguir:** Cria notificação do tipo `Seguidor (2)` para o utilizador seguido, se não for o próprio

**Ficheiros alterados:**
- `NzolaSPA/.../feed-principal.component.ts` — chamadas a `notificacaoService.criarNotificacao()` nos métodos `darBaze()`, `enviarComentario()` e `alternarSeguir()`

---

## 3. Toggle de Privacidade (Público/Privado)

**Backend:**
- Novo DTO: `AtualizarPrivacidadeRequestDto` (apenas `bool Privado`)
- Novo método: `UtilizadorService.AtualizarPrivacidadeAsync(int id, bool privado)`
- Novo endpoint: `PUT /api/Utilizadores/{id}/privacidade`

**Frontend:**
- Toggle visível apenas para o dono do perfil
- Optimistic update: o estado muda imediatamente na UI e reverte se o pedido falhar
- Estilo CSS de switch toggle (slider)

**Ficheiros alterados:**
- `NzolaWebAPI/Controllers/UtilizadoresController.cs`
- `NzolaWebAPI/Services/UtilizadorService.cs`
- `NzolaSPA/.../perfil.component.ts`, `.html`, `.css`

---

## 4. CSS Completo do Perfil

Estilização completa da página de perfil:
- Header com foto, nome, biografia e ações
- Detalhes do perfil (data de nascimento, género, privacidade)
- Abas de navegação (Publicações, Seguidores, Seguindo)
- Formulário de edição de perfil
- Toggle de privacidade com slider
- Responsivo para mobile/tablet

**Ficheiro:** `NzolaSPA/.../perfil.component.css`

---

## 5. Redirecionamento de Admin no Login

**Problema:** Admin era redirecionado para `/feed` como qualquer utilizador normal.

**Solução:** No `login.component.ts`, após o login bem-sucedido, verifica-se:
```typescript
res.utilizador?.nivelAcesso === 1 ? '/admin' : '/feed'
```

**Ficheiro:** `NzolaSPA/.../login.component.ts`

---

## 6. Preservação de `nivelAcesso` no FeedPrincipalComponent

**Problema:** `nivelAcesso` era perdido do `utilizadorLogado` no `FeedPrincipalComponent` sempre que `atualizarListaSeguidos()` era chamado, porque o método serializava/guardava o objeto em `localStorage` sem incluir `nivelAcesso`.

**Solução:** Adicionou-se `this.utilizadorLogado.nivelAcesso` ao objeto `utilizadorLogado` carregado em `carregarDadosDoUtilizador()`.

**Impacto:** O `adminGuard` (que lê `localStorage.utilizadorLogado.nivelAcesso`) passou a funcionar corretamente.

**Ficheiro:** `NzolaSPA/.../feed-principal.component.ts`

---

## 7. Responsividade (Media Queries)

### `feed.css` (layout global do feed)
- **Até 1024px:** `home-feed` muda para `flex-direction: column`; `.seccao-utilizador` e `.seccao-info` (sidebars) ocultam-se; `.seccao-publicacao` ocupa toda a largura com padding reduzido
- **Até 600px:** Ajustes finos no cabeçalho do feed, padding das publicações, altura dos media, layout da lista de interação

### `perfil.component.css`
- **Até 768px:** Header do perfil em coluna; foto menor (72px); detalhes em largura total; abas em coluna; formulário de edição adaptado; botões em largura total
- **769px–1024px:** Ajustes de padding e gaps para tablet

**Ficheiros alterados:**
- `NzolaSPA/.../feed.css`
- `NzolaSPA/.../perfil.component.css`

---

## 8. Link de Admin na UI (Pendente)

O botão/link de admin no feed navigation ainda **não foi implementado**. Para adicionar:
1. No `feed-principal.component.html`, dentro de `.sup-esq`, antes do botão "Sair":
```html
<button *ngIf="utilizadorLogado?.nivelAcesso === 1"
        type="button" class="btn btn-admin"
        routerLink="/admin">
  <span><fa-icon [icon="..."]></fa-icon> Admin</span>
</button>
```

---

## Tipos de Notificação (C# enum)
| Valor | Nome       | Descrição               |
|-------|------------|-------------------------|
| 0     | `Baze`     | Alguém deu baze         |
| 1     | `Comentario` | Alguém comentou       |
| 2     | `Seguidor` | Novo seguidor           |

