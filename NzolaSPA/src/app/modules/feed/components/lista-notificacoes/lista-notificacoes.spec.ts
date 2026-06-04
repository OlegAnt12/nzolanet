import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaNotificacoes } from './lista-notificacoes';

describe('ListaNotificacoes', () => {
  let component: ListaNotificacoes;
  let fixture: ComponentFixture<ListaNotificacoes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaNotificacoes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaNotificacoes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
