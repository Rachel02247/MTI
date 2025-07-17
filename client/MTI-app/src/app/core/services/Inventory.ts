import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Item } from '../models/item.model';
import { Observable } from 'rxjs';
import { CreateOrUpdateItem } from '../models/createOrUpdateItem.model';

@Injectable({
  providedIn: 'root'
})
export class Inventory {

  private http = inject(HttpClient);

  private apiUrl: string = `${environment.apiBaseUrl}/items`;

  getItems(): Observable<Item[]> {
    return this.http.get<Item[]>(this.apiUrl);
  }

  getItemById(id: number): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}/${id}`);
  }

  addItem(item: CreateOrUpdateItem): Observable<Item> {
    return this.http.post<Item>(this.apiUrl, item);
  }

  checkOutItem(itemId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${itemId}/checkout`, {});
  }

  checkInItem(itemId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${itemId}/checkin`, {});
  }

  deleteItem(itemId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${itemId}`);
  }

  updateItem(itemId: number, item: CreateOrUpdateItem): Observable<Item> {
    return this.http.put<Item>(`${this.apiUrl}/${itemId}`, item);
  }


}
