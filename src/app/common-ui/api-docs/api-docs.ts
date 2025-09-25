import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

declare const SwaggerUIBundle: any;

@Component({
  selector: 'app-api-docs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './api-docs.html',
  styleUrl: './api-docs.scss'
})
export class ApiDocsComponent implements OnInit {
  ngOnInit(): void {
    SwaggerUIBundle({
      url: 'https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/swagger/v1/swagger.json',
      dom_id: '#swagger-ui',
      presets: [SwaggerUIBundle.presets.apis],
      layout: 'BaseLayout',
      deepLinking: true,
      showExtensions: true,
      showCommonExtensions: true,
      tryItOutEnabled: true
    });
  }
}
