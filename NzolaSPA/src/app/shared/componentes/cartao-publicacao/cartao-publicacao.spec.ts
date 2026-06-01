import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartaoPublicacao } from './cartao-publicacao';

describe('CartaoPublicacao', () => {
  let component: CartaoPublicacao;
  let fixture: ComponentFixture<CartaoPublicacao>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartaoPublicacao]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CartaoPublicacao);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
