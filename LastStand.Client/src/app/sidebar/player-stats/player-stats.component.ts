import { ChangeDetectionStrategy, Component, Input, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Player } from '../../types';

@Component({
  selector: 'app-player-stats',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './player-stats.component.html',
  styleUrl: './player-stats.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlayerStatsComponent {
  players = signal<Player[]>([]);
  @Input({ required: true }) set setplayers(value: Player[]) {
    this.players.set(value)
  }

  val!: string | null;
  selectedPlayer = signal<string | null>(null);
  currentPlayer = computed(() => this.players().find(x => x.name == this.selectedPlayer()))
  set(name: string) {
    this.selectedPlayer.set(name);
  }
}
