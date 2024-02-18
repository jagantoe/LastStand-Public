import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { Observable, filter, merge, shareReplay, switchMap, tap, throttleTime, timer } from 'rxjs';
import { Game } from './types';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  http = inject(HttpClient)

  private hostUrl: string | null = null;

  private refresh = signal(0);
  private refresh$ = toObservable(this.refresh).pipe(throttleTime(10000));
  public currentGame = signal<string | null>(null);

  public games$ = merge(timer(1000, 60000), this.refresh$)
    .pipe(
      switchMap(x => this.getGames())
    );

  private lock = signal<boolean>(true);
  game$ = merge(timer(0, 950), this.refresh$).pipe(
    filter(x => !!this.currentGame() && this.lock()),
    tap(x => this.lock.set(false)),
    switchMap(x => this.get(this.currentGame()!)),
    tap(x => this.lock.set(true)),
    shareReplay()
  )

  constructor() {
    this.loadHost();
  }

  public getGames(): Observable<string[]> {
    return this.http.get(this.hostUrl + "/Grain/GetGames") as Observable<string[]>;
  }

  public get(game: string): Observable<Game> {
    return this.http.get(this.hostUrl + "/Grain/GetState/" + game) as Observable<Game>;
  }


  public setGame(game: string) {
    this.currentGame.set(game);
  }

  public refreshData(): void {
    this.refresh.set(Math.random());
  }

  public askHost(): void {
    let host = window.prompt("Please enter the host url");
    if (host) {
      this.setHost(host);
    }
    else this.askHost();
  }
  public setHost(host: string): void {
    this.hostUrl = host;
    this.refresh();
    localStorage.setItem("host", host);
  }
  private loadHost() {
    var host = localStorage.getItem("host");
    if (host) {
      this.setHost(host);
    }
    else this.askHost();
  }
}
