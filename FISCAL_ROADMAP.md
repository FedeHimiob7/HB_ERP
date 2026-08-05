# Roadmap redireccionado — Sistema de Facturación Homologado

> Este documento **reemplaza como fuente de verdad del rumbo activo** al `MIGRATION_PLAN.md` original. `MIGRATION_PLAN.md` se conserva como análisis histórico del legacy (sigue siendo válido para entender modelos, vicios y equivalencias), pero el orden de implementación y las prioridades vigentes son las de este documento.

## Por qué se redirecciona

El plan original apuntaba a reconstruir los 12 bounded contexts del legacy completo (Identity, MasterData, Inventory, FileManagement, HumanResources, Logistics, SAC, Procurement, Sales, Finance, A2Sync). Se decidió redirigir el proyecto hacia un **Sistema de Facturación Homologado** conforme a la Providencia SENIAT 2024/000121 (ver `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado2.pdf` — versión corregida, ver sección "Referencia" al final de este documento), que exige que todo software de facturación esté homologado y autorizado por el SENIAT, con garantías de integridad, trazabilidad, inalterabilidad y remisión electrónica de los registros fiscales.

**Premisa que gobierna esta redirección:** los modelos ya construidos (Identity, MasterData, Inventory) fueron diseñados tomando como referencia el legacy, precisamente para permitir una futura sincronización con el sistema administrativo que ya está en producción. Esa premisa **se mantiene**: no es un bloqueante, pero cada decisión de diseño evalúa el costo de esa futura sincronización. Ver también la "Premisa de migración de datos" en `CLAUDE.md`.

**Tesis central:** no cambia la arquitectura (Clean Architecture + DDD + CQRS + MediatR + ErrorOr + EF Core + Outbox/MassTransit se mantienen intactos), cambia el centro de gravedad. Hoy el centro es `Product` (catálogo); ahora el centro pasa a ser el documento fiscal. Todo lo construido (Identity, MasterData, Inventory) sobrevive como "periferia configurable" — el propio enfoque de homologación separa lo *regulado* (se implementa una sola vez en el núcleo) de lo *sectorial* (vive en la periferia configurable).

---

## Decisiones confirmadas

### D-1 · `Company` + `Branch` + `FiscalTerminal` — una instalación = una empresa/RIF, sin multi-tenancy

**Revisado el 2026-08-04** — descarta el diseño multi-tenant original (ver nota histórica al final de esta sección).

Modelo de despliegue confirmado: el sistema **no es un SaaS centralizado**. Se instala una vez por cada empresa/cliente que lo requiera — base de datos propia, y potencialmente servidor propio, por instalación. Nunca conviven dos empresas (dos RIF) en la misma base de datos.

**Por qué esto descarta el multi-tenancy:** el SENIAT audita por RIF — al auditar a la empresa A no debe poder ver ni rozar información de la empresa B. Con una BD física y exclusiva por RIF ese riesgo desaparece por diseño, sin necesidad de discriminador ni filtro a nivel de aplicación. Meter un discriminador `CompanyId` + filtro global habría resuelto un problema (fuga de datos entre tenants compartiendo BD) que en este modelo de despliegue no puede ocurrir.

**Caso "empresa matriz con sub-empresas de RIF distinto"** (ej. LA como grupo, con LA Zapatería / LA Service / LA Amazon cada una con su propio RIF): cada sub-empresa con RIF propio es una **instalación separada, con su propia base de datos**. La noción de "grupo" que las une (LA) no vive en este sistema homologado — es un concepto del sistema administrativo legacy, y es candidato natural para la sincronización futura diferida (ver D-5), no para una jerarquía dentro de este sistema.

**Jerarquía confirmada dentro de una misma instalación:**
- `Company` — **una sola fila** por instalación: RIF, razón social, domicilio fiscal, tipo de contribuyente de la empresa dueña de esa base de datos. Es el emisor que aparece en cada documento fiscal. No es raíz de tenant, es perfil fiscal de la instalación.
- `Branch` (Sucursal) — N por `Company`. Representa un local físico de esa misma empresa/RIF (ej. LA Zapatería con 4 locales, todos bajo el mismo RIF — el SENIAT audita la empresa, no la sucursal individual, así que las facturas de las 4 sucursales conviven sin problema). Resuelve de paso el pendiente ya documentado de `OfficeBranch` para completar el formato de `Product.Code`.
- `FiscalTerminal` (antes anotado como `WorkStation`) — N por `Branch`. Es el punto de emisión (caja física, máquina fiscal, canal digital) dentro de una sucursal. Necesita ser una entidad propia porque cada punto de emisión requiere su **propia secuencia correlativa e ininterrumpida de número de control** (mismo patrón UPDLOCK que ya usa `ProductCodeCounter`) — si dos cajas de la misma sucursal compartieran una sola secuencia, emitir en simultáneo generaría condición de carrera sobre quién obtiene el siguiente correlativo. También es el nivel donde se configura **cuál de los tres medios de emisión** (máquina fiscal / forma libre / digital) usa cada punto, ya que pueden variar dentro de la misma sucursal.
- **`ProductServiceLine` (PSL) no se toca** — sigue siendo exactamente lo que ya es, una dimensión de catálogo (línea de negocio, filtro de visibilidad, parte de `Product.Code`), sin datos fiscales ni jerarquía bajo `Company`. Se descartó fusionarlo con `Branch`: la relación entre PSL y sucursal es muchos-a-muchos (un PSL se vende en varias sucursales, una sucursal vende varios PSL), no son la misma dimensión.
- **No se necesita discriminador `CompanyId` ni filtro global** en ningún DbContext — queda fuera del alcance de F0.

**Nota histórica:** el diseño original de este punto (multi-tenant con `CompanyId` + filtro global en los tres DbContexts, PSL anidado bajo `Company`) se descartó en la sesión del 2026-08-04 al confirmar que el modelo de negocio es instalación dedicada por empresa, no SaaS compartido.

### D-2 · `ThirdParty` unificado (reemplaza la separación Customer/Supplier)

Se revierte la decisión documentada en `CLAUDE.md` ("No existe un módulo `CustomersAndSuppliers` — esa decisión fue descartada").

- `ThirdParty` es la entidad base compartida: RIF/identificación, nombre/razón social, domicilio fiscal, tipo de contribuyente (ordinario/formal/especial).
- `Customer` y `Supplier` son roles/especializaciones sobre `ThirdParty` (no entidades independientes) — datos de crédito, lista de precios, condición de pago (Customer); retenciones aplicables (Supplier).
- Razón: el RIF y el tipo de contribuyente son datos fiscales transversales — determinan retenciones y tratamiento en libros independientemente del rol comercial, y un mismo RIF frecuentemente es cliente y proveedor a la vez.
- Vive en un módulo nuevo (ver más abajo qué módulo).

### D-3 · IGTF: pendiente de corregir, se mueve del precio al pago

Hallazgo: hoy `CalculatePricesQueryHandler` calcula el IGTF de forma compuesta sobre el **precio del producto**. El cálculo compuesto en sí es correcto (IGTF sobre el monto que ya incluye IVA), pero el IGTF es un impuesto sobre el **pago en divisas**, no sobre el bien — dos clientes comprando lo mismo, uno pagando en Bs y otro en divisas, no pueden tener el mismo total.

**Acción (no urgente, anotada para cuando se construya el núcleo de facturación):** el IGTF fiscal se calcula en el momento del pago (`Payment`/`PaymentMethod`), no en el precio del producto. El cálculo actual en `CalculatePricesQueryHandler` se conserva como "precio referencial con IGTF" de uso comercial, pero se documenta que no es el cálculo fiscal definitivo.

### D-4 · `Tax` + `TaxRate` (vigencia temporal)

Se separa la identidad del valor, siguiendo el mismo patrón que ya usa `ExchangeRate` (un registro nuevo por cada cambio, nunca se sobreescribe uno existente):

- `Tax`: identidad estable — `Name`, `TaxType` (IVA/IGTF/ISLR), código de alícuota. Casi nunca cambia.
- `TaxRate` (hijo de `Tax`): `Rate`, `EffectiveFrom`, `EffectiveTo`. Cada cambio de alícuota crea un registro nuevo con su propia vigencia; el anterior se cierra, nunca se edita.
- El motor fiscal resuelve la tasa vigente según la **fecha del documento**, nunca "el valor actual" — así se preserva el historial fiscal para auditorías, reimpresión y reportes.
- Impacto sobre lo construido: bajo. `Product` sigue guardando `TaxId`; hay que ajustar los handlers que hoy suman `Rate` directamente (`UpdateProductPricesCommandHandler`, `CalculatePrices`) para que resuelvan por vigencia.

### D-5 · Sincronización con el legacy: diferida, pero con un requisito ya anotado

No se implementa todavía (`ConectorLegacy`/sync no es prioridad de corto plazo). Pero se deja anotado como requisito futuro innegociable:

- Cuando se implemente, cada entidad candidata a sincronizarse con el sistema administrativo actual (`Product`, `ThirdParty`, y potencialmente documentos fiscales) va a necesitar un campo tipo `LegacyId` para poder correlacionar registros entre ambos sistemas.
- **No se agrega todavía** — se revisará cuando el diseño de la sincronización esté más claro, pero queda documentado aquí para no perderlo de vista y decidir en su momento si conviene adelantarlo (es más barato agregarlo antes de tener datos productivos que después).

### Acciones inmediatas (independientes del roadmap por fases, ya priorizadas)

- **Zona horaria — ✅ hecho.** `IFiscalClock`/`FiscalClock` (offset fijo UTC-4) ya están implementados en `HB_ERP.SharedKernel` e inyectados en `ExchangeRate.Create` (vía handlers) y `BCVRateSyncWorker`. Ver `CLAUDE.md` sección SharedKernel.
- **Pruebas unitarias — en curso, base establecida.** Estructura de test montada (`HB_ERP.SharedKernel.Tests`, `Tests/Domain.Tests` en MasterData e Inventory — ver `CLAUDE.md` sección Testing). Cobertura actual: `Tax`, `ExchangeRate`, `Product`, `FiscalClock` (36 tests). Pendiente: `Application.Tests`/`Infrastructure.Tests` en todos los módulos, y tests de dominio para `Identity` (hoy sin ninguno).
- **Investigación legal — hecha, ver Memoria Descriptiva v2.** Se hizo una investigación dedicada (agentes con Opus 5, WebSearch/WebFetch contra fuentes primarias y secundarias) que verificó y corrigió el contenido legal de `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado.pdf` contra el texto real de las providencias y leyes citadas. Resultado: `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado2.pdf`, con una sección "Fe de erratas y correcciones v2.0" al inicio que lista 15 correcciones puntuales (algunas críticas para el diseño de datos — ver siguiente punto). **Tratar la v2 como la fuente de verdad legal actualizada; la v1 queda como referencia histórica.**
- **Hallazgo crítico de la investigación — definición legal de "alteración de registros fiscales".** El art. 13, Parágrafo Único de la PA 121 define la alteración incluyendo la *ocultación* de un registro, no solo su borrado/edición. Esto aplica únicamente a **registros fiscales** (`FiscalDocument`, registro de eventos, libros de IVA) — el catálogo/datos maestros (`Product`, `Tax`, etc.) puede seguir usando el patrón `IsActive` ya establecido en el código, sin conflicto legal. Ningún mecanismo de soft-delete/ocultación debe tocar entidades fiscales cuando se construya el módulo `Billing`.
- **Otros hallazgos con impacto directo en F0-F3 (detalle completo en la Memoria v2):** el plazo de transición de la PA 121 fue de 90 días continuos (no 60 hábiles) y ya venció (19/03/2025), con exigencia reforzada por el SENIAT en enero de 2026; la retención de IVA se rige por la PA SNAT/2025/000054 (la PA 0049 está derogada); la Unidad Tributaria vigente es Bs. 43,00 (subió 377,8% en junio de 2025) y debe tratarse como parámetro versionado, igual que las alícuotas de IVA y los dos tramos de IGTF (0% bolívares / 3% divisas); el redondeo fiscal no sale del "art. 76 del Reglamento IVA" (ese artículo regula el Libro de Ventas) sino de los lineamientos del BCV; el sistema debe soportar los tres medios de emisión (máquina fiscal, forma libre/imprenta física, medio digital) como igualmente de primera clase, no uno "principal" y los demás opcionales.

---

## Mapeo de módulos (arquitectura de referencia del sistema homologado vs código actual)

| Módulo | Cobertura actual | Qué ya sirve | Qué falta |
|---|---|---|---|
| **Security/IAM** | 🟨 Parcial | `User`, `Role`, `SystemAction` con JWT y permisos granulares (mejor que el Form/Action del legacy) | `Company`, `Branch`, `FiscalTerminal`, `EventLog` (bitácora append-only), `HomologatedVersion` |
| **ThirdParty** | 🟥 Casi nada | `Country`/`State`/`City` como soporte de dirección | `ThirdParty`, `Customer`, `Supplier`, `IdentificationType`, `TaxpayerType`, `Address`, `Contact`, `PaymentTerm` |
| **Catalog & Inventory** | 🟨 Parcial | Catálogo completo (`ProductType/Category/SubCategory/Brand/Warehouse/StorageType`) + `Product` + `ProductCodeCounter` + `ProductPriceHistory` | `Stock`/`InventoryMovement`, variantes, lotes, seriales, código de alícuota en `Product` |
| **Billing (Facturación)** | 🟥 0% | Nada | **Todo**: `FiscalDocument` (aggregate raíz), líneas, impuestos por documento, series/número de control, notas de crédito/débito, pagos |
| **Purchasing** | 🟥 0% | Nada | Órdenes de compra, recepción, factura de compra, retenciones emitidas |
| **Fiscal Compliance** | 🟥 Muy parcial | `Tax` (a evolucionar a Tax+TaxRate), Outbox+MassTransit (base para remisión), patrón de worker externo (BCV, reusable para SENIAT) | Libros de IVA, transmisión SENIAT, registro de eventos fiscal, clave de consulta |
| **Treasury** | 🟨 Parcial | `Currency`, `ExchangeRate` (con scraping BCV automático — activo superior a lo que pide la norma) | Caja, arqueo, cuentas por cobrar/pagar, bancos |
| **Reports** | 🟨 Parcial | Patrón CQRS de lectura + paginación ya establecido | Vistas de libros fiscales, dashboards |
| **Integration** | 🟨 Parcial | MassTransit+RabbitMQ+Outbox, Swagger, precedente de conector externo (BCV) | Conector SENIAT, driver de máquina fiscal, conector de imprenta digital |

## Destino de los módulos del plan anterior

| Módulo del plan viejo | Destino |
|---|---|
| IdentityAccess | Se mantiene y crece (Security/IAM) |
| MasterData | Se mantiene, reparte responsabilidad (Currency/ExchangeRate→Treasury, Tax→Fiscal Compliance, Country/State/City→ThirdParty, Unit/PSL→Catalog) |
| Inventory | Se mantiene y crece (Catalog & Inventory) |
| Finance | Se reduce a Treasury (caja, banco, CxC/CxP, arqueo). Ingresos/egresos/anticipos de caja del legacy quedan fuera |
| Sales | Se redefine como Billing — núcleo de facturación fiscal. El POS de restaurante del legacy (mesas, comensales) queda fuera; `Customer` se muda a ThirdParty |
| Procurement | Se redefine y reduce como Purchasing. El flujo de aprobación multinivel del legacy queda fuera; `Supplier` se muda a ThirdParty |
| SAC (Contratos/Valuaciones) | Fuera por completo |
| HumanResources | Fuera por completo (si el POS necesita bono de alimentación, se resuelve como un `PaymentMethod` que consulta el legacy, no como módulo) |
| Logistics | Fuera por completo |
| A2Sync | Se reemplaza conceptualmente por una futura sincronización con el legacy (diferida, ver D-5) |
| FileManagement & Notifications | Diferido — `Product.ImageUrl` como string alcanza por ahora |

---

## Roadmap por fases

### F0 · Cimientos regulados (bloqueante)
- `Company` (fila única) + `Branch` + `FiscalTerminal` — **pendiente.** Sin discriminador de tenant ni filtro global (ver D-1 revisado: una instalación = una empresa/RIF, no multi-tenant); la tarea se redujo a modelar las tres entidades y su jerarquía, sin tocar los DbContexts existentes.
- `ProductCodeCounter` con secuencia de sucursal (ya estaba planificado) — pendiente
- Parámetro fiscal genérico versionado por fecha — **✅ hecho.** `IEffectiveDated` (`HB_ERP.SharedKernel/Domain/Primitives`) + extension method `GetEffectiveAsOfAsync<T>` (`HB_ERP.SharedKernel/Infrastructure/Extensions/EffectiveDatedQueryExtensions.cs`), generalizando el patrón inmutable de `ExchangeRate`.
- `Tax` → `Tax` + `FiscalTaxRate` con vigencia — **✅ hecho.** Diseño final (revisado tras feedback: no reintroducir una segunda entidad "suelta" tipo `TaxRate`): `Tax` = identidad/catálogo mutable (`Name`, `TaxType`, `IsActive`, sin `Rate`) — sigue siendo lo que `Product.PurchaseTaxIds`/`SaleTaxIds` referencia de forma estable. `FiscalTaxRate` = valor versionado (`TaxId` FK, `Rate`, `EffectiveFrom`), inmutable, **sin `IsActive`** — nunca se oculta una fila puntual, solo se acumulan versiones; convención de nombre `Fiscal*` para la mitad versionada de cualquier futuro split identidad/valor (ej. Unidad Tributaria, tramos IGTF). Un Command/Query por intención de negocio (`CreateTaxCommand` crea ambas entidades atómicamente, `RegisterTaxRateCommand` solo agrega versión, `UpdateTaxDetailsCommand` solo toca identidad) — nadie en Application elige manualmente qué tabla tocar, mismo mecanismo que `UpdateProductPricesCommand`/`ProductPriceHistory`. `Inventory.UpdateProductPricesCommandHandler` ya migrado a `IFiscalTaxRateRepository.GetEffectiveManyAsync`. Migración `UnifyTaxWithFiscalTaxRate` aplicada (drop `Tax.Rate`, tabla `FiscalTaxRates` con índice compuesto `(TaxId, EffectiveFrom)`). Tests de dominio actualizados.
- `EventLog` append-only (Security/IAM) — pendiente
- Primitivas fiscales en SharedKernel: cadena de hash (pendiente), fecha fiscal en hora Venezuela — **✅ hecho** (`IFiscalClock`/`FiscalClock`)
- Estructura de pruebas unitarias, cubriendo motor de cálculo existente — **✅ hecho para Domain** (`Tax`, `ExchangeRate`, `Product`); pendiente `Application.Tests`/`Infrastructure.Tests`
- Decisión de negocio: con qué imprenta digital autorizada integrar — **pendiente, no bloquea el código de F0 pero sí F2** (ver Memoria v2, sección 13.3)
- Diseñar el sistema para soportar los tres medios de emisión (máquina fiscal, forma libre/imprenta física, medio digital) como igualmente soportados desde el `DocumentoFiscal`/`FiscalDocument`, no uno "principal digital" con los otros como añadidos posteriores

Cronograma estimado (1 desarrollador, ~19 días hábiles): ver diagrama de Gantt generado en la sesión del
2026-08-01 (artifact, no versionado en el repo — regenerar si hace falta consultarlo de nuevo).

### F1 · Terceros y catálogo listo para facturar
- Módulo ThirdParty: `ThirdParty`, `Customer`, `Supplier`, `IdentificationType`, `TaxpayerType`, `Address`, `Contact`
- `Product` + código de alícuota de IVA + banderas (`IsService`/`HasLots`/`HasSerials`/`HasVariants`)
- `Stock` (existencia por producto/almacén) + `InventoryMovement` (kardex valorado)

### F2 · Núcleo de facturación (Billing) — el corazón
- Series y número de control (reusa el patrón UPDLOCK de `ProductCodeCounter`)
- `FiscalDocument` (aggregate raíz inmutable) + líneas + impuestos por documento
- Notas de crédito/débito como único mecanismo de corrección
- `PaymentMethod`/`Payment` con IGTF calculado sobre el pago (corrige D-3)
- Notas de entrega/guías, presupuestos, pedidos

### F3 · Cumplimiento fiscal — lo que lo hace homologable
- Registro de eventos fiscal + restricciones de BD que impiden editar/borrar documentos emitidos
- Libros de Compras y Ventas del IVA
- Extender el Outbox existente para remisión SENIAT (estado, acuse, reintentos idempotentes)
- Clave de consulta + API de solo lectura para el SENIAT
- Retenciones de IVA/ISLR con comprobante

### F4 · Operación integral
- Purchasing: factura de compra, recepción, retenciones emitidas, cuentas por pagar
- Treasury: caja, arqueo, cuentas por cobrar, bancos
- Catálogo avanzado: variantes, lotes (FEFO), seriales — adelantar si el primer cliente real es farmacia o calzado
- Reportes: vistas de libros fiscales, dashboards

### F5 · Integración, POS y homologación
- POS offline-first (reserva de numeración, sincronización idempotente)
- Conector de máquina fiscal, conector de imprenta digital
- Sincronización con el legacy (D-5, cuando se decida abordarla)
- Ficha técnica y trámite de homologación ante el SENIAT

---

## Referencia

El PDF fuente de esta redirección está en `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado.pdf` (v1.0,
24/07/2026). **Usar `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado2.pdf` (v2.0, 01/08/2026) como
referencia legal vigente** — corrige contra fuentes primarias/secundarias verificadas quince puntos de la v1.0
(plazos, citas de artículos, normas derogadas, tasas vigentes) y agrega la definición legal de "alteración de
registros fiscales" que condiciona el diseño de persistencia del núcleo fiscal. Su sección inicial "Fe de
erratas y correcciones v2.0" resume cada corrección con su fuente. Para el análisis histórico del legacy
(arquitectura por capas técnicas, ~110 entidades, vicios detectados, equivalencias detalladas de cada módulo)
ver `MIGRATION_PLAN.md`.
