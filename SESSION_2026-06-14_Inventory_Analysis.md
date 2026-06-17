# Sesión 2026-06-14 — Análisis módulo Inventory

## Estado: pendiente de respuestas para arrancar código

---

## Contexto
Se analizó la entidad `Product` del sistema legado para migrarla al nuevo módulo `Inventory` siguiendo DDD + Clean Architecture + CQRS. Se tomaron varias decisiones de diseño.

---

## Decisiones tomadas

### 1. `ProductCargoEquipment` → entidad `Equipment` separada
Los productos de tipo equipo (carros, grúas, bombas, motores) tienen datos propios ricos:
- Placa, seriales (motor, chasis, NIV), año, color, asientos
- Registro INTT, propiedad
- Seguro (fecha vencimiento, archivos de póliza)
- GPS, firmas de autorización
- Asignación (persona, teléfono, documento, PSL del empleado)
- Ubicación física

**Decisión:** `Equipment` es un aggregate separado en `Inventory.Domain` con un `ProductId` FK.  
`Product` NO referencia a `Equipment` — es `Equipment` quien apunta a `Product`.  
**Razón:** lifecycle independiente, queries propias, opcionalidad (mayoría de productos no son equipo).

### 2. Tax con múltiples impuestos por producto
Un producto puede tener **varios** impuestos simultáneos (compra y venta por separado).

**Decisión:** `Tax` va en `MasterData`. `Product` almacena colecciones de IDs:
```
Product aggregate
├── _purchaseTaxIds : List<TaxId>   → tabla ProductPurchaseTax (ProductId, TaxId)
└── _saleTaxIds     : List<TaxId>   → tabla ProductSaleTax (ProductId, TaxId)
```
Sin navigation property cross-módulo — solo IDs, igual que `CurrencyId` y `UnitId`.

### 3. Campos del legado diferidos a otros módulos
| Campo(s) | Módulo correcto |
|----------|----------------|
| `SalesCommission`, `Price2/3/4/5`, `AdditionalPrice1/2/3`, `IsVisibleOnPOS`, `POSPrinterId` | `Sales` |
| `BudgetAccount`, `IncomeAccount`, `ExpenseAccount` | `Accounting` (futuro) |
| `ImportationCost`, `DefaultBuyResponsableId`, `PurchaseResponsableId` | `Procurement` (futuro) |

### 4. Precios simplificados para MVP
El legado tiene `New*` / `Current*` / `Previous*` para precios (flujo de aprobación).  
**Decisión MVP:** solo `Cost` y `SellingPrice` actuales. Historial de precios → futura entidad `ProductPriceHistory`.

### 5. `[NotMapped]` computed properties
`ProductTypeName`, `TotalCostCurrencyAmount`, `CurrentUnitPrice`, etc. → van en DTOs de respuesta de Application, NO en el dominio.

### 6. `ProductCargoEquipment` en legado necesita además: `ProductLocation`
Catálogo simple en `Inventory.Domain`.

---

## Estructura `Product` para MVP

```
Product (Inventory.Domain)
├── Identificadores: Code, UniqueCode, Barcode, ClientCodeRef, ItemNumberByDay
├── Descripción: Name, Description, ModelName, Tags
├── Clasificación: ProductTypeId, ProductCategoryId, ProductSubCategoryId, ProductBrandId
├── Línea: ProductServiceLineId (cross-módulo MasterData, solo ID)
├── Precios: BuyingCurrencyId, Cost, SellingCurrencyId, SellingPrice, ProfitMargin
├── Impuestos: _purchaseTaxIds List<TaxId>, _saleTaxIds List<TaxId> (cross-módulo MasterData)
├── Unidades: SaleUnitId, PurchaseUnitId, SaleUnitEquivalent (cross-módulo MasterData)
├── Inventario: WarehouseId, StorageTypeId, Minimum, Maximum, IsStored
├── Físico: Weight, Volume, ContentCapacity, Condition, SerialNumber, DaysToDeliver, IsRounded
├── Comportamiento: IsSalable, IsBuyingAble, IsOperationBuy, IsOperationManufacture, IsAssemblyProduct
├── Imagen: ImageUrl? (string simple, sin entidad FileStorage por ahora)
└── Auditoría: IsActive + shadow props CreatedAt/UpdatedAt via IAuditable
```

---

## Orden de implementación acordado

```
MasterData (PRIMERO — prerequisito para Inventory)
└── Tax  (Name, Code, Rate, TaxType, IsActive)

Inventory — Fase 1: catálogos simples (sin dependencias entre ellos)
├── ProductType
├── ProductCategory
├── ProductBrand
├── Warehouse
├── StorageType
└── ProductLocation          ← necesario para Equipment

Inventory — Fase 2: catálogo con FK interno
└── ProductSubCategory       → ProductCategory

Inventory — Fase 3: aggregates principales
├── Product                  → ref: ProductType, Category, SubCategory, Brand,
│                               Warehouse, StorageType + PSL/Unit/Currency/Tax por ID
└── Equipment                → ref: ProductId + ProductLocation + PSL empleado por ID
```

---

## Preguntas pendientes (necesarias antes de escribir código)

1. **¿`TaxType` es enum fijo o configurable por usuario?**
   - Enum fijo: solo "IVA", "Retención", etc. definidos en código
   - String libre: el usuario puede crear cualquier tipo de impuesto con nombre libre
   - Esto define si es un `enum` o una propiedad `string` en la entidad Tax

2. **¿Los archivos (póliza de seguro, autorización, misceláneos de Equipment) ya tienen soporte en el nuevo sistema?**
   - Si existe entidad `FileStorage` en HB_ERP → referenciarla
   - Si no → guardar solo URL como `string?` para el MVP de Equipment

---

## Notas adicionales
- El módulo Inventory aún no tiene proyectos creados (.csproj). Hay que crear la estructura completa.
- Patrón cross-módulo: igual que el resto del sistema — referencias solo por ID value object, sin navigation property, sin project reference entre módulos.
- `Equipment` es una entidad exclusiva de este ERP (equipos de carga/flota empresarial), no es un patrón estándar de ERPs de producto.
