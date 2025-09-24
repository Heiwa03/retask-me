import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService, UserRole } from '../../data/services/auth.service';
import { ChatGPTService } from '../../data/services/chatgpt.service';
import { DbContextService, DbCheckResponse } from '../../data/services/db-context.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {
  private auth = inject(AuthService);
  private router = inject(Router);
  private chatGPT = inject(ChatGPTService);
  private dbContext = inject(DbContextService);
  isProfileOpen = false;
  isAIAssistOpen = false;
  userAvatar = 'https://via.placeholder.com/40x40/6366f1/ffffff?text=AY'; // Default avatar
  userName = 'Amelia Yeldiz';
  userEmail = 'amelia@retaskme.com';
  aiMessage = '';
  aiResponse = '';
  isAILoading = false;
  dbStatus: DbCheckResponse | null = null;
  isDbLoading = false;
  
  tasks = [
    { id: 1, title: 'Create design for app', time: '07:00 - 09:00', priority: 'high', completed: false },
    { id: 2, title: 'Create UML diagram for Task Management module', time: '07:00 - 09:00', priority: 'medium', completed: false },
    { id: 3, title: 'Integrate Dark Mode toggle in header', time: '07:00 - 09:00', priority: 'low', completed: false },
    { id: 4, title: 'Go grocery shopping', time: '07:00 - 09:00', priority: 'medium', completed: false },
    { id: 5, title: 'Do 30 minutes of workout', time: '07:00 - 09:00', priority: 'high', completed: false }
  ];

  ngOnInit() {
    const saved = localStorage.getItem('theme') as 'light' | 'dark' | null;
    const dark = saved ? saved === 'dark' : false;
    document.documentElement.classList.toggle('theme-dark', dark);
    queueMicrotask(() => {
      const chk = document.getElementById('dashThemeSwitch') as HTMLInputElement | null;
      if (chk) chk.checked = !dark; // checked = Light
    });
    
    // Check database connection on dashboard load
    this.checkDatabaseStatus();
  }

  onThemeToggle(e: Event) {
    const isLight = (e.target as HTMLInputElement).checked;
    document.documentElement.classList.toggle('theme-dark', !isLight);
    localStorage.setItem('theme', isLight ? 'light' : 'dark');
  }

  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

  toggleProfile() {
    this.isProfileOpen = !this.isProfileOpen;
  }

  closeProfile() {
    this.isProfileOpen = false;
  }

  toggleAIAssist() {
    this.isAIAssistOpen = !this.isAIAssistOpen;
  }

  closeAIAssist() {
    this.isAIAssistOpen = false;
  }

  toggleTask(taskId: number) {
    const task = this.tasks.find(t => t.id === taskId);
    if (task) {
      task.completed = !task.completed;
    }
  }

  deleteTask(taskId: number) {
    this.tasks = this.tasks.filter(t => t.id !== taskId);
  }

  editTask(taskId: number) {
    // TODO: Implement edit modal
    console.log('Edit task:', taskId);
  }

  getPriorityClass(priority: string) {
    switch (priority) {
      case 'high': return 'priority--high';
      case 'medium': return 'priority--medium';
      case 'low': return 'priority--low';
      default: return '';
    }
  }

  getPriorityLabel(priority: string) {
    switch (priority) {
      case 'high': return 'High';
      case 'medium': return 'Medium';
      case 'low': return 'Low';
      default: return 'Unknown';
    }
  }

  sendAIMessage() {
    if (!this.aiMessage.trim()) return;
    
    this.isAILoading = true;
    this.aiResponse = '';
    
    // Use mock service for development (replace with real API call)
    this.chatGPT.sendMessageMock(this.aiMessage).subscribe({
      next: (response) => {
        this.aiResponse = response.choices[0].message.content;
        this.isAILoading = false;
      },
      error: (error) => {
        console.error('AI Error:', error);
        this.aiResponse = 'Sorry, I encountered an error. Please try again.';
        this.isAILoading = false;
      }
    });
  }

  useSuggestion(suggestion: string) {
    this.aiMessage = suggestion;
    this.sendAIMessage();
  }

  setLightTheme() {
    this.onThemeToggle({ target: { checked: true } } as any);
  }

  setDarkTheme() {
    this.onThemeToggle({ target: { checked: false } } as any);
  }

  checkDatabaseStatus() {
    this.isDbLoading = true;
    this.dbContext.checkDatabaseConnection().subscribe({
      next: (response) => {
        this.dbStatus = response;
        this.isDbLoading = false;
        console.log('Database Status:', response);
      },
      error: (error) => {
        console.error('Database check failed:', error);
        this.isDbLoading = false;
        this.dbStatus = { canConnect: false, userCount: 0 };
      }
    });
  }

  refreshDatabaseStatus() {
    this.checkDatabaseStatus();
  }

  testBackendConnection() {
    console.log('Testing backend connection...');
    this.auth.testConnection().subscribe({
      next: (response) => {
        console.log('Backend connection successful:', response);
        alert('Backend connection successful!');
      },
      error: (error) => {
        console.error('Backend connection failed:', error);
        alert('Backend connection failed. Check console for details.');
      }
    });
  }

  testRegisterAPI() {
    console.log('Testing register API...');
    const testData = {
      mail: 'test@example.com',
      password: 'testpassword',
      repeatPassword: 'testpassword',
      role: UserRole.USER
    };
    
    this.auth.register(testData).subscribe({
      next: (response) => {
        console.log('Register API Response:', response);
        alert(`Register test result: ${response.success ? 'Success' : 'Failed'} - ${response.message}`);
      },
      error: (error) => {
        console.error('Register API Error:', error);
        alert('Register API test failed. Check console for details.');
      }
    });
  }
}


