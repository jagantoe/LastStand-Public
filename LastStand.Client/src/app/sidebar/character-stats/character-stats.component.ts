import { ChangeDetectionStrategy, Component, Input, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Character } from '../../types';

@Component({
  selector: 'app-character-stats',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './character-stats.component.html',
  styleUrl: './character-stats.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CharacterStatsComponent {
  characters = signal<Character[]>([]);
  @Input({ required: true }) set setcharacters(value: Character[]) {
    this.characters.set(value)
  }

  val!: string | null;
  selectedCharacter = signal<string | null>(null);
  currentCharacter = computed(() => this.characters().find(x => x.name == this.selectedCharacter()))
  set(name: string) {
    this.selectedCharacter.set(name);
  }
}
