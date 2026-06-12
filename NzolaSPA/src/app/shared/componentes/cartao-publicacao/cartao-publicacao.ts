import { Component, OnInit } from '@angular/core';
import { PublicacaoDto } from '../../../dtos/publicacao/publicacao.dto';
import { PublicacaoService } from '../../../services/publicacao/publicacao.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cartao-publicacao',
  imports: [CommonModule],
  templateUrl: './cartao-publicacao.html',
  styleUrl: './cartao-publicacao.css',
})
export class CartaoPublicacao implements OnInit {
  
  listaPublicacoes : PublicacaoDto[]=[];
  carregar: boolean=true;
  mensagemErro = '';

  constructor(private publicacaoService: PublicacaoService)
  {

  }

  ngOnInit(): void {
    this.carregarPublicacoesRecentes();
  }

  carregarPublicacoesRecentes(): void
  {
    this.carregar = true;
    /*this.publicacaoService.obterRecentes().subscribe(
      {
        next: (dados) => {
          this.listaPublicacoes = dados;
          this.carregar = false;
        },
        error:(err) => {
          console.error("Erro ao buscar publicacções", err);
          this.mensagemErro = "Não foi possível carregar publicações recentes";
          this.carregar = false;
        }
      }
    )*/
  }

}
