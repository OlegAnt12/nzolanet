import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartaoSeguidor } from './cartao-seguidor';

describe('CartaoSeguidor', () => {
  let component: CartaoSeguidor;
  let fixture: ComponentFixture<CartaoSeguidor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartaoSeguidor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CartaoSeguidor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
