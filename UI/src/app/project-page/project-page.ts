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

  // Simulation
  protected simulationActive: boolean = false;
  protected totalPower: number = 0;
  protected turbineCount: number = 0;
  protected selectedTurbine: Cell | null = null;

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
