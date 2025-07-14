import { Component } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { TenantsSelector } from '../tenants-selector/tenants-selector';

@Component({
  selector: 'app-header',
  imports: [
    MatToolbar,
    TenantsSelector
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {

}
