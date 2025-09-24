import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  NonNullableFormBuilder,
  Validators,
  FormGroup,
  FormArray,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';

import { CreateTaskDto, Prio } from '../../data/services/add-task';

type CreateTaskDtoWithPrio = CreateTaskDto & { prio?: Prio };

@Component({
  selector: 'app-add-task-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-task-modal.html',
  styleUrls: ['./add-task-modal.scss'],
})
export default class AddTaskModal {
  @Input() open = false;
  @Output() closed = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<CreateTaskDtoWithPrio>(); // ← fixed

  form: FormGroup;

  constructor(private fb: NonNullableFormBuilder) {
    this.form = this.fb.group(
      {
        title: this.fb.control('', [Validators.required, Validators.minLength(2)]),
        desc: this.fb.control(''),
        dueDate: this.fb.control(''),
        dueTime: this.fb.control(''),
        prio: this.fb.control<Prio>('Medium'),  // keep default
        subtasks: this.fb.array<FormGroup>([]),
      },
      { validators: [timeNeedsDate] }
    );
  }

  get subtasks(): FormArray<FormGroup> {
    return this.form.get('subtasks') as FormArray<FormGroup>;
  }

  addSubtask(text = '') {
    this.subtasks.push(
      this.fb.group({
        text: this.fb.control(text),
        done: this.fb.control(false),
      })
    );
  }
  removeSubtask(i: number) { this.subtasks.removeAt(i); }

  onBackdropClick() { this.closed.emit(); }

  onSubmit() {
    if (this.form.invalid) return;

    const raw = this.form.getRawValue() as any;
    const date: string = raw.dueDate || '';
    const time: string = raw.dueTime || '';

    let dueDateTimeLocal: string | undefined;
    let dueDateTimeISO: string | undefined;
    if (date && time) {
      dueDateTimeLocal = `${date}T${time}`;
      dueDateTimeISO = new Date(dueDateTimeLocal).toISOString();
    }

    this.submitted.emit({
      title: raw.title,
      desc: raw.desc,
      dueDate: date,
      dueTime: time,
      dueDateTimeLocal,
      dueDateTimeISO,
      prio: raw.prio as Prio,   // now valid thanks to widened emitter type
    });

    this.resetForm();
  }

  private resetForm() {
    this.subtasks.clear();
    this.form.reset({
      title: '',
      desc: '',
      dueDate: '',
      dueTime: '',
      prio: 'Medium' as Prio,
    });
  }
}

function timeNeedsDate(group: AbstractControl): ValidationErrors | null {
  const date = (group.get('dueDate')?.value as string) || '';
  const time = (group.get('dueTime')?.value as string) || '';
  if (!time) return null;
  return date ? null : { timeWithoutDate: true };
}
