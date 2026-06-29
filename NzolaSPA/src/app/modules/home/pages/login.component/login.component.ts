import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../../services/auth/auth';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login.component',
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  carregar = false;


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private route: Router,
  ) {
    this.loginForm = this.fb.group({
      identificador: ['', [Validators.required]],
      palavraPasse: ['', Validators.required],
    });
  }

  onLogin(): void {
    this.carregar=true;
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (res) => {
          console.log('Login realizado com sucesso:', res);
          localStorage.setItem('token', res.token);
          localStorage.setItem('utilizadorId', res.id);
          localStorage.setItem('utilizadorLogado', JSON.stringify(res.utilizador));
          this.carregar=false;
          this.route.navigate(['/feed']);
        },
        error: (erro) => {
          console.log("Falha no Login", erro);
          this.carregar=false;
        }
      });
    }
  }

  paraRegisto()
  {
    this.route.navigate(['/home/registo']);
  }
}
