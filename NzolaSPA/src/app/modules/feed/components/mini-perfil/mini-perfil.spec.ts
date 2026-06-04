import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiniPerfil } from './mini-perfil';

describe('MiniPerfil', () => {
  let component: MiniPerfil;
  let fixture: ComponentFixture<MiniPerfil>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MiniPerfil]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MiniPerfil);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
