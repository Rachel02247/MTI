import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Item } from '../../../core/models/item.model';
import { InventoryStore } from '../../../shared/state/InventoryState';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatProgressSpinnerModule

  ],
  templateUrl: './inventory-list.html',
  styleUrls: ['./inventory-list.scss']
})

export class InventoryList {

   store = inject(InventoryStore);
  private router = inject(Router);


  displayedColumns: string[] = ['name', 'category', 'description', 'status', 'actions'];
  dataSource = signal(new MatTableDataSource<Item>());

  isLoading = signal(false);
  filterValue = signal('');
  selectedCategory = signal('');
  categories = ['Electronics', 'Tools', 'Office', 'Other'];


  
  borrowItem(item: Item) {

    if (!item.isCheckedOut) {
      this.store.checkoutItem(item.id.toString())

    }
  }


  returnItem(item: Item) {

    if (item.isCheckedOut) {
      this.store.checkinItem(item.id.toString());
    }

  }


  editItem(id: number) {
    this.router.navigate(['edit-item', id]);
  }


  deleteItem(item: Item) {
    this.store.softDeleteItem(item.id.toString())
  }

  addItem() {
    this.router.navigate(['/add-item']);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Available': return 'status-available';
      case 'Borrowed': return 'status-borrowed';
      case 'Unavailable': return 'status-unavailable';
      default: return '';
    }
  }
}