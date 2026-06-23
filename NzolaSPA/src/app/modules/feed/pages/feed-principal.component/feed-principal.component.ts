import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PublicacaoService } from '../../../../services/publicacao/publicacao.service';
import { AuthService } from '../../../../services/auth/auth';
import { BazeService } from '../../../../services/baze/baze.service';
import { ComentariosService } from '../../../../services/comentario/comentarios.service';
import { RequisicaoCriarPublicacaoDto } from '../../../../dtos/publicacao/requisicao-criar-publicacao.dto';
import { CriarComentarioDto } from '../../../../dtos/comentario/comentario-dto';
import { CommonModule, DatePipe, isPlatformBrowser } from '@angular/common';
import { SeguidorService } from '../../../../services/seguidor/seguidor.service';
import { UtilizadorService } from '../../../../services/utilizador/utilizador.service';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';
import { ChangeDetectorRef } from '@angular/core';
import {
  EstatisticasUtilizadorDto,
  UtilizadorSimplificadoDto,
} from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { SeguidorDto } from '../../../../dtos/seguidor/seguidor.dto';
import { RequisicaoEditarComentarioDto } from '../../../../dtos/comentario/requisicao-editar-comentario-dto';

@Component({
  selector: 'app-feed-principal.component',
  imports: [
    CommonModule, // <-- 2. ADICIONA AQUI para libertar diretivas como *ngIf e *ngFor
    DatePipe, // <-- 2. ADICIONA AQUI para libertar o pipe de formatação de datas
    ReactiveFormsModule,
    Base64ImagePipe,
  ],
  templateUrl: './feed-principal.component.html',
  styleUrl: './feed-principal.component.css',
})
export class FeedPrincipalComponent implements OnInit {
  // Lista de publicações que alimenta o Feed
  feedPublicacoes: any[] = [];
  comentariosPublicacao: any[] = [];
  estatisticaUtilizador: EstatisticasUtilizadorDto = new EstatisticasUtilizadorDto();
  listaSeguidoresMeu: SeguidorDto[] = [];

  // Informações do utilizador logado (para a barra lateral esquerda)
  utilizadorLogado: any = null;

  carregandoFeed = true;
  enviarPost = false;

  // 1. Formulário para Criar uma Publicação
  publicacaoForm = new FormGroup({
    texto: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
  });

  // 2. Formulário para Criar um Comentário
  comentarioForm = new FormGroup({
    textoComentario: new FormControl<string>('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  // Guarda o ID da publicação ativa onde o utilizador quer comentar
  publicacaoEmFocoId: number | null = null;
  ficheirosSelecionados: File[] = [];

  publicacaoAEditarId: number | null = null;
  comentarioAEditarId: number | null = null;
  textoEdicaoControl = new FormControl('', Validators.required);
  comentarioEdicaoControl = new FormControl('', Validators.required);
  estadoEditarPublicacao: boolean = false;
  estadoEditarComentario: boolean = false;
  estadoEliminarPublicacao: boolean = false;
  estadoEliminarComentario: boolean = false;
  modoEdicaoPerfil = false;
  fotoSelecionadaPerfil: File | null = null;
  previewFotoUrl: string | null = null;
  salvandoPerfilStatus = false;

  novoComentario: RequisicaoEditarComentarioDto = new RequisicaoEditarComentarioDto();

  estadoSeguirCache = new Map<number, boolean>();

  perfilForm = new FormGroup({
    nomeCompleto: new FormControl('', [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(100),
    ]),
    biografia: new FormControl('', [Validators.maxLength(200)]),
  });

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private publicacaoService: PublicacaoService,
    private bazeService: BazeService,
    private seguidorService: SeguidorService,
    private comentarioService: ComentariosService,
    private authService: AuthService,
    private utilizadorService: UtilizadorService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.carregarDadosDoUtilizador();
    this.obterTodosOsPosts();
    this.obterEstatisticaUtilizador();
  }

  iniciarEdicaoComentario(comentario: any) {
    this.comentarioAEditarId = comentario.id;
    this.comentarioEdicaoControl.setValue(comentario.conteudoComentario);
  }

  confirmarEliminarComentario(id: number) {
    this.estadoEliminarComentario = true;
    const confirmacao = confirm(
      'Tens a certeza de que queres eliminar esta publicação na NzolaNet? 🗑️',
    );
    if (!confirmacao) return;
    this.comentarioService.excluirComentario(id).subscribe({
      next: () => {
        setTimeout(() => {
          // Remove o post da lista local filtrando pelo ID
          alert('O comentario foi removido com sucesso!');
        }, 0);
      },
      error: (err) => {
        console.error('Erro ao tentar eliminar o comentario:', err);
        alert('Não foi possível eliminar o comentario. Tenta novamente mais tarde.');
      },
    });
  }

  salvarEdicaoComentario(id: number) {
    if (this.comentarioEdicaoControl.invalid || this.estadoEditarComentario) return;

    this.estadoEditarComentario = true;
    this.novoComentario.conteudoComentario = this.comentarioEdicaoControl.value!;

    this.comentarioService.editarComentario(id, this.novoComentario).subscribe({
      next: (comentarioActualizado) => {
        alert(`O comentario foi actualizado ${comentarioActualizado}`);
        setTimeout(() => {
          
          this.cancelarEdicaoComentario();

          // CORREÇÃO DA PROPRIEDADE INEXISTENTE: Liberta o estado correto do componente
          this.estadoEditarComentario = false;
        }, 0);
      },
      error: (err) => {
        console.error('Erro ao salvar edição textual:', err);
        setTimeout(() => {
          this.estadoEditarComentario = false;
        }, 0);
      },
    });
  }

  cancelarEdicaoComentario() {
    this.comentarioAEditarId = null;
    this.comentarioEdicaoControl.reset();
  }

  // NOVO: Carregar estado de seguir do localStorage
  private carregarEstadoSeguirCache(): void {
    if (isPlatformBrowser(this.platformId)) {
      const seguidosStr = localStorage.getItem('seguidosIds');
      if (seguidosStr) {
        const seguidosIds = JSON.parse(seguidosStr);
        this.estadoSeguirCache.clear();
        seguidosIds.forEach((id: number) => {
          this.estadoSeguirCache.set(id, true);
        });
        console.log('Cache de seguidos carregado:', Array.from(this.estadoSeguirCache.keys()));
      }
    }
  }

  // NOVO: Salvar estado de seguir no localStorage
  private salvarEstadoSeguirCache(seguidoId: number, estaSeguindo: boolean): void {
    if (estaSeguindo) {
      this.estadoSeguirCache.set(seguidoId, true);
    } else {
      this.estadoSeguirCache.delete(seguidoId);
    }

    // Salvar no localStorage
    const seguidosArray = Array.from(this.estadoSeguirCache.keys());
    localStorage.setItem('seguidosIds', JSON.stringify(seguidosArray));
    console.log('Cache atualizado:', seguidosArray);
  }

  // NOVO: Aplicar estado de seguir do cache aos autores
  private aplicarEstadoSeguirCache(): void {
    this.feedPublicacoes.forEach((pub) => {
      if (pub.autor && pub.autor.id !== this.utilizadorLogado?.id) {
        // Verificar no cache
        const estaSeguindo = this.estadoSeguirCache.has(pub.autor.id);
        pub.autor.jaSegues = estaSeguindo;
        console.log(
          `Autor ${pub.autor.id} (${pub.autor.nome}): ${estaSeguindo ? 'SEGUE' : 'NÃO SEGUE'}`,
        );
      }
    });
    this.cdr.detectChanges();
  }

  obterEstatisticaUtilizador() {
    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      console.error(
        'Erro: ID do utilizador não encontrado no localStorage. Valor atual:',
        this.utilizadorLogado,
      );
      return;
    }
    const utilizadorLogadoId = Number(this.utilizadorLogado.id);
    this.utilizadorService.obterEstatisticas(utilizadorLogadoId).subscribe({
      next: (res) => {
        this.estatisticaUtilizador = res;
        console.log(this.estatisticaUtilizador);
      },
      error: (erro) => {
        console.error('Erro em estatisticas do Utilizador', erro);
        if (erro.error && typeof erro.error === 'string') {
          alert(erro.error);
        } else {
          alert('Não foi possível processar a interação de momento.');
        }
      },
    });
  }

  // Carrega do localStorage o perfil guardado no Login
  carregarDadosDoUtilizador(): void {
    if (isPlatformBrowser(this.platformId)) {
      const dadosLocais = localStorage.getItem('utilizadorLogado');

      if (dadosLocais) {
        const utilizadorReal = JSON.parse(dadosLocais);

        // Converte fotoPerfil se for byte[]
        let fotoPerfilProcessada = utilizadorReal.fotoPerfil;
        if (fotoPerfilProcessada && Array.isArray(fotoPerfilProcessada)) {
          const byteArray = new Uint8Array(fotoPerfilProcessada);
          let binary = '';
          for (let i = 0; i < byteArray.length; i++) {
            binary += String.fromCharCode(byteArray[i]);
          }
          fotoPerfilProcessada = btoa(binary);
        }

        this.utilizadorLogado = {
          id: utilizadorReal.id,
          nomeCompleto: utilizadorReal.nomeCompleto || utilizadorReal.nomeUtilizador,
          nomeUtilizador: utilizadorReal.nomeUtilizador ? `${utilizadorReal.nomeUtilizador}` : '',
          email: utilizadorReal.email,
          seguidores: utilizadorReal.quantidadeSeguidores ?? 0,
          seguindo: utilizadorReal.quantidadeSeguindo || 0, // NOVO
          publicacoes: utilizadorReal.quantidadePublicacoes ?? 0,
          fotoPerfil: fotoPerfilProcessada,
          biografia: utilizadorReal.biografia ?? 'Sem biografia definida.',
          privacidade: utilizadorReal.privacidade,
          genero: utilizadorReal.genero,
          dataNascimento: utilizadorReal.dataNascimento,
        };

        this.carregarListaSeguidos();
      }
    } else {
      this.utilizadorLogado = null;
    }
  }

  carregarListaSeguidos(): void {
    if (!this.utilizadorLogado?.id) return;

    // Buscar todos os IDs que este usuário segue
    this.seguidorService.listarSeguidos(this.utilizadorLogado.id).subscribe({
      next: (seguidos: SeguidorDto[]) => {
        console.log('IDs que o usuário segue:', seguidos);
        this.listaSeguidoresMeu = seguidos;
        console.log('Dados completos de seguidos:', seguidos);

        // Extrair apenas os IDs dos utilizadores seguidos
        const seguidosIds = seguidos
          .map((item) => item.seguido?.id) // Pega o ID do seguido
          .filter((id) => id !== undefined && id !== null); // Remove nulos

        console.log('IDs dos seguidos:', seguidosIds);
        localStorage.setItem('seguidosIds', JSON.stringify(seguidosIds));
      },
      error: (erro) => {
        console.error('ERRO ao listar seguidos:', erro);
        // Ver detalhes do erro
        console.log('Status:', erro.status);
        console.log('Mensagem:', erro.message);
      },
    });
    this.cdr.detectChanges();
  }

  // NOVO MÉTODO: Aplicar estado "jaSegues" a todos os autores
  private aplicarEstadoSeguir(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const seguidosIdsStr = localStorage.getItem('seguidosIds');
    let seguidosIds: number[] = [];

    try {
      const parsed = JSON.parse(seguidosIdsStr || '[]');
      // Se for array de objetos, extrair IDs
      if (Array.isArray(parsed) && parsed.length > 0) {
        if (typeof parsed[0] === 'object' && parsed[0].id !== undefined) {
          // É um array de objetos SeguidorDto
          seguidosIds = parsed.map((item) => item.seguido?.id || item.id).filter((id) => id);
        } else if (typeof parsed[0] === 'number') {
          // Já é um array de números
          seguidosIds = parsed;
        }
      }
    } catch (e) {
      console.error('Erro ao parsear seguidosIds:', e);
      seguidosIds = [];
    }

    console.log('IDs seguidos processados:', seguidosIds);
    const seguidosSet = new Set(seguidosIds);

    // Criar nova cópia para evitar mutação direta
    this.feedPublicacoes = this.feedPublicacoes.map((pub) => {
      if (pub.autor && pub.autor.id !== this.utilizadorLogado?.id) {
        const novoEstado = seguidosSet.has(pub.autor.id);
        console.log(`Autor ${pub.autor.id}: ${novoEstado ? 'SEGUE' : 'NÃO SEGUE'}`);
        return {
          ...pub,
          autor: {
            ...pub.autor,
            jaSegues: novoEstado,
          },
        };
      }
      return pub;
    });
    this.cdr.detectChanges();
  }

  abrirEdicaoPerfil(): void {
    this.modoEdicaoPerfil = true;
    this.previewFotoUrl = null;
    this.fotoSelecionadaPerfil = null;

    // Injeta os dados atuais mapeados da tua tabela
    this.perfilForm.patchValue({
      nomeCompleto: this.utilizadorLogado.nomeCompleto,
      biografia: this.utilizadorLogado.biografia,
    });
  }

  aoMudarFotoPerfil(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      this.fotoSelecionadaPerfil = event.target.files[0];

      // Gera um preview instantâneo na tela para o utilizador ver a foto antes de clicar em Confirmar
      const reader = new FileReader();
      reader.onload = () => (this.previewFotoUrl = reader.result as string);
      if (this.fotoSelecionadaPerfil != null) reader.readAsDataURL(this.fotoSelecionadaPerfil);
    }
  }

  salvarPerfil(): void {
    if (this.perfilForm.invalid || this.salvandoPerfilStatus) return;

    this.salvandoPerfilStatus = true;
    const id = Number(this.utilizadorLogado.id);
    const nome = this.perfilForm.value.nomeCompleto!;
    const bio = this.perfilForm.value.biografia || '';

    this.utilizadorService
      .atualizarPerfil(id, nome, bio, this.fotoSelecionadaPerfil || undefined)
      .subscribe({
        next: (resBackend) => {
          // Converte byte[] para Base64 string para o frontend
          let fotoBase64: string | null = null;

          if (resBackend.fotoPerfil && Array.isArray(resBackend.fotoPerfil)) {
            // Converte o array de bytes para Base64
            const byteArray = new Uint8Array(resBackend.fotoPerfil);
            let binary = '';
            for (let i = 0; i < byteArray.length; i++) {
              binary += String.fromCharCode(byteArray[i]);
            }
            fotoBase64 = btoa(binary);
          }

          // Atualiza o objeto na memória do Angular
          this.utilizadorLogado.nomeCompleto = resBackend.nomeCompleto;
          this.utilizadorLogado.biografia = resBackend.biografia;
          this.utilizadorLogado.fotoPerfil = fotoBase64; // Agora é string Base64

          // Salva de volta no LocalStorage
          localStorage.setItem('utilizadorLogado', JSON.stringify(this.utilizadorLogado));

          setTimeout(() => {
            this.modoEdicaoPerfil = false;
            this.salvandoPerfilStatus = false;
            alert('Perfil atualizado com sucesso na base de dados da NzolaNet! 🔄');
          }, 0);
        },
        error: (err: any) => {
          console.error('Erro ao salvar dados do utilizador:', err);
          setTimeout(() => {
            this.salvandoPerfilStatus = false;
          }, 0);
          alert('Erro ao atualizar dados no servidor.');
        },
      });
  }

  private converterFotoUtilizador(utilizador: any): any {
    if (utilizador && utilizador.fotoPerfil && Array.isArray(utilizador.fotoPerfil)) {
      const byteArray = new Uint8Array(utilizador.fotoPerfil);
      let binary = '';
      for (let i = 0; i < byteArray.length; i++) {
        binary += String.fromCharCode(byteArray[i]);
      }
      return {
        ...utilizador,
        fotoPerfil: btoa(binary),
      };
    }
    return utilizador;
  }

  obterTodosOsPosts(): void {
    this.carregandoFeed = true;
    this.publicacaoService.listarRecentes().subscribe({
      next: (dados: any[]) => {
        const urlBaseBackend = 'http://localhost:5043';

        const postsTratados = dados.map((pub) => {
          // Converte a foto do autor se necessário
          if (pub.autor) {
            pub.autor = this.converterFotoUtilizador(pub.autor);
          }

          if (pub.ficheiros && pub.ficheiros.length > 0) {
            pub.ficheiros = pub.ficheiros.map((file: any) => ({
              ...file,
              urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`,
            }));
          }

          return pub;
        });
        // Aplicar estado de seguir
        // FORÇAR DETEÇÃO DE ALTERAÇÕES
        this.feedPublicacoes = postsTratados;
        setTimeout(() => {
          console.log(this.estadoSeguirCache, ' Novo!');
          
        }, 0);
        this.aplicarEstadoSeguirCache();
        this.aplicarEstadoSeguir();
        this.carregandoFeed = false;
        this.cdr.detectChanges();
        
      },
      error: (err) => {
        console.error('Erro ao carregar o feed', err);
        this.carregandoFeed = false;
      },
    });
  }

  aoAdicionarImagensDoFeed(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      // Guarda os objetos do tipo File diretamente para enviar via FormData
      this.ficheirosSelecionados = Array.from(event.target.files);
    }
  }

  fazerPublicacao(): void {
    if (this.publicacaoForm.invalid || this.enviarPost) return;

    const utilizadorIdStr = localStorage.getItem('utilizadorId');
    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      console.error(
        'Erro: ID do utilizador não encontrado no localStorage. Valor atual:',
        utilizadorIdStr,
      );
      alert('Sessão expirada ou inválida. Por favor, faz login novamente.');
      return;
    }

    console.log();
    const utilizadorId = this.utilizadorLogado.id;
    this.enviarPost = true;

    const dadosParaEnvio: RequisicaoCriarPublicacaoDto = {
      texto: this.publicacaoForm.controls.texto.value,
      ficheiros: this.ficheirosSelecionados,
    };

    this.publicacaoService.publicar(dadosParaEnvio, utilizadorId).subscribe({
      next: (novoPost) => {
        setTimeout(() => {
          // Insere a nova publicação no topo do feed imediatamente
          this.feedPublicacoes.unshift(novoPost);

          // Reseta o ecrã e limpa os anexos de ficheiros
          this.publicacaoForm.reset();
          this.ficheirosSelecionados = [];
          this.enviarPost = false;

          alert('Publicado com sucesso na NzolaNet!');
        }, 0);
      },
      error: (erro) => {
        console.error(erro);
        this.enviarPost = false;
        console.error('Erro ao submeter publicação para o C#:', erro);
        setTimeout(() => {
          this.enviarPost = false;
        }, 0);
        alert('Não foi possível processar a tua publicação no servidor.');
      },
    });
  }

  // Função para acionar o botão "Dar Baze" (Fogo/Raio)
  darBaze(publicacaoId: number): void {
    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      console.error(
        'Erro: ID do utilizador não encontrado no localStorage. Valor atual:',
        this.utilizadorLogado,
      );
      alert('Precisas de estar autenticado para dar Baze! ');
      return;
    }

    this.bazeService.alternarBaze(publicacaoId, Number(this.utilizadorLogado.id)).subscribe({
      next: (resposta) => {
        const post = this.feedPublicacoes.find((p) => p.id === publicacaoId);

        if (post) {
          if (resposta.mensagem && resposta.mensagem.includes('removido')) {
            console.log('Baze retirado pelo utilizador.');
            post.quantidadeBazes = resposta.quantidadeBazes;
            post.jaDeuBaze = false;
          } else {
            console.log('Baze adicionado com sucesso!');
            post.quantidadeBazes++;
            post.jaDeuBaze = true;
          }
        }
      },
      error: (erro) => {
        console.error('Erro ao processar a ação de Baze no servidor', erro);
        if (erro.error && typeof erro.error === 'string') {
          alert(erro.error);
        } else {
          alert('Não foi possível processar a interação de momento.');
        }
      },
    });
  }

  /*alternarSeguir(autor: any): void {
    // Validações de segurança
    if (!autor || !autor.id) return;

    if (!this.utilizadorLogado || isNaN(Number(this.utilizadorLogado.id))) {
      console.error('Erro: Utilizador não autenticado no sistema.');
      alert('Precisa estar autenticado para seguir este membro da NzolaNet!');
      return;
    }

    const seguidorId = Number(this.utilizadorLogado.id);
    const seguidoId = Number(autor.id);

    if (seguidorId === seguidoId) {
      console.warn('Não é possível seguir a si mesmo');
      return;
    }

    // Salva o estado atual para possível rollback
    const estadoAnterior = autor.jaSegues;

    // Atualização otimista (UI responde imediatamente)
    autor.jaSegues = !autor.jaSegues;

    // Atualiza todos os posts do mesmo autor
    this.atualizarEstadoAutorNosPosts(seguidoId, autor.jaSegues);

    this.seguidorService.alternarSeguir(seguidorId, seguidoId).subscribe({
      next: (resposta) => {
        console.log('Ação processada com sucesso:', resposta);
        // Não precisa fazer nada pois a UI já está atualizada
        this.atualizarContagensAposSeguir(seguidoId, !autor.jaSegues);
      },
      error: (erro) => {
        console.error('Erro ao alternar o estado de seguir:', erro);

        // Rollback em caso de erro
        autor.jaSegues = estadoAnterior;
        this.atualizarEstadoAutorNosPosts(seguidoId, estadoAnterior);

        // Mensagem de erro mais específica
        let mensagemErro = 'Não foi possível atualizar o estado no servidor.';
        if (erro.status === 400) {
          mensagemErro = erro.error || 'Operação inválida.';
        } else if (erro.status === 404) {
          mensagemErro = 'Utilizador não encontrado.';
        }
        alert(mensagemErro);
      },
    });
  }*/

  // Atualizar o método alternarSeguir
  alternarSeguir(autor: UtilizadorSimplificadoDto): void {
    if (!autor?.id || !this.utilizadorLogado?.id) return;
    if (this.utilizadorLogado.id === autor.id) return;

    const seguidorId = this.utilizadorLogado.id;
    const seguidoId = autor.id;
    const estadoAnterior = autor.jaSegues;
    const novoEstado = !estadoAnterior;

    setTimeout(() => {
      autor.jaSegues = novoEstado;
      this.feedPublicacoes.forEach((pub) => {
        if (pub.autor?.id === seguidoId) {
          pub.autor.jaSegues = novoEstado;
        }
      });
    }, 0);

    // Atualizar cache local
    this.salvarEstadoSeguirCache(seguidoId, novoEstado);

    this.seguidorService.alternarSeguir(seguidorId, seguidoId).subscribe({
      next: (resposta) => {
        console.log('Sucesso:', resposta);
        this.atualizarListaSeguidos(seguidoId, novoEstado);
      },
      error: (erro) => {
        console.error('Erro:', erro);
        // Rollback
        // Rollback em caso de erro
        autor.jaSegues = estadoAnterior;
        this.feedPublicacoes.forEach((pub) => {
          if (pub.autor?.id === seguidoId) {
            pub.autor.jaSegues = estadoAnterior;
          }
        });

        // Reverter cache também
        if (estadoAnterior) {
          this.estadoSeguirCache.set(seguidoId, true);
        } else {
          this.estadoSeguirCache.delete(seguidoId);
        }
        localStorage.setItem(
          'seguidosIds',
          JSON.stringify(Array.from(this.estadoSeguirCache.keys())),
        );

        alert('Não foi possível atualizar. Tente novamente.');
      },
    });
  }

  private atualizarContagensAposSeguir(autorId: number, estaSeguindo: boolean): void {
    // Se o autor é o próprio usuário logado
    if (autorId === this.utilizadorLogado.id) {
      if (estaSeguindo) {
        // Alguém começou a seguir o usuário logado
        this.utilizadorLogado.seguidores++;
      } else {
        // Alguém deixou de seguir o usuário logado
        this.utilizadorLogado.seguidores--;
      }
    }
    // Se o usuário logado está seguindo/parando de seguir alguém
    else if (this.utilizadorLogado.id === autorId) {
      // Isso não deve acontecer (não segue a si mesmo)
    } else {
      // Atualizar contagem de "seguindo" do usuário logado
      if (estaSeguindo) {
        this.utilizadorLogado.seguindo++;
      } else {
        this.utilizadorLogado.seguindo--;
      }
    }
  }

  private atualizarListaSeguidos(seguidoId: number, estaSeguindo: boolean): void {
    // Recuperar lista atual
    let seguidosIdsStr = localStorage.getItem('seguidosIds');
    let seguidosIds = seguidosIdsStr ? JSON.parse(seguidosIdsStr) : [];

    if (estaSeguindo) {
      // Adicionar à lista
      if (!seguidosIds.includes(seguidoId)) {
        seguidosIds.push(seguidoId);
      }
    } else {
      // Remover da lista
      seguidosIds = seguidosIds.filter((id: number) => id !== seguidoId);
    }

    // Salvar atualizado
    localStorage.setItem('seguidosIds', JSON.stringify(seguidosIds));

    // Atualizar contador de "seguindo"
    this.utilizadorLogado.seguindo = seguidosIds.length;
    localStorage.setItem('utilizadorLogado', JSON.stringify(this.utilizadorLogado));
  }

  private atualizarEstadoAutorNosPosts(autorId: number, estado: boolean): void {
    this.feedPublicacoes.forEach((pub) => {
      if (pub.autor && pub.autor.id === autorId) {
        pub.autor.jaSegues = estado;
      }
    });
  }

  /**
   * Controla a abertura da caixa e faz o GET dos comentários em tempo real
   */
  selecionarParaComentar(publicacaoId: number): void {
    if (this.publicacaoEmFocoId === publicacaoId) {
      this.publicacaoEmFocoId = null;
      return;
    }

    this.publicacaoEmFocoId = publicacaoId;
    this.comentarioForm.reset();

    const post = this.feedPublicacoes.find((p) => p.id === publicacaoId);
    if (post) {
      post.carregandoComentarios = true;

      this.comentarioService.listarPorPublicacao(publicacaoId).subscribe({
        next: (comentariosVindosDaApi) => {
          post.comentarios = comentariosVindosDaApi;
          this.comentariosPublicacao = comentariosVindosDaApi;
          post.carregandoComentarios = false;

          console.log(post.comentarios);
        },
        error: (erro) => {
          console.error('Erro ao listar comentários da publicação ' + publicacaoId, erro);
          post.carregandoComentarios = false;
        },
      });
    }
  }

  /**
   * Envia o texto digitado no input como um novo comentário para o C#
   */
  enviarComentario(publicacaoId: number): void {
    if (this.comentarioForm.invalid) return;

    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      console.error(
        'Erro: ID do utilizador não encontrado no localStorage. Valor atual:',
        this.utilizadorLogado,
      );
      alert('Sessão expirada ou inválida. Por favor, faz login novamente.');
      return;
    }

    const novoComentarioObjeto: CriarComentarioDto = {
      conteudoComentario: this.comentarioForm.controls.textoComentario.value,
    };

    this.comentarioService
      .adicionarComentario(publicacaoId, Number(this.utilizadorLogado.id), novoComentarioObjeto)
      .subscribe({
        next: (comentarioGeradoPeloBackend) => {
          const post = this.feedPublicacoes.find((p) => p.id === publicacaoId);
          if (post) {
            if (!post.comentarios) post.comentarios = [];

            post.comentarios.push(comentarioGeradoPeloBackend);
            post.quantidadeComentarios++;
          }

          this.comentarioForm.reset();
        },
        error: (erro) => {
          console.error('Erro ao submeter comentário', erro);
          alert('Não foi possível enviar o teu comentário.');
        },
      });
  }

  iniciarEdicao(pub: any): void {
    this.publicacaoAEditarId = pub.id;
    this.textoEdicaoControl.setValue(pub.texto);
  }

  cancelarEdicao(): void {
    this.publicacaoAEditarId = null;
    this.textoEdicaoControl.reset();
  }

  salvarEdicao(publicacaoId: number): void {
    if (this.textoEdicaoControl.invalid || this.estadoEditarPublicacao) return;

    this.estadoEditarPublicacao = true;
    const novoTexto = this.textoEdicaoControl.value!;

    this.publicacaoService.atualizarTextoPublicacao(publicacaoId, novoTexto).subscribe({
      next: (pubActualizado) => {
        const urlBaseBackend = 'http://localhost:5043';

        // Mantém os caminhos das imagens antigas válidos injetando novamente a URL completa
        if (pubActualizado.ficheiros && pubActualizado.ficheiros.length > 0) {
          pubActualizado.ficheiros = pubActualizado.ficheiros.map((file: any) => ({
            ...file,
            urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`,
          }));
        }
        // Altera o post na lista em tempo real
        setTimeout(() => {
          const index = this.feedPublicacoes.findIndex((p) => p.id === publicacaoId);
          if (index !== -1) {
            this.feedPublicacoes[index] = pubActualizado;
          }
          this.cancelarEdicao();

          // CORREÇÃO DA PROPRIEDADE INEXISTENTE: Liberta o estado correto do componente
          this.estadoEditarPublicacao = false;
        }, 0);
      },
      error: (err) => {
        console.error('Erro ao salvar edição textual:', err);
        setTimeout(() => {
          this.estadoEditarPublicacao = false;
        }, 0);
      },
    });
  }

  selecionarEliminarPublicacao(pub: any): void {
    this.publicacaoAEditarId = pub.id;
  }

  cancelarEliminacao(): void {
    this.publicacaoAEditarId = null;
  }

  confirmarEliminarPublicacao(publicacaoId: number) {
    this.estadoEliminarPublicacao = true;
    const confirmacao = confirm(
      'Tens a certeza de que queres eliminar esta publicação na NzolaNet? 🗑️',
    );
    if (!confirmacao) return;
    this.publicacaoService.eliminarPublicacao(publicacaoId).subscribe({
      next: () => {
        setTimeout(() => {
          // Remove o post da lista local filtrando pelo ID
          this.feedPublicacoes = this.feedPublicacoes.filter((pub) => pub.id !== publicacaoId);
          alert('A publicação foi removida com sucesso!');
        }, 0);
      },
      error: (err) => {
        console.error('Erro ao tentar eliminar a publicação:', err);
        alert('Não foi possível eliminar o post. Tenta novamente mais tarde.');
      },
    });
  }

  executarLogout(): void {
    this.authService.logout();
  }
}
