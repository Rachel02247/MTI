import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Item } from '../models/item.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Items{

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiBaseUrl}/items`;

  }
  private apiUrl: string = '';

  getItems(): Observable<Item[]> {
    return this.http.get<Item[]>(this.apiUrl);
  }

  getItemById(id: number): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}/${id}`);
  }

  addItem(item: Item): Observable<Item> {
    return this.http.post<Item>(this.apiUrl, item);
  }

  checkOutItem(itemId: string): Observable<void> {
  return this.http.post<void>(`${this.apiUrl}/items/${itemId}/checkout`, {});
}

checkInItem(itemId: string): Observable<void> {
  return this.http.post<void>(`${this.apiUrl}/items/${itemId}/checkin`, {});
}

}
