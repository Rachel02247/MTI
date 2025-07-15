import { Injectable, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { InventoryStore } from '../../shared/state/InventoryState';
import { TenantStore } from '../../shared/state/TenantState';
import { Item } from '../models/item.model';
import { environment } from '../../../environments/environment';
import { markItemAsHandled, wasItemHandled } from './../utils/signalr-utils'

@Injectable({ providedIn: 'root' })
export class InventorySignalR {

  private readonly inventoryStore = inject(InventoryStore);
  private readonly tenantStore = inject(TenantStore);

  private hubConnection!: signalR.HubConnection;
  private readonly tenantId = signal(this.tenantStore.selectedTenant());
  private apiUrl = environment.apiBaseUrl;


  public connectionStatus$ = new BehaviorSubject<'connected' | 'disconnected' | 'reconnecting'>('disconnected');

  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.apiUrl}/hubs/inventory`)
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    this.hubConnection.onclose(() => this.connectionStatus$.next('disconnected'));
    this.hubConnection.onreconnecting((error) => {
      console.warn('SignalR is reconnecting...', error);
      this.connectionStatus$.next('reconnecting');
    });
    this.hubConnection.onreconnected((connectionId) => {
      console.log('SignalR reconnected!', connectionId);
      this.connectionStatus$.next('connected');
    });

    this.hubConnection
      .start()
      .then(() => {
        this.connectionStatus$.next('connected');
        this.registerHandlers();
      })
      .catch(err => {
        console.error('SignalR connection failed', err);
        this.connectionStatus$.next('disconnected');
      });
  }



  private registerHandlers() {
    this.hubConnection.on('itemAdded', (item: Item) => {
      if (item.tenantId === this.tenantId() && !wasItemHandled(item.id)) {
        this.inventoryStore.itemAdded(item);
      }
    });

    this.hubConnection.on('itemUpdated', (item: Item) => {
      if (item.tenantId === this.tenantId() && !wasItemHandled(item.id)) {
        this.inventoryStore.itemUpdated(item);
      }
    });

    this.hubConnection.on('itemDeleted', (item: Item) => {
      if (item.tenantId === this.tenantId() && !wasItemHandled(item.id)) {
        this.inventoryStore.itemDeleted(item.id);
      }
    });
  }

}
