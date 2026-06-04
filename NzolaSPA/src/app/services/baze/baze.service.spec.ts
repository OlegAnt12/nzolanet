import { TestBed } from '@angular/core/testing';

import { BazeService } from './baze.service';

describe('BazeService', () => {
  let service: BazeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BazeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
