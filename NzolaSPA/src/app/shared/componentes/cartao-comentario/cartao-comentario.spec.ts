import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartaoComentario } from './cartao-comentario';

describe('CartaoComentario', () => {
  let component: CartaoComentario;
  let fixture: ComponentFixture<CartaoComentario>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartaoComentario]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CartaoComentario);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
