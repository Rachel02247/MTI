import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { TenantDetail } from '../models/tenant.model';

@Injectable({
  providedIn: 'root'
})
export class Tenant {
  
   private http = inject(HttpClient);

  private apiUrl: string = `${environment.apiBaseUrl}/tenant`;

  getTenants(): Observable<TenantDetail[]> {
    return this.http.get<TenantDetail[]>(this.apiUrl);
  }

}
