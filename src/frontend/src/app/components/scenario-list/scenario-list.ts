import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
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
    MatSortModule,
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
  dataSource = new MatTableDataSource<ScenarioListItem>();
  displayedColumns = ['name', 'createdAt', 'updatedAt', 'hasResults', 'actions'];
  loading = true;
  deleteTooltipText = $localize`:@@deleteTooltip:Delete`;
  duplicateTooltipText = $localize`:@@duplicateTooltip:Duplicate`;

  @ViewChild(MatSort) set matSort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

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
        this.dataSource.data = data;
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
        this.dataSource.data = this.dataSource.data.filter((s) => s.id !== id);
        this.snackBar.open($localize`:@@scenarioDeleted:Scenario deleted`, $localize`:@@snackClose:Close`, { duration: 2000 });
        this.cdr.markForCheck();
      },
      error: () => {
        this.snackBar.open($localize`:@@errorDeletingScenario:Error deleting scenario`, $localize`:@@snackClose:Close`, { duration: 3000 });
        this.cdr.markForCheck();
      },
    });
  }

  duplicateScenario(id: string, event: Event): void {
    event.stopPropagation();
    this.scenarioService.duplicate(id).subscribe({
      next: (created) => {
        this.dataSource.data = [...this.dataSource.data, created];
        this.snackBar.open($localize`:@@scenarioDuplicated:Scenario duplicated`, $localize`:@@snackClose:Close`, { duration: 2000 });
        this.cdr.markForCheck();
      },
      error: () => {
        this.snackBar.open($localize`:@@errorDuplicatingScenario:Error duplicating scenario`, $localize`:@@snackClose:Close`, { duration: 3000 });
        this.cdr.markForCheck();
      },
    });
  }
}
