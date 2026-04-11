import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
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
  deleteTooltipText = $localize`:@@deleteTooltip:Delete`;

  constructor(
    private readonly scenarioService: ScenarioService,
    private readonly router: Router,
    private readonly snackBar: MatSnackBar,
    private readonly cdr: ChangeDetectorRef,
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
        this.cdr.markForCheck();
      },
      error: () => {
        this.snackBar.open($localize`:@@errorLoadingScenarios:Error loading scenarios`, $localize`:@@snackClose:Close`, { duration: 3000 });
        this.loading = false;
        this.cdr.markForCheck();
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
        this.snackBar.open($localize`:@@scenarioDeleted:Scenario deleted`, $localize`:@@snackClose:Close`, { duration: 2000 });
        this.cdr.markForCheck();
      },
      error: () => {
        this.snackBar.open($localize`:@@errorDeletingScenario:Error deleting scenario`, $localize`:@@snackClose:Close`, { duration: 3000 });
        this.cdr.markForCheck();
      },
    });
  }
}
