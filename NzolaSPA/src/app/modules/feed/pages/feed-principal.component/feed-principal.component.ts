import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PublicacaoService } from '../../../../services/publicacao/publicacao.service';
import { AuthService } from '../../../../services/auth/auth';
import { BazeService } from '../../../../services/baze/baze.service';
import { ComentariosService } from '../../../../services/comentario/comentarios.service';
import { RequisicaoCriarPublicacaoDto } from '../../../../dtos/publicacao/requisicao-criar-publicacao.dto';
import { CriarComentarioDto } from '../../../../dtos/comentario/comentario-dto';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-feed-principal.component',
  imports: [CommonModule,         // <-- 2. ADICIONA AQUI para libertar diretivas como *ngIf e *ngFor
    DatePipe,             // <-- 2. ADICIONA AQUI para libertar o pipe de formatação de datas
    ReactiveFormsModule],
  templateUrl: './feed-principal.component.html',
  styleUrl: './feed-principal.component.css',
})
export class FeedPrincipalComponent implements OnInit {
  // Lista de publicações que alimenta o Feed
  feedPublicacoes: any[] = [];
  
  // Informações do utilizador logado (para a barra lateral esquerda)
  utilizadorLogado: any = null;
  
  carregandoFeed = true;
  enviandoPost = false;

  // 1. Formulário para Criar uma Publicação
  publicacaoForm = new FormGroup({
    texto: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] })
  });

  // 2. Formulário para Criar um Comentário
  comentarioForm = new FormGroup({
    textoComentario: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] })
  });

  // Guarda o ID da publicação ativa onde o utilizador quer comentar
  publicacaoEmFocoId: number | null = null; 
  ficheirosSelecionados: File[] = [];

  constructor(
    private publicacaoService: PublicacaoService,
    private bazeService: BazeService,
    private comentarioService: ComentariosService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.carregarDadosDoUtilizador();
    this.obterTodosOsPosts();
  }

  // Carrega do localStorage o perfil guardado no Login
  carregarDadosDoUtilizador(): void {
    // 1. Vai buscar a string do utilizador real armazenada no localStorage
    const dadosLocais = localStorage.getItem('utilizadorLogado');
  
    if (dadosLocais) {
      const utilizadorReal = JSON.parse(dadosLocais);
  
      // 2. Mapeia as propriedades reais que o teu C# devolve no DTO
      this.utilizadorLogado = {
        id: utilizadorReal.id,
        nome: utilizadorReal.nomeCompleto || utilizadorReal.nomeUtilizador,
        username: utilizadorReal.nomeUtilizador ? `@${utilizadorReal.nomeUtilizador}` : '',
        email: utilizadorReal.email,
        
        // Se o teu backend já calcula estes campos, ele usa-os. 
        // Se vierem nulos da API, assume 0 (em vez de dados inventados)
        seguidores: utilizadorReal.quantidadeSeguidores ?? 0,
        publicacoes: utilizadorReal.quantidadePublicacoes ?? 0,
        funcao: utilizadorReal.funcao || 'Membro da NzolaNet' 
      };
    } else {
      // Caso de contingência se o utilizador tentar burlar a rota sem dados de sessão
      this.utilizadorLogado = null;
    }
  }

  obterTodosOsPosts(): void {
    this.carregandoFeed = true;
    this.publicacaoService.listarRecentes().subscribe({
      next: (dados: any[]) => {
        const urlBaseBackend = 'http://localhost:5043';
  
        // Mapeia os posts para garantir que o caminho do upload aponta para o servidor C#
        this.feedPublicacoes = dados.map(pub => {
          if (pub.ficheiros && pub.ficheiros.length > 0) {
            pub.ficheiros = pub.ficheiros.map((file: any) => ({
              ...file,
              // Cria uma propriedade nova com o link completo para usar no src do HTML
              urlCompleta: `${urlBaseBackend}${file.caminhoFicheiro}`
            }));
          }
          return pub;
        });
  
        this.carregandoFeed = false;
      },
      error: (err) => {
        console.error('Erro ao carregar o feed', err);
        this.carregandoFeed = false;
      }
    });
  }

  aoAdicionarImagensDoFeed(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      // Guarda os objetos do tipo File diretamente para enviar via FormData
      this.ficheirosSelecionados = Array.from(event.target.files);
    }
  }
  
  fazerPublicacao(): void {
    if (this.publicacaoForm.invalid || this.enviandoPost) return;
  
    this.enviandoPost = true;

    const dadosParaEnvio: RequisicaoCriarPublicacaoDto = {
      texto: this.publicacaoForm.controls.texto.value,
      ficheiros: this.ficheirosSelecionados
    };
  
    this.publicacaoService.publicar(dadosParaEnvio).subscribe({
      next: (novoPost) => {
        this.feedPublicacoes.unshift(novoPost);
        this.publicacaoForm.reset();
        this.ficheirosSelecionados = []; // Limpa o anexo
        this.enviandoPost = false;
        alert('Publicado com sucesso na NzolaNet! ');
      },
      error: (erro) => {
        console.error(erro);
        this.enviandoPost = false;
      }
    });
  }

  // Função para acionar o botão "Dar Baze" (Fogo/Raio)
  darBaze(publicacaoId: number): void {
    const utilizadorIdStr = localStorage.getItem('utilizadorId');
    
    if (!utilizadorIdStr) {
      alert('Precisas de estar autenticado para dar Baze! ');
      return;
    }
  
    const utilizadorId = Number(utilizadorIdStr);
    const bazeDto = {};
  
    this.bazeService.alternarBaze(publicacaoId, utilizadorId, bazeDto).subscribe({
      next: (resposta) => {
        const post = this.feedPublicacoes.find(p => p.id === publicacaoId);
        
        if (post) {
          if (resposta.mensagem && resposta.mensagem.includes('removido')) {
            console.log('Baze retirado pelo utilizador.');
            post.quantidadeBazes = resposta.quantidadeBazes;
            post.jaDeuBaze = false;
          } 
          else {
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
  
    const post = this.feedPublicacoes.find(p => p.id === publicacaoId);
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
        }
      });
    }
  }
  
  /**
   * Envia o texto digitado no input como um novo comentário para o C#
   */
  enviarComentario(publicacaoId: number): void {
    if (this.comentarioForm.invalid) return;
  
    const utilizadorIdStr = localStorage.getItem('utilizadorId');
    if (!utilizadorIdStr) {
      alert('Sessão expirada. Por favor, faz login novamente.');
      return;
    }
  
    const novoComentarioObjeto: CriarComentarioDto = {
      publicacaoId: publicacaoId,
      utilizadorId: Number(utilizadorIdStr),
      conteudoComentario: this.comentarioForm.controls.textoComentario.value
    };
  
    this.comentarioService.adicionarComentario(novoComentarioObjeto).subscribe({
      next: (comentarioGeradoPeloBackend) => {
        const post = this.feedPublicacoes.find(p => p.id === publicacaoId);
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
      }
    });
  }

  executarLogout(): void {
    this.authService.logout();
  }
}
