import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Project} from '../models/project';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private http: HttpClient = inject(HttpClient);

  select(projectId: string | null): Observable<Project> {
    return this.http.get<Project>("/api/projects/select?id=" + projectId);
  }

  search(searchQuery: string): Observable<Project[]>
  {
    return this.http.get<Project[]>("/api/projects/search?query=" + searchQuery);
  }
}
