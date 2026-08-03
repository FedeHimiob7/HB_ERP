# Último chat — continuar desde acá

> Cuando retomemos: decime "leé `ULTIMO_CHAT.md` y continuamos" y arranco desde este punto.

## Contexto para retomar

Este documento es el resumen de corte de la sesión del **2026-08-01**. Antes de tocar código, leer:

- `FISCAL_ROADMAP.md` — secciones "Acciones inmediatas" y "F0 · Cimientos regulados" (estado real actualizado).
- `CLAUDE.md` — sección "Testing" (estándar de estructura de tests establecido esta sesión).
- `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado2.pdf` — referencia legal vigente (v2, corrige la v1 contra fuentes primarias/secundarias verificadas; tiene una "Fe de erratas y correcciones" al inicio).

## Qué se hizo en la sesión anterior

1. Se estandarizó y armó la estructura de proyectos de test (.NET/xUnit):
   - `HB_ERP.SharedKernel.Tests` — proyecto separado, anidado dentro de `HB_ERP.SharedKernel/`.
   - `src/Modules/MasterData/Tests/Domain.Tests/` — cubre `Tax` y `ExchangeRate`.
   - `src/Modules/Inventory/Tests/Domain.Tests/` — cubre `Product` (creación, precios, impuestos, activación).
   - Convención: clases de test `sealed`, xUnit puro, `Tests/{Modulo}` por módulo.
2. Se hizo una investigación legal completa (3 agentes con Opus 5, WebSearch/WebFetch) sobre la Providencia SNAT/2024/000121, las normas de emisión de facturas (PA 0071/0141/000102) y la normativa tributaria sustantiva (IVA/IGTF/ISLR/COT), verificando el contenido de `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado.pdf` contra fuentes reales.
3. Se reescribió el documento como `docs/Memoria_Descriptiva_Sistema_Facturacion_Homologado2.pdf`, con 15 correcciones documentadas (plazos, normas derogadas, tasas vigentes, definición legal de "alteración de registros fiscales", soporte parejo de los 3 canales de emisión).
4. Se actualizaron `CLAUDE.md` y `FISCAL_ROADMAP.md` para reflejar todo lo anterior.
5. Se limpiaron carpetas huérfanas (`MasterData.Application/`, `MasterData.Domain/` en la raíz, residuos de antes de la reestructuración a `src/Modules/`).
6. Se armó un cronograma (Gantt) estimado para F0 — 1 desarrollador, ~19 días hábiles (artifact de esa sesión, no versionado en el repo).

## Qué sigue — orden propuesto para F0

1. **Diseñar el parámetro fiscal genérico versionado por fecha** — generaliza el patrón inmutable ya usado por `ExchangeRate` (un registro nuevo por cambio, nunca se edita el anterior), para reutilizarlo en `TaxRate`, Unidad Tributaria y los tramos de IGTF, en vez de reinventar el patrón cuatro veces.
2. **`Tax` → `Tax` + `TaxRate` con vigencia** (depende del punto 1).
3. **`EventLog` append-only** (Security/IAM) — bitácora de eventos de negocio, sin ningún mecanismo de ocultación.
4. **`Company` + `Branch` + `WorkStation`** + discriminador de tenant en los tres `DbContext` — la tarea más grande de F0.
5. **Tests** para todo lo anterior + tests de dominio de `Identity` (hoy sin ninguno).

Pendiente en paralelo (no bloquea código): decisión de negocio sobre con qué **imprenta digital autorizada** integrar — necesaria antes de F2, no antes de F0.

## Nota de diseño a no perder de vista

El art. 13, Parágrafo Único de la Providencia 121 define "alteración de registros fiscales" incluyendo la
**ocultación** de un registro, no solo su borrado/edición. Aplica solo a **registros fiscales** (futuros
`FiscalDocument`, registro de eventos, libros de IVA) — el catálogo (`Product`, `Tax`, etc.) puede seguir
usando el patrón `IsActive` ya existente en el código sin conflicto legal.
