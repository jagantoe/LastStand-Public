import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
console.log("This was made with assets by Steve Colling. \nStore: https://stevencolling.itch.io/\nSite: https://stevencolling.com")