import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TenantStore } from '../../shared/state/TenantState';

export const tenantHeaderInterceptor: HttpInterceptorFn = (req, next) => {

  const store = inject(TenantStore);

  console.log('Tenant ID from store:', store.selectedTenant());
  const cloned = req.clone({
    setHeaders: {
      'X-Tenant-ID': store.selectedTenant() || '',
    },
  });
  return next(cloned);
};
