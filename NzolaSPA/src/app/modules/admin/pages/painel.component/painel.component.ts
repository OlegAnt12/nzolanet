import { Component } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft, faFileExport, faSearch } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-painel.component',
  imports: [FontAwesomeModule],
  templateUrl: './painel.component.html',
  styleUrl: './painel.component.css',
})
export class PainelComponent {
  voltarIcon = faArrowLeft;
    exportarIcon = faFileExport;
    pesquisaIcon = faSearch;
}
