import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaComentarios } from './lista-comentarios';

describe('ListaComentarios', () => {
  let component: ListaComentarios;
  let fixture: ComponentFixture<ListaComentarios>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaComentarios]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaComentarios);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
