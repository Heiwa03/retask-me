import { TestBed } from '@angular/core/testing';

import { AddTask } from './add-task';

describe('AddTask', () => {
  let service: AddTask;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AddTask);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
