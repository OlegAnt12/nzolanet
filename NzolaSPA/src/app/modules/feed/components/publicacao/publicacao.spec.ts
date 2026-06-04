import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Publicacao } from './publicacao';

describe('Publicacao', () => {
  let component: Publicacao;
  let fixture: ComponentFixture<Publicacao>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Publicacao]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Publicacao);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
