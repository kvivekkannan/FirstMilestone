import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardModule {
  id: number;
  key: string;
  name: string;
  description: string;
  icon: string;
  isInstalled: boolean;
}

export interface WeatherResponse {
  city: string;
  country: string;
  latitude: number;
  longitude: number;
  temperatureC: number;
  apparentTemperatureC: number;
  relativeHumidity: number;
  windSpeedKmh: number;
  weatherCode: number;
  condition: string;
  retrievedUtc: string;
  forecast: DailyForecast[];
}

export interface DailyForecast {
  date: string;
  minC: number;
  maxC: number;
  weatherCode: number;
  condition: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  getModules(): Observable<DashboardModule[]> {
    return this.http.get<DashboardModule[]>(`${this.baseUrl}/modules`);
  }

  installModule(key: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/modules/${key}/install`, {});
  }

  uninstallModule(key: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/modules/${key}/uninstall`, {});
  }

  getWeather(city: string): Observable<WeatherResponse> {
    return this.http.get<WeatherResponse>(`${this.baseUrl}/weather`, { params: { city } });
  }

  getSummary(): Observable<{ installedModules: number; weatherSearches: number }> {
    return this.http.get<{ installedModules: number; weatherSearches: number }>(`${this.baseUrl}/dashboard/summary`);
  }
}
