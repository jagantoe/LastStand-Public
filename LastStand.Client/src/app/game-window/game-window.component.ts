import { AsyncPipe } from '@angular/common';
import { Component, ElementRef, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DragScrollComponent } from 'ngx-drag-scroll';
import { tap } from 'rxjs';
import { AttackersOffsets, BuidingsOffsets, PlayersOffsets, TilesOffsets } from '../constants';
import { DataService } from '../data.service';
import { Character, Game, MapTile, Player, VectorStringToObject } from '../types';

@Component({
  selector: 'app-game-window',
  standalone: true,
  imports: [DragScrollComponent, AsyncPipe, FormsModule],
  templateUrl: './game-window.component.html',
  styleUrl: './game-window.component.scss',
})
export class GameWindowComponent {
  dataService = inject(DataService);
  game$ = this.dataService.game$.pipe(
    tap(x => this.game = x),
    tap(x => this.handleMap())
  );
  game!: Game;

  zoomOptions = [16, 8, 4, 2];
  zoom = 4; // 4 = 100%

  @ViewChild("clouds", { static: true }) cloudsCanvas!: ElementRef<HTMLCanvasElement>;
  cloudsContext!: CanvasRenderingContext2D;
  @ViewChild("background", { static: true }) backgroundCanvas!: ElementRef<HTMLCanvasElement>;
  backgroundContext!: CanvasRenderingContext2D;
  @ViewChild("characters", { static: true }) charactersCanvas!: ElementRef<HTMLCanvasElement>;
  charactersContext!: CanvasRenderingContext2D;

  @ViewChild("tilesimg", { static: true }) tilesimg!: ElementRef<HTMLImageElement>;
  @ViewChild("buildingsimg", { static: true }) buildingsimg!: ElementRef<HTMLImageElement>;
  @ViewChild("attackersimg", { static: true }) attackersimg!: ElementRef<HTMLImageElement>;
  @ViewChild("playersimg", { static: true }) playersimg!: ElementRef<HTMLImageElement>;

  baseImageSize = 840;
  imageScale = 12;
  targetImageSize = this.baseImageSize / this.zoom;
  borderThickness = this.targetImageSize * (80 / 804);
  marginx = this.targetImageSize * (24 / 840);
  marginy = this.targetImageSize * (77 / 840);
  tilewidth = this.targetImageSize - (2 * this.marginx) - this.borderThickness;
  tileheight = this.targetImageSize - (2 * this.marginy) - this.borderThickness;
  tileSizeHalf = this.tilewidth / 2; // aka size, radius outer circle
  mapSize = 0;

  async ngAfterViewInit(): Promise<void> {
    this.cloudsContext = this.cloudsCanvas.nativeElement.getContext("2d")!;
    this.backgroundContext = this.backgroundCanvas.nativeElement.getContext("2d")!;
    this.charactersContext = this.charactersCanvas.nativeElement.getContext("2d")!;
  }

  setScale(zoom: number) {
    this.zoom = zoom;
    this.targetImageSize = this.baseImageSize / this.zoom;// / (4 + this.imageScale);
    this.borderThickness = this.targetImageSize * (80 / 804);
    this.marginx = this.targetImageSize * (24 / 840);
    this.marginy = this.targetImageSize * (77 / 840);
    this.tilewidth = this.targetImageSize - (2 * this.marginx) - this.borderThickness;
    this.tileheight = this.targetImageSize - (2 * this.marginy) - this.borderThickness;
    this.tileSizeHalf = this.tilewidth / 2; // aka size, radius outer circle
    this.setCanvasSizeAndConfig();
    this.handleMap();
  }

  handleMap() {
    let game = this.game;
    var x = performance.now();
    this.charactersContext.clearRect(0, 0, this.charactersCanvas.nativeElement.width, this.charactersCanvas.nativeElement.height)
    var a = performance.now();
    if (this.mapSize != game.map.mapSize) {
      this.mapSize = game.map.mapSize;
      this.setCanvasSizeAndConfig();
    }
    var b = performance.now();
    this.drawMap(game.map.tiles);
    var c = performance.now();
    this.drawPlayers(game.players);
    var d = performance.now();
    this.drawAttackers(game.attackers);
    var y = performance.now();
    // console.log("clear Time:", a - x);
    // console.log("canvas Time:", b - a);
    // console.log("tiles Time:", c - b);
    // console.log("players Time:", d - c);
    // console.log("attackers Time:", y - d);
    // console.log("Total Time:", y - x);
  }

  drawMap(tiles: MapTile[]) {
    for (let index = 0; index < tiles.length; index++) {
      const element = tiles[index];
      var pos = this.vectorStringToPixel(element.pos);
      this.drawTile(pos.x, pos.y, element);
    }
  }
  neigbors = [
    { x: +1, y: 0, z: -1 },  // top left
    { x: +1, y: -1, z: 0 }, // top
    { x: 0, y: -1, z: +1 }, // top right
    { x: -1, y: 0, z: +1 }, // bottom left
    { x: -1, y: +1, z: 0 }, // bottom
    { x: 0, y: +1, z: -1 }  // bottom right
  ]
  drawClouds() {
    let distance = 28;
    for (let i = -28; i <= distance; i++) {
      for (let j = Math.max(-distance, -i - distance); j <= Math.min(+distance, -i + distance); j++) {
        var pos = this.vectorStringToPixel(`${i},${j},${-i - j}`);
        this.drawCloud(pos.x, pos.y);
      }
    }
  }
  drawPlayers(players: Character[]) {
    //this.charactersContext.filter = "drop-shadow(-12px 8px 5px black)"; // Too big performance impact
    for (let index = 0; index < players.length; index++) {
      const player = players[index];
      var pos = this.vectorStringToPixel(player.pos);
      this.drawPlayer(pos.x, pos.y, player as Player);
    }
  }
  drawAttackers(attackers: Character[]) {
    //this.charactersContext.filter = "drop-shadow(12px 8px 5px black)"; // Too big performance impact
    for (let index = 0; index < attackers.length; index++) {
      const attacker = attackers[index];
      var pos = this.vectorStringToPixel(attacker.pos);
      this.drawAttacker(pos.x, pos.y, attacker.name.split("_")[0]);
    }
  }

  canvasCenter = { x: 0, y: 0 };
  setCanvasSizeAndConfig() {
    var width = this.tileSizeHalf * (this.mapSize + 1) * 3;
    var height = this.tileheight * (this.mapSize + 1) * 2;
    this.canvasCenter.x = width / 2 | 0; // efficient Math.floor
    this.canvasCenter.y = height / 2 | 0; // efficient Math.floor
    this.cloudsCanvas.nativeElement.width = width;
    this.cloudsCanvas.nativeElement.height = height;
    this.backgroundCanvas.nativeElement.width = width;
    this.backgroundCanvas.nativeElement.height = height;

    this.charactersCanvas.nativeElement.width = width;
    this.charactersCanvas.nativeElement.height = height;
    this.drawClouds();
  }

  static sqrt3 = Math.sqrt(3);
  vectorStringToPixel(vector: string) {
    var pos = VectorStringToObject(vector);
    return {
      x: this.tileSizeHalf * (1.5 * pos.x) + this.canvasCenter.x,
      y: this.tileSizeHalf * (GameWindowComponent.sqrt3 / 2 * pos.x + GameWindowComponent.sqrt3 * pos.y) + this.canvasCenter.y
    }
  }

  drawCloud(x: number, y: number) {
    x -= this.tileSizeHalf;
    y -= this.tileSizeHalf;
    let offset = TilesOffsets["cloud"];
    let randomTile = Math.floor(Math.abs(x + y)) % 10;
    this.cloudsContext.drawImage(this.tilesimg.nativeElement, this.baseImageSize * randomTile, this.baseImageSize * offset, this.baseImageSize, this.baseImageSize, x, y, this.targetImageSize, this.targetImageSize);
  }
  drawTile(x: number, y: number, tile: MapTile) {
    x -= this.tileSizeHalf;
    y -= this.tileSizeHalf;
    let offset = TilesOffsets[tile.resource] ?? 1;
    let randomTile = (Math.abs(x + y) | 0) % 10;
    this.backgroundContext.drawImage(this.tilesimg.nativeElement, this.baseImageSize * randomTile, this.baseImageSize * offset, this.baseImageSize, this.baseImageSize, x, y, this.targetImageSize, this.targetImageSize);
    if (tile.building) {
      offset = BuidingsOffsets[tile.building] ?? -1;
      this.backgroundContext.drawImage(this.buildingsimg.nativeElement, this.baseImageSize * offset, 0, this.baseImageSize, this.baseImageSize, x, y, this.targetImageSize, this.targetImageSize);
    }
  }
  // MainHand,
  // Additional,
  // Head,
  // Body,
  // Hands,
  // Legs,
  // Feet
  drawPlayer(x: number, y: number, player: Player) {
    x = x - this.tileSizeHalf * 1.5;
    y = y - this.tileSizeHalf;
    var type = this.getPlayerType(player);
    let offset = PlayersOffsets[type] ?? 0;
    this.charactersContext.drawImage(this.playersimg.nativeElement, this.baseImageSize * offset, 0, this.baseImageSize, this.baseImageSize, x, y, this.targetImageSize, this.targetImageSize);
  }
  drawAttacker(x: number, y: number, type: string) {
    x = x - this.tileSizeHalf / 2;
    y = y - this.tileSizeHalf;
    let offset = AttackersOffsets[type] ?? 0;
    this.charactersContext.drawImage(this.attackersimg.nativeElement, this.baseImageSize * offset, 0, this.baseImageSize, this.baseImageSize, x, y, this.targetImageSize, this.targetImageSize);
  }

  getPlayerType(player: Player): string {
    if (player.equipedItems['Additional'] == "horse") {
      // With horses
      if (player.equipedItems['Head'] == "plate_helmet"
        && player.equipedItems['Body'] == "plate_armor"
        && player.equipedItems['Hands'] == "plate_gloves"
        && player.equipedItems['Legs'] == "plate_pants"
        && player.equipedItems['Feet'] == "plate_boots"
      ) return "horse_paladin";
      else if (player.equipedItems['Head'] == "plate_helmet"
        || player.equipedItems['Body'] == "plate_armor"
        || player.equipedItems['Hands'] == "plate_gloves"
        || player.equipedItems['Legs'] == "plate_pants"
        || player.equipedItems['Feet'] == "plate_boots"
      ) return "horse_knight";
      else return "horse_man";
    }
    else {
      let main = player.equipedItems["Main"];
      // Without horses
      if (player.equipedItems['Head'] == "plate_helmet"
        && player.equipedItems['Body'] == "plate_armor"
        && player.equipedItems['Hands'] == "plate_gloves"
        && player.equipedItems['Legs'] == "plate_pants"
        && player.equipedItems['Feet'] == "plate_boots"
      ) return "paladin";
      else if (player.equipedItems['Head'] == "plate_helmet"
        || player.equipedItems['Body'] == "plate_armor"
        || player.equipedItems['Hands'] == "plate_gloves"
        || player.equipedItems['Legs'] == "plate_pants"
        || player.equipedItems['Feet'] == "plate_boots"
      ) return "knight";
      else if (main == "sickle") return "druid";
      else if (main == "pickaxe") return "miner";
      else if (main == "scythe") return "farmer";
      else if (main == "dagger" || main == "sword" || main == "hammer" || main == "sword_of_bleeding") {
        if (player.equipedItems['Head']
          || player.equipedItems['Body']
          || player.equipedItems['Hands']
          || player.equipedItems['Legs']
          || player.equipedItems['Feet']) return "warrior"
        else return "hero";
      }
      else if (main == "spear") return "pikeman";
      else if (main == "sling" || main == "shortbow" || main == "longbow" || main == "crossbow") return "archer";
      else if (player.boy) return "boy"
      else return "girl";
    }
  }
}
