import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { ReactiveFormsModule } from '@angular/forms';
import { Inventory } from '../../../core/services/Inventory';
import {  CreateOrUpdateItem } from '../../../core/models/createOrUpdateItem.model';
import { ActivatedRoute, Router } from '@angular/router';
import { InventoryStore } from '../../../shared/state/InventoryState';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-item',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatCardModule,
    ReactiveFormsModule
  ],
  templateUrl: './add-item.html',
  styleUrls: ['./add-item.scss']
})
export class AddItem {

  private fb = inject(FormBuilder);
  private store = inject(InventoryStore);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  categories = [ "Electronics", "Furniture", "Books",  "Jwelery" ];

  form = this.fb.group({
    name: ['', Validators.required],
    category: ['', Validators.required],
    description: ['']
  });

  
cancel() {
    this.form.reset();
    this.router.navigate(['/items']);
  }
  
  onSubmit() {
    if (this.form.valid) {
      const newItem: CreateOrUpdateItem = 
      {
        name: this.form.value.name || '',
        category: this.form.value.category || '',
        description: this.form.value.description || ''
      }
      console.log('New item added:', newItem);

      this.store.addItem(newItem)
        
      if(!this.store.error()) {
        this.snackBar.open('Item added successfully', 'Close', {
           duration: 3000 
          });
          this.form.reset();
          this.router.navigate(['/items']);
        }
      } else {
        this.snackBar.open('Please fill in all required fields', 'Close', { 
          duration: 3000 
        });

      this.form.reset();
    }
  }
}
