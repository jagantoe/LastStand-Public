export interface Vector {
    x: number;
    y: number;
    z: number;
}

export function VectorStringToObject(value: string): Vector {
    var parts = value.split(",");
    return { x: +parts[0], y: +parts[1], z: +parts[2] };
}

export interface Game {
    name: string;
    roundHighScore: number;
    round: number;
    attackersKilled: number;
    timer: number;
    status: string;
    gameActive: boolean;
    baseHealth: number;
    resources: Resources;
    inventory: { [key: string]: number; };
    map: GameMap;
    players: Player[];
    attackers: Character[];
    staticCharacters: Character[];
    craftingEnabled: boolean;
    events: any[];
}

export interface Character {
    name: string;
    currentHealth: number;
    maxHealth: number;
    boy: boolean;
    pos: string;
    baseStats: Stats;
    dead: boolean;
    combatBehaviour: string;
    commands: string[];
    totalDamageDone: number;
}

export interface GameMap {
    tiles: MapTile[];
    mapSize: number;
}

export interface MapTile {
    pos: string;
    building: string;
    resource: string;
}

export interface Player extends Character {
    active: boolean;
    derivedStats: Stats;
    equipedItems: { [key: string]: string; };
    bag: Resources;
    deaths: number;
}

export interface Resources {
    grain: number;
    wood: number;
    stone: number;
    steel: number;
    limit: number;
    full: boolean;
}

export interface Stats {
    strength: number;
    defense: number;
    speed: number;
    damage: number;
    attackSpeed: number;
    attackRange: number;
    pierce: number;
    block: number;
    damageTakenModifier: number;
    health: number;
    maxHealth: number;
    grainModifier: number;
    woodModifier: number;
    stoneModifier: number;
    steelModifier: number;
    carryModifier: number;
}
