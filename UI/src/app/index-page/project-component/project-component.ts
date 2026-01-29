import {Component, computed, input, InputSignal} from '@angular/core';

@Component({
  selector: 'app-project-component',
  imports: [],
  templateUrl: './project-component.html',
  styleUrl: './project-component.css',
})
export class ProjectComponent {
  public name: InputSignal<string> = input("ProjectName");
  public id: InputSignal<string> = input("Id");

  protected nameV = computed(() => this.name());
  protected idV = computed(() => this.id());
}
