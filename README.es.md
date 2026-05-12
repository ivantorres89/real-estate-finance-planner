# Real Estate Finance Planner

> [Read in English](README.md)

Una herramienta local completa para la **toma de decisiones financieras inmobiliarias**. Modela el ciclo completo: venta de propiedad, calculo de liquidez, compra de nueva propiedad, comparacion de hipotecas entre bancos (con evaluacion de bonificaciones) y recomendacion estrategica (amortizar mas rapido vs. mantener capital para inversion).

## Arquitectura

```
src/
  backend/                         .NET 10 Web API (Arquitectura Hexagonal)
    src/
      RealEstateFinancePlanner.Domain/       Dominio: entidades, value objects, servicios
      RealEstateFinancePlanner.Application/  Casos de uso, interfaces
      RealEstateFinancePlanner.Infrastructure/ Persistencia MongoDB, DI
      RealEstateFinancePlanner.Api/          Controladores, DTOs, mapping
    tests/
      RealEstateFinancePlanner.Tests.Unit/        48 tests unitarios
      RealEstateFinancePlanner.Tests.Integration/ Tests de integracion con Testcontainers
  frontend/                        Angular 21 + Angular Material
  docker/                          Datos semilla de MongoDB
docker-compose.yml                 Orquestacion de 3 servicios
```

### Arquitectura Hexagonal

- **Domain** - Sin dependencias externas. Contiene entidades, value objects (`Money`, `Percentage`), enums y servicios de dominio (calculadoras, evaluadores, motor de recomendaciones).
- **Application** - Orquesta los servicios de dominio. Define interfaces de repositorio. Sin dependencias de framework.
- **Infrastructure** - Persistencia MongoDB con serializador `Decimal128` para precision financiera. Registro de inyeccion de dependencias.
- **API** - Controladores ASP.NET Core, DTOs de request/response con validacion, Scalar/OpenAPI.

## Stack Tecnologico

| Capa       | Tecnologia                                  |
|------------|---------------------------------------------|
| Backend    | .NET 10, ASP.NET Core Web API               |
| Frontend   | Angular 21, Angular Material, SCSS          |
| Base datos | MongoDB 7 (Decimal128 para precision monetaria) |
| Testing    | xUnit, FluentAssertions, Testcontainers     |
| Contenedores | Docker, Docker Compose                    |

## Inicio Rapido

### Docker Compose (recomendado)

```bash
docker compose up --build
```

- **Frontend**: http://localhost:4200
- **API Backend**: http://localhost:5000
- **API Docs (Scalar)**: http://localhost:5000/scalar/v1
- **MongoDB**: localhost:27017

### Desarrollo Local

**Prerequisitos**: .NET 10 SDK, Node.js 24+, MongoDB corriendo en localhost:27017

```bash
# Backend
cd src/backend
dotnet build
dotnet run --project src/RealEstateFinancePlanner.Api

# Frontend (en terminal separado)
cd src/frontend
npm install
npx ng serve
```

El servidor de desarrollo de Angular redirige las peticiones `/api/*` a `http://localhost:5000`.

## Endpoints de la API

### Escenarios

| Metodo | Endpoint                       | Descripcion                 |
|--------|--------------------------------|-----------------------------|
| GET    | `/api/scenarios`               | Listar todos los escenarios |
| GET    | `/api/scenarios/{id}`          | Obtener escenario por ID    |
| POST   | `/api/scenarios`               | Crear nuevo escenario       |
| PUT    | `/api/scenarios/{id}`          | Actualizar escenario        |
| DELETE | `/api/scenarios/{id}`          | Eliminar escenario          |
| POST   | `/api/scenarios/{id}/analyze`  | Ejecutar analisis completo  |

### Calculos (independientes)

| Metodo | Endpoint                              | Descripcion                    |
|--------|---------------------------------------|--------------------------------|
| POST   | `/api/calculations/sale-liquidity`    | Calcular liquidez por venta    |
| POST   | `/api/calculations/purchase-costs`    | Calcular costes de compra      |
| POST   | `/api/calculations/debt-capacity`     | Calcular capacidad de deuda    |
| POST   | `/api/calculations/mortgage`          | Calcular hipoteca individual   |
| POST   | `/api/calculations/bank-offer`        | Evaluar oferta bancaria        |
| POST   | `/api/calculations/strategy-comparison` | Comparar estrategias         |

## Formulas Financieras

### Amortizacion Francesa

$$M = P \times \frac{r(1+r)^n}{(1+r)^n - 1}$$

Donde: P = principal, r = tasa de interes mensual (TIN/12/100), n = meses totales.

### ITP (Impuesto de Transmisiones Patrimoniales)

- Estandar: **6%** del precio oficial de compra A
- Reducido (familia numerosa / discapacidad): **3%** del precio oficial de compra A

### Doble contabilidad A/B

La venta y la compra se parten en dos contabilidades:

- **Lado A (oficial)** — lo que se firma en escritura. La cancelacion de la deuda hipotecaria del vendedor, los gastos de venta, ITP, notaria, gestoria, tasacion, agencia, otros gastos de compra y la entrada hipotecaria provienen siempre de A.
- **Lado B (efectivo)** — dinero en negro. No cancela hipotecas, no computa para el LTV de la hipoteca nueva, y no puede ingresarse en una cuenta sin justificacion. Util para: sobreprecio en efectivo al vendedor de la compra y reformas pagables en efectivo.

```
totalSaleDeductionsA = saleRelatedCosts + outstandingMortgageDebt + plusvaliaMunicipal + costesExtraordinarios
netSaleLiquidityA    = officialSalePriceA - totalSaleDeductionsA
netSaleLiquidityB    = unofficialSalePriceB
realAvailableCashA   = currentCashBalance + netSaleLiquidityA
realAvailableCashB   = netSaleLiquidityB

tasacionEfectiva     = appraisalValue > 0 ? appraisalValue : officialPurchasePriceA
mortgageBaseValue    = min(officialPurchasePriceA, tasacionEfectiva)
maxMortgage          = mortgageBaseValue × financeablePercentage/100
itp                  = officialPurchasePriceA × itpRate
entryPaymentA        = max(0, officialPurchasePriceA - maxMortgage)
entryPaymentB        = unofficialPurchasePriceB
totalCashNeededA     = entryPaymentA + itp + notaria + gestoria + tasacion + agencia + otros
totalCashNeededB     = entryPaymentB + renovationCostsB
remainingLiquidityA  = realAvailableCashA - totalCashNeededA
remainingLiquidityB  = realAvailableCashB - totalCashNeededB
idleCashB            = max(0, remainingLiquidityB)
```

La operacion es **viable** cuando `realAvailableCashA >= 0`, `remainingLiquidityA >= 0` y `remainingLiquidityB >= 0`. El calculador emite un `BalancingAdvice` multilinea que avisa de deficit en A o B, cash B ocioso por encima de 5.000 EUR y la hipoteca topada por tasacion.

### Comparacion de Estrategias

Se comparan dos estrategias:
- **Estrategia A (Amortizar mas rapido)**: Usar todo el efectivo disponible mas alla del colchon de liquidez como entrada, minimizando el principal de la hipoteca.
- **Estrategia B (Mantener Capital)**: Usar la financiacion estandar del 80%, invertir la liquidez restante.

El valor futuro del capital invertido se proyecta bajo tres escenarios (conservador / base / optimista) usando interes compuesto. Una **diferencia neta ponderada por riesgo** determina la recomendacion, usando pesos especificos por perfil:

| Perfil       | Conservador | Base | Optimista |
|-------------|------------|------|-----------|
| Conservador  | 60%        | 30%  | 10%       |
| Equilibrado  | 25%        | 50%  | 25%       |
| Agresivo     | 10%        | 30%  | 60%       |

## Testing

```bash
# Tests unitarios (48 tests)
cd src/backend
dotnet test tests/RealEstateFinancePlanner.Tests.Unit

# Tests de integracion (requiere Docker)
dotnet test tests/RealEstateFinancePlanner.Tests.Integration

# Tests E2E (requiere stack Docker corriendo)
cd tests/e2e
npm install
npx playwright install chromium
npx playwright test
```

## Decisiones del Proyecto

Ver [DECISIONS_AND_GAPS.es.md](DECISIONS_AND_GAPS.es.md) para documentacion de resoluciones de ambiguedades, trade-offs de modelado y notas de evolucion futura.

## Licencia

MIT
