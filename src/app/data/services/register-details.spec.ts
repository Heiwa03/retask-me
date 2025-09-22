import { TestBed } from '@angular/core/testing';

import { RegisterDetailsService } from './register-details';

describe('RegisterDetails', () => {
  let service: RegisterDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RegisterDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
