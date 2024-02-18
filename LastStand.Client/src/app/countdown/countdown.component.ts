import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

@Component({
  selector: 'app-countdown',
  standalone: true,
  imports: [],
  templateUrl: './countdown.component.html',
  styleUrl: './countdown.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CountdownComponent {
  @Input({ required: true, transform: toString }) number!: string;
}
function toString(number: number): string {
  return number.toString();
}

