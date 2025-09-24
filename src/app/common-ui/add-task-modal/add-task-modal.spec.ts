import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddTaskModal } from './add-task-modal';

describe('AddTaskModal', () => {
  let component: AddTaskModal;
  let fixture: ComponentFixture<AddTaskModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddTaskModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddTaskModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
