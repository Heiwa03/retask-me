import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export type Prio = 'High' | 'Medium' | 'Low';

export interface Task {
  id: string;
  title: string;
  desc?: string;
  done: boolean;
  time: string;     // "07:00 - 09:00" sau "—"
  date?: string;
  dueDate?: string;
  dueTime?: string;
  prio?: Prio;      // <— persistăm prioritatea
}

export interface CreateTaskDto {
  title: string;
  desc?: string;
  dueDate?: string;
  dueTime?: string;
  dueDateTimeLocal?: string;
  dueDateTimeISO?: string;
  prio?: Prio;      // <— vine din formularul modalului
}

@Injectable({ providedIn: 'root' })
export class AddTask {
  private http = inject(HttpClient); // pregătit pentru backend mai târziu

  private readonly STORAGE_KEY = 'rtm-tasks';

  private _tasks = new BehaviorSubject<Task[]>(this.loadFromStorage());
  readonly tasks$ = this._tasks.asObservable();

  loadToday(): void {
    this._tasks.next(this.loadFromStorage());
  }

  create(dto: CreateTaskDto): Observable<Task> {
    const task: Task = {
      id: crypto?.randomUUID?.() ?? String(Date.now()),
      title: dto.title.trim(),
      desc: dto.desc || undefined,
      done: false,
      time: (dto.dueTime && dto.dueTime.trim()) ? dto.dueTime : '—',
      date: dto.dueDate || undefined,
      dueDate: dto.dueDate || undefined,
      dueTime: dto.dueTime || undefined,
      prio: dto.prio,              // <— SALVĂM prioritatea
    };

    const next = [task, ...this._tasks.value];
    this._tasks.next(next);
    this.saveToStorage(next);
    return of(task);
  }

  setDone(id: string, done: boolean): Observable<Task | null> {
    const updated = this.updateTask(id, t => ({ ...t, done }));
    return of(updated);
  }

  // <— NOU: persistă prioritatea
  setPriority(id: string, prio: Prio): Observable<Task | null> {
    const updated = this.updateTask(id, t => ({ ...t, prio }));
    return of(updated);
  }

  remove(id: string): Observable<boolean> {
    const next = this._tasks.value.filter(t => t.id !== id);
    this._tasks.next(next);
    this.saveToStorage(next);
    return of(true);
  }

  // ------- helpers -------
  private updateTask(id: string, fn: (t: Task) => Task): Task | null {
    const list = this._tasks.value;
    const idx = list.findIndex(t => t.id === id);
    if (idx === -1) return null;
    const next = [...list];
    next[idx] = fn(next[idx]);
    this._tasks.next(next);
    this.saveToStorage(next);
    return next[idx];
  }

  private loadFromStorage(): Task[] {
    try {
      const raw = JSON.parse(localStorage.getItem(this.STORAGE_KEY) || '[]') as Task[];
      return raw.map(t => ({ ...t }));
    } catch {
      return [];
    }
  }

  private saveToStorage(tasks: Task[]) {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(tasks));
  }
}
