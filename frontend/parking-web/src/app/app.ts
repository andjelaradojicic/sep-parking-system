import { Component, inject, signal } from '@angular/core';
import { JsonPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, JsonPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private readonly http = inject(HttpClient);
  protected readonly health = signal<unknown>(null);

  constructor() {
    this.http.get(`${environment.apiUrl}/api/health`).subscribe({
      next: r => this.health.set(r),
      error: e => this.health.set({ error: e.message })
    });
  }
}
