import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DataService } from '../data.service';
import { CharacterStatsComponent } from './character-stats/character-stats.component';
import { GameStatsComponent } from './game-stats/game-stats.component';
import { InventoryComponent } from './inventory/inventory.component';
import { PlayerStatsComponent } from './player-stats/player-stats.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, GameStatsComponent, InventoryComponent, PlayerStatsComponent, CharacterStatsComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
  dataService = inject(DataService);

  game$ = this.dataService.game$;

  refresh() {
    this.dataService.refreshData();
  }
  changeHost() {
    this.dataService.askHost();
  }
}
