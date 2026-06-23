# Requisitos Funcionais da Nzola

Este documento descreve as funcionalidades principais da plataforma Nzola, com base nos controladores da WebAPI, nos servicos consumidos pela SPA e nas regras de negocio ja implementadas no projeto.

Complementa a documentacao tecnica em [API.md](API.md) e [GarantiasTecnicas.md](GarantiasTecnicas.md).

## 1. Objetivo do sistema

A Nzola e uma rede social orientada para publicacoes, interacoes entre utilizadores e notificacoes. O sistema permite registo, autenticacao, partilha de conteudo, comentarios, reacoes do tipo baze, seguimento de utilizadores e gestao de notificacoes.

## 2. Atores do sistema

- Visitante: consulta informacao publica e pode criar conta.
- Utilizador autenticado: publica conteudo, comenta, reage, segue outros utilizadores e gere a sua atividade.
- Sistema de email: envia confirmacoes e notificacoes por email.
- Base de dados: guarda utilizadores, publicacoes, comentarios, bazes, seguidores e notificacoes.

## 3. Requisitos funcionais

### 3.1 Registo e autenticacao

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-01 | Criar conta | O sistema deve permitir o registo de um novo utilizador com dados validos. |
| RF-02 | Iniciar sessao | O sistema deve autenticar o utilizador com email e palavra-passe. |
| RF-03 | Emitir token | O sistema deve gerar um token apos login com sucesso para acesso a operacoes autenticadas. |
| RF-04 | Validar credenciais | O sistema deve rejeitar login quando as credenciais forem invalidas. |

### 3.2 Gestao de utilizadores

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-05 | Listar utilizadores | O sistema deve apresentar a lista de utilizadores existentes. |
| RF-06 | Consultar utilizador | O sistema deve permitir ver os detalhes de um utilizador por identificador. |
| RF-07 | Remover utilizador | O sistema deve permitir apagar um utilizador quando essa operacao for autorizada. |

### 3.3 Publicacoes

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-08 | Listar publicacoes | O sistema deve mostrar o feed de publicacoes mais recentes. |
| RF-09 | Consultar publicacao | O sistema deve permitir abrir uma publicacao especifica por identificador. |
| RF-10 | Criar publicacao | O sistema deve permitir publicar texto e/ou ficheiros associados. |
| RF-11 | Editar publicacao | O sistema deve permitir atualizar uma publicacao existente. |
| RF-12 | Eliminar publicacao | O sistema deve permitir apagar uma publicacao existente. |

### 3.4 Comentarios

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-13 | Listar comentarios | O sistema deve listar os comentarios de uma publicacao. |
| RF-14 | Consultar comentario | O sistema deve permitir ver um comentario pelo seu identificador. |
| RF-15 | Adicionar comentario | O sistema deve permitir comentar uma publicacao. |
| RF-16 | Editar comentario | O sistema deve permitir alterar um comentario ja criado. |
| RF-17 | Eliminar comentario | O sistema deve permitir remover um comentario. |

### 3.5 Bazes

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-18 | Dar baze | O sistema deve permitir ao utilizador reagir a uma publicacao com baze. |
| RF-19 | Remover baze | O sistema deve permitir retirar a baze previamente dada. |
| RF-20 | Contabilizar bazes | O sistema deve mostrar o numero total de bazes de cada publicacao. |
| RF-21 | Evitar duplicados | O sistema deve impedir que o mesmo utilizador dê mais do que uma baze na mesma publicacao. |

### 3.6 Seguidores

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-22 | Seguir utilizador | O sistema deve permitir que um utilizador siga outro utilizador. |
| RF-23 | Deixar de seguir | O sistema deve permitir remover uma relacao de seguimento. |
| RF-24 | Listar seguidores | O sistema deve apresentar os seguidores de um utilizador. |
| RF-25 | Listar a seguir | O sistema deve apresentar os utilizadores que cada conta acompanha. |

### 3.7 Notificacoes

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-26 | Criar notificacao | O sistema deve permitir registar notificacoes internas. |
| RF-27 | Listar notificacoes | O sistema deve apresentar as notificacoes associadas a um utilizador. |
| RF-28 | Marcar como lida | O sistema deve permitir assinalar uma notificacao como lida. |
| RF-29 | Eliminar notificacao | O sistema deve permitir apagar uma notificacao. |

### 3.8 Email

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-30 | Enviar email de confirmacao | O sistema deve enviar um email apos o registo de um utilizador. |
| RF-31 | Enviar emails transacionais | O sistema deve permitir o envio de emails para eventos do sistema, como testes ou notificacoes. |

### 3.9 Upload de ficheiros

| ID | Requisito | Descricao |
| --- | --- | --- |
| RF-32 | Anexar ficheiros a publicacoes | O sistema deve aceitar ficheiros associados a uma publicacao. |
| RF-33 | Limitar tamanho do upload | O sistema deve aceitar uploads ate 200 MB. |
| RF-34 | Documentar uploads no Swagger | O sistema deve apresentar corretamente os campos de ficheiros na documentacao da API. |

## 4. Regras de negocio

- O email de um utilizador deve ser unico.
- O nome de utilizador deve ser unico.
- O campo genero deve aceitar apenas valores validos previstos pelo dominio.
- Um utilizador nao pode dar mais do que uma baze na mesma publicacao.
- A eliminacao de dados relacionados deve ser controlada para evitar cascatas indesejadas.
- Uma publicacao pode existir com texto, ficheiros ou ambos.
- O conteudo publicado deve respeitar os limites de tamanho definidos pelo backend.
- O token de autenticacao deve ser usado para operacoes protegidas.

## 5. Fluxos principais

### 5.1 Registo e entrada na plataforma

1. O visitante cria conta.
2. O sistema valida os dados e guarda o utilizador.
3. O sistema pode enviar email de confirmacao.
4. O utilizador faz login com email e palavra-passe.
5. O sistema devolve um token para acesso ao restante sistema.

### 5.2 Criacao de publicacao

1. O utilizador autenticado abre o formulario de publicacao.
2. O sistema aceita texto, ficheiros ou ambos.
3. O backend valida o conteudo e o tamanho dos ficheiros.
4. A publicacao e guardada e passa a surgir no feed.

### 5.3 Interacao com conteudo

1. O utilizador consulta o feed.
2. Pode abrir uma publicacao, comentar ou dar baze.
3. O sistema actualiza os contadores e a relacao entre utilizadores e conteudos.

### 5.4 Seguimento e notificacoes

1. O utilizador segue outro perfil.
2. O sistema regista a relacao de seguimento.
3. O utilizador pode receber notificacoes sobre atividade relevante.

## 6. Cobertura tecnica associada

Os requisitos acima sao suportados pelos seguintes componentes:

- [AutenticacoesController.cs](../NzolaWebAPI/Controllers/AutenticacoesController.cs)
- [UtilizadoresController.cs](../NzolaWebAPI/Controllers/UtilizadoresController.cs)
- [PublicacoesController.cs](../NzolaWebAPI/Controllers/PublicacoesController.cs)
- [ComentariosController.cs](../NzolaWebAPI/Controllers/ComentariosController.cs)
- [BazesController.cs](../NzolaWebAPI/Controllers/BazesController.cs)
- [SeguidorController.cs](../NzolaWebAPI/Controllers/SeguidorController.cs)
- [NotificacaoController.cs](../NzolaWebAPI/Controllers/NotificacaoController.cs)
- [EmailController.cs](../NzolaWebAPI/Controllers/EmailController.cs)
- [Program.cs](../NzolaWebAPI/Program.cs)
- [ContextoBDNzola.cs](../NzolaWebAPI/Data/ContextoBDNzola.cs)

## 7. Conclusao

A Nzola cobre os principais cenarios de uma rede social moderna: conta de utilizador, feed, publicacoes, comentarios, reacoes, seguimento, notificacoes, emails e upload de ficheiros. Este documento organiza essas funcionalidades em requisitos verificaveis e facilita a manutencao futura do projeto.
