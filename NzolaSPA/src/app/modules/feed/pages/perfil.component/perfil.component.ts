import { Component, Inject, OnInit, PLATFORM_ID, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { UtilizadorService } from '../../../../services/utilizador/utilizador.service';
import { SeguidorService } from '../../../../services/seguidor/seguidor.service';
import { PedidoSeguirService } from '../../../../services/pedido-seguir/pedido-seguir.service';
import { UtilizadorDto } from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { AuthService } from '../../../../services/auth/auth';
import { NotificacaoService } from '../../../../services/Notificacao/notificacao.service';
import { NovaNotificacaoDto } from '../../../../dtos/notificacao/notificacao.dto';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';
import { RouterModule } from '@angular/router';
import { PublicacaoService } from '../../../../services/publicacao/publicacao.service';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Component({
  selector: 'app-perfil.component',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, Base64ImagePipe],
  templateUrl: './perfil.component.html',
  styleUrl: './perfil.component.css',
})
export class PerfilComponent implements OnInit {
  private readonly publicacaoService = inject(PublicacaoService);

  utilizador: UtilizadorDto = new UtilizadorDto();
  utilizadorLogadoId: number = 0;
  utilizadorLogadoNivelAcesso: number = 0;
  dadosCarregados = false;
  modoEdicao = false;
  fotoSelecionada: File | null = null;
  previewFotoUrl: string | null = null;
  salvando = false;
  pedidoPendente = false;
  perfilPrivado = false;
  mostrarModalEliminarConta = false;
  eliminandoConta = false;
  notificacao: { mensagem: string; tipo: 'sucesso' | 'erro' } | null = null;
  estadoCarregamento = 'A carregar perfil...';
  carregandoListas = false;
  abaAtiva: 'publicacoes' | 'seguidores' | 'seguindo' = 'publicacoes';
  seguidoresPerfil: any[] = [];
  seguindoPerfil: any[] = [];
  publicacoesPerfil: any[] = [];

  generoTexto(genero: number | null | undefined): string {
    switch (genero) {
      case 0:
        return 'Masculino';
      case 1:
        return 'Feminino';
      default:
        return 'Não especificado';
    }
  }

  temBiografia(): boolean {
    return !!this.utilizador.biografia && this.utilizador.biografia.trim().length > 0;
  }

  mostrarNotificacao(mensagem: string, tipo: 'sucesso' | 'erro'): void {
    this.notificacao = { mensagem, tipo };
    setTimeout(() => { this.notificacao = null; }, 4000);
  }

  perfilForm = new FormGroup({
    nomeCompleto: new FormControl('', Validators.required),
    biografia: new FormControl(''),
  });

  private getLocalStorageItem(key: string): string | null {
    return isPlatformBrowser(this.platformId) ? localStorage.getItem(key) : null;
  }

  private setLocalStorageItem(key: string, value: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(key, value);
    }
  }

  private aplicarPerfilCarregado(perfil: UtilizadorDto): void {
    this.utilizador = perfil;
    this.perfilPrivado = perfil.privacidade === 1;
    this.dadosCarregados = true;

    this.carregarDadosPerfil();

    if (!this.ehPerfilProprio && this.perfilPrivado) {
      this.verificarPedidoPendente();
    }
  }

  selecionarAba(aba: 'publicacoes' | 'seguidores' | 'seguindo'): void {
    this.abaAtiva = aba;
  }

  nomeOuUtilizador(item: any): string {
    return item?.nomeCompleto || item?.nomeUtilizador || 'Utilizador';
  }

  fotoOuPadrao(item: any): string | null {
    return item?.fotoPerfil ?? null;
  }

  private carregarDadosPerfil(): void {
    if (!this.perfilCompletoVisivel) {
      return;
    }

    this.carregandoListas = true;

    forkJoin({
      seguidores: this.seguidorService.obterSeguidores(this.utilizador.id).pipe(catchError(() => of([]))),
      seguindo: this.seguidorService.obterSeguindo(this.utilizador.id).pipe(catchError(() => of([]))),
      publicacoes: this.publicacaoService
        .listarFeed(this.utilizadorLogadoId || undefined, 1, 200)
        .pipe(
          map((res: any) => (res?.publicacoes ?? res ?? []).filter((pub: any) => pub?.autor?.id === this.utilizador.id)),
          catchError(() => of([])),
        ),
    }).subscribe({
      next: (res) => {
        this.seguidoresPerfil = res.seguidores;
        this.seguindoPerfil = res.seguindo;
        this.publicacoesPerfil = res.publicacoes;
        this.carregandoListas = false;
      },
      error: () => {
        this.carregandoListas = false;
      },
    });
  }

  private sincronizarPerfilLocal(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const stored = JSON.parse(this.getLocalStorageItem('utilizadorLogado') || '{}');
    const payload = {
      ...stored,
      id: this.utilizador.id,
      nomeCompleto: this.utilizador.nomeCompleto,
      nomeUtilizador: this.utilizador.nomeUtilizador,
      email: this.utilizador.email,
      biografia: this.utilizador.biografia,
      privacidade: this.utilizador.privacidade,
      estadoConta: this.utilizador.estadoConta,
      fotoPerfil: this.utilizador.fotoPerfil,
      genero: this.utilizador.genero,
      dataNascimento: this.utilizador.dataNascimento,
      seguidores: this.utilizador.seguidores,
      seguindo: this.utilizador.seguindo,
      publicacoes: this.utilizador.publicacoes,
      jaSegues: this.utilizador.jaSegues,
    };

    this.setLocalStorageItem('utilizadorLogado', JSON.stringify(payload));
  }

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private route: ActivatedRoute,
    private router: Router,
    private utilizadorService: UtilizadorService,
    private seguidorService: SeguidorService,
    private pedidoSeguirService: PedidoSeguirService,
    private authService: AuthService,
    private notificacaoService: NotificacaoService,
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const idStr = this.getLocalStorageItem('utilizadorId');
      this.utilizadorLogadoId = idStr ? Number(idStr) : 0;
      const userStr = this.getLocalStorageItem('utilizadorLogado');
      if (userStr) {
        try {
          const userData = JSON.parse(userStr);
          this.utilizadorLogadoNivelAcesso = userData.nivelAcesso ?? 0;
        } catch { }
      }
    }

    const perfilResolvido = this.route.snapshot.data['perfil'] as UtilizadorDto | null | undefined;
    if (perfilResolvido) {
      this.aplicarPerfilCarregado(perfilResolvido);
      return;
    }

    this.estadoCarregamento = 'Não foi possível carregar o perfil.';
    this.dadosCarregados = true;
  }

  verificarPedidoPendente(): void {
    this.pedidoSeguirService.listarPendentes(this.utilizador.id).subscribe({
      next: (pedidos) => {
        this.pedidoPendente = pedidos.some(p => p.seguidorId === this.utilizadorLogadoId);
      },
      error: () => {},
    });
  }

  get ehPerfilProprio(): boolean {
    return this.utilizador.id === this.utilizadorLogadoId;
  }

  get perfilCompletoVisivel(): boolean {
    return this.ehPerfilProprio || this.utilizador.jaSegues || !this.perfilPrivado;
  }

  abrirEdicao(): void {
    this.modoEdicao = true;
    this.perfilForm.patchValue({
      nomeCompleto: this.utilizador.nomeCompleto,
      biografia: this.utilizador.biografia,
    });
  }

  adicionarBiografia(): void {
    this.abrirEdicao();
  }

  aoMudarFoto(event: any): void {
    if (event.target.files?.length) {
      this.fotoSelecionada = event.target.files[0];
      const reader = new FileReader();
      reader.onload = () => (this.previewFotoUrl = reader.result as string);
      if (this.fotoSelecionada) reader.readAsDataURL(this.fotoSelecionada);
    }
  }

  salvarPerfil(): void {
    if (this.perfilForm.invalid || this.salvando) return;
    this.salvando = true;
    const nome = this.perfilForm.value.nomeCompleto!;
    const bio = this.perfilForm.value.biografia || '';
    this.utilizadorService.atualizarPerfil(this.utilizador.id, nome, bio, this.fotoSelecionada || undefined).subscribe({
      next: (res) => {
        const utilizadorAtualizado: UtilizadorDto = {
          ...this.utilizador,
          nomeCompleto: res.nomeCompleto ?? this.utilizador.nomeCompleto,
          biografia: res.biografia ?? this.utilizador.biografia,
          privacidade: res.privacidade ?? this.utilizador.privacidade,
          estadoConta: res.estadoConta ?? this.utilizador.estadoConta,
          fotoPerfil: this.utilizador.fotoPerfil,
          genero: res.genero ?? this.utilizador.genero,
          dataNascimento: res.dataNascimento ?? this.utilizador.dataNascimento,
          seguidores: res.seguidores ?? this.utilizador.seguidores,
          seguindo: res.seguindo ?? this.utilizador.seguindo,
          publicacoes: res.publicacoes ?? this.utilizador.publicacoes,
          jaSegues: res.jaSegues ?? this.utilizador.jaSegues,
        };

        if (res.fotoPerfil && Array.isArray(res.fotoPerfil)) {
          const byteArray = new Uint8Array(res.fotoPerfil);
          let binary = '';
          for (let i = 0; i < byteArray.length; i++) binary += String.fromCharCode(byteArray[i]);
          utilizadorAtualizado.fotoPerfil = btoa(binary);
        }

        this.utilizador = utilizadorAtualizado;
        this.modoEdicao = false;
        this.salvando = false;
        this.sincronizarPerfilLocal();
      },
      error: (err) => {
        console.error('Erro ao salvar perfil:', err);
        this.salvando = false;
      },
    });
  }

  alternarSeguir(): void {
    if (!this.utilizadorLogadoId || this.utilizadorLogadoId === this.utilizador.id) return;

    if (this.utilizador.jaSegues) {
      this.seguidorService.alternarSeguir(this.utilizadorLogadoId, this.utilizador.id).subscribe({
        next: (res) => {
          this.utilizador.jaSegues = res.estaSeguindo;
          if (!res.estaSeguindo) this.utilizador.seguidores = Math.max(0, this.utilizador.seguidores - 1);
        },
        error: (err) => console.error('Erro ao deixar de seguir:', err),
      });
    } else if (this.perfilPrivado && !this.utilizador.jaSegues && !this.pedidoPendente) {
      this.pedidoSeguirService.solicitarSeguimento(this.utilizadorLogadoId, this.utilizador.id).subscribe({
        next: () => {
          this.pedidoPendente = true;
          this.mostrarNotificacao('Pedido de seguimento enviado!', 'sucesso');
        },
        error: () => this.mostrarNotificacao('Erro ao enviar pedido de seguimento.', 'erro'),
      });
    } else if (!this.perfilPrivado) {
      this.seguidorService.alternarSeguir(this.utilizadorLogadoId, this.utilizador.id).subscribe({
        next: (res) => {
          this.utilizador.jaSegues = res.estaSeguindo;
          this.utilizador.seguidores = res.estaSeguindo
            ? this.utilizador.seguidores + 1
            : Math.max(0, this.utilizador.seguidores - 1);
          if (res.estaSeguindo) {
            const utilizadorLogado = JSON.parse(this.getLocalStorageItem('utilizadorLogado') || '{}');
            const notif: NovaNotificacaoDto = {
              utilizadorId: this.utilizador.id,
              tipo: 2,
              origemId: this.utilizadorLogadoId,
              mensagem: `${utilizadorLogado.nomeUtilizador} começou a seguir-te`
            };
            this.notificacaoService.criarNotificacao(this.utilizador.id, notif).subscribe();
          }
        },
        error: (err) => console.error('Erro ao seguir:', err),
      });
    }
  }

  alternarPrivacidade(event: Event): void {
    const privada = (event.target as HTMLInputElement).checked;
    this.utilizador.privacidade = privada ? 1 : 0;
    this.perfilPrivado = privada;

    this.utilizadorService.atualizarPrivacidade(this.utilizador.id, privada).subscribe({
      error: () => {
        this.utilizador.privacidade = privada ? 0 : 1;
        this.perfilPrivado = !privada;
        this.mostrarNotificacao('Erro ao alterar privacidade.', 'erro');
      },
    });
  }

  confirmarEliminarConta(): void {
    this.mostrarModalEliminarConta = true;
  }

  cancelarEliminarConta(): void {
    this.mostrarModalEliminarConta = false;
  }

  executarEliminarConta(): void {
    this.eliminandoConta = true;
    this.utilizadorService.eliminarConta(this.utilizador.id).subscribe({
      next: () => {
        this.mostrarModalEliminarConta = false;
        this.eliminandoConta = false;
        this.authService.logout();
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Erro ao eliminar conta:', err);
        this.eliminandoConta = false;
        this.mostrarNotificacao('Não foi possível eliminar a conta.', 'erro');
      },
    });
  }
}
