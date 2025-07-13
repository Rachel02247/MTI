import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Items } from '../../../core/services/items';

@Component({
  selector: 'app-item-detail',
  templateUrl: './item-detail.html',
  styleUrls: ['./item-detail.scss'],
})
export class ItemDetailComponent implements OnInit {
  itemId!: number;
  item: any;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private inventoryService: Items
  ) { }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.itemId = idParam ? parseInt(idParam, 10) : 0;
    if (this.itemId) {
      this.loadItem();
    } else {
      this.error = 'item is not found';
      this.loading = false;
    }
  }

  loadItem() {
    this.inventoryService.getItemById(this.itemId).subscribe({
      next: (data) => {
        this.item = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'error loading item details';
        this.loading = false;
      },
    });
  }
}
