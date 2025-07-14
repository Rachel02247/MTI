import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { InventoryStore } from '../../../shared/state/InventoryState';
import { Item } from '../../../core/models/item.model';
import { CreateOrUpdateItem } from '../../../core/models/createOrUpdateItem.model';

@Component({
  selector: 'app-edit-item',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule
  ],
  templateUrl: './edit-item.html',
  styleUrls: ['./edit-item.scss']
})
export class EditItem implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private store = inject(InventoryStore);

  item: CreateOrUpdateItem = {
    name: '',
    description: '',
    category: '',
  };

  categories = ['Electronics', 'Tools', 'Office', 'Other'];
  itemId = signal<number>(0);


  constructor() {
    this.itemId.set(parseInt(this.route.snapshot.paramMap.get('id') || '0'));
  }

  ngOnInit() {
    const itemId = this.route.snapshot.paramMap.get('id');
    const foundItem = this.store.items().find(i => i.id.toString() === itemId);
    if (foundItem) {
      this.item = { ...foundItem };
    } else {
      this.snackBar.open('Item not found', 'Close', { duration: 3000 });
      this.router.navigate(['/inventory']);
    }
  }

  save() {
    this.store.updateItem([this.itemId(), this.item]);
    if (!this.store.error()) {
      this.snackBar.open('Item updated successfully', 'Close', { duration: 3000 });
      this.router.navigate(['/items']);
    } else {
      this.snackBar.open(`Update failed: ${this.store.error()}`, 'Close', { duration: 3000 });
    }
  }

  cancel() {
    this.router.navigate(['/items']);
  }
}
