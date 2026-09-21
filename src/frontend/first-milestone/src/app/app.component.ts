import { Component } from '@angular/core';
import { DashboardComponent } from './dashboard/dashboard.component';

@Component({
  selector: 'fm-root',
  standalone: true,
  imports: [DashboardComponent],
  template: '<fm-dashboard />'
})
export class AppComponent {}
