import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../../services/auth/auth';
import { Router, RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule, isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-login.component',
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  carregar = false;
  private isBrowser: boolean;

  ngOnInit(): void {
    if (this.isBrowser && localStorage.getItem('token')) {
      this.route.navigate(['/feed']);
    }
  }

  constructor(
    @Inject(PLATFORM_ID) platformId: Object,
    private fb: FormBuilder,
    private authService: AuthService,
    private route: Router,
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
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
          if (this.isBrowser) {
            localStorage.setItem('token', res.token);
            localStorage.setItem('refreshToken', res.refreshToken);
            localStorage.setItem('utilizadorId', res.utilizador?.id?.toString() ?? res.id);
            localStorage.setItem('utilizadorLogado', JSON.stringify(res.utilizador));
          }
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
