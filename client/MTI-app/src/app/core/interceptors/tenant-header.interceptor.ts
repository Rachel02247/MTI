import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TenantStore } from '../../shared/state/TenantState';

export const tenantHeaderInterceptor: HttpInterceptorFn = (req, next) => {

  console.log(req.url)
  if(req.url.includes('hub')){
    return next(req);
  }

  const store = inject(TenantStore);

  console.log('Tenant ID from store:', store.selectedTenant());
  const cloned = req.clone({
    setHeaders: {
      'X-Tenant-ID': store.selectedTenant() || '',
      'X-SignalR-Connection-ID': store.signalRConnectionId() || ''
    },
  });
  return next(cloned);
};
