import {ChangeDetectorRef, Component, inject} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ApiService} from '../../services/api-service';
import {Project} from '../../models/project';
import {FormControl, ReactiveFormsModule} from '@angular/forms';
import {CellComponent} from './cell-component/cell-component';
import {Cell} from '../../models/cell';
@Component({
  selector: 'app-project-page',
  imports: [
    ReactiveFormsModule,
    CellComponent
  ],
  templateUrl: './project-page.html',
  styleUrl: './project-page.css',
})
export class ProjectPage {
  private apiService: ApiService = inject(ApiService);
  private route: ActivatedRoute = inject(ActivatedRoute);
  private readonly id: string;

  protected windDirection = new FormControl("");
  protected windSpeed  = new FormControl("");

  protected state: string = "Loading...";
  protected project: Project | null = null;

  // -- Simulation --
  protected simulationActive: boolean = false;
  protected totalPower: number = 0;
  protected turbineCount: number = 0;
  protected selectedTurbine: Cell | null = null;
  // --            --

  constructor(private changeDetector: ChangeDetectorRef) {
    this.id = this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit() {
    this.select(this.id);

    this.windDirection.valueChanges.subscribe(() => {
      if (this.simulationActive) {
        this.runSimulation();
      }
    });
    this.windSpeed.valueChanges.subscribe(() => {
      if (this.simulationActive) {
        this.runSimulation();
      }
    });
  }

  private select(projectId: string): void {
    this.apiService.select(projectId).subscribe({
      next: data => {
        this.project = data;
        if (this.simulationActive) {
          this.runSimulation();
        }
      },
      error: err => {
        console.error(err);
        this.state = `${err.status} ${err.statusText}`;
        this.changeDetector.detectChanges();
      },
      complete: () => {
        this.changeDetector.detectChanges();
      }
    });
  }

  private clearSimulation(): void {
    if (!this.project || !this.project.cells) return;
    this.turbineCount = this.project.cells.filter(c => c.hasTurbine).length;
    this.project.cells.forEach(c => {
      c.powerOutput = 0;
      c.modifiedWindSpeed = 0;
      c.modifiers = [];
      c.color = undefined;
    });
    this.totalPower = 0;
    this.changeDetector.detectChanges();
  }

  private setSimulation(cell: Cell, speed: number): void {
    cell.powerOutput = 0;
    cell.modifiedWindSpeed = speed;
    cell.modifiers = [];
    cell.color = "rgba(0, 255, 0, 0.5)";
  }
  private applyEnvironmentalModifiers(current: Cell, ix: number, iy: number, dx: number, dy: number, speed: number): void {
    if (!this.project || !this.project.cells) return;

    let modifierPercentage = 0;

    for (let i = 1; i <= 2; i++) {
      const ux = ix - dx * i;
      const uy = iy - dy * i;
      if (ux >= 0 && ux < 20 && uy >= 0 && uy < 20) {
        if (this.project.cells[ux + uy * 20].type === "Lake") {
          modifierPercentage += 20;
          current.modifiers?.push("+20% Lake");
          break;
        }
      }
    }

    for (let i = 1; i <= 5; i++) {
      const ux = ix - dx * i;
      const uy = iy - dy * i;
      if (ux >= 0 && ux < 20 && uy >= 0 && uy < 20) {
        if (this.project.cells[ux + uy * 20].type === "Mountain") {
          modifierPercentage -= 30;
          current.modifiers?.push("-30% Mountain");
          break;
        }
      }
    }

    for (let i = 1; i <= 3; i++) {
      const ux = ix - dx * i;
      const uy = iy - dy * i;
      if (ux >= 0 && ux < 20 && uy >= 0 && uy < 20) {
        if (this.project.cells[ux + uy * 20].hasTurbine) {
          modifierPercentage -= 15;
          current.modifiers?.push("-15% Wake Zone");
          break;
        }
      }
    }

    current.modifiedWindSpeed = speed * (1 + modifierPercentage / 100);
    if (current.modifiedWindSpeed! < 0) current.modifiedWindSpeed = 0;
  }

  private calculatePowerOutput(cell: Cell): void {
    const v = cell.modifiedWindSpeed!;
    if (v < 3.5) cell.powerOutput = 0;
    else if (v > 25) cell.powerOutput = 600;
    else cell.powerOutput = 600 * Math.pow((v - 3.5) / (25 - 3.5), 3);
  }

  private applyColors(turbines: Cell[]): void {
    if (turbines.length === 0) return;

    const powers = turbines.map(t => t.powerOutput || 0);
    const minP = Math.min(...powers);
    const maxP = Math.max(...powers);

    for (const t of turbines) {
      if (maxP - minP < 0.01) {
        t.color = "rgba(0, 255, 0, 0.5)";
      } else {
        const ratio = (t.powerOutput! - minP) / (maxP - minP);
        let r, g;
        if (ratio < 0.5) {
          r = 255;
          g = Math.round(510 * ratio);
        } else {
          r = Math.round(510 * (1 - ratio));
          g = 255;
        }
        t.color = `rgba(${r}, ${g}, 0, 0.5)`;
      }
    }
  }

  protected toggleSimulation(): void {
    this.simulationActive = !this.simulationActive;
    if (!this.simulationActive) {
      this.selectedTurbine = null;
    }
    this.runSimulation();
  }

  protected handleCellClick(cell: Cell): void {
    if (this.simulationActive) {
      this.showDetails(cell);
    } else {
      this.toggleTurbine(cell.x, cell.y);
    }
  }

  protected showDetails(cell: Cell): void {
    if (cell.hasTurbine) {
        this.selectedTurbine = cell;
    } else {
        this.selectedTurbine = null;
    }
    this.changeDetector.detectChanges();
  }

  protected toggleTurbine(x: number, y: number): void {
    this.apiService.toggleTurbine(this.id, x, y).subscribe({
      next: data => {
        this.select(this.id); // REFRESHING
      },
      error: err => {
        console.error(err);
      }
    });
  }

}
