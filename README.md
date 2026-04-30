# NET-4.8-Clean-architecure / CleanApp

This repository is named **NET-4.8-Clean-architecure** (folder spelling). The .NET product inside it is **CleanApp**: a **.NET Framework 4.8** sample arranged in a **layered “clean architecture” style** (Domain → Application → Infrastructure), aimed at **user accounts, role-based access control (RBAC), OTP-based messaging, and Stripe billing hooks**.

Use this document as **orientation for new developers**: what the system is trying to be, which concepts live where, and how the pieces connect.

---

## Functional features (what is implemented)

**Authentication**

- Sign up with email (registration flow).
- Log in with email and password.
- Send one-time password (OTP) by **SMS** (Twilio).
- Send OTP by **WhatsApp** (Twilio).
- Verify OTP.

**Users**

- Create user.
- List users with **paging**, **search**, and optional **include deleted**.
- Get user by id.
- Update user.
- **Soft delete** user.
- **Restore** soft-deleted user.

**Roles and permissions (RBAC)**

- Create role.
- List roles with **paging**, **search**, and optional **include deleted**.
- Get role by id.
- Update role.
- **Soft delete** role.
- **Restore** soft-deleted role.
- Domain model supports **permissions** and **role–permission** links (`Permission`, `RolePermission`).

**Stripe / billing**

- Create **Stripe Checkout** session from a list of **price IDs** for a given user.
- Handle **Stripe webhooks** (incoming event payload processing).
- Persist **customer billing profile**: link app **user** to **Stripe customer id** (`Customer` entity).

**Data and cross-cutting**

- **Entity Framework Core** persistence (MySQL as configured).
- **Soft delete** and **active** flags plus **created/updated** timestamps on base entities.

---

## 1. What this project is

**CleanApp** is a backend-oriented codebase that models:

- **People and access**: application users, optional roles, and permissions linked through a join entity.
- **Authentication flows**: email/password login and signup, plus **OTP** delivery via **Twilio** (SMS and WhatsApp paths exist in the application layer).
- **Payments (integration surface)**: **Stripe** checkout sessions and webhook handling, with a **Customer** record tying a Stripe customer id to an application **User**.

The **CleanApp.API** folder contains ASP.NET-era assets (`Web.config`, Razor views, scripts). In this tree, the **API host project is not fully represented** (for example, there is no `CleanApp.API.csproj` alongside the other projects, and no Web API controllers are present in source). The **core business and persistence logic** lives in **Domain**, **Application**, and **Infrastructure** class libraries. Treat **Infrastructure + Application** as the library you would reference from a proper Web API or MVC host once that project is wired up.

---

## 2. Repository layout

Typical path on disk (one level of nesting may vary):

`NET-4.8-Clean-architecure/.NET-4.8-Clean-architecure/`

| Project | Responsibility |
|--------|-----------------|
| **CleanApp.Domain** | Entities and shared base types. No EF, no HTTP, no external SDKs. |
| **CleanApp.Application** | “Use cases”: services that orchestrate calls into repositories (`AuthService`, `UserService`, `RoleService`, `StripeService`). |
| **CleanApp.Infrastructure** | EF Core `DbContext`, migrations, DTOs used by repositories, repository implementations, `ResponseDto`, `SingletonContext`. |
| **CleanApp.API** | Host-related files (`Web.config`, views). Intended entry point for HTTP; **complete the project file and controllers** if you need a runnable site from this repo alone. |

There is **no `.sln` file** in the repository at the time of writing. Open the individual `.csproj` files in Visual Studio or generate a solution that references all four projects.

---

## 3. Technology stack

| Area | Choice |
|------|--------|
| Runtime | **.NET Framework 4.8** |
| ORM | **Entity Framework Core 3.1** |
| Database provider (configured) | **MySQL** via `UseMySQL` in `ApplicationDbContext` |
| Messaging | **Twilio** (OTP / WhatsApp in `AuthRepository`) |
| Payments | **Stripe.net** (checkout + webhook in `StripeRepository`) |
| JSON / misc | Newtonsoft.Json, RestSharp (referenced from Infrastructure) |

NuGet packages are restored under a **`packages`** folder relative to the solution-style layout (see each project’s `packages.config` / references).

---

## 4. Domain model (for developers)

This section is the **domain documentation**: vocabulary and relationships you will see in code and in the database.

### 4.1 Cross-cutting: `BaseEntity`

Most persisted entities inherit **`BaseEntity`** (`CleanApp.Domain.CommonEntities`):

- **`ID`**: integer primary key (see note below on `User`).
- **`IsActive`**, **`IsDeleted`**: soft lifecycle flags (default active, not deleted).
- **`CreatedDateTime`**, **`UpdatedDateTime`**: audit timestamps. `CreatedDateTime` is set in the `BaseEntity` constructor.

### 4.2 `User`

Represents an application user: name, **email**, **password** (storage strategy is whatever the repository implements—treat as sensitive), **phone**, optional **role**, and **OTP** fields (`OTP`, `OTPExpireTime`) for verification flows.

**Note:** `User` inherits `BaseEntity` but also declares its own **`ID`** property. That is redundant with `BaseEntity.ID` and is worth reconciling if you evolve the schema (EF may map one or the other depending on configuration).

### 4.3 `Role` and `Permission` (RBAC)

- **`Role`**: named role (`RoleName`).
- **`Permission`**: named permission with a **`Type`** string (caller-defined categorization).
- **`RolePermission`**: many-to-many link between roles and permissions.

Together they form a classic **RBAC** model: assign permissions to roles, assign roles to users.

### 4.4 `Customer` (billing profile)

Links an internal **`UserId`** to **`Email`** and a **`stripeCustomerId`**. This is the persistence side of “this user has a Stripe customer record.”

### 4.5 Tables (EF migrations)

Initial migration and snapshot live under **`CleanApp.Infrastructure/Migrations`**. Table names align with entity pluralization (e.g. `Users`, `Roles`, `Permissions`, `RolePermissions`, `Customers`).

---

## 5. Application boundaries and flow

### 5.1 Layering rules (intended)

1. **Domain** references nothing outside .NET BCL / annotations used for mapping hints.
2. **Application** implements workflows and depends on **abstractions** (`IAuthRepository`, `IUserRepository`, etc.). In this codebase, **concrete repositories are still constructed inside services** (`new AuthRepository()`), so dependency inversion is **partial**—easy to refactor toward constructor injection later.
3. **Infrastructure** implements repositories, talks to **EF Core**, **Twilio**, **Stripe**, and **`ConfigurationManager`** (`app.config` / `web.config`).

### 5.2 Standard API shape: `ResponseDto`

Most service/repository methods return **`ResponseDto`** (`CleanApp.Infrastructure`):

- **`Status`**: success flag.
- **`Message`**: human-readable outcome.
- **`Data`**: arbitrary payload (`object`), e.g. anonymous type with `UserId` after login.

### 5.3 DbContext lifetime: `SingletonContext`

`SingletonContext` exposes a **single static `ApplicationDbContext` instance** for the whole AppDomain. That is simple for demos but **problematic for production** (threading, connection lifetime, testability). New work should move toward **scoped per request** contexts and injected `DbContext`.

### 5.4 Configuration

- **Connection string**: `ApplicationDbContext.OnConfiguring` uses `ConfigurationManager.ConnectionStrings["connectionString"]` when present; otherwise it falls back to a **hard-coded MySQL** connection string (localhost). Align this with your environment and remove machine-specific defaults for shared development.
- **Stripe / Twilio**: read from **`Web.config` `appSettings`** in the API project (keys such as `StripeApiKey`, `StripeEndpointSecret`, `TwilioAccountSid`, etc.).

**Security:** If this repository ever contained real keys, **rotate them** and use **User Secrets**, environment variables, or a secret store—never commit production credentials.

---

## 6. Major feature areas (by service)

| Service | Responsibility |
|---------|------------------|
| **AuthService** | Login, signup, send OTP (SMS / WhatsApp), verify OTP—delegates to `AuthRepository`. |
| **UserService** | CRUD-style user operations, paging, soft delete/restore—delegates to `UserRepository`. |
| **RoleService** | Role management—delegates to `RoleRepository`. |
| **StripeService** | Checkout session creation and webhook processing—delegates to `StripeRepository`. |

DTOs for HTTP-shaped input live under **`CleanApp.Infrastructure/DTOs`** (e.g. `LoginDto`, `SignupDto`, `AddUserDto`). In a stricter clean architecture, those types might live in Application or a dedicated Contracts project; here they sit with Infrastructure for convenience.

---

## 7. Getting started (new developer checklist)

1. **Clone** the repo and locate the nested folder:  
   `NET-4.8-Clean-architecure/.NET-4.8-Clean-architecure/`
2. **Install**: Visual Studio 2019/2022 with **.NET Framework 4.8** targeting pack; **MySQL** server locally or remote.
3. **Restore NuGet** packages for `CleanApp.Infrastructure` (and others as you add them to a solution).
4. **Set `connectionString`** in config to your MySQL database (name in fallback code: `CleanAppDB`).
5. **Apply database schema**: use EF Core migrations tooling compatible with this project version, or generate scripts from the existing migration—follow your team’s standard.
6. **Configure** Stripe test keys and Twilio sandbox numbers in configuration for local runs.
7. **Run**: You need a **host project** that references Application/Infrastructure and exposes HTTP endpoints. If `CleanApp.API` is incomplete in your clone, create or restore an ASP.NET Web API / MVC project and register services and routes there.

---

## 8. Known rough edges (good first issues)

- **No solution file** in repo; add a `.sln` for team onboarding.
- **API project completeness**: verify `CleanApp.API` builds and contains controllers / WebApiConfig / `Global.asax` as needed.
- **DI**: services instantiate repositories directly; introduce **DI container** (e.g. Unity, Autofac, or Simple Injector for .NET Framework) and constructor injection.
- **`SingletonContext`**: replace with scoped `DbContext` per request.
- **`StripeService` namespace** (`CleanApplication.Application.Services.Stripes`) vs assembly `CleanApp.Application`—align namespaces to reduce confusion.
- **`UserService` usings**: duplicate / wrong namespace imports (`CleanApplication.Infrastructure` vs `CleanApp.Infrastructure`) should be cleaned up.
- **Secrets in source**: move keys out of committed `Web.config` where applicable.

---

## 9. Glossary

| Term | Meaning here |
|------|----------------|
| **CleanApp** | The product name used in assemblies and namespaces. |
| **RBAC** | Role-based access control via `Role`, `Permission`, `RolePermission`. |
| **OTP** | One-time password sent via Twilio for verification. |
| **Customer** | Billing-side link between app user and Stripe customer id. |

---

## 10. Summary

**Repository name:** `NET-4.8-Clean-architecure`.  
**Product / assemblies:** **CleanApp** — a **.NET 4.8** layered backend sketch centered on **users, RBAC, Twilio OTP auth, and Stripe**, with **EF Core + MySQL** persistence. Use **Domain** for the vocabulary of the system, **Application** for orchestration, **Infrastructure** for persistence and integrations, and plan the **API** layer as the HTTP boundary that should sit on top of these libraries.
