import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DataService } from '../data.service';

@Component({
  selector: 'app-game-selection',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-selection.component.html',
  styleUrl: './game-selection.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GameSelectionComponent {
  dataService = inject(DataService);

  games$ = this.dataService.games$;
  activeGame = this.dataService.currentGame;

  select(game: string) {
    this.dataService.setGame(game);
  }
}
