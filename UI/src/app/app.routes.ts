import { Routes } from '@angular/router';
import {IndexPage} from './index-page/index-page';
import {ProjectPage} from './project-page/project-page';
import {NotFound} from './not-found/not-found';

export const routes: Routes = [
  { path: "", component: IndexPage },
  { path: "project/:id", component: ProjectPage},
  { path: "**", component: NotFound}
];
