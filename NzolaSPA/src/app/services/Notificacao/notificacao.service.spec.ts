import { TestBed } from '@angular/core/testing';

import { NotificacaoService } from './notificacao.service';

describe('Notificacao', () => {
  let service: NotificacaoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificacaoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
