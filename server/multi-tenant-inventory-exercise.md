
# 🎓 Exercise: Configurable Access Control for a Multi-Tenant Inventory API

This exercise challenges students to build a multi-tenant inventory management system using **.NET Minimal APIs**, **middleware**, **IOptions**, **configuration**, **EF Core**, and **DTOs with record types**.

It is intentionally designed to be difficult for AI tools to solve entirely due to architectural decisions, real-world ambiguity, and domain-driven requirements.

---

## 🧩 Scenario

You are building an inventory management system for a company that serves multiple tenants (e.g., logistics or warehouse providers). Each tenant (company) must have isolated data, configurable behavior, and custom rules that govern their usage of the system.

---

## 🛠 Requirements

### Backend Stack:
- **.NET 8 or higher**
- **Minimal APIs**
- **Entity Framework Core (SQLite or InMemory for demo)**
- **IOptions<T>** for tenant-based configuration
- **Middleware** for tenant resolution and enforcement
- **Records** and **DTOs** for request/response modeling

---

## 🧱 Functional Requirements

1. **Tenant Identification**
   - Tenant is determined using the `X-Tenant-ID` header.
   - If header is missing or invalid, return a `401 Unauthorized`.

2. **Tenant Settings (via Configuration)**
   - Each tenant has configuration loaded via `IOptions<T>`:
     - `EnableCheckout` (bool)
     - `MaxItemsPerUser` (int)
     - `AllowedItemCategories` (list of strings)
   - These settings must be enforced by middleware or services.

3. **Inventory Management**
   - Each tenant can:
     - Add new items
     - List their items
     - Check in/out items **only if allowed by config**

4. **Check-out Logic**
   - Validate:
     - If checkout is allowed for this tenant
     - If the user has exceeded `MaxItemsPerUser`
     - If the item belongs to an allowed category

5. **Soft Deletes**
   - Items should not be physically deleted, just marked inactive

---

## 🚧 Architectural Constraints

- Middleware must handle:
  - Tenant resolution
  - Configuration enforcement (e.g., disable checkout)
  - Per-request logging (store access log to file or console)
- Use `record` types for:
  - All DTOs and payloads (create, update, responses)
- Do not leak domain models in API responses
- Use dependency injection for services and repositories

---

## ✍️ Example Scenarios to Implement

1. Tenant `alpha-logistics` supports checkouts, max 3 items per user.
2. Tenant `beta-supply` has checkouts disabled and only allows `Electronics`.

Test these configurations and demonstrate enforcement.

---

## 🧠 Bonus Challenges

- Implement `IOptionsMonitor` of `IOptionsSnapshot` to support dynamic configuration reloading
- Add per-tenant feature toggles
- Implement a unit test project to test middleware edge cases

---

## ✅ Evaluation Criteria

| Aspect                  | Expectation                                                                 |
|------------------------|------------------------------------------------------------------------------|
| Middleware              | Proper tenant resolution and rejection                                       |
| Configuration           | Uses `IOptions<T>` properly for multi-tenant configs                        |
| EF Core modeling        | Tenant-based data isolation and relationship modeling                       |
| DTO and Record usage    | Proper separation of concerns; record types only for external contracts     |
| Real-world robustness   | Handles invalid headers, limits, and edge cases                             |
| Clean Architecture      | No business logic in Program.cs or DTOs                                     |
