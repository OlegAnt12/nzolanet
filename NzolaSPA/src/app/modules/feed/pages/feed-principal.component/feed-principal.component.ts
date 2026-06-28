import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PublicacaoService } from '../../../../services/publicacao/publicacao.service';
import { AuthService } from '../../../../services/auth/auth';
import { BazeService } from '../../../../services/baze/baze.service';
import { ComentariosService } from '../../../../services/comentario/comentarios.service';
import { RequisicaoCriarPublicacaoDto } from '../../../../dtos/publicacao/requisicao-criar-publicacao.dto';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { SeguidorService } from '../../../../services/seguidor/seguidor.service';
import { UtilizadorService } from '../../../../services/utilizador/utilizador.service';
import { ChangeDetectorRef } from '@angular/core';
import { EstatisticasUtilizadorDto } from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { NotificacaoDto, NovaNotificacaoDto } from '../../../../dtos/notificacao/notificacao.dto';
import { NotificacaoService } from '../../../../services/Notificacao/notificacao.service';
import { DenunciaService } from '../../../../services/denuncia/denuncia.service';
import { CriarDenunciaDto } from '../../../../dtos/denuncia/denuncia.dto';
import { Router } from '@angular/router';
import { Publicacao } from '../../components/publicacao/publicacao';
import { MiniPerfil } from '../../components/mini-perfil/mini-perfil';
import { ListaNotificacoes } from '../../components/lista-notificacoes/lista-notificacoes';

@Component({
  selector: 'app-feed-principal.component',
  imports: [
    CommonModule, ReactiveFormsModule,
    Publicacao, MiniPerfil, ListaNotificacoes,
  ],
  templateUrl: './feed-principal.component.html',
  styleUrl: './feed-principal.component.css',
})
export class FeedPrincipalComponent implements OnInit {
  feedPublicacoes: any[] = [];
  estatisticaUtilizador: EstatisticasUtilizadorDto = new EstatisticasUtilizadorDto();
  utilizadorLogado: any = null;
  carregandoFeed = true;
  enviarPost = false;

  publicacaoForm = new FormGroup({
    texto: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
  });

  ficheirosSelecionados: File[] = [];

  modoEdicaoPerfil = false;
  fotoSelecionadaPerfil: File | null = null;
  salvandoPerfilStatus = false;
  novaNotificacao: NovaNotificacaoDto = new NovaNotificacaoDto();
  listaNotificacoes: NotificacaoDto[] = [];
  carregandoNotificacoes = false;
  seguindoAutores = new Set<number>();

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private publicacaoService: PublicacaoService,
    private bazeService: BazeService,
    private seguidorService: SeguidorService,
    private comentarioService: ComentariosService,
    private authService: AuthService,
    private utilizadorService: UtilizadorService,
    private notificacaoService: NotificacaoService,
    private denunciaService: DenunciaService,
    private cdr: ChangeDetectorRef,
    private router: Router,
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.carregarDadosDoUtilizador();
      this.obterTodosOsPosts();
      this.obterEstatisticaUtilizador();
      this.obterNotificacoes();
    }
  }

  obterNotificacoes() {
    if (!isPlatformBrowser(this.platformId)) return;
    this.carregandoNotificacoes = true;
    const userId = Number(localStorage.getItem('utilizadorId'));
    if (!userId || isNaN(userId)) return;
    this.notificacaoService.listarPorUtilizador(userId).subscribe({
      next: (res: NotificacaoDto[]) => {
        this.listaNotificacoes = res;
        this.cdr.detectChanges();
      },
      error: (err) => console.log('Erro ao carregar notificações: ', err),
    });
  }

  obterEstatisticaUtilizador() {
    if (!this.utilizadorLogado?.id) return;
    this.utilizadorService.obterEstatisticas(Number(this.utilizadorLogado.id)).subscribe({
      next: (res) => { this.estatisticaUtilizador = res; },
      error: (erro) => console.error('Erro em estatisticas', erro),
    });
  }

  carregarDadosDoUtilizador(): void {
    if (isPlatformBrowser(this.platformId)) {
      const dadosLocais = localStorage.getItem('utilizadorLogado');
      if (dadosLocais) {
        const utilizadorReal = JSON.parse(dadosLocais);
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
          nomeUtilizador: utilizadorReal.nomeUtilizador || '',
          email: utilizadorReal.email,
          seguidores: utilizadorReal.quantidadeSeguidores ?? 0,
          seguindo: utilizadorReal.quantidadeSeguindo || 0,
          publicacoes: utilizadorReal.quantidadePublicacoes ?? 0,
          fotoPerfil: fotoPerfilProcessada,
          biografia: utilizadorReal.biografia ?? 'Sem biografia definida.',
          privacidade: utilizadorReal.privacidade,
        };
      }
    }
    this.cdr.detectChanges();
  }

  obterTodosOsPosts(): void {
    this.carregandoFeed = true;
    const utilizadorLogadoId = this.utilizadorLogado?.id;
    const obs = utilizadorLogadoId
      ? this.publicacaoService.listarFeed(utilizadorLogadoId)
      : this.publicacaoService.listarRecentes();

    obs.subscribe({
      next: (dados: any[]) => {
        const urlBaseBackend = 'http://localhost:5043';
        this.feedPublicacoes = dados.map((pub) => {
          if (pub.autor && pub.autor.fotoPerfil && Array.isArray(pub.autor.fotoPerfil)) {
            const byteArray = new Uint8Array(pub.autor.fotoPerfil);
            let binary = '';
            for (let i = 0; i < byteArray.length; i++) binary += String.fromCharCode(byteArray[i]);
            pub.autor.fotoPerfil = btoa(binary);
          }
          if (pub.ficheiros?.length) {
            pub.ficheiros = pub.ficheiros.map((f: any) => ({
              ...f, urlCompleta: `${urlBaseBackend}${f.caminhoFicheiro}`,
            }));
          }
          pub.mostrarComentarios = false;
          return pub;
        });
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
    if (event.target.files?.length) {
      this.ficheirosSelecionados = Array.from(event.target.files);
    }
  }

  fazerPublicacao(): void {
    if (this.publicacaoForm.invalid || this.enviarPost) return;
    if (!this.utilizadorLogado?.id) { alert('Sessão expirada.'); return; }
    this.enviarPost = true;
    const dadosParaEnvio: RequisicaoCriarPublicacaoDto = {
      texto: this.publicacaoForm.controls.texto.value,
      ficheiros: this.ficheirosSelecionados,
    };
    this.publicacaoService.publicar(dadosParaEnvio, Number(this.utilizadorLogado.id)).subscribe({
      next: (novoPost) => {
        this.feedPublicacoes.unshift(novoPost);
        this.publicacaoForm.reset();
        this.ficheirosSelecionados = [];
        this.enviarPost = false;
        this.cdr.detectChanges();
      },
      error: (erro) => {
        console.error(erro);
        this.enviarPost = false;
        alert('Não foi possível publicar.');
      },
    });
  }

  onBaze(publicacaoId: number): void {
    if (!this.utilizadorLogado?.id) return;
    this.bazeService.alternarBaze(publicacaoId, Number(this.utilizadorLogado.id)).subscribe({
      next: (resposta) => {
        const post = this.feedPublicacoes.find((p) => p.id === publicacaoId);
        if (post) {
          if (resposta.quantidadeBazes !== undefined) {
            post.quantidadeBazes = resposta.quantidadeBazes;
          } else {
            post.quantidadeBazes = (post.quantidadeBazes || 0) + 1;
          }
          if (post.autor?.id && post.autor.id !== this.utilizadorLogado.id) {
            this.novaNotificacao.utilizadorId = post.autor.id;
            this.novaNotificacao.tipo = 0;
            this.novaNotificacao.origemId = this.utilizadorLogado.id;
            this.novaNotificacao.mensagem = `${this.utilizadorLogado.nomeUtilizador} deu um baze à tua publicação`;
            this.notificacaoService.criarNotificacao(post.autor.id, this.novaNotificacao).subscribe();
          }
        }
        this.cdr.detectChanges();
      },
      error: (erro) => console.error('Erro no baze', erro),
    });
  }

  onComentar(publicacaoId: number): void {
    const post = this.feedPublicacoes.find((p) => p.id === publicacaoId);
    if (!post) return;
    post.mostrarComentarios = !post.mostrarComentarios;
    if (post.mostrarComentarios && !post.comentarios?.length) {
      this.comentarioService.listarPorPublicacao(publicacaoId).subscribe({
        next: (comentarios) => { post.comentarios = comentarios; this.cdr.detectChanges(); },
        error: (err) => console.error('Erro ao carregar comentários', err),
      });
    }
  }

  onEnviarComentario(event: { publicacaoId: number; texto: string }): void {
    if (!this.utilizadorLogado?.id) return;
    const post = this.feedPublicacoes.find((p) => p.id === event.publicacaoId);
    if (!post) return;
    this.comentarioService.adicionarComentario(
      event.publicacaoId,
      Number(this.utilizadorLogado.id),
      { conteudoComentario: event.texto }
    ).subscribe({
      next: (comentario) => {
        if (!post.comentarios) post.comentarios = [];
        post.comentarios.push(comentario);
        post.quantidadeComentarios = (post.quantidadeComentarios || 0) + 1;
        if (post.autor?.id && post.autor.id !== this.utilizadorLogado.id) {
          this.novaNotificacao.utilizadorId = post.autor.id;
          this.novaNotificacao.tipo = 1;
          this.novaNotificacao.origemId = this.utilizadorLogado.id;
          this.novaNotificacao.mensagem = `${this.utilizadorLogado.nomeUtilizador} comentou a tua publicação`;
          this.notificacaoService.criarNotificacao(post.autor.id, this.novaNotificacao).subscribe();
        }
        this.cdr.detectChanges();
      },
      error: (erro) => alert('Erro ao enviar comentário.'),
    });
  }

  onAlternarSeguir(autorId: number): void {
    if (!this.utilizadorLogado?.id || this.utilizadorLogado.id === autorId) return;
    this.seguidorService.alternarSeguir(this.utilizadorLogado.id, autorId).subscribe({
      next: (resposta) => {
        if (resposta.estaSeguindo) {
          this.seguindoAutores.add(autorId);
        } else {
          this.seguindoAutores.delete(autorId);
        }
        this.cdr.detectChanges();
      },
      error: (erro) => console.error('Erro ao seguir:', erro),
    });
  }

  onDenunciar(publicacaoId: number): void {
    if (!this.utilizadorLogado?.id) return;
    const motivo = prompt('Motivo da denúncia:');
    if (!motivo?.trim()) return;
    const descricao = prompt('Descrição (opcional):');
    const dto: CriarDenunciaDto = {
      tipoEntidade: 3,
      idEntidade: publicacaoId,
      motivo: motivo,
      descricao: descricao || '',
      denuncianteId: this.utilizadorLogado.id,
    };
    this.denunciaService.criarDenuncia(dto).subscribe({
      next: () => alert('Denúncia registada!'),
      error: () => alert('Erro ao registar denúncia.'),
    });
  }

  onEditarPublicacao(event: { id: number; texto: string }): void {}

  onSalvarEdicao(publicacaoId: number): void {}

  onEliminarPublicacao(publicacaoId: number): void {
    this.publicacaoService.eliminarPublicacao(publicacaoId).subscribe({
      next: () => {
        this.feedPublicacoes = this.feedPublicacoes.filter((pub) => pub.id !== publicacaoId);
        this.cdr.detectChanges();
      },
      error: () => alert('Erro ao eliminar publicação.'),
    });
  }

  onSalvarPerfil(dados: { nomeCompleto: string; biografia: string }): void {
    if (this.salvandoPerfilStatus) return;
    this.salvandoPerfilStatus = true;
    const id = Number(this.utilizadorLogado.id);
    this.utilizadorService.atualizarPerfil(id, dados.nomeCompleto, dados.biografia, this.fotoSelecionadaPerfil || undefined).subscribe({
      next: (resBackend) => {
        let fotoBase64: string | null = null;
        if (resBackend.fotoPerfil && Array.isArray(resBackend.fotoPerfil)) {
          const byteArray = new Uint8Array(resBackend.fotoPerfil);
          let binary = '';
          for (let i = 0; i < byteArray.length; i++) binary += String.fromCharCode(byteArray[i]);
          fotoBase64 = btoa(binary);
        }
        this.utilizadorLogado.nomeCompleto = resBackend.nomeCompleto;
        this.utilizadorLogado.biografia = resBackend.biografia;
        this.utilizadorLogado.fotoPerfil = fotoBase64;
        localStorage.setItem('utilizadorLogado', JSON.stringify(this.utilizadorLogado));
        this.modoEdicaoPerfil = false;
        this.salvandoPerfilStatus = false;
        this.cdr.detectChanges();
      },
      error: () => { this.salvandoPerfilStatus = false; alert('Erro ao salvar perfil.'); },
    });
  }

  onFotoPerfilSelecionada(file: File): void {
    this.fotoSelecionadaPerfil = file;
  }

  irParaPesquisa(): void {
    this.router.navigate(['/feed/pesquisa']);
  }

  executarLogout(): void {
    this.authService.logout();
  }
}
