import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router'; // Importe para carregar as rotas

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet], 
  template: '<router-outlet></router-outlet>', 
})
export class App {
  title = 'tech-challenge';
}