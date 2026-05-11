## Utilizador

- Id: int
- NomeCompleto: string
- NomeUtilizador: string
- Email: string
- PlavraPasse: string
- NivelAcesso: NivelAcesso
- FotoPerfil: byte[]
- Biografia: string
- Privacidade: EstadoAcesso
- EstadoConta: EstadoConta
- DataRegisto: DateTime
- Seguidores: List<Seguidor>
- Publicacoes: List<Publicacao>
- Comentarios: List<Comentario>


## Seguidor

- Id: int
- SeguidorId: int
- SeguidoId: int
- DataInicio: DateTime


## Publicacao

- Id: int
- AutorId: int
- QuantidadeBazes: int
- QuantidadeComentarios: int
- DataPublicacao: DateTime
- Conteudos: List<ConteudoPublicacao>


## ConteudoPublicacao

- Id: int
- PublicacaoId: int
- Conteudo: string
- TipoConteudo: TipoConteudo
- Ordem: int


## Comentario

- Id: int
- PublicacaoId: int
- UtilizadorId: int
- Texto: string
- DataComentario: DateTime
- DataActualizacao: DateTime


## Notificacao

- Id: int
- UtilizadorId: int
- OrigemId: int
- Mensagem: string
- Lida: bool
- Tipo: TipoNotificacao
- DataCriacao: DateTime
- ReferenciaId: int


## Baze

- Id: int
- PublicacaoId: int
- UtilizadorId: int
- DataInteracao: DateTime