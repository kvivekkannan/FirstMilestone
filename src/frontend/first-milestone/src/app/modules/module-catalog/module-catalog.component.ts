import { Component, inject, signal } from '@angular/core';
import { ApiService, DashboardModule } from '../../core/api.service';

@Component({
  selector: 'fm-module-catalog',
  standalone: true,
  template: `
    <section class="catalog module-card">
      <div class="module-header">
        <div><span class="eyebrow">EXTENSIBLE</span><h2>Module catalog</h2></div>
        <span class="module-icon">+</span>
      </div>
      <p class="muted">Install modules now; new dashboard modules can be added without changing the dashboard shell.</p>
      <div class="module-list">
        @for (module of modules(); track module.key) {
          <article class="catalog-item">
            <div class="catalog-icon">{{ module.icon }}</div>
            <div class="catalog-copy"><strong>{{ module.name }}</strong><span>{{ module.description }}</span></div>
            <button (click)="toggle(module)" [disabled]="busyKey() === module.key">
              {{ busyKey() === module.key ? '…' : (module.isInstalled ? 'Installed' : 'Install') }}
            </button>
          </article>
        }
      </div>
    </section>
  `,
  styles: [`
    .muted { color:#64748b; margin-top:0; }
    .module-list { display:grid; gap:10px; margin-top:18px; }
    .catalog-item { display:flex; align-items:center; gap:14px; padding:14px; border:1px solid #e5e7eb; border-radius:14px; }
    .catalog-icon { width:38px; height:38px; border-radius:10px; display:grid; place-items:center; background:#f1f5f9; font-size:20px; }
    .catalog-copy { flex:1; display:flex; flex-direction:column; gap:4px; }
    .catalog-copy span { color:#64748b; font-size:13px; }
    button { border:1px solid #d1d5db; background:white; border-radius:10px; padding:8px 12px; cursor:pointer; font-weight:700; }
    button:disabled { opacity:.6; cursor:wait; }
  `]
})
export class ModuleCatalogComponent {
  private readonly api = inject(ApiService);
  readonly modules = signal<DashboardModule[]>([]);
  readonly busyKey = signal<string | null>(null);

  ngOnInit() { this.load(); }

  load() { this.api.getModules().subscribe(data => this.modules.set(data)); }

  toggle(module: DashboardModule) {
    if (module.isInstalled) return;
    this.busyKey.set(module.key);
    this.api.installModule(module.key).subscribe({ next: () => { this.busyKey.set(null); this.load(); }, error: () => this.busyKey.set(null) });
  }
}
