import {Cell} from './cell';

export interface Project
{
  id: string;
  name: string;
  mapData: string;
  cells: Cell[]
}
