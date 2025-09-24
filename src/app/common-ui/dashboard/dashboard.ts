import { Component, HostListener, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import AddTaskModal from '../add-task-modal/add-task-modal';
import { AddTask as TasksApi, Task as ApiTask, CreateTaskDto, Prio } from '../../data/services/add-task';

type TaskVM = ApiTask & { prioUI: '' | Prio };

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, AddTaskModal],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private api = inject(TasksApi);

  tasks: TaskVM[] = [];
  openIdx: number | null = null;

  selected: 'Light' | 'Dark' = 'Light';
  isAddOpen = false;

  ngOnInit(): void {
    this.api.tasks$.subscribe((list: ApiTask[]) => {
      this.tasks = list.map((t) => {
        const prevUI= this.tasks.find(x => x.id === t.id)?.prioUI;
        const prioUI: '' | Prio = (t.prio as Prio | undefined) ?? (prevUI ?? '');
        return { ...t, prioUI };
      });
    });
    this.api.loadToday();

    const saved = (localStorage.getItem('rtm-theme') as 'light' | 'dark') || 'light';
    this.apply(saved);
    this.selected = saved === 'dark' ? 'Dark' : 'Light';
  }

  toggleDone(i: number, ev: Event): void {
    const checked = (ev.target as HTMLInputElement).checked;
    const id = this.tasks[i].id;
    this.api.setDone(id, checked).subscribe();
  }

  togglePrio(i: number): void {
    this.openIdx = (this.openIdx === i) ? null : i;
  }

  setPrio(i: number, val: Prio): void {
    const id = this.tasks[i].id;
    // optimist în UI
    this.tasks[i].prioUI = val;
    this.openIdx = null;
    // persistă în storage
    this.api.setPriority(id, val).subscribe();
  }

  dotClass(prio: '' | Prio) {
    return { 'is-high': prio === 'High', 'is-med': prio === 'Medium', 'is-low': prio === 'Low' };
  }

  @HostListener('document:click') closeMenus() { this.openIdx = null; }

  setLight() { this.apply('light'); this.selected = 'Light'; }
  setDark()  { this.apply('dark');  this.selected = 'Dark'; }
  private apply(mode: 'light' | 'dark') {
    const html = document.documentElement;
    if (mode === 'dark') html.classList.add('theme-dark'); else html.classList.remove('theme-dark');
    localStorage.setItem('rtm-theme', mode);
  }

  openAddTask(){ this.isAddOpen = true;  document.body.style.overflow = 'hidden'; }
  closeAddTask(){ this.isAddOpen = false; document.body.style.overflow = ''; }
  saveTask(dto: CreateTaskDto){ this.api.create(dto).subscribe(() => this.closeAddTask()); }
}
