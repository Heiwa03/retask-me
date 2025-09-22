import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterDetails } from './register-details';

describe('RegisterDetails', () => {
  let component: RegisterDetails;
  let fixture: ComponentFixture<RegisterDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterDetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
