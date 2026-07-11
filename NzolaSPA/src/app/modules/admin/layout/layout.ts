import { ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID, afterNextRender } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faGear, faSignOutAlt, faChartLine, faUsers, faFileAlt, faFlag, faBell } from '@fortawesome/free-solid-svg-icons';
import { isPlatformBrowser, CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth/auth';
import { AdminService } from '../../../services/admin/admin.service';
import { Base64ImagePipe } from '../../../core/pipes/base64-image.pipe';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, FontAwesomeModule, CommonModule, Base64ImagePipe],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout implements OnInit {
  configIcon = faGear;
  notifIcon = faBell;
  sairIcon = faSignOutAlt;

  dashIcon = faChartLine;
  usersIcon = faUsers;
  postsIcon = faFileAlt;
  flagIcon = faFlag;

  adminNome: string = '';
  adminFoto: string | null = null;
  denunciasPendentes: number = 0;
  isBrowser: boolean;

  dropdownAberto = false;

  constructor(
    @Inject(PLATFORM_ID) platformId: Object,
    private authService: AuthService,
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {
    this.isBrowser = isPlatformBrowser(platformId);

    // Só faz pedidos HTTP no browser, após hidratação (evita falhas no SSR)
    /*afterNextRender(() => {
      
    });*/
  }

  private carregarDashboard(): void {
    this.adminService.obterDashboard().subscribe({
      next: (dados) => {
        this.denunciasPendentes = dados.denunciasPendentes;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Erro ao obter dashboard:', err);
      },
    });
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      const stored = localStorage.getItem('utilizadorLogado');
      if (stored) {
        try {
          const utilizador = JSON.parse(stored);
          this.adminNome = utilizador.nomeCompleto || utilizador.nomeUtilizador || 'Admin';
          this.adminFoto = utilizador.fotoPerfil || null;
        } catch {
          this.adminNome = 'Admin';
        }
      }
      this.carregarDashboard();
      this.cdr.markForCheck();
    }
  }

  alternarDropdown(): void {
    this.dropdownAberto = !this.dropdownAberto;
  }

  sair(): void {
    this.authService.logout();
  }
}
