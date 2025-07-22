import {
    patchState,
    signalStore,
    withComputed,
    withMethods,
    withState
} from '@ngrx/signals';
import { Item } from '../../core/models/item.model';
import { computed, effect, inject } from '@angular/core';
import { Inventory } from '../../core/services/Inventory';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { CreateOrUpdateItem } from '../../core/models/createOrUpdateItem.model';
import { TenantStore } from './TenantState';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';


type InventoryState = {
    items: Item[];
    isLoading: boolean;
    error: string | null;
    recentlyHighlightedId: number | null;
    recentlyHighlightedAction: 'add' | 'update' | 'delete' | 'checkin' | 'checkout' | null;
};

const initialState: InventoryState = {
    items: [],
    isLoading: false,
    error: null,
    recentlyHighlightedId: null,
    recentlyHighlightedAction: null,


};

export const InventoryStore = signalStore(
    { providedIn: 'root' },

    withState(initialState),

    withMethods((store, inventoryService = inject(Inventory), tenantStore = inject(TenantStore), snack = inject(MatSnackBar)) => {


        effect(() => {

            if (tenantStore.selectedTenant()) {
                loadItems();
            }
            else {
                clearItems();
            }
        });

        const loadItems = rxMethod<void>(
            pipe(
                switchMap(() => {
                    patchState(store, { isLoading: true });
                    return inventoryService.getItems().pipe(
                        tapResponse({

                            next: (items) => {
                                patchState(store, { items, error: null, isLoading: false });
                            },
                            error: (error: HttpErrorResponse) =>
                                patchState(store, {
                                    error: error.message ?? 'error loading items'
                                }),
                            finalize: () => patchState(store, { isLoading: false })
                        })
                    );
                })
            )
        );

        const clearItems = () => {
            patchState(store, { items: [], error: null });
        }

        const addItem = rxMethod<CreateOrUpdateItem>(
            pipe(
                switchMap((item) =>
                    inventoryService.addItem(item).pipe(
                        tapResponse({
                            next: (newItem) => {

                                patchState(store, {
                                    items: [...store.items(), newItem as Item], error: null

                                }),
                                    snack.open(`Item "${newItem.name}" added successfully`, 'Close', { duration: 4000 });
                                loadItems()
                            },
                            error: (error: HttpErrorResponse) => {
                                patchState(store, { error: error.error ?? 'error adding item' });
                                snack.open(`Failed to add item: ${error.error}`, 'Close', { duration: 4000 });
                            }
                        })
                    )
                )
            )
        );

        const checkoutItem = rxMethod<string>(
            pipe(
                switchMap((itemId) =>
                    inventoryService.checkOutItem(itemId).pipe(
                        tapResponse({
                            next: () => {
                                patchState(store, {
                                    items: store.items().map((item) =>
                                        item.id.toString() === itemId ? { ...item, isCheckedOut: true } : item
                                    ), error: null
                                });
                                snack.open(`Item checked out successfully`, 'Close', { duration: 4000 });
                                loadItems();
                            },
                            error: (error: HttpErrorResponse) => {

                                patchState(store, { error: error.message ?? 'error checkin out item' })
                                snack.open(`Failed to check out item: ${error.error}`, 'Close', { duration: 4000 });

                            }
                        })
                    )
                )
            )
        );

        const checkinItem = rxMethod<string>(
            pipe(
                switchMap((itemId) =>
                    inventoryService.checkInItem(itemId).pipe(
                        tapResponse({
                            next: () => {
                                patchState(store, {
                                    items: store.items().map((item) =>
                                        item.id.toString() === itemId ? { ...item, isCheckedOut: false } : item
                                    ), error: null
                                });
                                snack.open(`Item checked in successfully`, 'Close', { duration: 4000 });
                                loadItems();
                            },
                            error: (error: HttpErrorResponse) => {
                                patchState(store, { error: error.message ?? 'error checking in item' })
                                snack.open(`Failed to check in item: ${error.error}`, 'Close', { duration: 4000 });
                            }
                        })
                    )
                )
            )
        );

        const softDeleteItem = rxMethod<string>(
            pipe(
                switchMap((itemId) =>
                    inventoryService.deleteItem(itemId).pipe(
                        tapResponse({
                            next: () => {
                                patchState(store, {
                                    items: store.items().map((item) =>
                                        item.id.toString() === itemId ? { ...item, isDeleted: true } : item
                                    ), error: null
                                });
                                snack.open(`Item deleted successfully`, 'Close', { duration: 4000 });
                                loadItems();
                            },
                            error: (error: HttpErrorResponse) => {
                                patchState(store, { error: error.message ?? 'error delete tem' })
                                snack.open(`Failed to delete item: ${error.error}`, 'Close', { duration: 4000 });
                            }
                        })
                    )
                )
            )
        );

        const updateItem = rxMethod<[number, CreateOrUpdateItem]>(
            pipe(
                switchMap(([itemId, updatedItem]) =>
                    inventoryService.updateItem(itemId, updatedItem).pipe(
                        tapResponse({
                            next: (resultItem) => {
                                patchState(store, {
                                    items: store.items().map((item) =>
                                        itemId === item.id ? resultItem : item
                                    ), error: null
                                });
                                snack.open(`Item "${resultItem.name}" updated successfully`, 'Close', { duration: 4000 });
                                loadItems();
                            },
                            error: (error: HttpErrorResponse) => {
                                patchState(store, {
                                    error: error.message ?? 'error updating item'
                                })
                                snack.open(`Failed to update item: ${error.error}`, 'Close', { duration: 4000 });
                            }

                        })
                    )
                )
            )
        );



        const itemAdded = (item: Item) => {

            console.log("SignalR - item added", item);

            patchState(store, {
                items: [...store.items(), item],
                recentlyHighlightedId: item.id,
                recentlyHighlightedAction: 'add'

            });

            snack.open(`🟢 Item "${item.name}" was added by another user`, 'Close', { duration: 4000 });

            setTimeout(() => {
                patchState(store, { recentlyHighlightedId: null, recentlyHighlightedAction: null });
            }, 3000);
        };



        const itemUpdated = (item: Item) => {
            patchState(store, {
                items: store.items().map(i => i.id === item.id ? item : i),
                recentlyHighlightedId: item.id,
                recentlyHighlightedAction: 'update'
            });

            snack.open(`🔄 Item "${item.name}" was updated by another user`, 'Close', { duration: 4000 });

            setTimeout(() => {
                patchState(store, { recentlyHighlightedId: null, recentlyHighlightedAction: null });
            }, 3000);
        };




        const itemDeleted = (id: number) => {
            const deletedItem = store.items().find(i => i.id === id);
            patchState(store, {
                recentlyHighlightedId: id,
                recentlyHighlightedAction: 'delete',

            });
            snack.open(`🗑️ Item "${deletedItem?.name ?? id}" was deleted by another user`, 'Close',
                { duration: 4000 });

            setTimeout(() => {
                patchState(store, {
                    items: store.items().filter(i => i.id !== id),
                    recentlyHighlightedId: null,
                    recentlyHighlightedAction: null
                });
            }, 3000);
        };

        const itemCheckedOut = (id: number) => {
            const checkedOutItem = store.items().find(i => i.id === id);
            if (!checkedOutItem) return;
            patchState(store, {
                items: store.items().map(i =>
                    i.id === id ? { ...i, isCheckedOut: true } : i
                ),
                recentlyHighlightedId: id,
                recentlyHighlightedAction: 'checkout'
            });
            snack.open(`📤 Item "${checkedOutItem.name}" was checked out by another user`, 'Close', { duration: 4000 });
            setTimeout(() => {
                patchState(store, { recentlyHighlightedId: null, recentlyHighlightedAction: null });
            }, 3000);
        };

        const itemCheckedIn = (id: number) => {
            const checkedInItem = store.items().find(i => i.id === id);
            if (!checkedInItem) return;
            patchState(store, {
                items: store.items().map(i =>
                    i.id === id ? { ...i, isCheckedOut: false } : i
                ),
                recentlyHighlightedId: id,
                recentlyHighlightedAction: 'checkin'
            });
            snack.open(`📥 Item "${checkedInItem.name}" was checked in by another user`, 'Close', { duration: 4000 });
            setTimeout(() => {
                patchState(store, { recentlyHighlightedId: null, recentlyHighlightedAction: null });
            }, 3000);
        };



        return {
            loadItems,
            clearItems,
            addItem,
            checkoutItem,
            checkinItem,
            softDeleteItem,
            updateItem,
            itemAdded,
            itemUpdated,
            itemDeleted,
            itemCheckedOut,
            itemCheckedIn
        };
    })

);