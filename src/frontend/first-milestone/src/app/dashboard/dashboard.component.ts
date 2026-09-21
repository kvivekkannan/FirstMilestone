import { Component, inject, signal } from '@angular/core';
import { ApiService, DashboardModule } from '../core/api.service';
import { WeatherComponent } from '../modules/weather/weather.component';
import { ModuleCatalogComponent } from '../modules/module-catalog/module-catalog.component';

@Component({
  selector: 'fm-dashboard',
  standalone: true,
  imports: [WeatherComponent, ModuleCatalogComponent],
  template: `
    <div class="page">
      <header class="topbar">
        <div class="brand"><div class="logo">FM</div><div><strong>FirstMilestone</strong><span>Senior engineering practice app</span></div></div>
        <div class="stack"><span>.NET 8</span><span>Angular</span><span>SQL Server</span><span>Docker</span></div>
      </header>

      <main>
        <section class="hero">
          <div><span class="eyebrow">MODULAR DASHBOARD</span><h1>Your engineering playground.</h1><p>Start small, then evolve the application toward cloud deployment, CI/CD, observability, AI integrations and more.</p></div>
          <div class="hero-stat"><strong>{{ installedCount() }}</strong><span>installed modules</span></div>
        </section>

        <section class="grid">
          @if (hasWeather()) { <fm-weather /> }
          <fm-module-catalog />
        </section>
      </main>
      <footer>FirstMilestone · Practice project · Weather data by Open-Meteo</footer>
    </div>
  `,
  styles: [`
    .page { min-height:100vh; background:#f8fafc; color:#0f172a; }
    .topbar { height:72px; background:white; border-bottom:1px solid #e5e7eb; display:flex; align-items:center; justify-content:space-between; padding:0 5vw; }
    .brand { display:flex; align-items:center; gap:12px; }
    .brand > div:last-child { display:flex; flex-direction:column; gap:2px; }
    .brand span { color:#64748b; font-size:11px; }
    .logo { width:38px; height:38px; border-radius:11px; display:grid; place-items:center; background:#111827; color:white; font-weight:800; }
    .stack { display:flex; gap:8px; flex-wrap:wrap; justify-content:flex-end; }
    .stack span { font-size:11px; font-weight:700; padding:6px 9px; border-radius:999px; background:#f1f5f9; color:#475569; }
    main { max-width:1200px; margin:0 auto; padding:48px 24px 30px; }
    .hero { display:flex; justify-content:space-between; align-items:end; gap:30px; margin-bottom:30px; }
    h1 { font-size:44px; line-height:1.05; letter-spacing:-2px; margin:8px 0 12px; max-width:650px; }
    .hero p { max-width:680px; color:#64748b; line-height:1.65; margin:0; }
    .eyebrow { color:#0284c7; font-size:11px; font-weight:800; letter-spacing:1.5px; }
    .hero-stat { background:white; border:1px solid #e5e7eb; border-radius:18px; padding:18px 24px; min-width:150px; display:flex; flex-direction:column; }
    .hero-stat strong { font-size:36px; }
    .hero-stat span { color:#64748b; font-size:12px; }
    .grid { display:grid; grid-template-columns:1.5fr 1fr; gap:18px; }
    footer { text-align:center; padding:30px; color:#94a3b8; font-size:12px; }
    @media (max-width:900px) { .grid { grid-template-columns:1fr; } .stack { display:none; } .hero { align-items:start; flex-direction:column; } h1 { font-size:36px; } }
  `]
})
export class DashboardComponent {
  private readonly api = inject(ApiService);
  readonly installedCount = signal(1);
  readonly hasWeather = signal(true);

  ngOnInit() {
    this.api.getModules().subscribe((modules: DashboardModule[]) => {
      this.installedCount.set(modules.filter(m => m.isInstalled).length);
      this.hasWeather.set(modules.some(m => m.key === 'weather' && m.isInstalled));
    });
  }
}
