import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegistoRequestDto } from '../../../../dtos/utilizador/auth/registo/registo-request.dto';
import { AuthService } from '../../../../services/auth/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-registo.component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registo.component.html',
  styleUrl: './registo.component.css',
})
export class RegistoComponent {
  carregar = false;
  imagemBase64 : string | null = null;

  registoForm = new FormGroup(
    {
      nome: new FormControl<string>('',{nonNullable: true, validators:[Validators.required, Validators.minLength(3)]}),
      sobrenome: new FormControl<string>('',{nonNullable: true, validators:[Validators.required, Validators.minLength(3)]}),
      nomeUtilizador: new FormControl<string>('',{nonNullable: true, validators:[Validators.required, Validators.minLength(3)]}),
      email: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    palavraPasse: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
    genero: new FormControl<number>(0, { nonNullable: true, validators: [Validators.required] }),
    dataNascimento: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] })
    }
  );

  constructor(
      private fb: FormBuilder,
      private authService: AuthService,
      private route: Router,
    )
    {

    }

    aoSelecionarFoto(evento: any): void
    {
      const ficheiro  =  evento.target.files[0];
      if(ficheiro){
        const leitor = new FileReader();
        leitor.onload = () => {
          const resultadoStr = leitor.result as string;
          this.imagemBase64=resultadoStr.split(',')[1];
        };
        leitor.readAsDataURL(ficheiro);
      }
    }

    fazerRegisto(): void
    {
      if (this.registoForm.invalid) return;

    this.carregar = true;
      const dadosRegisto: RegistoRequestDto = {
        nomeCompleto: this.registoForm.controls.nome.value +' '+ this.registoForm.controls.sobrenome.value,
        nomeUtilizador: this.registoForm.controls.nomeUtilizador.value,
        email: this.registoForm.controls.email.value,
        palavraPasse: this.registoForm.controls.palavraPasse.value,
        genero: Number(this.registoForm.controls.genero.value),
        dataNascimento: this.registoForm.controls.dataNascimento.value,
        fotoPerfil: this.imagemBase64 // Aqui vai a string que o C# converterá em byte[]
      };

      this.authService.registar(dadosRegisto).subscribe({
        next: (resposta) => {
          alert('Conta criada com sucesso na NzolaNet!');
          this.carregar = false;
          this.route.navigate(['/home/login']);
        },
        error: (erro) => {
          console.error('Erro ao registar', erro);
          this.carregar = false;
          alert('Ocorreu um erro ao processar o registo no servidor.');
        }
      });
    }
}
