import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegistoRequestDto } from '../../../../dtos/utilizador/auth/registo/registo-request.dto';
import { AuthService } from '../../../../services/auth/auth';
import { Router, RouterModule } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { LoginDtos } from '../../../../dtos/utilizador/auth/login/login.dtos';
import { passwordMatch, dataNaoFuturaValidator, idadeMinimaValidator } from '../../../../core/validators/custom-validators';

@Component({
  selector: 'app-registo.component',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './registo.component.html',
  styleUrl: './registo.component.css',
})
export class RegistoComponent {
  currentStep = 1;
  carregar = false;
  imagemBase64: string | null = null;
  notificacao: { mensagem: string; tipo: 'sucesso' | 'erro' } | null = null;
  private isBrowser: boolean;

  mostrarNotificacao(mensagem: string, tipo: 'sucesso' | 'erro'): void {
    this.notificacao = { mensagem, tipo };
    setTimeout(() => { this.notificacao = null; }, 4000);
  }

  private camposPasso1 = ['nome', 'sobrenome', 'email', 'palavraPasse', 'confirmarPalavraPasse', 'concordaComTermos'];
  private camposPasso2 = ['nomeUtilizador', 'dataNascimento', 'genero'];

  registoForm = new FormGroup(
    {
      nome: new FormControl<string>('',{nonNullable: true, validators:[Validators.required, Validators.minLength(3)]}),
      sobrenome: new FormControl<string>('',{nonNullable: true, validators:[Validators.required, Validators.minLength(3)]}),
      nomeUtilizador: new FormControl<string>('',{nonNullable: true, validators:[Validators.required, Validators.minLength(3)]}),
      email: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
      palavraPasse: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
      confirmarPalavraPasse: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
      genero: new FormControl<number>(0, { nonNullable: true, validators: [Validators.required] }),
      dataNascimento: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, dataNaoFuturaValidator(), idadeMinimaValidator(14)] }),
      concordaComTermos: new FormControl<boolean>(false, { nonNullable: true, validators: [Validators.requiredTrue] })
    },
    { validators: passwordMatch('palavraPasse', 'confirmarPalavraPasse') }
  );

  constructor(
      @Inject(PLATFORM_ID) platformId: Object,
      private authService: AuthService,
      private route: Router,
    ) {
      this.isBrowser = isPlatformBrowser(platformId);
    }

  aoSelecionarFoto(evento: any): void {
    const ficheiro = evento.target.files[0];
    if (ficheiro) {
      const leitor = new FileReader();
      leitor.onload = () => {
        this.imagemBase64 = (leitor.result as string).split(',')[1];
      };
      leitor.readAsDataURL(ficheiro);
    }
  }

  camposDoPasso(passo: number): string[] {
    switch (passo) {
      case 1: return this.camposPasso1;
      case 2: return this.camposPasso2;
      default: return [];
    }
  }

  passoValido(passo: number): boolean {
    const campos = this.camposDoPasso(passo);
    return campos.every(c => {
      const control = this.registoForm.get(c);
      return control && control.valid;
    });
  }

  marcarCamposPasso(passo: number): void {
    this.camposDoPasso(passo).forEach(c => {
      const control = this.registoForm.get(c);
      if (control) control.markAsTouched();
    });
  }

  proximoPasso(): void {
    this.marcarCamposPasso(this.currentStep);
    if (!this.passoValido(this.currentStep)) return;
    this.currentStep++;
  }

  passoAnterior(): void {
    if (this.currentStep > 1) this.currentStep--;
  }

  ignorarFoto(): void {
    this.currentStep = 4;
  }

  get isUltimoPasso(): boolean {
    return this.currentStep === 3;
  }

  fazerRegisto(): void {
    if (this.registoForm.invalid) return;

    this.carregar = true;
    const dadosRegisto: RegistoRequestDto = {
      nomeCompleto: this.registoForm.controls.nome.value +' '+ this.registoForm.controls.sobrenome.value,
      nomeUtilizador: this.registoForm.controls.nomeUtilizador.value,
      email: this.registoForm.controls.email.value,
      palavraPasse: this.registoForm.controls.palavraPasse.value,
      genero: Number(this.registoForm.controls.genero.value),
      dataNascimento: this.registoForm.controls.dataNascimento.value,
      fotoPerfil: this.imagemBase64,
      concordaComTermos: this.registoForm.controls.concordaComTermos.value
    };

    this.authService.registar(dadosRegisto).subscribe({
      next: () => {
        const loginDados: LoginDtos = {
          identificador: dadosRegisto.email,
          palavraPasse: dadosRegisto.palavraPasse,
        };
        this.authService.login(loginDados).subscribe({
          next: (res) => {
            if (this.isBrowser) {
              localStorage.setItem('token', res.token);
              localStorage.setItem('refreshToken', res.refreshToken);
              localStorage.setItem('utilizadorId', res.id);
              if (res.utilizador) {
                localStorage.setItem('utilizadorLogado', JSON.stringify(res.utilizador));
              }
            }
            this.carregar = false;
            this.route.navigate(['/feed']);
          },
          error: () => {
            this.carregar = false;
            this.route.navigate(['/home/login']);
          }
        });
      },
      error: (erro) => {
        console.error('Erro ao registar', erro);
        this.carregar = false;
        this.mostrarNotificacao('Erro ao processar o registo.', 'erro');
      }
    });
  }
}