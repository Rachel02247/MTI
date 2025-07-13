import { Component } from '@angular/core';
import { InventoryList } from '../inenvtory-list/inventory-list';

@Component({
  selector: 'app-inventory-management',
  standalone: true,
  imports: [InventoryList],
  templateUrl: './inventory-management.html',
  styleUrl: './inventory-management.scss'
})
export class InventoryManagement {

}
