import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [],
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InventoryComponent {
  @Input({ required: true, transform: convertToList }) inventory!: { name: string, amount: number }[];
}
function convertToList(inventory: { [key: string]: number }): { name: string, amount: number }[] {
  return Object.getOwnPropertyNames(inventory).sort().map(x => ({ name: x, amount: inventory[x] }));
}
