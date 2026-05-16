import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Naosituado } from './naosituado';

describe('Naosituado', () => {
  let component: Naosituado;
  let fixture: ComponentFixture<Naosituado>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Naosituado]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Naosituado);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
