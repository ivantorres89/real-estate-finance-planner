Ahora entra en modo cierre de proyecto y validación E2E completa para `real-estate-finance-planner` / `RealEstateFinancePlanner`.

## Objetivo
Quiero que, una vez generado todo el proyecto, completes la fase final de validación real end-to-end en local.

No quiero una validación teórica. Quiero que actúes como si fueras a dejar el proyecto listo para que cualquier arquitecto senior lo clone, lo levante y lo pruebe sin sorpresas.

## Regla principal
No me digas solo cómo debería ejecutarse.
Quiero que prepares TODO lo necesario para que el stack completo pueda levantarse y validarse end-to-end de forma consistente.

## Qué debes hacer ahora
Trabaja sobre el proyecto ya generado, sin reiniciar arquitectura ni cambiar nombres.

### 1. Validación de consistencia
Primero:
- resume brevemente el estado actual del proyecto
- detecta qué piezas faltan para un flujo E2E real
- si faltan tests E2E o configuración para ellos, añádelos
- si faltan scripts de arranque o espera entre servicios, añádelos
- si faltan health checks, añádelos
- si faltan seeds mínimos, añádelos
- si faltan utilidades para esperar a backend/frontend/mongodb antes de lanzar pruebas, añádelas

### 2. E2E obligatorio
Quiero validación end-to-end real del sistema completo.

Debe cubrir como mínimo:
- arranque completo con Docker Compose
- backend operativo
- frontend operativo
- MongoDB operativo
- comunicación frontend-backend correcta
- persistencia real en MongoDB
- carga de datos iniciales o seed si aplica
- flujo principal usable desde UI

### 3. Tecnología E2E preferida
Usa **Playwright** como herramienta preferida para tests E2E, salvo que encuentres una razón técnica fuerte y la documentes explícitamente.

Si usas otra herramienta, debes justificarlo en:
- `DECISIONS_AND_GAPS.md`
- `DECISIONS_AND_GAPS.es.md`

## 4. Casos E2E mínimos obligatorios
Quiero al menos estos escenarios automáticos con Playwright:

### Escenario 1: carga inicial
- la aplicación abre correctamente
- la UI principal carga sin errores
- el backend responde
- se puede acceder al flujo principal

### Escenario 2: operación base
- introducir datos de venta
- introducir saldo
- introducir deuda pendiente
- introducir nómina
- calcular liquidez neta tras venta
- calcular capacidad de endeudamiento
- verificar que aparecen resultados correctos en pantalla

### Escenario 3: simulación de compra
- introducir precio de compraventa
- introducir precio de escritura
- introducir porcentaje financiable
- introducir gastos
- activar ITP 3%
- comprobar cálculo de entrada, gastos, liquidez remanente y viabilidad

### Escenario 4: comparativa bancaria
- crear al menos una oferta bancaria
- añadir varias bonificaciones
- activar y desactivar bonificaciones
- comprobar que cambia el TIN real
- comprobar que cambian cuota, coste total e intereses
- verificar recomendación por banco

### Escenario 5: estrategia óptima
- introducir rentabilidad esperada de inversión alternativa
- elegir horizonte temporal
- elegir perfil de riesgo
- lanzar comparación entre amortizar más rápido vs mantener capital libre
- verificar que aparece recomendación automática y explicación

### Escenario 6: persistencia
- guardar un escenario
- recargar o volver a consultar
- comprobar que persiste y se recupera correctamente desde MongoDB

## 5. Health checks y robustez
Añade si no existe:
- health endpoint en backend
- readiness razonable para servicios
- healthcheck en Docker Compose cuando tenga sentido
- waits o polling controlados entre servicios si es necesario
- lógica para que los tests E2E no arranquen antes de que el stack esté realmente listo

No quiero un docker-compose que “a veces funciona”.

## 6. Scripts y automatización
Añade scripts claros para ejecutar:

### Backend
- build
- unit tests
- integration tests

### Frontend
- build
- unit tests

### E2E
- run e2e
- run e2e headless
- run e2e against dockerized stack

### Full validation
Quiero un comando o script único que, en la medida de lo razonable:
- levante el stack
- espere a que esté listo
- ejecute tests E2E
- reporte resultado
- deje claro cómo apagar el stack después

## 7. Compatibilidad con mi entorno
Mi entorno es:
- Windows 11
- WSL disponible
- Docker Desktop / Docker Engine local
- ejecución principal en localhost

Quiero soporte práctico al menos para:
- shell script para WSL/Linux, por ejemplo:
  - `scripts/run-e2e.sh`
  - `scripts/full-validate.sh`
- PowerShell script para Windows, por ejemplo:
  - `scripts/run-e2e.ps1`
  - `scripts/full-validate.ps1`

Si necesitas utilidades auxiliares, añádelas.

## 8. Seed y datos mínimos
Si la aplicación necesita datos mínimos para que los tests E2E sean fiables:
- añade seed inicial razonable
- o añade mecanismos de creación controlada de datos en tests
- pero no dependas de datos manuales cargados a mano

Los tests E2E deben ser reproducibles.

## 9. Corrección iterativa
Si detectas huecos, incoherencias o fallos:
- no me los dejes como observación pasiva
- corrígelos
- actualiza los archivos afectados
- documenta el cambio

Quiero que trabajes en modo:
detecta -> corrige -> valida -> continúa

## 10. Documentación obligatoria
Actualiza:
- `README.md`
- `README.es.md`
- `DECISIONS_AND_GAPS.md`
- `DECISIONS_AND_GAPS.es.md`

Y añade si hace falta una sección específica de:
- local full-stack startup
- Playwright E2E execution
- Dockerized E2E validation
- troubleshooting
- common failure points
- what was added specifically to support E2E validation
- WSL/Linux execution
- PowerShell execution

## 11. Resultado esperado
Quiero que el proyecto quede con:
- stack local arrancable
- tests unitarios
- tests de integración
- tests E2E con Playwright
- scripts de ejecución para WSL/Linux y PowerShell
- documentación clara
- validación de flujo principal completa

## 12. Formato de tu respuesta
Quiero que procedas así:

1. Estado actual del proyecto
2. Gaps para E2E real
3. Estrategia técnica elegida para E2E
4. Archivos nuevos o modificados
5. Código completo de los archivos necesarios
6. Scripts de ejecución
7. Pasos exactos de validación
8. Resumen de correcciones realizadas
9. Qué comprobarías al final para darlo por listo

## 13. Restricciones
- No reinicies la arquitectura
- No cambies nombres del proyecto
- No sustituyas código real por pseudocódigo
- No simplifiques por longitud
- No me digas “esto dependerá del entorno” sin proponer una solución
- No dejes el E2E como algo manual
- No des por terminado el proyecto sin cubrir el flujo principal completo
- No omitas los scripts para WSL/Linux y PowerShell

## 14. Recordatorio de consistencia
Mantén:
- repo: `real-estate-finance-planner`
- solution: `RealEstateFinancePlanner`
- código en inglés
- comunicación conmigo en español
- documentación bilingüe
- backend como fuente de verdad de cálculos
- MongoDB
- Angular
- Docker Compose
- Playwright como opción preferida de E2E
- precisión por encima de rapidez

Continúa ahora con la fase de validación E2E completa.