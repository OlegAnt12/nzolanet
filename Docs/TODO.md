# NZOLANET — TODO

## Segurança
- [ ] **Hashing de passwords** — usar BCrypt em vez de texto plano
- [x] **Refresh token / expiração de JWT** — manter sessão segura sem login constante
- [ ] **Rate limiting** — proteção contra brute force no login/registo
- [ ] **SQL Server** — serviço `SQLEXPRESS` precisa de ser iniciado manualmente (`services.msc` ou SSMS)
- [ ] **Sanitização de input** — prevenir XSS e SQL injection

## Autenticação & Utilizador
- [ ] **Recuperação de password** — fluxo "esqueci a palavra-passe" com email
- [ ] **Confirmação de email** — enviar link de verificação após registo
- [x] **Editar perfil** — página de definições do utilizador (apenas na página de perfil)
- [x] **Eliminar conta** — opção de apagar conta com confirmação

## Funcionalidades Core
- [x] **Notificações em tempo real** — SignalR para notificar seguidores, bazes, comentários
- [x] **Baze em tempo real** — atualizar contagem de bazes via SignalR sem refresh
- [ ] **Mensagens privadas / Chat** — comunicação direta entre utilizadores
- [x] **Paginação infinita** — feed com scroll infinito
- [x] **Pesquisa global** — pesquisar utilizadores e publicações
- [ ] **Hashtags** — clicáveis e pesquisáveis
- [ ] **Partilha de publicações** — link copiável ou partilha interna

## Feed & Publicações
- [ ] **Feed algorítmico** — mostrar publicações relevantes primeiro
- [ ] **Reações múltiplas** — além de "baze" (gostos, risos, etc.)
- [ ] **Guardar publicações** — bookmarks/saved posts
- [x] **Denunciar conteúdo** — flag para moderação (frontend + backend ligados)

## UI/UX
- [x] **Responsividade completa** — media queries mobile/tablet para feed, perfil, admin, login, registo, esqueci-password, redefinir-password
- [ ] **Modo escuro** — tema dark/light
- [ ] **Animações de transição** — entre rotas e steps do wizard
- [x] **Toast/Snackbar** — feedback visual de ações (sucesso/erro)
- [ ] **Skeleton loading** — placeholders durante carregamento
- [ ] **Upload otimizado** — compressão de imagens antes de enviar

## Bugs Corrigidos
- [x] **StackOverflowException nos mappers** — ciclo infinito entre `ToAutorPublicacaoDto` ↔ `ToSeguidorFeedDto` (substituído `AutorPublicacaoDto` por `UtilizadorSimplificadoDto` no `SeguidorFeedDto`)
- [x] **Admin não acedia ao painel** — `nivelAcesso` perdido do `utilizadorLogado` ao serializar para `localStorage` no `FeedPrincipalComponent.atualizarListaSeguidos()`
- [x] **Login não redirecionava admin** — adicionada verificação `res.utilizador?.nivelAcesso === 1 ? '/admin' : '/feed'`
- [x] **Shadow FK `Baze.UtilizadorId1`** — `.WithMany()` sem especificar coleção `Bazes` causava segunda relação inferida por convenção (corrigido: `.WithMany(u => u.Bazes)`)
- [x] **Cores inconsistentes no módulo home** — `esqueci-password` e `redefinir-password` usavam `#4A90E2` (azul) em vez de `#ff375f` (padrão da app)
- [x] **AdminController sem [Authorize]** — qualquer endpoint admin podia ser chamado sem autenticação; adicionado `[Authorize(Roles = "Admin")]` + claim `ClaimTypes.Role` no JWT

## Funcionalidades Implementadas
- [x] **Notificações criadas pelo frontend** — ao dar baze, comentar e seguir, o frontend chama `POST /api/Notificacoes`
- [x] **Toggle de privacidade (Público/Privado)** — endpoint `PUT /api/Utilizadores/{id}/privacidade` + UI com optimistic update
- [x] **CSS completo do perfil** — header, detalhes, abas, listas, formulário de edição, toggle, responsivo
- [x] **Link de admin na UI** — botão "Admin" visível na navegação do feed apenas para `nivelAcesso === 1`
- [x] **Admin UI restrita** — admin não vê métricas, formulário de publicação, botões de seguir, tabs do perfil, nem "Editar Perfil"/"Eliminar Conta"
- [x] **[Authorize] no AdminController** — endpoints admin exigem token com role "Admin"; JWT inclui `ClaimTypes.Role` e `nivelAcesso`
- [x] **Responsividade completa** — media queries mobile/tablet em feed, perfil, admin, login, registo, esqueci-password, redefinir-password
- [x] **Documentação das alterações** — `Docs/SPA/Alteracoes_Sessao.md`

## Qualidade & Manutenção
- [ ] **Testes unitários (frontend)** — Jasmine/Karma para componentes e serviços
- [ ] **Testes unitários (backend)** — xUnit para controllers e serviços
- [ ] **Testes de integração** — fluxos completos (registo → login → publicar)
- [ ] **Logging estruturado** — Serilog ou similar no backend
- [ ] **Documentação da API** — Swagger com descrições detalhadas
- [ ] **CI/CD** — pipeline de build, teste e deploy

## Infraestrutura
- [ ] **Docker** — contentorização da SPA e WebAPI
- [ ] **Variáveis de ambiente** — centralizar configurações sensíveis
- [ ] **Backup de base de dados** — estratégia de backup automático
