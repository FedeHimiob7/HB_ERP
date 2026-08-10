# CLAUDE.md
te vas a comunicar conmigo simpre en español
This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project src/API/WebAPI/WebAPI.csproj

# Start RabbitMQ (required for messaging)
docker-compose up -d
```

Swagger UI is available at `/swagger` in Development mode. CORS is configured for `http://localhost:4200` by default (change `origenesPermitidos` in `appsettings.json`).

## Database Migrations

Each module has its own `DbContext` and migration history. Always specify both `--project` and `--startup-project`:

```bash
# Identity module
dotnet ef migrations add <Name> --project src/Modules/Identity/Identity.Infrastructure --startup-project src/API/WebAPI
dotnet ef database update        --project src/Modules/Identity/Identity.Infrastructure --startup-project src/API/WebAPI

# MasterData module
dotnet ef migrations add <Name> --project src/Modules/MasterData/MasterData.Infrastructure --startup-project src/API/WebAPI
dotnet ef database update        --project src/Modules/MasterData/MasterData.Infrastructure --startup-project src/API/WebAPI

# Inventory module
dotnet ef migrations add <Name> --project src/Modules/Inventory/Inventory.Infrastructure --startup-project src/API/WebAPI
dotnet ef database update        --project src/Modules/Inventory/Inventory.Infrastructure --startup-project src/API/WebAPI
```

## Architecture

This is a modular monolith following **DDD + Clean Architecture + CQRS**. There are three modules (`Identity`, `MasterData`, `Inventory`) plus a shared kernel.

### Layer structure (same in every module)
```
*.Domain          → Entities, Value Objects, Domain Events (no external dependencies)
*.Application     → CQRS handlers (MediatR), FluentValidation validators, ErrorOr results
*.Infrastructure  → EF Core DbContext, repositories, EF interceptors, outbox publisher
```

### SharedKernel (`HB_ERP.SharedKernel/`)
All domain entities extend `AggregateRoot<TId>`. Key primitives:
- `AggregateRoot<TId>` — base class; call `Raise(domainEvent)` to queue domain events
- `DomainEvent` — base record for domain events
- `IAuditable` — shadow properties `CreatedAt`/`UpdatedAt` auto-set by `UpdateAuditableEntitiesInterceptor`
- `OutboxMessage` — used by both modules for reliable event publishing
- `PublishDomainEventsInterceptor` — fires domain events via MediatR on `SaveChangesAsync`
- Integration events live in `HB_ERP.SharedKernel/IntegrationEvents/` and are shared across modules
- `ICurrencyConverter` / `CurrencyConverter` — injectable, stateless, registered as Singleton. Converts VES↔USD↔EUR. Used **only in Query Handlers**, never in domain entities. `ToUSD(amount, currencyCode, exchangeRate)` / `FromUSD(amountUSD, currencyCode, exchangeRate)`.
- `IFiscalClock` / `FiscalClock` — injectable, registered as Singleton. Expone `UtcNow`, `VenezuelaNow`, `VenezuelaToday` (offset fijo UTC-4, sin DST). Se usa para que toda fecha fiscal (`ExchangeRate.RegisterDate`, horarios de `BCVRateSyncWorker`, y en el futuro cualquier documento fiscal) refleje el calendario de Venezuela y no la hora del servidor. Los command handlers inyectan `IFiscalClock` y pasan `_fiscalClock.VenezuelaNow` a la entidad — la entidad nunca llama `DateTime.UtcNow` internamente, recibe la fecha como parámetro del caller.
- `IEffectiveDated` (`HB_ERP.SharedKernel/Domain/Primitives/`) — interfaz con una sola propiedad `DateTime EffectiveFrom`. Generaliza el patrón inmutable que ya usaba `ExchangeRate` (un registro nuevo por cada cambio, nunca se edita uno existente) para cualquier valor fiscal versionado por fecha. `GetEffectiveAsOfAsync<T>(this IQueryable<T>, DateOnly asOfDate, CancellationToken)` (`HB_ERP.SharedKernel/Infrastructure/Extensions/EffectiveDatedQueryExtensions.cs`) resuelve el registro vigente a una fecha dada (`EffectiveFrom < asOfDate + 1 día`, ordenado descendente, primero). Primer consumidor: `FiscalTaxRate` (ver tabla de entidades de MasterData). Reusar para futuros parámetros versionados (Unidad Tributaria, tramos IGTF) en vez de reinventar el patrón `Current/Previous`.

### Cross-module communication
Modules do **not** reference each other directly. Communication is async:
1. A domain event handler serializes an integration event into the `OutboxMessage` table (same transaction as the aggregate save).
2. The background service (`MasterDataOutboxPublisher` / `IdentityOutboxPublisher`) polls the outbox and publishes to RabbitMQ via MassTransit.
3. The consuming module has a MassTransit consumer that handles the integration event.

### CQRS conventions
- Commands/queries are records implementing `IRequest<ErrorOr<T>>`.
- Handlers return `ErrorOr<T>` — never throw for business errors.
- `ValidationBehavior<TRequest, TResponse>` (MediatR pipeline) runs FluentValidation before every handler.
- Controllers map `ErrorOr` results to HTTP responses using the `MatchFirst` / `Problem` pattern.

### Paginación — patrón establecido
Todas las entidades de MasterData e Inventory siguen el mismo patrón de paginación:

**Rutas del controller:**
- `GET /api/{entity}` → `GetAll` — devuelve la lista completa sin parámetros
- `GET /api/{entity}/paged` → `GetPaged` — paginado con filtro

**Capas involucradas:**
1. **APIModels** (`src/API/WebAPI/APIModels/{Module}/{Entity}/Get{Entity}PagedRequest.cs`) — record con `PageNumber`, `PageSize`, `SearchTerm` (y filtros extra si los hay). Se usa como `[FromQuery]` en el controller.
2. **Domain** (`{Module}.Domain/SearchParametersModel/{Entity}Filter.cs`) — record que hereda de `PaginationFilter` (SharedKernel). Para filtros simples (solo `SearchTerm`) basta con la herencia directa. Para filtros extra (ej. `CountryId`, `IsActive`) se agregan propiedades adicionales.
3. **Application** — dos queries por entidad:
   - `GetAll{Entity}Query` / `GetAll{Entity}QueryHandler` — llama `GetAllAsync`, devuelve `IReadOnlyList<{Entity}Response>`
   - `Get{Entity}PagedQuery(Filter)` / handler — llama `GetPagedAsync(filter)`, devuelve `Paged{Entity}Result`
4. **Domain repository** — `GetPagedAsync({Entity}Filter filter, CancellationToken)` 
5. **Infrastructure repository** — aplica filtros dinámicos antes de `CountAsync`:
   - `SearchTerm` → `.Where(e => e.Name.ToLower().Contains(term) || ...)` sobre los campos relevantes
   - Para buscar por nombre de entidad relacionada sin navigation property: `_dbContext.OtherSet.Any(o => o.Id == e.ForeignKeyId && o.Name.ToLower().Contains(term))`

**Entidades MasterData con paginación implementada:**
| Entidad | SearchTerm busca en | Filtros extra |
|---------|-------------------|---------------|
| `Country` | `Name` | — |
| `State` | `Name`, `Code`, nombre del `Country` | `CountryId`, `IsActive` |
| `City` | `Name`, nombre del `State` | `StateId` |
| `Currency` | `Code`, `Name`, `Symbol` | — |
| `ProductServiceLine` | `Name`, `Description` | — |
| `Unit` | `Name`, `Description` | — |
| `Tax` | `Name` | `TaxType` |
| `Branch` | `Name`, `Address` | — |
| `FiscalTerminal` | `Name` | `BranchId` |

**Nota:** `Company` no tiene paginación — es fila única por instalación (`CompanyId.Singleton`), se consulta vía `GET /api/companies/current`, no vía `GetAll`/`GetPaged`. `FiscalTaxRate` tampoco tiene paginación propia — es hijo de `Tax`, se consulta vía `GET /api/taxes/{id}/effective?date=` (resuelve la tasa vigente a una fecha).

**Entidades Inventory con paginación implementada:**
| Entidad | SearchTerm busca en | Filtros extra |
|---------|-------------------|---------------|
| `ProductType` | `Name` | — |
| `ProductCategory` | `Name` | — |
| `ProductSubCategory` | `Name` | `ProductCategoryId` |
| `ProductBrand` | `Name` | — |
| `Warehouse` | `Name` | — |
| `StorageType` | `Name` | — |
| `Product` | `Name`, `Code`, `Barcode` | `ProductTypeId`, `ProductCategoryId`, `ProductSubCategoryId`, `ProductBrandId`, `ProductServiceLineId` |

**Nota:** `State` y `City` usan filtros extra con FK + `IsActive`; el resto solo tienen `SearchTerm` heredado de `PaginationFilter`. `ExchangeRate` tiene paginado propio (sin SearchTerm, ordenado por `RegisterDate` desc).

### Response models — qué propiedades exponer al UI

**Regla:** `IsActive` **nunca se incluye** en los response models (`*Response.cs`). Es una propiedad interna de dominio que el UI no consume y no debe conocer.

Propiedades que sí se exponen: `Id`, campos de negocio (nombre, código, descripción, etc.), y en entidades con enum tipo, también el valor entero del enum y su nombre legible (`TaxType` + `TaxTypeName`).

### Adding a new entity to an existing module
1. Create the aggregate in `*.Domain` extending `AggregateRoot<TId>`.
2. Add commands/queries + handlers in `*.Application`; add a FluentValidation validator in the same folder.
3. Add the DbSet and Fluent API configuration in `*.Infrastructure/Persistence`.
4. Run `dotnet ef migrations add` for the relevant module (see above).
5. Register any new services in the module's `DependencyInjection` extension method.
6. Para paginación: seguir el patrón descrito en la sección **Paginación** arriba.
7. Para el response model: **no incluir `IsActive`** — ver sección "Response models" arriba.

### Solution layout
```
src/
  API/WebAPI/                         ← Entry point; Program.cs wires all modules
  Modules/
    Identity/
      Identity.Domain/                ← User, Role, SystemAction entities + VOs + domain events
      Identity.Application/           ← CQRS handlers, validators, login, JWT interface
      Identity.Infrastructure/        ← IdentityDbContext, repositories, JwtTokenService, Pbkdf2PasswordHasher
      Identity.Integration/           ← MassTransit consumers for incoming events
      Identity.Shared/                ← DTOs shared with API
    MasterData/
      MasterData.Domain/              ← Currency, PSL, Country, State, City, Unit, Tax, ExchangeRate
      MasterData.Application/         ← CQRS handlers y validators por entidad; IBCVRateScrapingService
      MasterData.Infrastructure/      ← MasterDataDbContext, repositories, migrations
                                         BCVRateScrapingService, BCVRateSyncWorker (BackgroundService)
    Inventory/
      Inventory.Domain/               ← ProductType, ProductCategory, ProductSubCategory, ProductBrand,
                                         Warehouse, StorageType, Product, ProductCodeCounter, ProductPriceHistory
      Inventory.Application/          ← CQRS handlers y validators por entidad; IInventoryUnitOfWork
      Inventory.Infrastructure/       ← InventoryDbContext, repositories, migrations
HB_ERP.SharedKernel/                  ← DDD primitives, interceptors, integration events, ICurrencyConverter
tests/
```

### Identity module — entities
| Entity | Description |
|--------|-------------|
| `User` | Usuario del sistema; tiene roles, password hash, estado activo |
| `Role` | Rol asignable a usuarios; agrupa SystemActions |
| `SystemAction` | Permiso granular (ej. `"products.create"`); se asigna a roles |

El JWT generado incluye claims: `sub` (userId), `email`, `unique_name` (firstName), `roles[]`, `permissions[]`.

### MasterData module — entities
| Entity | Description |
|--------|-------------|
| `Currency` | Moneda (código ISO, símbolo, nombre) |
| `ProductServiceLine` | Línea de producto/servicio |
| `Country` | País; tiene estados/provincias |
| `State` | Estado/provincia; siempre vinculado a un `Country` |
| `City` | Ciudad; siempre vinculada a un `State` |
| `Unit` | Unidad de medida |
| `Tax` | Identidad/catálogo del impuesto: nombre, `TaxType` (enum: `IVA=1`, `IGTF=2`, `ISLR=3`). **Ya no tiene `Rate`** — la alícuota vive en `FiscalTaxRate`. Agregar nuevos tipos al enum. |
| `FiscalTaxRate` | Valor versionado de `Tax`: `TaxId` (FK), `Rate`, `EffectiveFrom`. Implementa `IEffectiveDated`. Inmutable y **sin `IsActive`** — nunca se oculta una fila puntual, solo se acumulan versiones; el motor fiscal resuelve la tasa vigente a una fecha con `GetEffectiveAsOfAsync`. Convención de nombre `Fiscal*` para la mitad versionada de cualquier futuro split identidad/valor (ver `IEffectiveDated` en SharedKernel). |
| `ExchangeRate` | Tasa de cambio Bs/USD. `Source` enum: `BCV=1`, `Manual=2`. Un registro por cambio de valor. |
| `Company` | Perfil fiscal de la instalación — **fila única** (`CompanyId.Singleton`, no multi-tenant: una instalación = una empresa/RIF). `Rif` (validado con regex `^[VEJPG]-\d{8,9}-\d$`), `LegalName`, `RegisteredAddress`, `TaxpayerType` (enum: `Ordinario=1`, `Formal=2`, `Especial=3`). |
| `Branch` | Sucursal física de la `Company`; N por `Company`. `CompanyId` (FK), `Name`, `Address`, `SequenceNumber` (correlativo para completar `Product.Code` cuando se implemente el segmento de sucursal — ver "Premisa de migración" abajo). |
| `FiscalTerminal` | Punto de emisión (caja/máquina fiscal/canal digital) dentro de una `Branch`; N por `Branch`. `BranchId` (FK), `Name`, `EmissionMethod` (enum: `MaquinaFiscal=1`, `FormaLibre=2`, `Digital=3`). Necesita secuencia propia de número de control fiscal (mismo patrón UPDLOCK que `ProductCodeCounter`) — pendiente de implementar junto con `FiscalDocument`. |

Todos los aggregates de MasterData implementan activación/desactivación (`IsActive`), excepto `ExchangeRate`, `FiscalTaxRate` (inmutables — cada cambio genera un registro nuevo) y `Company` (fila única, no se desactiva).

### Inventory module — entities
| Entity | Description |
|--------|-------------|
| `ProductType` | Tipo de producto (ej. Bien, Servicio). Catálogo con activación/desactivación. |
| `ProductCategory` | Categoría del producto. Catálogo con activación/desactivación. |
| `ProductSubCategory` | Sub-categoría; siempre vinculada a una `ProductCategory`. |
| `ProductBrand` | Marca del producto. Catálogo con activación/desactivación. |
| `Warehouse` | Almacén físico. Catálogo con activación/desactivación. |
| `StorageType` | Tipo de almacenamiento (ej. Estantería, Refrigerado). Catálogo con activación/desactivación. |
| `Product` | Entidad central del sistema — ver diseño detallado abajo. |
| `ProductCodeCounter` | Tabla de contadores por PSL + fecha para generar `Product.Code` sin colisiones (usa UPDLOCK). |
| `ProductPriceHistory` | Child entity de `Product`; registra cada cambio de precios/costos con snapshot completo. |

Todos los aggregates de Inventory implementan activación/desactivación (`IsActive`), excepto `ProductCodeCounter` y `ProductPriceHistory` (son registros inmutables).

### ExchangeRate — comportamiento especial
- **`GET /api/exchangerates/current`**: va al BCV en ese momento, guarda en BD si la tasa cambió, devuelve la tasa fresca. Si BCV no responde, devuelve la última tasa guardada (fallback). **El UI siempre usa este endpoint** al abrir cualquier formulario con tasa de cambio.
- **`GET /api/exchangerates/by-date?date=YYYY-MM-DD`**: devuelve la última tasa registrada **en o antes** de esa fecha. No falla si ese día no hubo sync. Usar cuando el usuario cambia la fecha de una transacción.
- **`BCVRateSyncWorker`** (BackgroundService): despierta a las **12:00 y 18:00** hora local para sincronizar con BCV, garantizando que todos los días haya al menos un registro aunque nadie use el sistema.
- **`POST /api/exchangerates/sync-bcv`**: fuerza sync manual (uso administrativo/emergencias).
- `IBCVRateScrapingService` vive en `MasterData.Application/Interfaces/`; implementación con HtmlAgilityPack en `MasterData.Infrastructure/Services/`.

## Domain events — limitación importante en Identity

El `PublishDomainEventsInterceptor` está registrado en `IdentityDbContext` pero **nunca dispara eventos** en la práctica. Motivo: el interceptor busca `IHasDomainEvents` en las entidades rastreadas por EF (`RoleEntity`, `UserEntity`, etc.), que son POCOs simples sin esa interfaz. Los domain events viven en los objetos de dominio (`Role`, `User`) que no son trackeados directamente por EF (se usa el patrón mapper).

**Patrón correcto en Identity:** el cleanup de relaciones se hace **directamente en el command handler**, no vía domain event handlers. Ejemplos establecidos:
- `DeleteSystemActionCommandHandler` — al desactivar una `SystemAction`, busca los roles que la tienen y les revoca la acción en el mismo handler.
- `DeleteRoleCommandHandler` — al desactivar un `Role`, busca los usuarios que lo tienen y les elimina el rol en el mismo handler.

No crear domain event handlers en Identity para lógica de cleanup — serán código muerto.

---

## Plan de migración y módulos futuros

> **⚠️ REDIRECCIONADO.** El rumbo activo del proyecto está en `FISCAL_ROADMAP.md` (raíz del repo): el objetivo ya no es reconstruir los 12 módulos completos del legacy, sino un **Sistema de Facturación Homologado** conforme a la Providencia SENIAT 2024/000121 (ver `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado.pdf`). El contenido de esta sección y de `MIGRATION_PLAN.md` se conserva como **análisis histórico del legacy** (los modelos, vicios y equivalencias siguen siendo válidos), pero el orden de implementación, los módulos vigentes y las decisiones de diseño activas están en `FISCAL_ROADMAP.md`. En particular, la decisión de `Customer`/`Supplier` separados de abajo **fue revertida** — ver D-2 en `FISCAL_ROADMAP.md` (ahora `ThirdParty` unificado).

El archivo `MIGRATION_PLAN.md` en la raíz del proyecto documenta la arquitectura completa del sistema legacy. Decisiones que se tomaron en su momento (ver nota de redirección arriba antes de asumir que siguen vigentes):

### Módulos planificados y dónde viven las entidades clave (histórico — ver nota de redirección arriba)
| Entidad | Módulo | Razón |
|---------|--------|-------|
| `Customer` | `Sales` | Cliente es un concepto de ventas |
| `Supplier` | `Procurement` | Proveedor es un concepto de compras |
| `Tax` | `MasterData` | Catálogo compartido; se usa en Sales, Procurement, Inventory |
| `ExchangeRate` | `MasterData` | Catálogo compartido; lo consumen todos los módulos transaccionales |
| `SaleOrder`, POS | `Sales` | Todo el flujo de venta |
| `PurchaseOrder` | `Procurement` | Todo el flujo de compra |

**No existe** un módulo `CustomersAndSuppliers` — esa decisión fue descartada *(histórico: ahora revertida, ver `FISCAL_ROADMAP.md` D-2 — `ThirdParty` unificado con `Customer`/`Supplier` como roles)*.

### Módulo Inventory — estado actual
Inventory es **netamente de inventario**.

#### ✅ Implementado (completo: Domain + Application + Infrastructure + Controller)
| Entidad | CRUD | Paginado | Notas |
|---------|------|----------|-------|
| `ProductType` | ✅ | ✅ | Catálogo simple |
| `ProductCategory` | ✅ | ✅ | Catálogo simple |
| `ProductSubCategory` | ✅ | ✅ | Filtro extra: `ProductCategoryId` |
| `ProductBrand` | ✅ | ✅ | Catálogo simple |
| `Warehouse` | ✅ | ✅ | Catálogo simple |
| `StorageType` | ✅ | ✅ | Catálogo simple |
| `Product` | ✅ | ✅ | Entidad central; ver diseño detallado abajo |
| `ProductCodeCounter` | — | — | Solo se accede vía `GenerateProductCodeCommand` |
| `ProductPriceHistory` | — | — | Child entity de Product; se crea automáticamente al actualizar precios |

#### ⏳ Pendiente de implementar
- Stock por almacén (`ProductStock` / `InventoryMovement`) — un producto puede estar en múltiples almacenes
- Movimientos internos (entradas, salidas, transferencias)
- `Equipment` — aggregate separado que referenciará `ProductId`

**No incluye** (pertenecen a otros módulos):
- `Customer` ni `Supplier` — Inventory no los referencia directamente
- Compras, ventas, devoluciones, facturación — responsabilidad de `Procurement` y `Sales`
- La integración con órdenes de compra/venta vendrá vía eventos cuando esos módulos existan

### Product — diseño del aggregate (Inventory)
Product es la entidad más transversal del sistema (ventas, compras, inventario, mantenimiento, contratos). **Un único `CreateProductCommand` es el responsable de crear productos** — ningún otro módulo instancia `Product` directamente. Si otro módulo necesita crear un producto, llama al endpoint REST o publica un integration event que Inventory consume.

#### Estructura de campos confirmada
```
IDENTIDAD
  ProductId              VO (Guid)
  Code                   string, requerido — auto-generado: {Year}{Month}{Day}-{PslNumber}-{DailyCounter}
                         editable por el usuario. Se genera vía GenerateProductCodeCommand ANTES de crear el producto.
                         El UI llama a POST /api/products/generate-code?pslId=X, muestra el código prefilled,
                         el usuario puede editarlo, y lo envía junto con CreateProductCommand.
  ItemNumberByDay        int — correlativo diario por PSL. Se persiste junto con Code para referencia histórica.
  Barcode                string?
  ClientCode             string?  (código que asigna el proveedor a este producto)

BÁSICO
  Name                   string, requerido
  Description            string?
  Model                  string?

CLASIFICACIÓN (IDs únicamente, sin navigation properties)
  ProductServiceLineId   requerido
  ProductTypeId?
  ProductCategoryId?
  ProductSubCategoryId?
  ProductBrandId?

FLAGS
  IsSalable              bool
  IsPurchasable          bool
  IsStored               bool  (tiene stock físico en almacén)

COSTO (puede estar en cualquier moneda — depende del proveedor)
  Cost                   decimal        ← costo final, con impuestos ya aplicados (lo que se persiste como "Cost")
  CostBase               decimal?       ← monto base sin impuestos (el que el usuario ingresa/edita)
  CostCurrencyId         CurrencyId
  CostExchangeRate       decimal  (tasa BCV vigente cuando se seteó)

PRECIO PRINCIPAL
  Price                  decimal        ← Price1, precio final de venta (con impuestos aplicados)
  PriceBase              decimal?       ← monto base de Price1 sin impuestos
  PriceCurrencyId        CurrencyId     ← moneda de TODOS los precios (Price1..5)
  PriceExchangeRate      decimal        ← tasa de TODOS los precios (Price1..5)

PRECIOS ADICIONALES (heredan PriceCurrencyId y PriceExchangeRate del precio principal)
  Price2                 decimal?
  Price3                 decimal?
  Price4                 decimal?
  Price5                 decimal?

IMPUESTOS (many-to-many via IDs — reemplazan los strings del legado)
  _purchaseTaxIds        List<TaxId>
  _saleTaxIds            List<TaxId>

UNIDADES
  PurchaseUnitId         UnitId?
  SaleUnitId             UnitId?
  UnitConversionFactor   decimal?  (cuántas unidades de compra = 1 unidad de venta)

FÍSICO / EMPAQUE
  Weight                 decimal?
  Volume                 decimal?
  ContentCapacity        decimal?  (capacidad del envase/contenedor)

COMPRA (info referencial para Procurement) — DISEÑADO, AÚN NO IMPLEMENTADO en el aggregate
  PurchaseDescription    string?
  DaysToDeliver          int?
  ImportationCost        decimal?

MISC
  Tags                   string?   (campo libre, sin lógica backend)
  ImageUrl               string?
  ProfitMargin           decimal?

HISTORIAL
  _priceHistory          List<ProductPriceHistory>  (child entity)

  IsActive               bool
```

#### ProductPriceHistory (child entity)
Registra cada cambio de precios. Reemplaza el patrón `New/Current/Previous` del legado.
```
  ProductId, ChangedAt, ChangedByUserId
  OldCost, OldCostBase, OldCostCurrencyId, OldCostExchangeRate
  NewCost, NewCostBase, NewCostCurrencyId, NewCostExchangeRate
  OldPrice, OldPriceBase, OldPriceCurrencyId, OldPriceExchangeRate
  NewPrice, NewPriceBase, NewPriceCurrencyId, NewPriceExchangeRate
  OldPrice2..5, NewPrice2..5
  OldPurchaseTaxRate, NewPurchaseTaxRate   (suma de tasas de _purchaseTaxIds antes/después)
  OldSaleTaxRate, NewSaleTaxRate           (suma de tasas de _saleTaxIds antes/después)
  OldProfitMargin, NewProfitMargin
```

#### Reglas de negocio clave
- `Code` se genera mediante `GenerateProductCodeCommand` (endpoint dedicado) **antes** de abrir el formulario de creación. Usa la tabla `ProductCodeCounters` con UPDLOCK para garantizar que dos usuarios simultáneos reciban códigos distintos. El código se envía en `CreateProductCommand` — el handler no lo genera, lo valida y persiste. Sin patrón draft — si el usuario abandona el formulario, el número queda saltado (gap aceptable, igual que un IDENTITY de SQL).
- Cuando se actualiza `PriceCurrencyId` o `PriceExchangeRate`, Price2-5 quedan bajo la nueva moneda/tasa automáticamente (comparten el mismo bloque).
- Precios de venta (Price..5) estandarizados en USD. Costo puede ser en cualquier moneda.
- Conversiones de moneda **solo en Query Handlers** vía `ICurrencyConverter`. Nunca en la entidad.
- `[NotMapped]` computados del legado van a DTOs en Application, nunca al dominio.
- `Cost`/`Price` (final, con impuestos) vs `CostBase`/`PriceBase` (monto neto sin impuestos): `CalculatePricesQuery` calcula ambos — `*Base` es el monto antes de impuestos, `Cost`/`Price1` es el resultado ya con impuestos aplicados. El IGTF se aplica de forma compuesta, sobre el monto que ya incluye los impuestos regulares (no IGTF), no sobre el monto base.
- `UpdateProductPricesCommand` / `Product.UpdatePrices` actualiza en un solo paso: costo, precios (Price..5, base y final), `PurchaseTaxIds`/`SaleTaxIds` (vía `SetTaxes`) y `ProfitMargin`. El handler resuelve las tasas de impuesto antes/después (`ITaxRepository.GetAllAsync` + suma de `Rate` por los IDs) para dejar registro en `ProductPriceHistory`.

#### Propiedades diferidas a otros módulos
| Propiedad legado | Módulo destino |
|---|---|
| `WareHouseId`, `StorageTypeId`, `Minimum`, `Maximum` | Inventory stock (un producto puede estar en múltiples almacenes) |
| `PurchaseResponsableId`, `DefaultBuyResponsableId` | Procurement |
| `SerialNumber` | Equipment (serial es por unidad física) |
| `SalesCommission` | Sales |
| `OptionalProductId` | Sales |
| `IsVisibleOnPOS`, `POSPrinterId` | POS |
| `IsOperationManufacture`, `IsAssemblyProduct` | Manufacturing |
| `BudgetAccount`, `IncomeAccount`, `ExpenseAccount` | Accounting |
| `ProductCargoEquipmentId` | Equipment/Logistics |

### Estrategia de desarrollo incremental
No se completa un módulo al 100% antes de pasar al siguiente. Se implementan las responsabilidades core del módulo, se avanza al siguiente, y se vuelve a agregar integraciones cuando los módulos que necesitan interactuar ya existen. Esto evita over-engineering y permite avanzar.

---

## Premisa de migración de datos (legacy → nuevo sistema)

**El objetivo final es migrar la información del sistema legacy a este sistema nuevo mediante scripts SQL/queries.** Esta premisa es un factor de segundo plano que debe considerarse en cada decisión de diseño, sin que sea un bloqueante para mejorar o cambiar lo necesario.

### Principio general
- Si hay que mejorar, cambiar o eliminar algo respecto al legacy, se hace — la migración se adapta.
- Pero siempre evaluar: *¿cuánto trabajo de migración genera este cambio?* Un cambio que rompe 10 tablas necesita más justificación que uno que afecta 1.
- Los scripts de migración se escriben al final, cuando los módulos estén estables, no durante el desarrollo.

### Implicaciones concretas ya identificadas

#### Código de producto (`Product.Code`)
El legacy generaba el código como: `{Year}{Month}{Day}-{PSLId_int}-{OfficeBranchId_int}-{lastItemNumber}`
- `PSLId_int` era el `int` identity del PSL en el legacy.
- En el nuevo sistema los PSLs tienen `Guid`. El número entero del PSL legacy **debe preservarse** en `ProductCodeCounter.PslSequenceNumber` durante la migración, para que los códigos de productos migrados y los nuevos sean coherentes.
- Durante la migración se pre-populará `ProductCodeCounters` con: `PslId` (GUID nuevo) + `PslSequenceNumber` (int del legacy) + el MAX de `DailyCounter` por fecha.
- Los PSLs nuevos (creados después de la migración) reciben el siguiente `PslSequenceNumber` disponible (MAX + 1).

#### PSL (ProductServiceLine)
Los PSLs ya existen en el nuevo sistema. La migración de productos debe usar la correspondencia `legacy_psl_int_id → new_psl_guid`.

#### OfficeBranch
Entidad pendiente de implementar. Cuando exista, `ProductCodeCounter` agrega `OfficeBranchSequenceNumber` y el formato del código queda completo: `{Year}{Month}{Day}-{PslNumber}-{OfficeBranchNumber}-{DailyCounter}`. Por ahora el segmento de sucursal se omite del código.

---

## Testing

Estándar de estructura para proyectos de test .NET en este repo:

**SharedKernel — caso especial (no sigue el patrón de módulos):** `HB_ERP.SharedKernel.Tests` es un `.csproj`
**separado**, anidado físicamente dentro de `HB_ERP.SharedKernel/`:
```
HB_ERP.SharedKernel/
  HB_ERP.SharedKernel.csproj
  HB_ERP.SharedKernel.Tests/
    HB_ERP.SharedKernel.Tests.csproj
```
En el `.sln`, ambos quedan agrupados bajo una carpeta de solución "SharedKernel" (creada en Visual Studio,
no confundir con anidado físico).

**Cada módulo** (`MasterData`, `Inventory`, `Identity`, futuros): una carpeta `Tests/` dentro del módulo, con
**tres proyectos de nombre corto** (sin prefijo del módulo, ya implícito por la ruta):
```
src/Modules/{Modulo}/
  {Modulo}.Domain/
  {Modulo}.Application/
  {Modulo}.Infrastructure/
  Tests/
    Domain.Tests/
    Application.Tests/
    Infrastructure.Tests/
```
**Estado actual:** `MasterData` e `Inventory` ya tienen `Domain.Tests` (cubriendo `Tax`, `ExchangeRate`,
`Product`, `ProductCodeCounter`). `Identity` ya tiene `Domain.Tests` (cubriendo `EventLog` únicamente —
`User`/`Role`/`SystemAction` todavía no tienen tests propios). `Inventory` ya tiene el primer
`Application.Tests` del repo (cubriendo `GenerateProductCodeCommandHandler`, con `NSubstitute`) — el resto
de los módulos todavía no tiene `Application.Tests`/`Infrastructure.Tests`.

**Convenciones de código:**
- Framework: **xUnit** puro (sin MSTest/NUnit). Las clases de test no llevan atributo de clase (`[TestClass]`
  no existe en xUnit); los métodos usan `[Fact]` (caso simple) o `[Theory]` + `[InlineData]`/`[MemberData]`/
  `[ClassData]` (parametrizado).
- Toda clase de test debe declararse `sealed` (nunca se hereda; comunica intención y evita herencias
  accidentales).
- Cada entidad y cada método nuevo debe llevar su prueba unitaria en el proyecto de test de la capa
  correspondiente — expectativa permanente del flujo de trabajo, no algo puntual.
- **Todo test debe llevar comentarios explícitos** explicando qué hace cada bloque y por qué (qué se está
  mockeando, qué guard/rama del código se está activando, por qué se espera tal resultado). No hace falta
  que sean extensos ni repitan lo obvio línea por línea, pero sí que una persona sin experiencia en testing
  pueda seguir el archivo y entender la intención de cada `Arrange`/mock/assert. Motivo: quien mantiene este
  repo está aprendiendo testing/mocking recién ahora — el código de test es, en sí mismo, la documentación
  de cómo funciona el mecanismo.
- **Mocking**: `NSubstitute` es la librería elegida para los tests de `Application`/`Infrastructure` que
  necesitan simular dependencias (repositorios, `ICurrentUserProvider`, etc.) — no hay otra en el repo, no
  mezclar con Moq/FakeItEasy. Patrón básico: `Substitute.For<IInterfaz>()` crea el doble; `.Returns(...)`
  define qué devuelve; `Arg.Any<T>()` acepta cualquier argumento, `Arg.Is<T>(predicado)` exige que cumpla
  una condición puntual; `.Received(n)` verifica que se haya llamado tal método tal cantidad de veces (en
  vez de verificar un valor devuelto, verifica que pasó una interacción concreta).
- Al anidar un proyecto de test dentro de la carpeta del proyecto que testea (caso SharedKernel), hay que
  excluir esa subcarpeta del globbing del `.csproj` padre (`<Compile Remove="HB_ERP.SharedKernel.Tests\**" />`
  + `EmbeddedResource`/`None` iguales) — si no, el SDK de .NET intenta compilar los archivos del proyecto de
  test anidado dentro del ensamblado principal y falla con errores de atributos duplicados. No aplica al
  patrón de módulos (`Tests/` es sibling de `{Modulo}.Domain/`, no anidado dentro).

## Key packages
| Package | Purpose |
|---------|---------|
| MediatR 14 | CQRS dispatcher |
| ErrorOr 2 | Result type — use instead of exceptions for business errors |
| FluentValidation 12 | Command/query validation via MediatR pipeline |
| MassTransit 9 + RabbitMQ | Async integration events |
| EF Core 9 | ORM; three separate DbContexts (`IdentityDbContext`, `MasterDataDbContext`, `InventoryDbContext`) |
| Ardalis.GuardClauses | Input guards in domain constructors |
| RT.Comb | Sequential GUIDs for PKs |
| Serilog | Structured logging; writes to SQL Server (`LogErrorHB_ERP` DB) |
| xUnit 2.9 | Unit testing framework (ver sección Testing) |
