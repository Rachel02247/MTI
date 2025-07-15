// src/app/core/utils/signalr-utils.ts

const HANDLED_ITEMS_KEY = 'handledItems';

export function markItemAsHandled(itemId: number): void {
  const raw = localStorage.getItem(HANDLED_ITEMS_KEY);
  const handled = new Set<number>(raw ? JSON.parse(raw) : []);
  handled.add(itemId);
  localStorage.setItem(HANDLED_ITEMS_KEY, JSON.stringify(Array.from(handled)));
}

export function wasItemHandled(itemId: number): boolean {
  const raw = localStorage.getItem(HANDLED_ITEMS_KEY);
  const handled = new Set<number>(raw ? JSON.parse(raw) : []);
  return handled.has(itemId);
}

export function clearHandledItems(): void {
  localStorage.removeItem(HANDLED_ITEMS_KEY);
}
