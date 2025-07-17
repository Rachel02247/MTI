import { Component, computed, effect, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TenantStore } from '../../state/TenantState';
import { Tenant } from '../../../core/services/tenant';
import { TenantDetail } from '../../../core/models/tenant.model';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/core';
import { MatIcon } from '@angular/material/icon';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-tenants-selector',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSelect,
    MatOption,
    MatIcon,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './tenants-selector.html',
  styleUrls: ['./tenants-selector.scss']
})
export class TenantsSelector {

  store = inject(TenantStore);
  tenantService = inject(Tenant);



  tenants = this.store.tenants;
  selectedTenantName = this.store.selectedTenant;

  onChange(name: string) {
    this.store.setSelectedTenant(name);
  }

  clearSelection() {
    this.store.setSelectedTenant('');
  }
}







