import { Component, OnInit, signal } from '@angular/core';
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
import { Items } from '../../../core/services/items';
import { Item } from '../../../core/models/item.model';


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
    MatTooltipModule
  ],
  templateUrl: './inventory-list.html',
  styleUrls: ['./inventory-list.scss']
})
export class InventoryList implements OnInit {

  displayedColumns: string[] = ['name', 'description', 'category', 'status', 'actions'];
  dataSource = new MatTableDataSource<Item>();

  filterValue = signal('');
  selectedCategory = signal('');
  categories = ['Electronics', 'Tools', 'Office', 'Other'];

  constructor(private snackBar: MatSnackBar, private itemService: Items) { }

  ngOnInit() {
    this.loadItems();
  }

  loadItems() {
    // // Sample data
    // const items: Item[] = [
    //   {
    //     id: 1,
    //     name: 'Dell Laptop #001',
    //     description: 'Dell Latitude',
    //     category: 'Electronics',
    //     isCheckedOut: true
    //   },
    //   {
    //     id: 2,
    //     name: 'HP Projector #002',
    //     description: 'HP Projector with HDMI Cable',
    //     category: 'Electronics',
    //     isCheckedOut: false
    //   },
    //   {
    //     id: 3,
    //     name: 'Bosch Drill #003',
    //     description: 'Bosch Electric Drill',
    //     category: 'Tools',
    //     isCheckedOut: false
    //   }
    // ];

    this.itemService.getItems().subscribe(

      
      (data: Item[]) => {
        debugger;
        this.dataSource.data = data;
        this.setupFilter();
      },
      (error) => {
        console.error('Failed to load items:', error);
      }
    )

  }

  setupFilter() {
    this.dataSource.filterPredicate = (data: Item, filter: string) => {
      const searchString = filter.toLowerCase();
      const categoryMatch = this.selectedCategory ? data.category === this.selectedCategory() : true;
      const textMatch = data.name.toLowerCase().includes(searchString) ||
        data.description.toLowerCase().includes(searchString);

      return categoryMatch && textMatch;
    };
  }

  applyFilter() {
    this.dataSource.filter = this.filterValue().trim().toLowerCase();
    this.setupFilter();
    this.dataSource._updateChangeSubscription();
  }

  onCategoryChange() {
    this.applyFilter();
  }

  clearFilters() {
    this.filterValue.set('');
    this.selectedCategory.set('');
    this.dataSource.filter = '';
  }

  borrowItem(item: Item) {
    if (!item.isCheckedOut) {
      item.isCheckedOut = true;
      this.itemService.checkOutItem(item.id.toString()).subscribe(
        () => {
          this.snackBar.open(`Item "${item.name}" borrowed successfully`, 'Close', {
            duration: 3000
          });
        }
      );
    }
  }

  returnItem(item: Item) {
    if (item.isCheckedOut) {
      item.isCheckedOut = false;
      this.itemService.checkInItem(item.id.toString()).subscribe(
        () => {
          this.snackBar.open(`Item "${item.name}" returned successfully`, 'Close', {
            duration: 3000
          });
        }
      );
    }
  }

  editItem(item: Item) {
    // Open edit dialog here
    console.log('Edit item:', item);
  }

  deleteItem(item: Item) {
    const index = this.dataSource.data.indexOf(item);
    if (index > -1) {
      this.dataSource.data.splice(index, 1);
      this.dataSource._updateChangeSubscription();
      this.snackBar.open(`Item "${item.name}" deleted successfully`, 'Close', {
        duration: 3000
      });
    }
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