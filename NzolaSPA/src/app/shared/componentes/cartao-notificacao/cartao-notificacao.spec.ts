import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartaoNotificacao } from './cartao-notificacao';

describe('CartaoNotificacao', () => {
  let component: CartaoNotificacao;
  let fixture: ComponentFixture<CartaoNotificacao>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartaoNotificacao]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CartaoNotificacao);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
