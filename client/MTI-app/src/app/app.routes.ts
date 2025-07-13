import { Routes } from '@angular/router';
import { AddItem } from './features/inventory-management/add-item/add-item';
import { ItemDetailComponent } from './features/inventory-management/item-detail/item-detail';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { authGuard } from './core/guards/auth-guard';
import { InventoryManagement } from './features/inventory-management/inventory-managment/inventory-management';


export const routes: Routes = [

  { path: 'login', component: Login },
  { path: 'register', component: Register },

  {
    path: 'items',
    component: InventoryManagement,
    canActivate: [authGuard],
  },
    {
    path: 'item/:id',
    component: ItemDetailComponent,
    canActivate: [authGuard], 
    },
  {
    path: 'add-item',
    component: AddItem,
    canActivate: [authGuard],
  },

  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' },
];


