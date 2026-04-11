import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ScenarioService } from '../../services/scenario.service';
import { ScenarioListItem } from '../../models';

@Component({
  selector: 'app-scenario-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatTooltipModule,
    MatSnackBarModule,
  ],
  templateUrl: './scenario-list.html',
  styleUrl: './scenario-list.scss',
})
export class ScenarioListComponent implements OnInit {
  scenarios: ScenarioListItem[] = [];
  displayedColumns = ['name', 'createdAt', 'updatedAt', 'hasResults', 'actions'];
  loading = true;

  constructor(
    private readonly scenarioService: ScenarioService,
    private readonly router: Router,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadScenarios();
  }

  loadScenarios(): void {
    this.loading = true;
    this.scenarioService.getAll().subscribe({
      next: (data) => {
        this.scenarios = data;
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Error loading scenarios', 'Close', { duration: 3000 });
        this.loading = false;
      },
    });
  }

  createNew(): void {
    this.router.navigate(['/scenarios', 'new']);
  }

  open(id: string): void {
    this.router.navigate(['/scenarios', id]);
  }

  deleteScenario(id: string, event: Event): void {
    event.stopPropagation();
    this.scenarioService.delete(id).subscribe({
      next: () => {
        this.scenarios = this.scenarios.filter((s) => s.id !== id);
        this.snackBar.open('Scenario deleted', 'Close', { duration: 2000 });
      },
      error: () => this.snackBar.open('Error deleting scenario', 'Close', { duration: 3000 }),
    });
  }
}
