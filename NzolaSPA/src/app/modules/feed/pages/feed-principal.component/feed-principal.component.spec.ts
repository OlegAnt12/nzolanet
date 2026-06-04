import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeedPrincipalComponent } from './feed-principal.component';

describe('FeedPrincipalComponent', () => {
  let component: FeedPrincipalComponent;
  let fixture: ComponentFixture<FeedPrincipalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeedPrincipalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeedPrincipalComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
