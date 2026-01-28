import {Component, inject} from '@angular/core';
import {ApiService} from '../../services/api-service';

@Component({
  selector: 'app-index-page',
  imports: [],
  templateUrl: './index-page.html',
  styleUrl: './index-page.css',
})
export class IndexPage {
  private apiService: ApiService = inject(ApiService);

  ngOnInit()
  {
    this.apiService.search();
  }
}
