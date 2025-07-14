import { Component, inject, OnInit } from '@angular/core';
import { InventoryList } from '../inenvtory-list/inventory-list';
import { InventoryStore } from '../../../shared/state/InventoryState';
import { TenantStore } from '../../../shared/state/TenantState';

@Component({
  selector: 'app-inventory-management',
  standalone: true,
  imports: [InventoryList],
  templateUrl: './inventory-management.html',
  styleUrl: './inventory-management.scss'
})
export class InventoryManagement implements OnInit {

  Inventorystore = inject(InventoryStore);
  TenantStore = inject(TenantStore);



  
  ngOnInit() {
    this.Inventorystore.loadItems();
    this.TenantStore.loadTenants();

  }



}
