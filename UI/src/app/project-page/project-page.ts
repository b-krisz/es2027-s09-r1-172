import {Component, inject} from '@angular/core';
import {ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-project-page',
  imports: [],
  templateUrl: './project-page.html',
  styleUrl: './project-page.css',
})
export class ProjectPage {
  private route = inject(ActivatedRoute);
  protected readonly id: string;
  constructor() {
    this.id = this.route.snapshot.paramMap.get('id')!;
  }
}
