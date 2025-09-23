import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

type Prio = '' | 'High' | 'Medium' | 'Low';

interface Task {
  title: string;
  time: string;
  done: boolean;
  prio: Prio;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {
  // lista de taskuri
  tasks: Task[] = [
    { title: 'Create design for app',                                     time: '07:00 - 09:00', done: false, prio: '' },
    { title: 'Create UML diagram for the Task Management module',         time: '07:00 - 09:00', done: false, prio: '' },
    { title: 'Integrate Dark Mode toggle in header',                       time: '07:00 - 09:00', done: false, prio: '' },
    { title: 'Go grocery shopping (milk, bread, eggs)',                    time: '07:00 - 09:00', done: false, prio: '' },
    { title: 'Create design for app',                                     time: '07:00 - 09:00', done: false, prio: '' },
  ];
  // indexul meniului deschis (sau null)
  openIdx: number | null = null;

  /** bifa task */
  toggleDone(i: number, event: Event) {
    const input = event.target as HTMLInputElement;
    this.tasks[i].done = input.checked;
  }

  /** deschide/închide meniul de prioritate pentru rândul i */
  togglePrio(i: number) {
    this.openIdx = (this.openIdx === i) ? null : i;
  }
  /** setează prioritatea și închide meniul */
  setPrio(i: number, val: Prio) {
    this.tasks[i].prio = val;
    this.openIdx = null;
  }
  /** clase pentru punctul colorat */
  dotClass(prio: Prio) {
    return {
      'is-high': prio === 'High',
      'is-med':  prio === 'Medium',
      'is-low':  prio === 'Low'
    };
  }
  /** click în afara meniului => închide */
  @HostListener('document:click')
  closeMenus() { this.openIdx = null; }


  selected: 'Light' | 'Dark' = 'Light';

  constructor() {
    const saved = (localStorage.getItem('rtm-theme') as 'light' | 'dark') || 'light';
    this.apply(saved);
    this.selected = saved === 'dark' ? 'Dark' : 'Light';
  }

  setLight() { this.apply('light'); this.selected = 'Light'; }
  setDark() { this.apply('dark'); this.selected = 'Dark'; }

  private apply(mode: 'light' | 'dark') {
    const html = document.documentElement;
    if (mode === 'dark') html.classList.add('theme-dark');
    else html.classList.remove('theme-dark');
    localStorage.setItem('rtm-theme', mode);
  }
}
