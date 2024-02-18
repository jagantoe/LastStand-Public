import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CountdownComponent } from '../../countdown/countdown.component';
import { Game } from '../../types';

@Component({
  selector: 'app-game-stats',
  standalone: true,
  imports: [CountdownComponent],
  templateUrl: './game-stats.component.html',
  styleUrl: './game-stats.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GameStatsComponent {

  @Input({ required: true }) game!: Game;
  resources = {
    grain: 100,
    wood: 100,
    stone: 100,
    steel: 100,
    limit: 100
  }
}
