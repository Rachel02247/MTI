import { Component, inject } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIcon } from '@angular/material/icon';
import { TenantsSelector } from '../tenants-selector/tenants-selector';
import { CommonModule } from '@angular/common';
import { InventorySignalR } from '../../../core/services/inventoy-signal-r';
import { MatTooltip } from '@angular/material/tooltip';


@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatToolbar,
    TenantsSelector,
    CommonModule,
    MatIcon,
    MatTooltip
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {

  private readonly signalR = inject(InventorySignalR);
  connectionStatus$ = this.signalR.connectionStatus$;

}
