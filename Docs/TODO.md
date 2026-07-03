# NZOLANET — TODO

## Segurança
- [ ] **Hashing de passwords** — usar BCrypt em vez de texto plano
- [x] **Refresh token / expiração de JWT** — manter sessão segura sem login constante
- [ ] **Rate limiting** — proteção contra brute force no login/registo
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
- [ ] **Responsividade** — adaptar interfaces para mobile/tablet
- [ ] **Modo escuro** — tema dark/light
- [ ] **Animações de transição** — entre rotas e steps do wizard
- [x] **Toast/Snackbar** — feedback visual de ações (sucesso/erro)
- [ ] **Skeleton loading** — placeholders durante carregamento
- [ ] **Upload otimizado** — compressão de imagens antes de enviar

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
