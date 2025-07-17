import { Injectable, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { InventoryStore } from '../../shared/state/InventoryState';
import { TenantStore } from '../../shared/state/TenantState';
import { Item } from '../models/item.model';
import { environment } from '../../../environments/environment';

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

  private startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.apiUrl}/hubs/inventory`)
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();


    this.hubConnection.onclose(() => {
      console.warn('SignalR disconnected');
      this.connectionStatus$.next('disconnected');
    });

    this.hubConnection.onreconnecting((error) => {
      console.warn('SignalR is reconnecting...', error);
      this.connectionStatus$.next('reconnecting');
    });

    this.hubConnection.onreconnected(async (connectionId) => {
      console.log('SignalR reconnected with connectionId:', connectionId);
      this.connectionStatus$.next('connected');


      await this.registerTenant();



    });

    this.hubConnection
      .start()
      .then(async () => {
        console.log('SignalR connected');
        this.connectionStatus$.next('connected');

        await this.registerTenant();
        this.registerHandlers();

        const curConnectionId = signal<string | null>(this.hubConnection.connectionId);
        this.tenantStore.signalRConnectionId = curConnectionId;
      })
      .catch((err) => {
        console.error('SignalR connection failed:', err);
        this.connectionStatus$.next('disconnected');
      });
  }


  private async registerTenant(): Promise<void> {
    const tenantId = this.tenantId();
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected && tenantId) {
      try {
        await this.hubConnection.invoke("RegisterTenant", tenantId);
        console.log(`Registered to tenant group: ${tenantId}`);
      } catch (error) {
        console.error(`Failed to register tenant ${tenantId}`, error);
      }
    }
  }


  private registerHandlers() {
    this.hubConnection.on('itemAdded', (item: Item) => {
      if (item.tenantId === this.tenantId()) {
        this.inventoryStore.itemAdded(item);
      }
    });

    this.hubConnection.on('itemUpdated', (item: Item) => {
      if (item.tenantId === this.tenantId()) {
        this.inventoryStore.itemUpdated(item);
      }
    });

    this.hubConnection.on('itemDeleted', (item: Item) => {
      if (item.tenantId === this.tenantId()) {
        this.inventoryStore.itemDeleted(item.id);
      }
    });

    this.hubConnection.on('itemCheckOut', (itemId: number, tenantId: string) => {
      if (tenantId === this.tenantId()) {
        this.inventoryStore.itemCheckedOut(itemId);
      }
    });

    this.hubConnection.on('itemCheckIn', (itemId: number, tenantId: string) => {
      if (tenantId === this.tenantId()) {
        this.inventoryStore.itemCheckedIn(itemId);
      }
    });

  }

}
