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

@Component({
  selector: 'app-feed-principal.component',
  imports: [
    CommonModule, // <-- 2. ADICIONA AQUI para libertar diretivas como *ngIf e *ngFor
    DatePipe, // <-- 2. ADICIONA AQUI para libertar o pipe de formatação de datas
    ReactiveFormsModule,
  ],
  templateUrl: './feed-principal.component.html',
  styleUrl: './feed-principal.component.css',
})
export class FeedPrincipalComponent implements OnInit {
  // Lista de publicações que alimenta o Feed
  feedPublicacoes: any[] = [];

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
  textoEdicaoControl = new FormControl('', Validators.required);
  estadoEditarPublicacao: boolean = false;
  estadoEliminarPublicacao: boolean= false;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private publicacaoService: PublicacaoService,
    private bazeService: BazeService,
    private seguidorService : SeguidorService,
    private comentarioService: ComentariosService,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.carregarDadosDoUtilizador();
    this.obterTodosOsPosts();
  }
  

  // Carrega do localStorage o perfil guardado no Login
  carregarDadosDoUtilizador(): void {
    // 1. Vai buscar a string do utilizador real armazenada no localStorage
    if (isPlatformBrowser(this.platformId)) {
      const dadosLocais = localStorage.getItem('utilizadorLogado');

      if (dadosLocais) {
        const utilizadorReal = JSON.parse(dadosLocais);
        this.utilizadorLogado = {
          id: utilizadorReal.id,
          nome: utilizadorReal.nomeCompleto || utilizadorReal.nomeUtilizador,
          username: utilizadorReal.nomeUtilizador ? `@${utilizadorReal.nomeUtilizador}` : '',
          email: utilizadorReal.email,
          seguidores: utilizadorReal.quantidadeSeguidores ?? 0,
          publicacoes: utilizadorReal.quantidadePublicacoes ?? 0,
          funcao: utilizadorReal.funcao || 'Membro da NzolaNet',
        };
      }
    } else {
      // Enquanto estiver a renderizar no Servidor (Node.js), deixamos valores vazios temporários
      this.utilizadorLogado = null;
    }
  }

  obterTodosOsPosts(): void {
    this.carregandoFeed = true;
    this.publicacaoService.listarRecentes().subscribe({
      next: (dados: any[]) => {
        const urlBaseBackend = 'http://localhost:5043';

        // Mapeia os posts para garantir que o caminho do upload aponta para o servidor C#
        const postsTratados = dados.map(pub => {
          if (pub.ficheiros && pub.ficheiros.length > 0) {
            pub.ficheiros = pub.ficheiros.map((file: any) => ({
              ...file,
              urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`
            }));
          }
          return pub;
        });

        // Proteção contra o bug NG0100
      setTimeout(() => {
        this.feedPublicacoes = postsTratados;
        this.carregandoFeed = false;
      }, 0);
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
      console.error('Erro: ID do utilizador não encontrado no localStorage. Valor atual:', utilizadorIdStr);
      alert('Sessão expirada ou inválida. Por favor, faz login novamente.');
      return;
    }

    console.log();
    const utilizadorId =this.utilizadorLogado.id;
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
      setTimeout(() => { this.enviarPost = false; }, 0);
      alert('Não foi possível processar a tua publicação no servidor.');
      },
    });
  }

  // Função para acionar o botão "Dar Baze" (Fogo/Raio)
  darBaze(publicacaoId: number): void {

    if (!this.utilizadorLogado || isNaN(this.utilizadorLogado.id)) {
      console.error('Erro: ID do utilizador não encontrado no localStorage. Valor atual:', this.utilizadorLogado);
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

  alternarSeguir(autor: any): void {

    // Validações de segurança
    if (!autor || !autor.id) return;

  if (!this.utilizadorLogado || isNaN(Number(this.utilizadorLogado.id))) {
    console.error('Erro: Utilizador não autenticado no sistema.');
    alert('Precisa estar autenticado para seguir este membro da NzolaNet!');
    return;
  }

  const seguidorId = Number(this.utilizadorLogado.id);
  const seguidoId = Number(autor.id);

  if (seguidorId === seguidoId) return;

  this.seguidorService.alternarSeguir(seguidorId, seguidoId).subscribe({
    next: (resposta) => {
      console.log('Ação processada:', resposta);
      
      // Atualização visual instantânea
      setTimeout(() => {
        // 1. Inverte o estado booleano do autor deste post específico
        autor.jaSegues = !autor.jaSegues;

        // 2. Varre todo o feed para atualizar os posts do MESMO autor, 
        // garantindo que todos os botões dele mudam ao mesmo tempo no ecrã
        this.feedPublicacoes.forEach(pub => {
          if (pub.autor && pub.autor.id === seguidoId) {
            pub.autor.jaSegues = autor.jaSegues;
          }
        });

      }, 0);
    },
    error: (erro) => {
      console.error('Erro ao alternar o estado de amizade:', erro);
      alert('Não foi possível atualizar o estado no servidor de momento.');
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
          post.carregandoComentarios = false;
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
      console.error('Erro: ID do utilizador não encontrado no localStorage. Valor atual:', this.utilizadorLogado);
      alert('Sessão expirada ou inválida. Por favor, faz login novamente.');
      return;
    }
    
    const novoComentarioObjeto: CriarComentarioDto = {
      conteudoComentario: this.comentarioForm.controls.textoComentario.value,
    };

    this.comentarioService.adicionarComentario(publicacaoId, Number(this.utilizadorLogado.id), novoComentarioObjeto).subscribe({
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
            urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`
          }));
        }
        // Altera o post na lista em tempo real
      setTimeout(() => {
        const index = this.feedPublicacoes.findIndex(p => p.id === publicacaoId);
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
        setTimeout(() => { this.estadoEditarPublicacao = false; }, 0);
      }
    });
  }

  selecionarEliminarPublicacao(pub: any): void {
    this.publicacaoAEditarId = pub.id;
  }

  cancelarEliminacao(): void {
    this.publicacaoAEditarId = null;
  }

  confirmarEliminarPublicacao(publicacaoId: number)
  {
    this.estadoEliminarPublicacao = true;
    const confirmacao = confirm('Tens a certeza de que queres eliminar esta publicação na NzolaNet? 🗑️');
  if (!confirmacao) return;
    this.publicacaoService.eliminarPublicacao(publicacaoId).subscribe(
      {
        next:() =>{
          setTimeout(() => {
            // Remove o post da lista local filtrando pelo ID
            this.feedPublicacoes = this.feedPublicacoes.filter(pub => pub.id !== publicacaoId);
            alert('A publicação foi removida com sucesso!');
          }, 0);
        },
        error: (err) => {
          console.error('Erro ao tentar eliminar a publicação:', err);
          alert('Não foi possível eliminar o post. Tenta novamente mais tarde.');
        }
      }
    );
  }

  executarLogout(): void {
    this.authService.logout();
  }
}
