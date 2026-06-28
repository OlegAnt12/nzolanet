import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { UtilizadorService } from '../../../../services/utilizador/utilizador.service';
import { SeguidorService } from '../../../../services/seguidor/seguidor.service';
import { PedidoSeguirService } from '../../../../services/pedido-seguir/pedido-seguir.service';
import { UtilizadorDto } from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';

@Component({
  selector: 'app-perfil.component',
  imports: [CommonModule, ReactiveFormsModule, Base64ImagePipe],
  templateUrl: './perfil.component.html',
  styleUrl: './perfil.component.css',
})
export class PerfilComponent implements OnInit {
  utilizador: UtilizadorDto = new UtilizadorDto();
  utilizadorLogadoId: number = 0;
  modoEdicao = false;
  fotoSelecionada: File | null = null;
  previewFotoUrl: string | null = null;
  salvando = false;
  pedidoPendente = false;
  perfilPrivado = false;

  perfilForm = new FormGroup({
    nomeCompleto: new FormControl('', Validators.required),
    biografia: new FormControl(''),
  });

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private route: ActivatedRoute,
    private utilizadorService: UtilizadorService,
    private seguidorService: SeguidorService,
    private pedidoSeguirService: PedidoSeguirService,
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const idStr = localStorage.getItem('utilizadorId');
      this.utilizadorLogadoId = idStr ? Number(idStr) : 0;
      const userId = Number(this.route.snapshot.paramMap.get('id')) || this.utilizadorLogadoId;
      this.carregarUtilizador(userId);
    }
  }

  carregarUtilizador(id: number): void {
    this.utilizadorService.obterPorId(id, this.utilizadorLogadoId || undefined).subscribe({
      next: (res) => {
        this.utilizador = res;
        this.perfilPrivado = res.privacidade === 1;
        if (!this.ehPerfilProprio && this.perfilPrivado) {
          this.verificarPedidoPendente();
        }
      },
      error: (err) => console.error('Erro ao carregar perfil:', err),
    });
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
        this.utilizador.nomeCompleto = res.nomeCompleto;
        this.utilizador.biografia = res.biografia;
        if (res.fotoPerfil && Array.isArray(res.fotoPerfil)) {
          const byteArray = new Uint8Array(res.fotoPerfil);
          let binary = '';
          for (let i = 0; i < byteArray.length; i++) binary += String.fromCharCode(byteArray[i]);
          this.utilizador.fotoPerfil = btoa(binary);
        }
        this.modoEdicao = false;
        this.salvando = false;
        const stored = JSON.parse(localStorage.getItem('utilizadorLogado') || '{}');
        stored.nomeCompleto = res.nomeCompleto;
        stored.biografia = res.biografia;
        stored.fotoPerfil = this.utilizador.fotoPerfil;
        localStorage.setItem('utilizadorLogado', JSON.stringify(stored));
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
          alert('Pedido de seguimento enviado!');
        },
        error: () => alert('Erro ao enviar pedido de seguimento.'),
      });
    } else if (!this.perfilPrivado) {
      this.seguidorService.alternarSeguir(this.utilizadorLogadoId, this.utilizador.id).subscribe({
        next: (res) => {
          this.utilizador.jaSegues = res.estaSeguindo;
          this.utilizador.seguidores = res.estaSeguindo
            ? this.utilizador.seguidores + 1
            : Math.max(0, this.utilizador.seguidores - 1);
        },
        error: (err) => console.error('Erro ao seguir:', err),
      });
    }
  }
}
