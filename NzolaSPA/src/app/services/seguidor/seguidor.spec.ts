import { TestBed } from '@angular/core/testing';

import { Seguidor } from './seguidor';

describe('Seguidor', () => {
  let service: Seguidor;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Seguidor);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
