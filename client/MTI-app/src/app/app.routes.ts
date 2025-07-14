import { Routes } from '@angular/router';
import { AddItem } from './features/inventory-management/add-item/add-item';
import { InventoryManagement } from './features/inventory-management/inventory-managment/inventory-management';
import { EditItem } from './features/inventory-management/edit-item/edit-item';


export const routes: Routes = [


  {
    path: 'items',
    component: InventoryManagement,
  
  },
    {
    path: 'edit-item/:id',
    component: EditItem,

    },
  {
    path: 'add-item',
    component: AddItem,
 
  },

  { path: '', redirectTo: '/items', pathMatch: 'full' },
  { path: '**', redirectTo: '/items' },
];


