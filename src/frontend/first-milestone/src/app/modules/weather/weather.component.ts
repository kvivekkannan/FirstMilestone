import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService, WeatherResponse } from '../../core/api.service';

@Component({
  selector: 'fm-weather',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe],
  template: `
    <section class="module-card weather-card">
      <div class="module-header">
        <div><span class="eyebrow">LIVE MODULE</span><h2>Weather</h2></div>
        <span class="module-icon">☀</span>
      </div>

      <form class="search-row" (ngSubmit)="search()">
        <input [(ngModel)]="city" name="city" placeholder="Enter a city" aria-label="City" />
        <button type="submit" [disabled]="loading()">{{ loading() ? 'Loading…' : 'Search' }}</button>
      </form>

      @if (error()) { <p class="error">{{ error() }}</p> }

      @if (weather(); as w) {
        <div class="current-weather">
          <div>
            <div class="location">{{ w.city }}, {{ w.country }}</div>
            <div class="temperature">{{ w.temperatureC | number:'1.0-0' }}°C</div>
            <div class="condition">{{ w.condition }}</div>
          </div>
          <div class="weather-stats">
            <span>Feels {{ w.apparentTemperatureC | number:'1.0-0' }}°C</span>
            <span>Humidity {{ w.relativeHumidity | number:'1.0-0' }}%</span>
            <span>Wind {{ w.windSpeedKmh | number:'1.0-0' }} km/h</span>
          </div>
        </div>
        <div class="forecast">
          @for (day of w.forecast; track day.date) {
            <div class="forecast-day">
              <strong>{{ day.date | date:'EEE' }}</strong>
              <span>{{ day.condition }}</span>
              <b>{{ day.maxC | number:'1.0-0' }}° / {{ day.minC | number:'1.0-0' }}°</b>
            </div>
          }
        </div>
        <div class="attribution">Weather data by Open-Meteo</div>
      } @else if (!loading()) {
        <div class="empty-state">Search for a city to load current conditions and a 5-day forecast.</div>
      }
    </section>
  `,
  styles: [`
    .weather-card { min-height: 380px; }
    .search-row { display:flex; gap:10px; margin:20px 0; }
    input { flex:1; border:1px solid #d1d5db; border-radius:12px; padding:12px 14px; font:inherit; }
    button { border:0; border-radius:12px; padding:12px 18px; background:#111827; color:white; font-weight:700; cursor:pointer; }
    button:disabled { opacity:.55; cursor:wait; }
    .current-weather { display:flex; justify-content:space-between; gap:20px; padding:22px; border-radius:18px; background:linear-gradient(135deg,#eff6ff,#f8fafc); }
    .location { font-size:15px; color:#475569; font-weight:600; }
    .temperature { font-size:58px; font-weight:800; letter-spacing:-3px; margin:8px 0; }
    .condition { font-size:18px; font-weight:650; }
    .weather-stats { display:flex; flex-direction:column; justify-content:center; gap:10px; color:#475569; font-size:14px; text-align:right; }
    .forecast { display:grid; grid-template-columns:repeat(5,1fr); gap:8px; margin-top:14px; }
    .forecast-day { padding:12px; background:#f8fafc; border-radius:12px; display:flex; flex-direction:column; gap:7px; min-width:0; }
    .forecast-day span { color:#64748b; font-size:12px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
    .forecast-day b { font-size:13px; }
    .attribution { font-size:11px; color:#94a3b8; margin-top:14px; }
    .error { color:#b91c1c; background:#fef2f2; padding:10px 12px; border-radius:10px; }
    .empty-state { padding:40px 20px; text-align:center; color:#64748b; }
    @media (max-width:700px) { .current-weather { flex-direction:column; } .weather-stats { text-align:left; } .forecast { grid-template-columns:1fr 1fr; } }
  `]
})
export class WeatherComponent {
  private readonly api = inject(ApiService);
  readonly weather = signal<WeatherResponse | null>(null);
  readonly loading = signal(false);
  readonly error = signal('');
  city = 'Chennai';

  ngOnInit() { this.search(); }

  search() {
    if (!this.city.trim()) return;
    this.loading.set(true);
    this.error.set('');
    this.api.getWeather(this.city.trim()).subscribe({
      next: data => { this.weather.set(data); this.loading.set(false); },
      error: err => { this.error.set(err?.error?.message ?? 'Unable to load weather right now.'); this.loading.set(false); }
    });
  }
}
