# Decisiones, Gaps y Trade-offs

## Ambiguedades detectadas y resoluciones

### 1. Base imponible del ITP
**Ambiguedad:** El prompt dice "3% de la base imponible configurable/documentada" sin especificar cual es la base imponible.
**Resolucion:** La base imponible del ITP (Impuesto de Transmisiones Patrimoniales) es el precio de escritura (`deedPrice`), que es la practica estandar en Espana. Es configurable en el modelo.

### 2. Precio de compra vs. Precio de escritura
**Ambiguedad:** El prompt referencia tanto `purchasePrice` como `deedPrice`. Pueden diferir en transacciones reales.
**Resolucion:** Ambos se modelan por separado. El maximo hipotecario se calcula sobre el precio de escritura (referencia de tasacion bancaria). La entrada es `purchasePrice - maxMortgageAmount`. Si el precio de escritura es menor que el de compra, el comprador debe cubrir la diferencia con fondos propios.

### 3. Porcentaje financiable por defecto
**Ambiguedad:** El prompt menciona "hasta el 90% del precio de escritura" sin especificar un valor por defecto.
**Resolucion:** El valor por defecto es 80% (estandar para vivienda habitual en Espana). El usuario puede configurar hasta 100% por escenario bancario. El sistema valida que el ratio de endeudamiento resultante se mantenga dentro de limites.

### 4. Duracion de bonificaciones
**Ambiguedad:** "Duracion si la bonificacion no aplica durante toda la vida de la hipoteca" - no queda claro como calcular ahorros cuando una bonificacion expira a mitad de hipoteca.
**Resolucion:** Cada bonificacion tiene un `durationYears` opcional. Si es null, aplica durante todo el plazo. El coste de la bonificacion se calcula solo para su periodo activo. La reduccion de TIN tambien aplica solo durante ese periodo.
**Simplificacion aplicada:** Para el MVP, la reduccion de TIN de la bonificacion se aplica durante todo el plazo pero el coste solo se cobra por la duracion de la bonificacion. Esto sobreestima ligeramente el beneficio de bonificaciones limitadas en el tiempo. Una mejora futura podria dividir el cuadro de amortizacion.

### 5. Entrada minima
**Ambiguedad:** Que pasa si `purchasePrice <= maxMortgageAmount`?
**Resolucion:** La entrada es `Math.Max(0, purchasePrice - maxMortgageAmount)`. Si el banco financia el 100% o mas del precio de compra, la entrada es cero.

### 6. Plusvalia municipal
**Ambiguedad:** No siempre aplica y el calculo varia por municipio.
**Resolucion:** Se modela como un importe opcional simple. El usuario debe calcularlo o estimarlo externamente ya que los metodos de calculo municipal varian significativamente.

### 7. Modelado de rentabilidad de inversion
**Ambiguedad:** "Rentabilidad bruta vs neta esperada" y tres escenarios.
**Resolucion:** El usuario proporciona tres tasas de retorno (conservador, base, optimista). Si `useNetReturns` es true, se tratan como netas. En caso contrario, son brutas y el usuario debe introducir valores netos directamente. Una mejora futura podria anadir deduccion fiscal automatica.

### 8. Linea base de comparacion de estrategias
**Ambiguedad:** Que se compara exactamente en Estrategia A vs Estrategia B?
**Resolucion:**
- **Estrategia A (Amortizar mas rapido):** Usa toda la liquidez disponible menos el colchon minimo como entrada. Resulta en una hipoteca menor, cuota menor, menos intereses totales.
- **Estrategia B (Mantener capital):** Usa solo la entrada minima requerida (precio compra - hipoteca maxima). Preserva maximo capital para reinversion. Resulta en una hipoteca mayor, cuota mayor, mas intereses totales, pero mas capital para invertir.
- La diferencia de capital entre estrategias es el "delta invertible", que se proyecta con los tres escenarios de retorno.

### 9. Ponderacion de recomendacion
**Decision:** El perfil de riesgo afecta la ponderacion de los tres escenarios de inversion:
| Perfil de riesgo | Conservador | Base | Optimista |
|-------------------|-------------|------|-----------|
| Conservador       | 60%         | 30%  | 10%       |
| Equilibrado       | 25%         | 50%  | 25%       |
| Agresivo          | 10%         | 30%  | 60%       |

El beneficio neto ponderado debe ser positivo Y la liquidez restante debe cumplir el colchon minimo para que se recomiende la Estrategia B.

### 10. Umbrales de recomendacion bancaria
**Decision:** La recomendacion por banco se basa en el coste global real (principal + intereses + costes de bonificaciones):
- **Recomendado:** Menor coste global real entre todos los bancos Y ratio de endeudamiento dentro de limites
- **Cuestionable:** Dentro del 5% de la mejor opcion O ratio de endeudamiento acercandose al limite (>30%)
- **No merece la pena:** Mas del 5% por encima de la mejor opcion O cualquier violacion del ratio de endeudamiento

### 11. Framework objetivo
**Decision:** Se usa .NET 10 (`net10.0`) al ser la version LTS actual.

### 12. Modelo de documento MongoDB
**Decision:** Un unico documento por escenario (desnormalizado). Al ser una herramienta local de un solo usuario, no hay problemas de concurrencia. Un documento unico mantiene lecturas y escrituras simples y atomicas.

## Decisiones de modelado

### Formula de amortizacion francesa
Cuota mensual = `P * [r * (1 + r)^n] / [(1 + r)^n - 1]`
Donde:
- `P` = capital prestado
- `r` = tipo de interes mensual (TIN anual / 12 / 100)
- `n` = numero total de cuotas mensuales (anos * 12)

Todos los calculos usan `decimal` con precision suficiente. No se usa `double` para valores monetarios.

### Interes compuesto para proyeccion de inversion
Valor Futuro = `Capital * (1 + rentabilidadAnual / 100) ^ anos`

Se usa capitalizacion anual por simplicidad y claridad. La capitalizacion mensual seria mas precisa pero anade complejidad sin impacto significativo en una recomendacion estrategica.

## Trade-offs

| Decision | Trade-off |
|----------|-----------|
| Documento unico MongoDB | Simplicidad sobre flexibilidad de consultas. Aceptable para herramienta local monousuario. |
| Capitalizacion anual para inversiones | Simplicidad sobre precision. La diferencia es marginal para propositos de recomendacion. |
| Sin autenticacion | Segun lo solicitado. Necesitaria anadirse para cualquier escenario multiusuario. |
| Scalar API docs en todos los entornos | Conveniencia para desarrollo y pruebas. |
| TIN de bonificacion aplicado todo el plazo | Ligera sobreestimacion del beneficio para bonificaciones con duracion limitada. |

## Doble contabilidad A/B (venta y compra)

### Motivacion
El modelo de venta y compra ha pasado de un unico precio a una particion explicita entre dos contabilidades:
- **Contabilidad A:** lo que se firma en escritura (declarable, bancable, sujeto a ITP, visible para Hacienda y banco).
- **Contabilidad B:** lo que se entrega en efectivo "B", no declarado. No puede ingresarse en cuenta sin justificacion fiscal, no cancela hipoteca pendiente, no computa para el `loan-to-value` de la hipoteca nueva.

### Invariantes aplicadas
| Concepto | Contabilidad |
|---|---|
| Cancelacion deuda hipotecaria del vendedor | A |
| Gastos de venta (notaria, agencia, etc.) | A |
| Plusvalia municipal | A (base imponible = A) |
| Gastos extraordinarios | A |
| Cash actual ahorrado en cuenta | A |
| Liquidez B generada en la venta | solo B o reforma B |
| ITP, notaria, gestoria, tasacion, agencia, otros gastos de compra | A |
| Entrada hipotecaria de la compra | A |
| Sobreprecio negro al vendedor de la compra | B |
| Reformas | B |

### Restriccion de viabilidad
- **Venta viable:** `currentCashBalance + NetSaleLiquidityA >= 0`
- **Compra viable:** `RealAvailableCashA >= TotalCashNeededA` Y `RealAvailableCashB >= TotalCashNeededB`
- **Cuadre B optimo:** `IdleCashB` cercano a 0 (B sobrante minimizado, dado que no puede ingresarse limpio).

### Hipoteca con tasacion
`maxMortgage = financeablePercentage × min(officialPurchasePriceA, effectiveAppraisalValue)`. Si `AppraisalValue` es 0, se asume `tasacion = A` ("tasarla baja para que la operacion entre"). Si la tasacion queda por debajo de A, la hipoteca se topa por la tasacion y se emite aviso en `BalancingAdvice`.

### Motor de cuadre (`BalancingAdvice`)
El calculador emite recomendaciones automaticas en orden:
1. A de venta no cubre deuda + gastos oficiales.
2. A de compra no cubre entrada + ITP + gastos.
3. B de compra no cubre sobreprecio + reformas.
4. `IdleCashB > 5.000 EUR` (umbral fijo configurable como constante interna).
5. Tasacion tope la hipoteca por debajo de A.
6. Si nada falla: "Operacion cuadrada".

### Estrategia patrimonial sobre A
La comparacion patrimonial **amortizar mas vs mantener capital** opera unicamente sobre el lado A. El cash B no es capital reinvertible libre (no puede moverse a un broker sin justificacion fiscal), por lo que queda fuera de esa matematica. La matematica de hipoteca tambien opera sobre A, ya que el banco solo ve A.

### Migracion legacy en lectura
Los escenarios previos guardados con `SalePrice`, `PurchasePrice`, `DeedPrice` se migran on-read en el repositorio MongoDB:
- `SalePrice` -> `OfficialSalePriceA`, `UnofficialSalePriceB = 0`
- `PurchasePrice` -> `OfficialPurchasePriceA`, `UnofficialPurchasePriceB = 0`
- `DeedPrice` -> `AppraisalValue`
- `RenovationCostsB = 0`
- `LastResult` se descarta (esquema cambio) y obliga a re-ejecutar el analisis.

Se logra capturando campos antiguos con un `Dictionary<string, object>? LegacyExtraElements` mapeado como BSON `ExtraElements` mediante `BsonClassMap.MapExtraElementsProperty`. El Domain no referencia tipos MongoDB.

## Oportunidades de evolucion futura
1. **Exportacion de cuadro de amortizacion:** Generar tabla mes a mes (PDF/CSV)
2. **Soporte de tipo variable:** Modelado de Euribor + diferencial con escenarios de tipos
3. **Modelado de deducciones fiscales:** Incluir deducciones fiscales por hipoteca donde aplique
4. **Dashboard de comparacion multi-escenario:** Comparacion visual lado a lado
5. **Amortizacion dividida para bonificaciones temporales:** Calculo en dos fases
6. **Tracking de datos historicos:** Seguir como evolucionan los escenarios en el tiempo
7. **Importar/exportar escenarios:** Import/export JSON para backup y compartir
