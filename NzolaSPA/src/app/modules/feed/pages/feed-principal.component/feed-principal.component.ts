import { Component, ChangeDetectorRef, Inject, OnDestroy, OnInit, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PublicacaoService } from '../../../../services/publicacao/publicacao.service';
import { AuthService } from '../../../../services/auth/auth';
import { BazeService } from '../../../../services/baze/baze.service';
import { ComentariosService } from '../../../../services/comentario/comentarios.service';
import { RequisicaoCriarPublicacaoDto } from '../../../../dtos/publicacao/requisicao-criar-publicacao.dto';
import { CriarComentarioDto } from '../../../../dtos/comentario/comentario-dto';
import { CommonModule, DatePipe, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SeguidorService } from '../../../../services/seguidor/seguidor.service';
import { UtilizadorService } from '../../../../services/utilizador/utilizador.service';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { EstatisticasUtilizadorDto } from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { SeguidorDto } from '../../../../dtos/seguidor/seguidor.dto';
import { RequisicaoEditarComentarioDto } from '../../../../dtos/comentario/requisicao-editar-comentario-dto';
import { DenunciaService } from '../../../../services/denuncia/denuncia.service';
import { CriarDenunciaDto } from '../../../../dtos/denuncia/denuncia.dto';
import { PedidoSeguirService } from '../../../../services/pedido-seguir/pedido-seguir.service';
import { SignalRService } from '../../../../services/signalr/signalr.service';
import { NotificacaoService } from '../../../../services/Notificacao/notificacao.service';
import { NotificacaoDto } from '../../../../dtos/notificacao/notificacao.dto';
import { PesquisaService } from '../../../../services/pesquisa/pesquisa.service';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';
import { filter, Subscription } from 'rxjs';
import { faBell, faFlag, faHome, faMagnifyingGlass, faPowerOff } from '@fortawesome/free-solid-svg-icons';
import { faComment, faMessage, faUser } from '@fortawesome/free-regular-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-feed-principal.component',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    Base64ImagePipe,
    FontAwesomeModule
  ],
  templateUrl: './feed-principal.component.html',
  styleUrl: './feed-principal.component.css',
})
export class FeedPrincipalComponent implements OnInit, OnDestroy {
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
  publicacaoAEliminarId: number | null = null;
  comentarioAEditarId: number | null = null;
  comentarioAEliminarId: number | null = null;
  textoEdicaoControl = new FormControl('', Validators.required);
  comentarioEdicaoControl = new FormControl('', Validators.required);
  estadoEditarPublicacao: boolean = false;
  estadoEditarComentario: boolean = false;
  estadoEliminarPublicacao: boolean = false;
  estadoEliminarComentario: boolean = false;
  novoComentario: RequisicaoEditarComentarioDto = new RequisicaoEditarComentarioDto();

  estadoSeguirCache = new Set<number>();

  paginaAtual = 1;
  totalPublicacoes = 0;
  carregandoMais = false;
  fimDoFeed = false;
  private routerSub: Subscription | null = null;

  vistaAtiva: 'feed' | 'notificacoes' | 'pesquisa' = 'feed';
  termoPesquisa = '';
  resultadosPesquisa: any = null;
  pedidosPendentes: any[] = [];
  notificacoes: NotificacaoDto[] = [];
  carregandoNotificacoes = false;

  notifIcon=faBell;
  feedIcon=faHome;
  pesquisaIcon=faMagnifyingGlass;
  utilizadorIcon=faUser;
  comentarioIcon=faComment;
  bandeiraIcon=faFlag;
  mensagemIcon=faMessage;
  shutdownIcon=faPowerOff;

  base64Image(base64String: string | undefined | null): string {
    if (!base64String) return './profile/Deafultdavy3k.jfif';
    if (base64String.startsWith('data:image')) return base64String;
    let mimeType = 'image/jpeg';
    if (base64String.startsWith('iVBOR')) mimeType = 'image/png';
    else if (base64String.startsWith('R0lGOD')) mimeType = 'image/gif';
    else if (base64String.startsWith('UklGR')) mimeType = 'image/webp';
    return `data:${mimeType};base64,${base64String}`;
  }

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    public router: Router,
    private publicacaoService: PublicacaoService,
    private bazeService: BazeService,
    private seguidorService: SeguidorService,
    private comentarioService: ComentariosService,
    private authService: AuthService,
    private utilizadorService: UtilizadorService,
    private denunciaService: DenunciaService,
    private pedidoSeguirService: PedidoSeguirService,
    private signalRService: SignalRService,
    private notificacaoService: NotificacaoService,
    private pesquisaService: PesquisaService,
    private cdr: ChangeDetectorRef,
  ) {}

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  private getLocalStorageItem(key: string): string | null {
    return this.isBrowser() ? localStorage.getItem(key) : null;
  }

  private setLocalStorageItem(key: string, value: string): void {
    if (this.isBrowser()) {
      localStorage.setItem(key, value);
    }
  }

  ngOnInit(): void {
    this.carregarDadosDoUtilizador();
    this.obterTodosOsPosts();
    if (this.utilizadorLogado?.id) {
      this.obterEstatisticaUtilizador();
    }
    if (typeof window !== 'undefined') {
      window.addEventListener('scroll', this.onScroll.bind(this));
    }
    this.routerSub = this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        if (isPlatformBrowser(this.platformId)) {
          this.carregarDadosDoUtilizador();
          if (this.utilizadorLogado?.id) {
            this.obterEstatisticaUtilizador();
          }
          this.cdr.markForCheck();
        }
      });
    this.signalRService.baze$.subscribe((data: any) => {
      const post = this.feedPublicacoes.find((p) => p.id === data.publicacaoId);
      if (post) {
        post.quantidadeBazes = data.quantidadeBazes;
        post.jaDeuBaze = data.jaDeuBaze;
        this.cdr.markForCheck();
      }
    });
    if (isPlatformBrowser(this.platformId)) {
      this.carregarPedidosPendentes();
    }
  }

  ngOnDestroy(): void {
    if (this.routerSub) {
      this.routerSub.unsubscribe();
    }
  }

  iniciarEdicaoComentario(comentario: any) {
    this.comentarioAEditarId = comentario.id;
    this.comentarioEdicaoControl.setValue(comentario.conteudoComentario);
  }

  confirmarEliminarComentario(id: number) {
    this.comentarioAEliminarId = id;
  }

  cancelarEliminarComentario(): void {
    this.comentarioAEliminarId = null;
  }

  executarEliminarComentario(): void {
    if (!this.comentarioAEliminarId) return;
    this.estadoEliminarComentario = true;
    const id = this.comentarioAEliminarId;
    this.comentarioService.excluirComentario(id).subscribe({
      next: () => {
        this.comentarioAEliminarId = null;
        this.estadoEliminarComentario = false;
        this.mostrarNotificacao('Comentário removido com sucesso!', 'sucesso');
      },
      error: (err) => {
        console.error('Erro ao tentar eliminar o comentário:', err);
        this.estadoEliminarComentario = false;
        this.mostrarNotificacao('Não foi possível eliminar o comentário.', 'erro');
      },
    });
  }

  salvarEdicaoComentario(id: number) {
    if (this.comentarioEdicaoControl.invalid || this.estadoEditarComentario) return;

    this.estadoEditarComentario = true;
    this.novoComentario.conteudoComentario = this.comentarioEdicaoControl.value!;

    this.comentarioService.editarComentario(id, this.novoComentario).subscribe({
      next: (comentarioActualizado) => {
        this.mostrarNotificacao('Comentário atualizado com sucesso!', 'sucesso');
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
    if (this.isBrowser()) {
      const seguidosStr = this.getLocalStorageItem('seguidosIds');
      if (seguidosStr) {
        const seguidosIds = JSON.parse(seguidosStr);
        this.estadoSeguirCache.clear();
        seguidosIds.forEach((id: number) => {
            this.estadoSeguirCache.add(id);
        });
        //console.log('Cache de seguidos carregado:', Array.from(this.estadoSeguirCache));
      }
    }
  }

  // NOVO: Salvar estado de seguir no localStorage
  private salvarEstadoSeguirCache(seguidoId: number, estaSeguindo: boolean): void {
    if (estaSeguindo) {
      this.estadoSeguirCache.add(seguidoId);
    } else {
      this.estadoSeguirCache.delete(seguidoId);
    }

    // Salvar no localStorage
    const seguidosArray = Array.from(this.estadoSeguirCache);
    this.setLocalStorageItem('seguidosIds', JSON.stringify(seguidosArray));
    //console.log('Cache atualizado:', seguidosArray);
  }

  private aplicarEstadoSeguirCache(): void {
    this.feedPublicacoes.forEach((pub) => {
      if (pub.autor && pub.autor.id !== this.utilizadorLogado?.id) {
        const estaSeguindo = this.estadoSeguirCache.has(pub.autor.id);
        pub.autor.jaSegues = estaSeguindo;
      }
    });
    this.cdr.detectChanges();
  }

  obterEstatisticaUtilizador() {
    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      return;
    }
    const utilizadorLogadoId = Number(this.utilizadorLogado.id);
    this.utilizadorService.obterEstatisticas(utilizadorLogadoId).subscribe({
      next: (res) => {
        this.estatisticaUtilizador = res;
        //console.log(this.estatisticaUtilizador);
      },
      error: (erro) => {
        console.error('Erro em estatisticas do Utilizador', erro);
        if (erro.error && typeof erro.error === 'string') {
          this.mostrarNotificacao(erro.error, 'erro');
        } else {
          this.mostrarNotificacao('Não foi possível processar a interação.', 'erro');
        }
      },
    });
  }

  // Carrega do localStorage o perfil guardado no Login
  carregarDadosDoUtilizador(): void {
    if (this.isBrowser()) {
      const dadosLocais = this.getLocalStorageItem('utilizadorLogado');

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
        //console.log('IDs que o usuário segue:', seguidos);
        this.listaSeguidoresMeu = seguidos;
        //console.log('Dados completos de seguidos:', seguidos);

        // Extrair apenas os IDs dos utilizadores seguidos
        const seguidosIds = seguidos
          .map((item) => item.seguido?.id) // Pega o ID do seguido
          .filter((id) => id !== undefined && id !== null); // Remove nulos

        //console.log('IDs dos seguidos:', seguidosIds);
        this.setLocalStorageItem('seguidosIds', JSON.stringify(seguidosIds));
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

  carregarMaisPosts(): void {
    if (this.carregandoMais || this.fimDoFeed) return;

    this.carregandoMais = true;
    this.paginaAtual++;

    this.publicacaoService.listarFeed(this.utilizadorLogado?.id, this.paginaAtual).subscribe({
      next: (res: any) => {
        const dados = res.publicacoes ?? res;
        const urlBaseBackend = 'http://localhost:5043';

        const novosPosts = dados.map((pub: any) => {
          if (pub.autor) pub.autor = this.converterFotoUtilizador(pub.autor);
          if (pub.ficheiros && pub.ficheiros.length > 0) {
            pub.ficheiros = pub.ficheiros.map((file: any) => ({
              ...file,
              urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`,
            }));
          }
          return pub;
        });

        this.feedPublicacoes = [...this.feedPublicacoes, ...novosPosts];
        this.totalPublicacoes = res.total ?? this.feedPublicacoes.length;
        this.fimDoFeed = novosPosts.length === 0 || this.feedPublicacoes.length >= this.totalPublicacoes;
        this.carregandoMais = false;
        this.aplicarEstadoSeguirCache();
      },
      error: () => {
        this.carregandoMais = false;
        this.paginaAtual--;
      },
    });
  }

  onScroll(): void {
    if (this.carregandoMais || this.fimDoFeed) return;

    const scrollPos = window.innerHeight + window.scrollY;
    const threshold = document.documentElement.scrollHeight - 200;

    if (scrollPos >= threshold) {
      this.carregarMaisPosts();
    }
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
    this.paginaAtual = 1;
    this.fimDoFeed = false;
    this.publicacaoService.listarFeed(this.utilizadorLogado?.id, this.paginaAtual).subscribe({
      next: (res: any) => {
        const dados = res.publicacoes ?? res;
        this.totalPublicacoes = res.total ?? dados.length;
        this.fimDoFeed = dados.length === 0 || this.feedPublicacoes.length >= this.totalPublicacoes;
        const urlBaseBackend = 'http://localhost:5043';

        const postsTratados = dados.map((pub: any) => {
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
        this.feedPublicacoes = postsTratados;
        this.aplicarEstadoSeguirCache();
        this.carregandoFeed = false;
        this.carregandoMais = false;
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

    const utilizadorIdStr = this.getLocalStorageItem('utilizadorId');
    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      this.mostrarNotificacao('Sessão expirada. Faz login novamente.', 'erro');
      return;
    }
    
    const utilizadorId = this.utilizadorLogado.id;
    this.enviarPost = true;

    const dadosParaEnvio: RequisicaoCriarPublicacaoDto = {
      texto: this.publicacaoForm.controls.texto.value,
      ficheiros: this.ficheirosSelecionados,
    };

    this.publicacaoService.publicar(dadosParaEnvio, utilizadorId).subscribe({
      next: (novoPost: any) => {
        setTimeout(() => {
          const urlBaseBackend = 'http://localhost:5043';
          if (novoPost.ficheiros && novoPost.ficheiros.length > 0) {
            novoPost.ficheiros = novoPost.ficheiros.map((file: any) => ({
              ...file,
              urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`,
            }));
          }
          // Insere a nova publicação no topo do feed imediatamente
          this.feedPublicacoes.unshift(novoPost);

          // Reseta o ecrã e limpa os anexos de ficheiros
          this.publicacaoForm.reset();
          this.ficheirosSelecionados = [];
          this.enviarPost = false;

          this.mostrarNotificacao('Publicado com sucesso!', 'sucesso');
        }, 0);
      },
      error: (erro) => {
        console.error(erro);
        this.enviarPost = false;
        console.error('Erro ao submeter publicação para o C#:', erro);
        setTimeout(() => {
          this.enviarPost = false;
        }, 0);
        this.mostrarNotificacao('Não foi possível publicar. Tenta novamente.', 'erro');
      },
    });
  }

  // Função para acionar o botão "Dar Baze" (Fogo/Raio)
  darBaze(publicacaoId: number): void {
    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      this.mostrarNotificacao('Precisas de estar autenticado para dar Baze!', 'erro');
      return;
    }

    this.bazeService.alternarBaze(publicacaoId, Number(this.utilizadorLogado.id)).subscribe({
      next: (resposta) => {
        const post = this.feedPublicacoes.find((p) => p.id === publicacaoId);

        if (post) {
          if (resposta.mensagem && resposta.mensagem.includes('removido')) {
            //console.log('Baze retirado pelo utilizador.');
            post.quantidadeBazes = resposta.quantidadeBazes;
            post.jaDeuBaze = false;
          } else {
            //console.log('Baze adicionado com sucesso!');
            post.quantidadeBazes++;
            post.jaDeuBaze = true;
          }
          this.cdr.markForCheck();
        }
      },
      error: (erro) => {
        console.error('Erro ao processar a ação de Baze no servidor', erro);
        if (erro.error && typeof erro.error === 'string') {
          this.mostrarNotificacao(erro.error, 'erro');
        } else {
          this.mostrarNotificacao('Não foi possível processar a interação.', 'erro');
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

  alternarSeguir(autor: any): void {
    if (!autor?.id || !this.utilizadorLogado?.id) return;
    if (this.utilizadorLogado.id === autor.id) return;

    const seguidorId = this.utilizadorLogado.id;
    const seguidoId = autor.id;
    const estadoAnterior = this.estadoSeguirCache.has(seguidoId);
    const novoEstado = !estadoAnterior;

    this.salvarEstadoSeguirCache(seguidoId, novoEstado);
    this.atualizarEstadoAutorNosPosts(seguidoId, novoEstado);

    this.seguidorService.alternarSeguir(seguidorId, seguidoId).subscribe({
      next: (resposta) => {
        //console.log('Sucesso:', resposta);
        this.atualizarListaSeguidos(seguidoId, novoEstado);
      },
      error: (erro) => {
        console.error('Erro:', erro);
        this.salvarEstadoSeguirCache(seguidoId, estadoAnterior);
        this.atualizarEstadoAutorNosPosts(seguidoId, estadoAnterior);

        this.mostrarNotificacao('Não foi possível atualizar. Tente novamente.', 'erro');
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
    let seguidosIdsStr = this.getLocalStorageItem('seguidosIds');
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
    this.setLocalStorageItem('seguidosIds', JSON.stringify(seguidosIds));

    // Atualizar contador de "seguindo"
    this.utilizadorLogado.seguindo = seguidosIds.length;
    this.setLocalStorageItem('utilizadorLogado', JSON.stringify(this.utilizadorLogado));
  }

  private atualizarEstadoAutorNosPosts(autorId: number, estado: boolean): void {
    this.feedPublicacoes.forEach((pub) => {
      if (pub.autor && pub.autor.id === autorId) {
        pub.autor.jaSegues = estado;
      }
    });
    this.cdr.detectChanges();
  }

  isSeguindo(autorId: number): boolean {
    return this.estadoSeguirCache.has(autorId);
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

          //console.log(post.comentarios);
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
      this.mostrarNotificacao('Sessão expirada. Faz login novamente.', 'erro');
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
          this.mostrarNotificacao('Não foi possível enviar o comentário.', 'erro');
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
    this.publicacaoAEliminarId = publicacaoId;
  }

  cancelarEliminarPublicacao(): void {
    this.publicacaoAEliminarId = null;
  }

  executarEliminarPublicacao(): void {
    if (!this.publicacaoAEliminarId) return;
    this.estadoEliminarPublicacao = true;
    const id = this.publicacaoAEliminarId;
    this.publicacaoService.eliminarPublicacao(id).subscribe({
      next: () => {
        this.publicacaoAEliminarId = null;
        this.estadoEliminarPublicacao = false;
        this.feedPublicacoes = this.feedPublicacoes.filter((pub) => pub.id !== id);
        this.mostrarNotificacao('Publicação removida com sucesso!', 'sucesso');
      },
      error: (err) => {
        console.error('Erro ao tentar eliminar a publicação:', err);
        this.estadoEliminarPublicacao = false;
        this.mostrarNotificacao('Não foi possível eliminar a publicação.', 'erro');
      },
    });
  }

  denunciaEmProgresso: { id: number; tipo: number } | null = null;
  motivoDenuncia = '';
  mostrarFormDenuncia = false;

  notificacao: { mensagem: string; tipo: 'sucesso' | 'erro' } | null = null;

  mostrarNotificacao(mensagem: string, tipo: 'sucesso' | 'erro'): void {
    this.notificacao = { mensagem, tipo };
    this.cdr.markForCheck();
    setTimeout(() => {
      this.notificacao = null;
      this.cdr.markForCheck();
    }, 4000);
  }

  abrirDenuncia(idEntidade: number, tipoEntidade: number): void {
    this.denunciaEmProgresso = { id: idEntidade, tipo: tipoEntidade };
    this.mostrarFormDenuncia = true;
    this.motivoDenuncia = '';
  }

  cancelarDenuncia(): void {
    this.denunciaEmProgresso = null;
    this.mostrarFormDenuncia = false;
    this.motivoDenuncia = '';
  }

  enviarDenuncia(): void {
    if (!this.motivoDenuncia.trim() || !this.denunciaEmProgresso || !this.utilizadorLogado?.id) return;

    const dto: CriarDenunciaDto = {
      tipoEntidade: this.denunciaEmProgresso.tipo,
      idEntidade: this.denunciaEmProgresso.id,
      motivo: this.motivoDenuncia,
      descricao: this.motivoDenuncia,
      denuncianteId: this.utilizadorLogado.id,
    };

    this.denunciaService.criarDenuncia(dto).subscribe({
      next: () => {
        this.mostrarNotificacao('Denúncia enviada com sucesso!', 'sucesso');
        this.cancelarDenuncia();
      },
      error: () => this.mostrarNotificacao('Erro ao enviar denúncia.', 'erro'),
    });
  }

  executarLogout(): void {
    this.authService.logout();
  }

  alternarVista(vista: 'feed' | 'notificacoes' | 'pesquisa'): void {
    this.vistaAtiva = vista;
    if (vista === 'notificacoes' && this.utilizadorLogado?.id) {
      this.carregarNotificacoes();
    }
  }

  fazerPesquisa(): void {
    if (this.termoPesquisa.length < 2) return;
    this.vistaAtiva = 'pesquisa';
    this.pesquisaService.pesquisar(this.termoPesquisa).subscribe({
      next: (res) => { this.resultadosPesquisa = res; this.cdr.markForCheck(); },
      error: () => { this.resultadosPesquisa = null; this.cdr.markForCheck(); },
    });
  }

  carregarPedidosPendentes(): void {
    if (!this.utilizadorLogado?.id) return;
    this.pedidoSeguirService.listarPendentes(this.utilizadorLogado.id).subscribe({
      next: (res) => { this.pedidosPendentes = res; this.cdr.markForCheck(); },
      error: () => { this.pedidosPendentes = []; this.cdr.markForCheck(); },
    });
  }

  aceitarPedido(pedidoId: number): void {
    this.pedidoSeguirService.aceitarPedido(pedidoId).subscribe({
      next: () => {
        this.pedidosPendentes = this.pedidosPendentes.filter(p => p.id !== pedidoId);
        this.cdr.markForCheck();
      },
    });
  }

  rejeitarPedido(pedidoId: number): void {
    this.pedidoSeguirService.rejeitarPedido(pedidoId).subscribe({
      next: () => {
        this.pedidosPendentes = this.pedidosPendentes.filter(p => p.id !== pedidoId);
        this.cdr.markForCheck();
      },
    });
  }

  carregarNotificacoes(): void {
    if (!this.utilizadorLogado?.id) return;
    this.carregandoNotificacoes = true;
    this.notificacaoService.listarPorUtilizador(this.utilizadorLogado.id).subscribe({
      next: (res) => {
        this.notificacoes = res;
        this.carregandoNotificacoes = false;
        this.cdr.markForCheck();
      },
      error: () => { this.carregandoNotificacoes = false; this.cdr.markForCheck(); },
    });
  }
}
