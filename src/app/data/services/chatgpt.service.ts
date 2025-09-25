import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatGPTService {
  private http = inject(HttpClient);
  private apiKey = 'YOUR_OPENAI_API_KEY'; // Replace with your actual API key
  private apiUrl = 'https://api.openai.com/v1/chat/completions';

  sendMessage(message: string): Observable<any> {
    const headers = {
      'Authorization': `Bearer ${this.apiKey}`,
      'Content-Type': 'application/json'
    };

    const body = {
      model: 'gpt-3.5-turbo',
      messages: [
        {
          role: 'system',
          content: 'You are a helpful task management assistant. Help users with task organization, prioritization, and productivity tips.'
        },
        {
          role: 'user',
          content: message
        }
      ],
      max_tokens: 500,
      temperature: 0.7
    };

    return this.http.post(this.apiUrl, body, { headers });
  }

  // Mock response for development (remove when using real API)
  sendMessageMock(message: string): Observable<any> {
    return new Observable(observer => {
      setTimeout(() => {
        const responses = [
          "NO CONNECTION"
        ];

        const randomResponse = responses[Math.floor(Math.random() * responses.length)];

        observer.next({
          choices: [{
            message: {
              content: randomResponse
            }
          }]
        });
        observer.complete();
      }, 1000);
    });
  }
}

