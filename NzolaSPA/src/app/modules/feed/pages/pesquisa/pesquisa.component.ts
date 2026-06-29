import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { PesquisaService } from '../../../../services/pesquisa/pesquisa.service';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-pesquisa',
  imports: [CommonModule, ReactiveFormsModule, Base64ImagePipe],
  templateUrl: './pesquisa.component.html',
  styleUrl: './pesquisa.component.css',
})
export class PesquisaComponent {
  termoControl = new FormControl('');
  tipoFiltro: string = 'tudo';
  publicacoes: any[] = [];
  perfis: any[] = [];
  carregando = false;
  pesquisou = false;

  constructor(
    private pesquisaService: PesquisaService,
    private router: Router,
  ) {
    this.termoControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      switchMap((termo) => {
        if (!termo || termo.length < 2) {
          this.publicacoes = [];
          this.perfis = [];
          this.pesquisou = false;
          return of(null);
        }
        this.carregando = true;
        return this.pesquisaService.pesquisar(termo, this.tipoFiltro);
      }),
    ).subscribe({
      next: (res) => {
        if (res) {
          this.publicacoes = res.publicacoes || [];
          this.perfis = res.perfis || [];
          this.pesquisou = true;
        }
        this.carregando = false;
      },
      error: () => { this.carregando = false; },
    });
  }

  definirFiltro(tipo: string): void {
    this.tipoFiltro = tipo;
    const termo = this.termoControl.value;
    if (termo && termo.length >= 2) {
      this.carregando = true;
      this.pesquisaService.pesquisar(termo, tipo).subscribe({
        next: (res) => {
          this.publicacoes = res.publicacoes || [];
          this.perfis = res.perfis || [];
          this.pesquisou = true;
          this.carregando = false;
        },
        error: () => { this.carregando = false; },
      });
    }
  }

  irParaPerfil(id: number): void {
    this.router.navigate(['/feed/perfil', id]);
  }

  voltar(): void {
    this.router.navigate(['/feed']);
  }
}
