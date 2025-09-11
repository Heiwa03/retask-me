import { Component } from '@angular/core';
import { RouterLink } from '@angular/router'; // Import RouterLink for the [routerLink] directive

@Component({
  selector: 'app-register',         // CORRECTED: Changed from 'app-login'
  standalone: true,                // ADDED: Required for standalone components
  imports: [RouterLink],           // ADDED: RouterLink is needed for the template
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {     // CORRECTED: Added 'Component' suffix
  
}