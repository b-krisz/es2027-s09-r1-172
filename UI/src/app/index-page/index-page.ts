import {Component, inject, ChangeDetectorRef} from '@angular/core';
import {ApiService} from '../../services/api-service';
import {Project} from '../../models/project';
import {FormControl, ReactiveFormsModule} from '@angular/forms';
import {debounceTime} from 'rxjs';
import {ProjectComponent} from './project-component/project-component';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-index-page',
  imports: [
    ReactiveFormsModule,
    ProjectComponent,
    RouterLink
  ],
  templateUrl: './index-page.html',
  styleUrl: './index-page.css',
})
export class IndexPage {
  private apiService: ApiService = inject(ApiService);
  protected projects: Project[] = [];
  searchTerm = new FormControl("");

  constructor(private changeDetector: ChangeDetectorRef) {
    this.searchTerm.valueChanges.pipe(debounceTime(400)).subscribe(st => {this.search(st ?? "")});
  }

  ngOnInit() {
    this.search("");
  }

  private search(searchQuery: string): void {
    this.apiService.search(searchQuery).subscribe({
      next: data => {
        this.projects = [...data];
      },
      error: err => {
        console.error(err);
      },
      complete: () => {
        this.changeDetector.detectChanges();
      }
    });
  }

}
