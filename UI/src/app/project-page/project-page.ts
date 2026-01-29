import {ChangeDetectorRef, Component, inject} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ApiService} from '../../services/api-service';
import {Project} from '../../models/project';
import {NgOptimizedImage} from '@angular/common';

@Component({
  selector: 'app-project-page',
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './project-page.html',
  styleUrl: './project-page.css',
})
export class ProjectPage {
  private apiService: ApiService = inject(ApiService);
  private route: ActivatedRoute = inject(ActivatedRoute);
  private readonly id: string;

  protected state: string = "Loading...";
  protected project: Project | null = null;

  constructor(private changeDetector: ChangeDetectorRef) {
    this.id = this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit() {
    this.select(this.id);
  }

  private select(projectId: string): void {
    this.apiService.select(projectId).subscribe({
      next: data => {
        this.project = data;
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

}
