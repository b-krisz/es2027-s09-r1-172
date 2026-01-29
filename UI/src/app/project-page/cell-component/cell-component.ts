import {ChangeDetectorRef, Component, computed, input, InputSignal} from '@angular/core';
import {Cell} from '../../../models/cell';

@Component({
  selector: 'app-cell-component',
  imports: [],
  templateUrl: './cell-component.html',
  styleUrl: './cell-component.css',
})
export class CellComponent {
  public cell: InputSignal<Cell | undefined> = input();
  protected cellV = computed(() => this.cell());

  public cells: InputSignal<Cell[] | undefined> = input();
  protected cellsV = computed(() => this.cells());

  public windDirection: InputSignal<string | undefined> = input();

  protected imageSource: string = "/Grid.png";

  protected get arrowRotation(): string {
    const dir = this.windDirection()?.toLowerCase();
    if (dir === 'n') return 'rotate(180deg)';
    if (dir === 'ne') return 'rotate(225deg)';
    if (dir === 'e') return 'rotate(270deg)';
    if (dir === 'se') return 'rotate(315deg)';
    if (dir === 's') return 'rotate(0deg)';
    if (dir === 'sw') return 'rotate(45deg)';
    if (dir === 'w') return 'rotate(90deg)';
    if (dir === 'nw') return 'rotate(135deg)';
    return '';
  }

  constructor(private changeDetector: ChangeDetectorRef) {}

  ngOnInit() {
    this.imageCheck();
  }

  private imageCheck()
  {
    this.imageSource = "/Grid.png";

    if (this.cellV()?.hasTurbine)
      this.imageSource = "/WindTurbineDark.png";
    else if (this.cellV()?.type != "Mountain" && this.cellV()?.type != "Lake")
    {
      let selfX: number = this.cellV()?.x ?? 0;
      let selfY: number = this.cellV()?.y ?? 0;
      let cells: Cell[] = this.cellsV() ?? [];

      // OFFSET VALUES
      let leftOffset: number = (selfX - 1);
      let rightOffset: number = (selfX + 1);
      let topOffset: number = (selfY - 1);
      let bottomOffset: number = (selfY + 1);

      // OFFSET CELLS
      let lt: Cell | undefined = cells[leftOffset + topOffset * 20]; // LEFT TOP OFFSET
      let rt: Cell | undefined = cells[rightOffset + topOffset * 20]; // RIGHT TOP OFFSET
      let lb: Cell | undefined = cells[leftOffset + bottomOffset * 20]; // LEFT BOTTOM OFFSET
      let rb: Cell | undefined = cells[rightOffset + bottomOffset * 20]; // RIGHT BOTTOM OFFSET

      let l: Cell | undefined = cells[leftOffset + selfY * 20];      // LEFT OFFSET
      let r: Cell | undefined = cells[rightOffset + selfY * 20];      // RIGHT OFFSET
      let t: Cell | undefined = cells[(selfX) + topOffset * 20];      // TOP OFFSET
      let b: Cell | undefined = cells[(selfX) + bottomOffset * 20];      // BOTTOM OFFSET

      // OFFSET OUT OF BOUNDS
      if (leftOffset < 0 || leftOffset >= 20)
      {
        lt = undefined;
        lb = undefined;
        l = undefined;
      }
      if (rightOffset < 0 || rightOffset >= 20)
      {
        rt = undefined;
        rb = undefined;
        r = undefined;
      }
      if (topOffset < 0 || topOffset >= 20)
      {
        lt = undefined;
        rt = undefined;
        t = undefined;
      }
      if (bottomOffset < 0 || bottomOffset >= 20)
      {
        lb = undefined;
        rb = undefined;
        b = undefined;
      }

      // CELL CHECK
      if (lt?.hasTurbine || rt?.hasTurbine ||
        lb?.hasTurbine || rb?.hasTurbine ||
        l?.hasTurbine  || r?.hasTurbine ||
        t?.hasTurbine  || b?.hasTurbine)
        this.imageSource = "/ExclusionZone.png";
    }
  }

}
