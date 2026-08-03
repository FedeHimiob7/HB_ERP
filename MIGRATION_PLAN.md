# Migration Plan — LAExportGroup ERP

> **⚠️ HISTÓRICO / SUPERSEDED.** Este documento describe el plan original de reconstruir los 12 módulos completos del legacy. El proyecto fue redireccionado hacia un **Sistema de Facturación Homologado** (Providencia SENIAT 2024/000121) — el roadmap activo, las prioridades y las decisiones de diseño vigentes están en `FISCAL_ROADMAP.md` (raíz del repo). El análisis del legacy que sigue abajo (arquitectura, vicios detectados, entidades y equivalencias por módulo) **sigue siendo válido como referencia** para entender el legacy y para la futura sincronización con el sistema administrativo actual, pero el orden de implementación y el destino final de cada módulo ya no son los de este documento — ver la tabla "Destino de los módulos del plan anterior" en `FISCAL_ROADMAP.md`.

## Resumen ejecutivo

El sistema legacy es un **monolito por capas técnicas** (.NET Core 3.1) con cinco proyectos (`PublicInterface`, `Service`, `Data`, `DB.Core`, `Util`). Todo el dominio comparte **un único `ApplicationDbContext`** con ~110 entidades, una **única tabla de migraciones** y un esquema SQL Server (`lauser`). La organización por carpetas (`SAC/`, `Inventory/`, `InternalPOS/`, `HumanResources/`, `A2Sync/`, `PurchaseOrderIntegrated/`) ya insinúa los **bounded contexts** reales, pero no hay aislamiento: las entidades se referencian libremente entre dominios por FK directa (p. ej. `InventoryTransacction` apunta a `Clients`, `Providers`, `Employee`, `SaleOrder`, `CashAdvance`, `ProductServiceLine` y a sí misma).

Los principales vicios detectados:

- **Entidades anémicas + "fat data classes"**: las entidades son DTOs con `[Key]`, `[ForeignKey]`, decenas de `[NotMapped]` con getters que navegan propiedades (`CustomerName`, `EmployeeFullName`, `TotalCostCurrencyAmount`, `CurrentUnitPrice`...). No hay invariantes, ni constructores, ni métodos de negocio.
- **Lógica de negocio dispersa**: parte en `*Service`, parte en `*CoordinatorService` (orquestadores que tocan 8-10 servicios distintos, ej. `IncomesCoordinatorService`), y parte en controllers (asignación de auditoría `CreatedBy/DateCreated`, parseo de claims, mapeo condicional de PSL).
- **Patrón Repository genérico + UnitOfWork monolítico**: `IUnitOfWork` expone TODOS los repos; cualquier servicio puede tocar cualquier tabla, rompiendo límites de contexto.
- **Auditoría y soft-delete copiados en cada clase**: `DateCreated/CreatedBy/DateModified/ModifiedBy` + `IsActive` (`IModelBase`) repetidos ~110 veces, manejados a mano.
- **Multi-tenancy frágil**: el `office-branch-id` viaja por header y se lee de `HttpContext.Items` en middleware.
- **Conversión monetaria duplicada**: cálculo `valor / ExchangeRate` repetido como `[NotMapped]` en Product, InventoryTransacction, SaleOrder, etc.

**Estrategia de migración**: descomponer en un **Modular Monolith** donde cada bounded context es un módulo con sus tres proyectos (`*.Domain`, `*.Application`, `*.Infrastructure`), su **propio DbContext** y su **propia tabla de migraciones**. Los módulos **no se referencian entre sí**: las FK cross-context se reemplazan por **IDs sueltos + datos replicados vía eventos de integración** (MassTransit + RabbitMQ + Outbox). La lógica se reorganiza con **CQRS (MediatR)**, validación con **FluentValidation** y resultados con **ErrorOr**. Las entidades pasan a ser **Aggregates ricos** con Value Objects (Money, ExchangeRate, Address, ContactInfo) y Domain Events. Migración incremental por módulo siguiendo el patrón Strangler Fig, empezando por los contextos de catálogo (poco acoplados) y terminando por los transaccionales (muy acoplados).

---

## Módulos propuestos

### 1. IdentityAccess
**Responsabilidad:** Autenticación, autorización, usuarios, roles, permisos por formulario/acción, módulos de UI.
**Bounded Context:** Identity & Access Management. ASP.NET Identity (`User`, `Role` con PK `long`), `Form`, `Action`, `FormAction`, `RoleFormAction`, `Module`.
**Dependencias:** Ninguna entrante de negocio. Provee la identidad de usuario al resto vía claims/token; los demás módulos solo guardan `UserId` como valor.
**Prioridad de migración:** Alta (es transversal y todos dependen de él para auditoría/seguridad).

#### Domain
- Entities/Aggregates: `User`, `Role`, `Module`, `Form`, `ActionPermission`, `FormAction`, `RoleFormAction`
- Value Objects: `Email`, `FullName`, `PermissionKey`
- Domain Events: `UserRegisteredEvent`, `UserDeactivatedEvent`, `RolePermissionsChangedEvent`
- Errors: `UserErrors`, `RoleErrors`, `PermissionErrors`

#### Application
- Commands: `RegisterUserCommand`/`Handler`, `AssignRoleCommand`/`Handler`, `UpdateRolePermissionsCommand`/`Handler`, `ChangePasswordCommand`/`Handler`
- Queries: `AuthenticateUserQuery`/`Handler`, `GetUserPermissionsQuery`/`Handler`, `GetUserModulesQuery`/`Handler`, `ListRolesQuery`/`Handler`
- Validators: `RegisterUserCommandValidator`, `AssignRoleCommandValidator`
- Responses: `AuthTokenResponse`, `UserResponse`, `UserPermissionsResponse`

#### Infrastructure
- DbContext: `IdentityDbContext`
- Repositories: `IUserRepository`/`UserRepository`, `IRoleRepository`/`RoleRepository`
- Configurations: `UserConfiguration`, `RoleConfiguration`, `FormActionConfiguration`, `RoleFormActionConfiguration`
- Otros: `JwtTokenGenerator`, integración con `UserManager`/`SignInManager`

> **Nota legacy:** `AuthenticateController` y lógica de generación de JWT viven hoy parcialmente en el controller; deben moverse a `AuthenticateUserQueryHandler` + `JwtTokenGenerator`.

---

### 2. MasterData (Catalog)
**Responsabilidad:** Datos de referencia compartidos: países, estados, ciudades, monedas, unidades de medida, impuestos, líneas de servicio (PSL), parámetros del sistema, sucursales de empresa.
**Bounded Context:** Shared Reference Data. `Countries`, `States`, `Cities`, `Currencies`, `Units`, `TaxesCurrent`, `ProductServiceLine`, `SystemParameters`, `CompanyBranch`, `ExchangeRate`.
**Dependencias:** Ninguna. Es proveedor puro de referencia.
**Prioridad de migración:** Alta (todos los módulos consumen monedas, PSL y tipos de cambio).

#### Domain
- Entities/Aggregates: `Country`, `State`, `City`, `Currency`, `UnitOfMeasure`, `Tax`, `ProductServiceLine`, `SystemParameter`, `CompanyBranch`, `ExchangeRateEntry`
- Value Objects: `Money`, `ExchangeRate`, `CurrencyCode`
- Domain Events: `ExchangeRatePublishedEvent`, `ProductServiceLineCreatedEvent`
- Errors: `CurrencyErrors`, `ExchangeRateErrors`, `LocationErrors`

#### Application
- Commands: `CreateExchangeRateCommand`/`Handler`, `CreateProductServiceLineCommand`/`Handler`, `UpsertSystemParameterCommand`/`Handler`
- Queries: `GetCurrentExchangeRateQuery`/`Handler`, `ListCurrenciesQuery`/`Handler`, `ListLocationsQuery`/`Handler`, `ListProductServiceLinesQuery`/`Handler`, `GetSystemParameterQuery`/`Handler`
- Validators: `CreateExchangeRateCommandValidator`
- Responses: `CurrencyResponse`, `ExchangeRateResponse`, `LocationResponse`, `ProductServiceLineResponse`

#### Infrastructure
- DbContext: `MasterDataDbContext`
- Repositories: `IExchangeRateRepository`/`ExchangeRateRepository`, `IProductServiceLineRepository`/..., `ILocationRepository`/...
- Configurations: `CurrencyConfiguration`, `ExchangeRateConfiguration`, `LocationConfiguration`, `ProductServiceLineConfiguration`
- Integración: publica `ExchangeRatePublishedIntegrationEvent` (los módulos transaccionales cachean la tasa al momento de la operación).

> **Decisión:** `Money` y `ExchangeRate` se modelan como Value Objects en el SharedKernel o aquí, y se reutilizan; elimina la duplicación de cálculos `valor/ExchangeRate` que hoy están como `[NotMapped]` en Product, InventoryTransacction y SaleOrder.

---

### 3. Inventory
**Responsabilidad:** Catálogo de productos, stock por almacén, almacenes, tipos de almacenamiento, categorías/subcategorías/marcas/tipos, componentes (BOM), ubicaciones, imágenes, equipo de carga, y movimientos de inventario (transacciones de entrada/salida).
**Bounded Context:** Inventory & Warehousing. `Product`, `ProductStock`, `ProductWarehouse`, `ProductStorageType`, `ProductCategory`, `ProductSubCategory`, `ProductBrand`, `ProductType`, `ProductComponent`, `ProductLocation`, `ProductImage`, `ProductCargoEquipment`, `ProductPreOrder`, `InventoryTransacction`, `InvTransactionProductDetail`, `WareHouseByUser`.
**Dependencias:** MasterData (Currency, Unit, PSL), Sales (CustomerId por ID), Procurement (SupplierId por ID). Las transacciones de pago/cobro se desacoplan hacia Finance vía eventos.
**Prioridad de migración:** Media (núcleo grande; `Product` e `InventoryTransacction` son las entidades más sobrecargadas).

#### Domain
- Entities/Aggregates: `Product` (aggregate root), `Warehouse`, `StockItem`, `InventoryMovement` (separa el aggregate de movimiento del de producto), `ProductComponent` (BOM)
- Value Objects: `ProductCode`, `Barcode`, `PricingInfo` (precios de venta/compra + margen + comisión), `CostInfo`, `Dimensions` (peso/volumen/capacidad), `StockQuantity`, `Money`
- Domain Events: `ProductCreatedEvent`, `ProductPriceChangedEvent`, `StockAdjustedEvent`, `InventoryMovementPostedEvent`, `StockBelowMinimumEvent`
- Errors: `ProductErrors`, `StockErrors`, `InventoryMovementErrors`

#### Application
- Commands: `CreateProductCommand`/`Handler`, `UpdateProductPricingCommand`/`Handler`, `PostInventoryMovementCommand`/`Handler`, `AdjustStockCommand`/`Handler`, `AssignWarehouseToUserCommand`/`Handler`
- Queries: `GetProductByIdQuery`/`Handler`, `ListProductsQuery`/`Handler`, `GetStockByWarehouseQuery`/`Handler`, `GetStockReportByDateQuery`/`Handler` (reemplaza `Sp_InventoryReportStockByDate`), `ListInventoryMovementsQuery`/`Handler`
- Validators: `CreateProductCommandValidator`, `PostInventoryMovementCommandValidator`
- Responses: `ProductResponse`, `StockResponse`, `InventoryMovementResponse`, `StockByDateReportResponse`

#### Infrastructure
- DbContext: `InventoryDbContext`
- Repositories: `IProductRepository`/`ProductRepository`, `IStockRepository`/`StockRepository`, `IInventoryMovementRepository`/...
- Configurations: `ProductConfiguration`, `StockItemConfiguration`, `WarehouseConfiguration`, `InventoryMovementConfiguration`, `ProductComponentConfiguration`
- Integración: consume `CustomerCreatedIntegrationEvent` (de Sales) y `SupplierCreatedIntegrationEvent` (de Procurement) para réplica liviana; publica `InventoryMovementPostedIntegrationEvent` para que Finance genere el cobro/pago asociado.

> **Nota legacy:** los getters `[NotMapped]` `CustomerName/ProviderName/EmployeeFullName/...` de `InventoryTransacction` desaparecen: el nombre se proyecta en el `Response` del Query, no en la entidad. Las múltiples auto-FK (`SaleInvoicePedidoId`, `PurchaseInvoicePedidoId`, `PedidoId`, `ComponentTransactionId`) deben modelarse como relaciones explícitas dentro del aggregate `InventoryMovement` o como referencias por ID con tipo de documento.

---

### 4. Finance
**Responsabilidad:** Ingresos, egresos, anticipos de caja, movimientos bancarios internos, cuentas bancarias, categorías de ingreso/costo, cierres contables, y los balances/dashboards financieros.
**Bounded Context:** Finance & Treasury. `Income`, `IncomeDetail`, `IncomeCategory`, `Expense`, `ExpenseDetail`, `CostCategories`, `CashAdvance`, `InternalBankMovements`, `BankAccounts`, `AccountsClosing`, vínculos de pago/cobro por transacción (`IncomeByTransaction`, `PaymentByTransaction`, `PaymentTransactionByExpense`, `PaymentMethod`), `IncomeBySaleOrder`.
**Dependencias:** Catalog (Currency, ExchangeRate, PSL), CustomersAndSuppliers (por ID), IdentityAccess (aprobador/ejecutor por UserId). Recibe eventos de Inventory y POS.
**Prioridad de migración:** Baja (es el contexto más acoplado; depende de casi todos los demás y concentra los orquestadores).

#### Domain
- Entities/Aggregates: `Income` (root con `IncomeDetail`), `Expense` (root con `ExpenseDetail`), `CashAdvance`, `BankAccount`, `InternalBankTransfer`, `AccountClosing`, `PaymentMethod`
- Value Objects: `Money`, `ApprovalInfo` (AprovedById + fecha), `PaymentAllocation`, `TaxBreakdown`
- Domain Events: `IncomeRegisteredEvent`, `ExpenseRegisteredEvent`, `ExpenseApprovedEvent`, `CashAdvanceIssuedEvent`, `CashAdvanceSettledEvent`, `BankTransferExecutedEvent`, `AccountsClosedEvent`
- Errors: `IncomeErrors`, `ExpenseErrors`, `CashAdvanceErrors`, `BankAccountErrors`

#### Application
- Commands: `RegisterIncomeCommand`/`Handler`, `RegisterIncomeFromTransactionCommand`/`Handler` (reemplaza `IncomesCoordinatorService`), `RegisterExpenseCommand`/`Handler`, `ApproveExpenseCommand`/`Handler`, `IssueCashAdvanceCommand`/`Handler`, `SettleCashAdvanceCommand`/`Handler`, `ExecuteBankTransferCommand`/`Handler`, `CloseAccountsCommand`/`Handler`
- Queries: `GetTotalBalanceQuery`/`Handler`, `GetBalancesByBankQuery`/`Handler`, `GetFinancialSummaryDashboardQuery`/`Handler`, `GetExpensesByCategoryReportQuery`/`Handler`, `ListIncomesQuery`/`Handler`, `ListExpensesQuery`/`Handler`
- Validators: `RegisterIncomeCommandValidator`, `RegisterExpenseCommandValidator`, `IssueCashAdvanceCommandValidator`
- Responses: `BalanceResponse`, `BankAccountBalanceResponse`, `FinancialSummaryResponse`, `IncomeResponse`, `ExpenseResponse`

#### Infrastructure
- DbContext: `FinanceDbContext`
- Repositories: `IIncomeRepository`/..., `IExpenseRepository`/..., `ICashAdvanceRepository`/..., `IBankAccountRepository`/...
- Configurations: `IncomeConfiguration`, `ExpenseConfiguration`, `CashAdvanceConfiguration`, `BankAccountConfiguration`, `InternalBankTransferConfiguration`
- Integración: consume `InventoryMovementPostedIntegrationEvent` y `SaleOrderPaidIntegrationEvent` para generar cobros/pagos; consume eventos de Partners para réplica liviana.

> **Nota legacy:** `BalanceService` (820 líneas) y `IncomesCoordinatorService` mezclan consultas, cálculo de balances y orquestación multi-tabla. Las consultas van a Query Handlers; la orquestación de "cobro a partir de una transacción de inventario" se convierte en un Command Handler que reacciona a eventos de integración en lugar de inyectar 10 servicios.

---

### 5. SAC (Contracts & Valuations)
**Responsabilidad:** Gestión de contratos, modalidades, secciones, ítems, órdenes de servicio (ODS), valuaciones y sus estados, y la relación de contratos con ingresos/egresos.
**Bounded Context:** Service Contracts (SAC). `Contract`, `ContractModality`, `ContractSections`, `ContractItem`, `ContractItemsByValuation`, `ODS`, `Valuation`, `ValuationStatus`, `StatusByValuation`, `ContractIncomeRelation`, `ContractExpenseRelation`.
**Dependencias:** CustomersAndSuppliers (ClientId), Catalog (Location, PSL). Las relaciones con Income/Expense se vuelven referencias por ID + eventos.
**Prioridad de migración:** Media.

#### Domain
- Entities/Aggregates: `Contract` (root con `ContractSection`, `ContractItem`), `WorkOrder` (ODS), `Valuation` (root con `ValuationItem`, historial de estados)
- Value Objects: `ContractCode`, `DateRange` (StartDate/EndDate), `Money`, `ValuationStatusTransition`
- Domain Events: `ContractSignedEvent`, `WorkOrderCreatedEvent`, `ValuationSubmittedEvent`, `ValuationStatusChangedEvent`, `ValuationApprovedEvent`
- Errors: `ContractErrors`, `WorkOrderErrors`, `ValuationErrors`

#### Application
- Commands: `CreateContractCommand`/`Handler`, `AddContractItemCommand`/`Handler`, `CreateWorkOrderCommand`/`Handler`, `CreateValuationCommand`/`Handler`, `ChangeValuationStatusCommand`/`Handler`
- Queries: `GetContractByIdQuery`/`Handler`, `ListContractsQuery`/`Handler`, `GetValuationByIdQuery`/`Handler`, `ListValuationsByContractQuery`/`Handler`, `GetValuationStatusHistoryQuery`/`Handler`
- Validators: `CreateContractCommandValidator`, `CreateValuationCommandValidator`, `ChangeValuationStatusCommandValidator`
- Responses: `ContractResponse`, `WorkOrderResponse`, `ValuationResponse`

#### Infrastructure
- DbContext: `SacDbContext`
- Repositories: `IContractRepository`/..., `IValuationRepository`/..., `IWorkOrderRepository`/...
- Configurations: `ContractConfiguration`, `ContractItemConfiguration`, `WorkOrderConfiguration`, `ValuationConfiguration`, `StatusByValuationConfiguration`
- Integración: consume `CustomerCreatedIntegrationEvent`; publica `ValuationApprovedIntegrationEvent` (consumible por Finance para generar el ingreso).

> **Nota legacy:** la máquina de estados de valuación está hoy implícita (`CurrentValuationStatusId` + tabla `StatusByValuation`). Modelarla como transiciones de estado validadas dentro del aggregate `Valuation`.

---

### 6. HumanResources
**Responsabilidad:** Empleados, estructura organizacional (departamentos, oficinas, cargos) y el programa de Bono de Alimentación (FoodBonus): balance, recargas y consumos.
**Bounded Context:** Human Resources & Food Bonus. `Employee`, `HRDepartment`, `HROffice`, `HRWorkPosition`, `FoodBonusBalance`, `FoodBonusReceived`, `FoodBonusConsumption`.
**Dependencias:** Catalog (PSL). El FoodBonus se consume desde POS/Finance vía eventos (el saldo no debe ser leído/escrito directamente por POS).
**Prioridad de migración:** Media.

#### Domain
- Entities/Aggregates: `Employee` (root), `Department`, `Office`, `WorkPosition`, `FoodBonusAccount` (root: balance + token + PIN), `FoodBonusRecharge`, `FoodBonusConsumption`
- Value Objects: `IdentificationNumber`, `CardCode`, `Money`, `ActivationPin`, `PersonName`
- Domain Events: `EmployeeHiredEvent`, `EmployeeDeactivatedEvent`, `FoodBonusRechargedEvent`, `FoodBonusConsumedEvent`, `FoodBonusInsufficientBalanceEvent`
- Errors: `EmployeeErrors`, `FoodBonusErrors`

#### Application
- Commands: `CreateEmployeeCommand`/`Handler`, `UpdateEmployeeCommand`/`Handler`, `RechargeFoodBonusCommand`/`Handler`, `ConsumeFoodBonusCommand`/`Handler`, `ActivateFoodBonusCardCommand`/`Handler`, `ValidateFoodBonusPinCommand`/`Handler`
- Queries: `GetEmployeeByIdQuery`/`Handler`, `ListEmployeesQuery`/`Handler`, `GetFoodBonusBalanceQuery`/`Handler`, `GetFoodBonusConsumptionReportQuery`/`Handler`, `GetEmployeeBalanceReportQuery`/`Handler`
- Validators: `CreateEmployeeCommandValidator`, `ConsumeFoodBonusCommandValidator`
- Responses: `EmployeeResponse`, `FoodBonusBalanceResponse`, `FoodBonusConsumptionResponse`

#### Infrastructure
- DbContext: `HumanResourcesDbContext`
- Repositories: `IEmployeeRepository`/..., `IFoodBonusRepository`/...
- Configurations: `EmployeeConfiguration`, `DepartmentConfiguration`, `OfficeConfiguration`, `WorkPositionConfiguration`, `FoodBonusAccountConfiguration`
- Integración: expone `ConsumeFoodBonusCommand` para POS (vía request/response sync o evento); publica `FoodBonusConsumedIntegrationEvent` para Finance.

> **Nota legacy:** `FoodBonusBalance` declara DOS `[Key]` (`Id` y `EmployeeId`) — modelar como aggregate `FoodBonusAccount` con identidad por `EmployeeId`. El consumo de bono desde `SaleOrderController`/`IncomesCoordinatorService` debe pasar por el aggregate `FoodBonusAccount` (que valida saldo y emite el evento), no por escritura directa al balance.

---

### 7. Sales
**Responsabilidad:** Maestro de Clientes y todo el flujo de venta: órdenes de venta, punto de venta interno (ambientes, mesas, impresoras, sucursales POS) y reportes de venta.
**Bounded Context:** Sales & Point of Sale. `Customer`, `PersonType`, `SaleOrder`, `SaleOrderDetail`, `PosEnvironment`, `PosTable`, `PosPrinter`, `OfficeBranch`, `IncomeBySaleOrder`, `POSTab`.
**Dependencias:** MasterData (Currency, PSL), Inventory (Product por ID), HumanResources (consumo de FoodBonus vía evento), Finance (pago genera ingreso vía evento).
**Prioridad de migración:** Baja (orquesta venta + stock + pago + bono; muy acoplado).

#### Domain
- Entities/Aggregates: `Customer` (root), `SaleOrder` (root con `SaleOrderLine`), `PosEnvironment`, `PosTable`, `PosPrinter`, `OfficeBranch`
- Value Objects: `ContactInfo`, `Address`, `TaxId`, `PhoneNumber`, `PaymentTerms`, `Money`, `TaxBreakdown`, `SaleOrderStatus`, `DinersCount`
- Domain Events: `CustomerCreatedEvent`, `CustomerDeactivatedEvent`, `SaleOrderOpenedEvent`, `SaleOrderLineAddedEvent`, `SaleOrderPaidEvent`, `SaleOrderCancelledEvent`
- Errors: `CustomerErrors`, `SaleOrderErrors`, `PosTableErrors`

#### Application
- Commands: `CreateCustomerCommand`/`Handler`, `UpdateCustomerCommand`/`Handler`, `DeactivateCustomerCommand`/`Handler`, `OpenSaleOrderCommand`/`Handler`, `AddSaleOrderLineCommand`/`Handler`, `RegisterSaleOrderPaymentCommand`/`Handler`, `CancelSaleOrderCommand`/`Handler`
- Queries: `GetCustomerByIdQuery`/`Handler`, `ListCustomersQuery`/`Handler`, `GetSaleOrderByIdQuery`/`Handler`, `GetSaleOrderByTableQuery`/`Handler`, `ListSaleOrdersQuery`/`Handler`, `GetPosSalesReportQuery`/`Handler`
- Validators: `CreateCustomerCommandValidator`, `OpenSaleOrderCommandValidator`, `RegisterSaleOrderPaymentCommandValidator`
- Responses: `CustomerResponse`, `SaleOrderResponse`, `SaleOrderFullResponse`, `PosSalesReportResponse`

#### Infrastructure
- DbContext: `SalesDbContext`
- Repositories: `ICustomerRepository`/`CustomerRepository`, `ISaleOrderRepository`/..., `IPosTableRepository`/..., `IOfficeBranchRepository`/...
- Configurations: `CustomerConfiguration`, `PersonTypeConfiguration`, `SaleOrderConfiguration`, `SaleOrderLineConfiguration`, `PosTableConfiguration`, `OfficeBranchConfiguration`
- Integración: publica `CustomerCreatedIntegrationEvent` (réplica liviana para Inventory/Finance), `SaleOrderPaidIntegrationEvent` (Finance crea el ingreso); consume `ProductCreatedIntegrationEvent` (réplica liviana del catálogo POS); coordina `ConsumeFoodBonusCommand` con HumanResources.

> **Nota legacy:** `SaleOrderController` asigna `CreatedBy/DateCreated`, parsea claims y mapea PSL condicionalmente — todo eso pasa al Command Handler + un `IUserContext`/behavior de auditoría. La inyección de `IFoodBonusBalanceService` + `IEmployeeService` directamente en el controller del POS rompe el límite de contexto: reemplazar por evento/command cross-módulo.

---

### 8. Procurement (Purchase Orders)
**Responsabilidad:** Maestro de Proveedores y todo el flujo de compra: órdenes de compra, flujo integrado con niveles de aprobación, detalles, y vínculo con gastos/transacciones. Líneas de producto-servicio por usuario.
**Bounded Context:** Procurement. `Supplier`, `ProviderType`, `ProcurementResponsible`, `PurchaseOrder`, `PurchaseOrderDetail`, `PurchaseOrderIntegrated`, `PurchaseOrderDetailsIntegrated`, `PurchaseOrderByExpense`, `PurchaseOrderByTransaction`, `PurchaseApprovalLevel`, `UserPurchaseApprovalLevel`, `UserProductServiceLine`, `PurchaseOrderTotalPaymentStatus`.
**Dependencias:** MasterData (PSL, Currency), Inventory (Product por ID), IdentityAccess (aprobadores por UserId). Vínculo con Finance (gasto) y Inventory (transacción) vía eventos.
**Prioridad de migración:** Media.

#### Domain
- Entities/Aggregates: `Supplier` (root), `ProcurementResponsible`, `PurchaseOrder` (root con `PurchaseOrderLine`), `IntegratedPurchaseOrder` (root del flujo con aprobaciones), `ApprovalLevel`
- Value Objects: `ContactInfo`, `Address`, `TaxId`, `PhoneNumber`, `PaymentTerms`, `PurchaseOrderCode`, `Priority`, `ApprovalChain`, `Money`, `PaymentStatus`
- Domain Events: `SupplierCreatedEvent`, `SupplierDeactivatedEvent`, `PurchaseOrderRequestedEvent`, `PurchaseOrderApprovedEvent`, `PurchaseOrderRejectedEvent`, `PurchaseOrderCompletedEvent`, `PurchaseOrderClosedEvent`
- Errors: `SupplierErrors`, `PurchaseOrderErrors`, `ApprovalErrors`

#### Application
- Commands: `CreateSupplierCommand`/`Handler`, `UpdateSupplierCommand`/`Handler`, `DeactivateSupplierCommand`/`Handler`, `CreatePurchaseOrderCommand`/`Handler`, `ApprovePurchaseOrderCommand`/`Handler`, `RejectPurchaseOrderCommand`/`Handler`, `CompletePurchaseOrderCommand`/`Handler`, `LinkPurchaseOrderToExpenseCommand`/`Handler`
- Queries: `GetSupplierByIdQuery`/`Handler`, `ListSuppliersQuery`/`Handler`, `GetPurchaseOrderByIdQuery`/`Handler`, `ListPurchaseOrdersQuery`/`Handler`, `GetPurchaseOrdersWithTotalAmountQuery`/`Handler` (reemplaza `PurchaseOrderWithTotalAmountVW`), `GetPendingApprovalsForUserQuery`/`Handler`
- Validators: `CreateSupplierCommandValidator`, `CreatePurchaseOrderCommandValidator`, `ApprovePurchaseOrderCommandValidator`
- Responses: `SupplierResponse`, `PurchaseOrderResponse`, `IntegratedPurchaseOrderResponse`, `ApprovalLevelResponse`

#### Infrastructure
- DbContext: `ProcurementDbContext`
- Repositories: `ISupplierRepository`/`SupplierRepository`, `IPurchaseOrderRepository`/..., `IIntegratedPurchaseOrderRepository`/..., `IApprovalLevelRepository`/...
- Configurations: `SupplierConfiguration`, `ProviderTypeConfiguration`, `ProcurementResponsibleConfiguration`, `PurchaseOrderConfiguration`, `IntegratedPurchaseOrderConfiguration`, `ApprovalLevelConfiguration`, `UserProductServiceLineConfiguration`
- Integración: publica `SupplierCreatedIntegrationEvent` (réplica liviana para Inventory/Finance), `PurchaseOrderApprovedIntegrationEvent`; consume `ProductCreatedIntegrationEvent`.

> **Nota legacy:** existen DOS modelos de OC (`PurchaseOrder` clásico y `PurchaseOrderIntegrated`). Evaluar consolidar en un solo aggregate `PurchaseOrder` con el flujo de aprobación integrado, marcando el modelo viejo como obsoleto durante la transición.

---

### 9. Logistics (Transport & Weighing)
**Responsabilidad:** Transporte y báscula: compañías de transporte, vehículos, remolques y tipos, conductores, guías, tickets de pesaje e información externa del ticket.
**Bounded Context:** Logistics & Transport. `TransportCompany`, `Vehicle`, `Trailer`, `TrailerType`, `CarDriver`, `Guide`, `TicketWeighin`, `TicketExternalInfo`.
**Dependencias:** CustomersAndSuppliers (Supplier transportista por ID), Inventory (Product/transacción asociada por ID). Bajo acoplamiento.
**Prioridad de migración:** Media.

#### Domain
- Entities/Aggregates: `TransportCompany`, `Vehicle`, `Trailer`, `Driver`, `ShippingGuide` (Guide), `WeighingTicket` (root con `TicketExternalInfo`)
- Value Objects: `PlateNumber`, `Weight`, `TrailerType`
- Domain Events: `WeighingTicketRegisteredEvent`, `ShippingGuideIssuedEvent`
- Errors: `TransportErrors`, `WeighingTicketErrors`

#### Application
- Commands: `RegisterTransportCompanyCommand`/`Handler`, `RegisterVehicleCommand`/`Handler`, `IssueShippingGuideCommand`/`Handler`, `RegisterWeighingTicketCommand`/`Handler`
- Queries: `ListTransportCompaniesQuery`/`Handler`, `ListVehiclesQuery`/`Handler`, `GetWeighingTicketByIdQuery`/`Handler`, `ListShippingGuidesQuery`/`Handler`
- Validators: `RegisterVehicleCommandValidator`, `RegisterWeighingTicketCommandValidator`
- Responses: `TransportCompanyResponse`, `VehicleResponse`, `WeighingTicketResponse`, `ShippingGuideResponse`

#### Infrastructure
- DbContext: `LogisticsDbContext`
- Repositories: `ITransportCompanyRepository`/..., `IVehicleRepository`/..., `IWeighingTicketRepository`/...
- Configurations: `TransportCompanyConfiguration`, `VehicleConfiguration`, `TrailerConfiguration`, `WeighingTicketConfiguration`

---

### 10. A2Sync (External Accounting Integration)
**Responsabilidad:** Sincronización con el sistema contable externo A2: clientes, ventas y cuentas por cobrar recibidos/enviados a A2.
**Bounded Context:** External Integration / Anti-Corruption Layer. `A2Customer`, `A2Sale`, `A2ReceivableAccount`.
**Dependencias:** Consume datos de CustomersAndSuppliers, Finance e InternalPOS vía eventos de integración. Actúa como ACL hacia el sistema externo.
**Prioridad de migración:** Baja (depende de que los módulos fuente ya emitan eventos).

#### Domain
- Entities/Aggregates: `A2CustomerSync`, `A2SaleSync`, `A2ReceivableSync`
- Value Objects: `A2SyncStatus`, `RifNumber`, `OfficeBranchId`, `Money`
- Domain Events: `A2SyncSucceededEvent`, `A2SyncFailedEvent`
- Errors: `A2SyncErrors`

#### Application
- Commands: `PushSaleToA2Command`/`Handler`, `PushCustomerToA2Command`/`Handler`, `RetryFailedA2SyncCommand`/`Handler`
- Queries: `GetA2SyncStatusQuery`/`Handler`, `ListPendingA2SyncQuery`/`Handler`
- Validators: `PushSaleToA2CommandValidator`
- Responses: `A2SyncStatusResponse`

#### Infrastructure
- DbContext: `A2SyncDbContext`
- Repositories: `IA2SaleRepository`/..., `IA2CustomerRepository`/..., `IA2ReceivableRepository`/...
- Configurations: `A2CustomerConfiguration`, `A2SaleConfiguration`, `A2ReceivableConfiguration`
- Integración: consume `SaleOrderPaidIntegrationEvent`, `CustomerCreatedIntegrationEvent`, `IncomeRegisteredIntegrationEvent`; `A2ApiClient` (ACL hacia el sistema externo).

---

### 11. FileManagement & Notifications (Shared Supporting Module)
**Responsabilidad:** Almacenamiento de archivos/imágenes/fotos (FTP) y notificaciones in-app por PSL. Servicios de soporte transversales que hoy están sueltos en la capa Service/Util.
**Bounded Context:** Supporting Subdomain. `FileStorage`, `Photo`, `ProductImage` (almacenamiento), `Notifications`, `NotificationTypes`.
**Dependencias:** Consumido por todos. No referencia dominios de negocio (solo IDs/ModuleName).
**Prioridad de migración:** Media (FileStorage es dependencia de Inventory/Product; Notifications es transversal).

#### Domain
- Entities/Aggregates: `StoredFile` (root), `Notification` (root)
- Value Objects: `FileUrl`, `FileName`, `NotificationChannel`, `NotificationStatus`
- Domain Events: `FileUploadedEvent`, `NotificationCreatedEvent`, `NotificationReadEvent`
- Errors: `FileStorageErrors`, `NotificationErrors`

#### Application
- Commands: `UploadFileCommand`/`Handler`, `DeleteFileCommand`/`Handler`, `CreateNotificationCommand`/`Handler`, `MarkNotificationAsReadCommand`/`Handler`
- Queries: `GetFileQuery`/`Handler`, `ListNotificationsByUserQuery`/`Handler`, `GetUnreadNotificationsCountQuery`/`Handler`
- Validators: `UploadFileCommandValidator`, `CreateNotificationCommandValidator`
- Responses: `StoredFileResponse`, `NotificationResponse`

#### Infrastructure
- DbContext: `FilesNotificationsDbContext`
- Repositories: `IFileStorageRepository`/..., `INotificationRepository`/...
- Configurations: `StoredFileConfiguration`, `NotificationConfiguration`
- Otros: `FtpFileStorageService` (migrar `Util/FTP`), consumidores de eventos de integración que disparan notificaciones.

> **Nota:** Notifications podría modelarse como reacción a eventos de integración de los demás módulos (p. ej. `PurchaseOrderApprovedIntegrationEvent` -> notificación), en vez de que cada servicio llame directamente a `NotificationsService`.

---

## Orden de migración recomendado

1. **SharedKernel + infraestructura común** — confirmar primitivas (`AggregateRoot`, `DomainEvent`, `OutboxMessage`), Value Objects compartidos (`Money`, `ExchangeRate`), behaviors de MediatR (validación, auditoría, transacción), bus de integración (MassTransit/RabbitMQ) y la convención de auditoría/soft-delete (interceptor EF) para eliminar la copia manual de `CreatedBy/IsActive`.
2. **IdentityAccess** — todos dependen de la identidad para auditoría y permisos; sin acoplamiento de negocio entrante.
3. **MasterData (Catalog)** — provee Currency, ExchangeRate, PSL, Location, Units; requerido por casi todos.
4. **FileManagement & Notifications** — soporte transversal (FileStorage lo necesita Inventory/Product).
5. **HumanResources** — relativamente autónomo; habilita el FoodBonus consumido por Sales.
6. **Inventory** — núcleo grande; una vez listo MasterData. Separar `Product` de `InventoryMovement`.
7. **Logistics** — bajo acoplamiento; depende de Procurement e Inventory por ID.
8. **SAC** — depende de Sales/MasterData; emite eventos a Finance.
9. **Procurement** — incluye Supplier; emite eventos a Finance/Inventory. Consolidar los dos modelos de OC.
10. **Sales** — incluye Customer + POS; orquesta venta+stock+bono+pago; migrar cuando Inventory/HR/Finance ya emitan/consuman eventos.
11. **Finance** — el más acoplado; concentra balances y coordinadores. Migrar último para que reciba eventos de Inventory/Sales/SAC/Procurement ya migrados.
12. **A2Sync** — al final; depende de que las fuentes (Sales, Finance, Procurement) emitan eventos de integración.

---

## Vicios del legacy a evitar

- **DbContext único compartido (~110 entidades):** rompe los límites de contexto y obliga a cargar todo el grafo. **Solución:** un DbContext + esquema/tabla de migraciones por módulo; nada de navegar FK entre módulos.
- **FK directas entre dominios (`InventoryTransacction` -> Clients/Providers/Employee/SaleOrder/CashAdvance):** acoplamiento físico entre contextos. **Solución:** guardar solo el ID externo; replicar los datos mínimos necesarios (Id + nombre) vía eventos de integración (eventual consistency).
- **Entidades anémicas con decenas de `[NotMapped]` getters de presentación (`CustomerName`, `EmployeeFullName`, `ProviderName`...):** mezclan modelo de dominio con proyección de lectura. **Solución:** entidades ricas con invariantes; la proyección de nombres se hace en los `*Response` de los Query Handlers (CQRS read side).
- **Cálculos monetarios duplicados (`valor / ExchangeRate` repetido en Product, InventoryTransacction, SaleOrder):** lógica de negocio copiada en getters `[NotMapped]`. **Solución:** Value Object `Money` + `ExchangeRate` con la conversión encapsulada y reutilizada.
- **Lógica de negocio en controllers (asignar `CreatedBy/DateCreated`, parsear claims, mapear PSL condicional):** **Solución:** controllers delgados que solo mapean request -> Command/Query y devuelven el resultado; auditoría vía `IUserContext` + behavior/interceptor; validación vía FluentValidation.
- **Coordinadores que inyectan 8-10 servicios (`IncomesCoordinatorService`) y servicios de 800+ líneas (`BalanceService`):** "god services" sin límite de contexto. **Solución:** un Command Handler por caso de uso; la orquestación cross-módulo se reemplaza por eventos de integración (un módulo reacciona a lo que otro publicó).
- **Acceso cross-módulo directo (POS escribiendo `FoodBonusBalance` y `Employee`):** **Solución:** el consumo de bono pasa por el aggregate `FoodBonusAccount` de HR (valida saldo, emite evento); POS no toca tablas de HR.
- **Auditoría y soft-delete copiados en cada entidad (`IModelBase` + 4 campos x110):** boilerplate y manejo manual. **Solución:** clase base/interfaz en SharedKernel + interceptor EF que setea auditoría y aplica filtro global de soft-delete automáticamente.
- **Reportes vía Stored Procedures y Views (`Sp_InventoryReportStockByDate`, `PurchaseOrderWithTotalAmountVW`, `Summary...VW`):** lógica oculta en la base. **Solución:** Query Handlers dedicados (read model) o vistas mantenidas por el módulo dueño; si se conservan SPs, encapsularlos en el Infrastructure del módulo correspondiente.
- **Multi-tenancy por header leído en `HttpContext.Items`:** frágil y disperso. **Solución:** `IOfficeBranchContext`/`ITenantContext` inyectable, resuelto una sola vez y aplicado como filtro global por el interceptor del DbContext.
- **Claves e inconsistencias de modelado (doble `[Key]` en `FoodBonusBalance`, dos modelos de OC, nombres con typos `InventoryTransacction`, `PhoneNumer`):** **Solución:** normalizar nombres y claves al modelar los aggregates; consolidar duplicados.
- **`UnitOfWork` monolítico que expone todos los repos:** cualquier servicio puede tocar cualquier tabla. **Solución:** repositorios por aggregate dentro de cada módulo; sin UoW global compartido.

---

## Estado de migración

| Módulo | Estado | Notas |
|--------|--------|-------|
| IdentityAccess | 🔄 En progreso | Módulo `Identity` — User, Role, SystemAction con JWT, CQRS y permisos granulares (reemplaza el modelo legacy Form/Action/FormAction por permisos tipo `"products.create"`). Pendiente: `Module`/navegación de UI si se necesita |
| MasterData (Catalog) | ✅ Completo (catálogo base) | Country, State, City, Currency, ProductServiceLine, Unit, Tax, ExchangeRate — todos con CRUD + paginación. Pendiente: `SystemParameter`, `CompanyBranch`/`OfficeBranch` |
| FileManagement & Notifications | ⬜ Pendiente | — |
| HumanResources | ⬜ Pendiente | — |
| Inventory | 🔄 En progreso | Catálogo completo: ProductType, ProductCategory, ProductSubCategory, ProductBrand, Warehouse, StorageType + `Product` (aggregate central) con `ProductCodeCounter` y `ProductPriceHistory`. Pendiente: stock por almacén (`ProductStock`/`InventoryMovement`), `Equipment` |
| Logistics | ⬜ Pendiente | — |
| SAC (Contracts & Valuations) | ⬜ Pendiente | — |
| Procurement | ⬜ Pendiente | Incluye entidad Supplier y ProcurementResponsible |
| Sales | ⬜ Pendiente | Incluye entidad Customer + POS |
| Finance | ⬜ Pendiente | — |
| A2Sync | ⬜ Pendiente | — |

**Nota:** esta tabla refleja el estado de los módulos del nuevo sistema (Strangler Fig), no la migración de datos legacy→nuevo en sí (ver sección "Premisa de migración de datos" en `CLAUDE.md`). Los scripts SQL de migración de datos se escriben al final, cuando los módulos estén estables.
