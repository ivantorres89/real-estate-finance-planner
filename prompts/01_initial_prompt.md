Actúa como un Principal .NET Solutions Architect + Staff Frontend Engineer + QA Lead + DevOps Engineer + Financial Modeling Engineer.

Quiero que diseñes e implementes una aplicación local, precisa y mantenible para calcular mi capacidad real de endeudamiento, analizar operaciones inmobiliarias y recomendar la estrategia patrimonial óptima entre amortizar deuda más rápido o mantener más capital disponible para reinversión.

No quiero una demo superficial ni una app “bonita”. Quiero una herramienta seria de decisión patrimonial. La prioridad absoluta es la CORRECCIÓN FUNCIONAL, la TRAZABILIDAD de los cálculos, la UTILIDAD REAL para tomar decisiones financieras y la CALIDAD DEL CÓDIGO.

## Nombre del proyecto
Quiero que el proyecto use estos nombres de forma consistente:

- Repository name: `real-estate-finance-planner`
- .NET solution name: `RealEstateFinancePlanner`

Usa esos nombres de forma coherente en:
- estructura del proyecto
- solution
- documentación
- Docker
- ejemplos
- títulos del README
- nombres visibles del proyecto

## Idioma y documentación
- La comunicación conmigo debe ser en español.
- Este prompt está en español y debes responderme en español.
- TODO el código fuente debe estar escrito en inglés:
  - nombres de clases
  - métodos
  - variables
  - tests
  - comentarios de código si los hubiera
  - nombres de archivos
  - nombres de carpetas
  - endpoints
  - DTOs
  - modelos
- La documentación del proyecto debe estar en inglés y en español.
- Debes generar dos READMEs:
  - `README.md` en inglés (este será el principal visible en GitHub)
  - `README.es.md` en español
- Si generas documentación adicional, también debe existir versión inglesa y española cuando tenga sentido.

## Modo de trabajo y autonomía
No quiero que esperes mi autosupervisión constante.

Quiero que trabajes como un arquitecto senior autónomo que:
- se revisa a sí mismo
- detecta sus propios gaps
- toma decisiones razonables sin bloquearse
- ante una duda, propone la mejor solución y la ejecuta
- no se queda parado esperando aclaraciones innecesarias
- prioriza precisión y calidad por encima de velocidad

Actúa como si NO tuvieras límite de premium requests o coste de razonamiento. No recortes profundidad ni calidad “por ahorrar”. Quiero la mejor solución razonable, no una versión simplificada por economía de tokens o esfuerzo.

## Obligación de autovalidación
No quiero que generes código y ya.
Quiero que:
1. Pienses la arquitectura primero
2. Detectes ambigüedades
3. Las resuelvas con criterio profesional
4. Documentes esas dudas y cómo las resolviste
5. Implementes
6. Revises tu propia implementación
7. Verifiques consistencia técnica
8. Incluyas pruebas útiles
9. Compruebes que todo arranca en local

Debes generar además un documento específico tipo:
- `DECISIONS_AND_GAPS.md` (inglés)
- `DECISIONS_AND_GAPS.es.md` (español)

Ese documento debe incluir:
- dudas detectadas
- gaps funcionales o ambigüedades del problema
- decisiones de modelado tomadas
- trade-offs
- supuestos aplicados
- por qué elegiste esa solución
- qué dejarías preparado para evolución futura

## Contexto del usuario
Soy Platform / Solutions Architect especializado en Azure y .NET, con más de 13 años de experiencia en backend y cloud. No quiero depender de hojas Excel ni de simulaciones opacas de bancos. Mi máquina es Windows 11 con WSL. Todo debe ejecutarse en local vía Docker Compose. No quiero despliegue cloud ni autenticación ni multiusuario.

## Objetivo de negocio
Necesito una aplicación que modele una operación inmobiliaria de sustitución de vivienda habitual:

- Tengo una vivienda actual (activo) que quiero vender.
- En función del precio final de venta y de todos los gastos reales asociados a la venta, quiero calcular la liquidez neta real disponible tras cancelar deuda pendiente y pagar gastos.
- Con esa liquidez neta y mi situación laboral actual, quiero simular la compra de una nueva vivienda.
- Quiero comparar ofertas hipotecarias de distintos bancos, cada uno con TIN fijo.
- Quiero analizar bonificaciones bancarias y decidir cuáles compensan realmente y cuáles no.
- Quiero ver mi capacidad de endeudamiento respetando ratio de esfuerzo / ratio de endeudamiento.
- Quiero ver el coste real total de cada operación hipotecaria a distintos plazos.
- Quiero contemplar escenarios donde la hipoteca se calcule sobre el precio de escritura real y el banco financie hasta el 90% del precio de escritura.
- Quiero contemplar los gastos reales de compra: notaría, gestoría, tasación, inmobiliaria e ITP reducido del 3%.
- Quiero una recomendación automática sobre qué estrategia es mejor:
  1. meter más entrada y amortizar más rápido
  2. mantener más capital disponible para reinvertir

## Stack obligatorio

### Backend
- .NET 8 o superior
- ASP.NET Core Web API
- Arquitectura hexagonal
- 3 capas explícitas:
  - API
  - BLL / Application / Domain
  - Persistencia / Infrastructure
- SOLID
- SRP estricto
- Abstracciones limpias
- Value Objects donde tenga sentido
- Validaciones robustas
- Manejo claro de errores
- DTOs bien definidos
- MongoDB como persistencia
- Swagger/OpenAPI habilitado

### Frontend
- Angular (última versión estable)
- UI simple, sobria y funcional
- Tipado fuerte
- Formularios reactivos
- Componentes desacoplados
- Servicios bien separados
- Validación visual clara

### Base de datos
- MongoDB
- Persistir:
  - escenarios
  - bancos
  - bonificaciones
  - simulaciones
  - resultados
  - configuraciones por defecto

## Contenedores / ejecución local
Todo debe arrancar en localhost con Docker Compose.

### Obligatorio:
- `docker-compose.yml` para:
  - frontend
  - backend
  - mongodb
- Cada parte con su Dockerfile
- Arranque automático y simple
- Instrucciones exactas para levantar todo

## Testing obligatorio

### Backend
- Tests unitarios para toda lógica importante:
  - ratio de endeudamiento
  - cálculo de liquidez neta tras venta
  - cálculo de gastos de compra
  - cálculo del ITP al 3%
  - cálculo del importe hipotecable al 90% del precio de escritura
  - cálculo de cuota hipotecaria fija
  - cálculo de intereses totales
  - evaluación de bonificaciones
  - comparación de estrategias patrimoniales
  - recomendación automática
  - validaciones de entrada
- Tests de integración contra MongoDB
- Se pueden usar Testcontainers

### Frontend
- Tests unitarios de componentes y servicios críticos

### Requisito de validación
Cada vez que plantees que el proyecto está “terminado”, debes asumir que:
- el backend compila
- los tests backend pasan
- el frontend compila
- el stack completo arranca con Docker Compose

No quiero una entrega a medias.

## Reglas financieras generales
- Todas las cantidades monetarias deben modelarse con `decimal` en backend.
- El backend debe ser la única fuente de verdad de los cálculos.
- El frontend solo presenta y permite introducir / seleccionar datos.
- No uses `double` para dinero.
- No simplifiques fórmulas financieras por comodidad.

## Reglas funcionales de cálculo

### 1. Venta de vivienda actual
Inputs:
- sale price
- sale-related costs
- current outstanding mortgage debt
- current available cash balance
- optional municipal capital gains tax
- optional extraordinary costs

Cálculos:
- net sale liquidity =
  sale price
  - sale-related costs
  - outstanding mortgage debt
  - municipal capital gains tax if applicable
  - extraordinary costs if applicable

- real available cash after sale =
  current available cash balance
  + net sale liquidity

### 2. Compra de nueva vivienda
Inputs:
- purchase price
- real deed price
- financeable percentage over deed price
- notary costs
- administrative/gestoría costs
- appraisal costs
- real estate agency costs
- other costs
- apply reduced ITP 3% yes/no
- main residence yes/no
- buyer age

Cálculos:
- ITP = 3% of the configurable/documented taxable base when applicable
- maximum mortgage amount = financeable percentage * deed price
- non-financed entry payment = purchase price - maximum mortgage amount (if positive)
- total cash needed =
  non-financed entry payment
  + ITP
  + notary costs
  + administrative costs
  + appraisal costs
  + agency costs
  + other costs

- remaining liquidity after purchase =
  real available cash after sale
  - total cash needed

### 3. Endeudamiento
Inputs:
- monthly net salary
- monthly payment of remaining outstanding loans
- maximum debt ratio configurable (default 35%)

Cálculos:
- maximum monthly payment capacity =
  monthly net salary * debt ratio
  - monthly payment of outstanding loans

### 4. Hipoteca fija
Asume SIEMPRE hipoteca a TIN fijo.

Inputs:
- loan principal
- fixed TIN
- term in years

Cálculos:
- fixed monthly payment
- total amount paid
- total interest paid
- resulting debt ratio
- remaining monthly free cash

Usa correctamente la fórmula de amortización francesa.

## Bonificaciones bancarias
No quiero hipoteca variable.
Sí quiero modelar bonificaciones que reduzcan el TIN fijo.

### Por banco
- bank name
- base fixed TIN
- available bonuses

### Cada bonificación debe tener:
- name
- category (home insurance, life insurance, alarm/camera package, payroll, card, pension plan, others)
- TIN reduction in percentage points
- monthly cost
- yearly cost
- one-time cost if applicable
- mandatory or optional
- accepted or rejected by the user
- duration if the bonus does not apply during the full life of the mortgage
- optional notes

### Cálculos obligatorios por banco
- base TIN
- maximum theoretical discounted TIN
- final real TIN based on accepted bonuses
- monthly payment with final real TIN
- total paid at the end
- total interests
- total accumulated cost of accepted bonuses
- real global cost = principal + interests + accepted bonus costs
- net gain or loss per bonus

### Regla crítica
Una bonificación no debe evaluarse solo por la bajada de cuota mensual.
Debe evaluarse por coste total real acumulado.

La app debe decir claramente:
- qué bonificaciones compensan
- cuáles no compensan
- cuál es el escenario real óptimo
- si una bonificación comercial empeora la operación aunque baje el TIN

## Estrategia patrimonial avanzada
Quiero que la app no se limite a calcular cuotas. Quiero que recomiende automáticamente la estrategia óptima entre:

### Estrategia A: amortizar más rápido / menor apalancamiento
- mayor entrada
- menor capital financiado
- menor cuota
- menor coste total de intereses
- menor riesgo
- menos capital líquido para reinvertir

### Estrategia B: mantener más capital libre / mayor apalancamiento
- menor entrada
- mayor capital financiado
- mayor cuota
- mayor coste total de intereses
- más capital disponible para invertir

### Inputs adicionales para esta comparación
- expected annual return of alternative investment
- analysis horizon (for example 5, 10, 15, 20, 30 years)
- minimum desired liquidity cushion after purchase
- optional risk preference:
  - conservative
  - balanced
  - aggressive

### Rentabilidad de reinversión
No trates la rentabilidad alternativa como una cifra mágica única.

Permite modelar al menos:
- conservative scenario
- base/central scenario
- optimistic scenario

Y, si es razonable, permite introducir:
- gross expected return
- net expected return after taxes/fees

La recomendación automática debe ser sensible a esos escenarios.

### Reglas de recomendación
La aplicación debe comparar:
- financial cost of borrowing more money
- potential return of the capital kept available for reinvestment
- net difference between:
  1. saving interests by reducing debt faster
  2. preserving capital and investing it

### Resultado esperado
La app debe recomendar automáticamente una solución óptima y justificarla.

### Muy importante
La recomendación automática debe tener en cuenta:
- total mortgage cost
- accepted bonus costs
- remaining liquidity after purchase
- debt ratio
- expected reinvestment return
- time horizon
- risk profile
- minimum liquidity cushion required

No quiero una recomendación simplista basada solo en la cuota o solo en los intereses.

## UI requerida

### Pantalla principal
Tabs o secciones claras.

### Tab 1: Base data
Inputs:
- sale price
- sale-related costs
- current available cash balance
- current outstanding mortgage debt
- monthly net salary
- monthly payment of remaining loans
- debt ratio (%)

Mostrar:
- net sale liquidity
- real available cash after sale
- maximum monthly payment capacity

### Tab 2: Purchase
Inputs:
- purchase price
- real deed price
- financeable percentage
- notary costs
- administrative costs
- appraisal costs
- agency costs
- other costs
- apply 3% ITP
- main residence yes/no
- age

Mostrar:
- calculated ITP
- maximum mortgage amount
- required entry payment
- total cash needed
- remaining liquidity
- viability of operation

### Tab 3+: Banks
Una tab por banco.

Inputs:
- bank name
- base TIN
- available bonuses with selectable activation/deactivation
- mortgage terms (15, 20, 25, 30 years, etc.)

Mostrar:
- base TIN
- final real TIN
- monthly payment
- total interests
- total bonus cost
- global real cost
- recommendation per bank:
  - recommended
  - questionable
  - not worth it

### Tab adicional: Optimal strategy
Inputs:
- additional capital to preserve or contribute
- expected alternative investment return
- time horizon
- minimum desired liquidity cushion
- risk profile
- investment return scenario (conservative / base / optimistic)
- optional gross vs net expected return

Mostrar:
- comparison between faster amortization vs keeping capital available
- financial cost of each strategy
- future estimated value of reinvested capital
- net difference
- automatic final recommendation
- clear explanatory text

## Persistencia
Quiero guardar escenarios con nombre.

Cada escenario debe persistir:
- sale data
- purchase data
- banks
- selected bonuses
- calculation results
- optimal strategy result
- timestamps

## Diseño técnico esperado
Quiero una estructura aproximada:

/real-estate-finance-planner
  /src
    /backend
      /Api
      /Application
      /Domain
      /Infrastructure
      /Tests.Unit
      /Tests.Integration
    /frontend
    /docker

O equivalente si propones una mejor, pero justifícala.

## Calidad y mantenibilidad
- clean code
- clear naming
- business logic outside controllers and Angular components
- frontend and backend validation
- configuration via appsettings / environment variables
- excellent documentation
- no useless overengineering

## Entregables
Quiero que me entregues:

1. arquitectura breve pero seria
2. estructura de carpetas
3. modelo de dominio
4. fórmulas y reglas de negocio explicadas
5. API REST documentada
6. código completo backend
7. código completo frontend
8. configuración MongoDB
9. tests unitarios e integración
10. Dockerfiles
11. docker-compose.yml
12. README.md en inglés
13. README.es.md en español
14. DECISIONS_AND_GAPS.md en inglés
15. DECISIONS_AND_GAPS.es.md en español
16. seed opcional de datos de ejemplo
17. comandos para ejecutar tests
18. verificación final de que arranca en localhost

## Modo de trabajo
Antes de escribir código:
1. define fórmulas y decisiones de modelado
2. detecta ambigüedades y resuélvelas con criterio profesional
3. documenta esas ambigüedades y resoluciones
4. propón arquitectura
5. propón modelo de dominio
6. propón endpoints
7. propón estructura frontend
8. propón estrategia de recomendación automática
9. y después genera el proyecto completo

## Restricciones
- no pseudocódigo
- no fragmentos incompletos
- quiero archivos completos
- no cloud
- no autenticación
- no multiusuario
- localhost únicamente
- precisión sobre velocidad
- calidad sobre rapidez
- claridad sobre complejidad innecesaria
- no esperes validación mía paso a paso para cada decisión menor
- ante duda razonable, decide y ejecuta la mejor solución y documéntala

## Anti-patrones prohibidos
- lógica de negocio en controladores
- lógica financiera en Angular components
- repositorios vacíos o sin sentido
- servicios dios
- DTOs mezclados con dominio
- tests decorativos
- cálculos con double en dinero
- recomendaciones simplistas sin justificar
- generar solo una base de proyecto sin implementación real

Empieza proponiendo:
1. architecture
2. domain model
3. formulas
4. endpoints
5. frontend structure
6. automatic recommendation strategy
7. identified ambiguities and how you will resolve them

Y después genera el proyecto completo.