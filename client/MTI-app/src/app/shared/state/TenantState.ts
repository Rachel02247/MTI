import {
    signalStore,
    withState,
    withMethods,
    patchState
} from '@ngrx/signals';
import { inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { TenantDetail } from '../../core/models/tenant.model';
import { Tenant } from '../../core/services/tenant';

const SELECTED_TENANT_KEY = 'selectedTenant';

type TenantState = {
    tenants: TenantDetail[];
    selectedTenant: string | null;
    error: string | null;
    signalRConnectionId: string | null;
};

const initialState: TenantState = {
    tenants: [],
    selectedTenant: sessionStorage.getItem(SELECTED_TENANT_KEY),
    error: null,
    signalRConnectionId: null

};

export const TenantStore = signalStore(

    { providedIn: 'root' },

    withState(initialState),

    withMethods((store, tenantService = inject(Tenant)) => {

        const loadTenants = rxMethod<void>(
            pipe(
                switchMap(() =>
                    tenantService.getTenants().pipe(
                        tapResponse({
                            next: (tenants: TenantDetail[]) =>{
                                console.log('Tenants loaded:', tenants);
                                patchState(store, { tenants })
                                console.log('Tenants in store:', store.tenants());
                            },
                            error: (error: Error) =>
                                patchState(store, {
                                    error: error.message ?? 'error loading tenants'
                                })
                        })
                    )
                )
            )
        );

        const setSelectedTenant = (tenantId: string) => {
            sessionStorage.setItem(SELECTED_TENANT_KEY, tenantId);
            patchState(store, { selectedTenant: tenantId });
        };

        const clearSelectedTenant = () => {
            sessionStorage.removeItem(SELECTED_TENANT_KEY);
            patchState(store, { selectedTenant: null });
        };

        const getSelectedTenant = () => {
            const id = store.selectedTenant();
            return store.tenants().find(t => t.id.toString() === id) ?? null;
        };

        const getTenants = () => 
            {
                store.tenants();   
            }

        return {
            loadTenants,
            setSelectedTenant,
            clearSelectedTenant,
            getSelectedTenant,
            getTenants
        };
    })
);
