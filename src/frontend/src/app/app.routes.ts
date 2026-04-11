import { Routes } from '@angular/router';
import { ScenarioListComponent } from './components/scenario-list/scenario-list';
import { ScenarioDetailComponent } from './components/scenario-detail/scenario-detail';

export const routes: Routes = [
  { path: '', component: ScenarioListComponent },
  { path: 'scenarios/:id', component: ScenarioDetailComponent },
  { path: '**', redirectTo: '' },
];
