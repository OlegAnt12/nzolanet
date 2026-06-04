import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FaConfig, FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faGear } from '@fortawesome/free-solid-svg-icons';
import { faBell } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, FontAwesomeModule],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {
  configIcon = faGear;
  notifIcon = faBell;
}
