# Usage Guide

> [Leer en Espanol](GUIA_DE_USO.md)

This guide walks you through the complete workflow of the Real Estate Finance Planner, from creating a scenario to interpreting the analysis results.

## Prerequisites

Make sure the application is running:

```bash
docker compose up --build
```

Then open **http://localhost:4200** in your browser.

---

## Step 1: Scenario List (Home Page)

When you first open the app, you will see the scenario list. If no scenarios exist yet, you will see an empty state with a **+ Nuevo Escenario** button.

![Empty scenario list](docs/screenshots/01-scenario-list-empty.png)

Click **+ Nuevo Escenario** to begin.

---

## Step 2: Create a New Scenario

You will be taken to the scenario editor. Start by typing a descriptive name in the top input field (e.g., "Mi Primer Analisis Inmobiliario").

![New scenario editor](docs/screenshots/02-new-scenario.png)

The editor is organized in **tabs**:
- **Venta y Liquidez** - Your current property sale details
- **Compra** - New property purchase details
- **Bank tabs** - One tab per bank offer (added dynamically)
- **Estrategia** - Investment strategy parameters
- **Resultados** - Appears after running an analysis

---

## Step 3: Fill Sale & Liquidity Data

In the **Venta y Liquidez** tab, fill in two sections:

### Venta de Propiedad Actual (Current Property Sale)
| Field | Description | Example |
|-------|-------------|--------|
| Precio de Venta | Expected sale price of your current property | 320,000 |
| Costes Asociados a la Venta | Agency fees, repairs, or other sale costs | 310,000 |
| Deuda Hipotecaria Pendiente | Remaining mortgage on the property you are selling | 95,000 |
| Saldo en Efectivo Actual | Your current savings / available cash | 45,000 |
| Plusvalia Municipal | Plusvalia tax (estimate from your municipality) | 1,800 |
| Costes Extraordinarios | Any other one-time costs | 0 |

### Capacidad de Endeudamiento (Debt Capacity)
| Field | Description | Example |
|-------|-------------|--------|
| Salario Neto Mensual | Your net monthly income | 3,200 |
| Cuotas de Prestamos Mensuales | Existing monthly loan obligations (car, personal) | 800 |
| Ratio Max. Endeudamiento (%) | Maximum percentage of income for debt servicing | 35 |

![Sale data filled](docs/screenshots/03-sale-data-filled.png)

---

## Step 4: Fill Purchase Data

Click the **Compra** tab. Fill in the details of the property you want to buy:

| Field | Description | Example |
|-------|-------------|--------|
| Precio de Compra | Agreed price for the new property | 280,000 |
| Precio de Escritura | Price registered in the deed (taxable base for ITP) | 280,000 |
| Financiable (%) | Percentage the bank will finance (typically 80%) | 80 |
| Edad del Comprador | Your age (affects mortgage term limits) | 35 |
| Costes de Notaria | Estimated notary fees | 900 |
| Costes Administrativos | Registration and administrative fees | 400 |
| Costes de Tasacion | Property appraisal fee | 350 |
| Costes de Agencia | Real estate agency fee (if any) | 0 |
| Otros Costes | Any additional purchase costs | 0 |

Checkboxes:
- **Aplicar ITP Reducido** - Check if you qualify for 3% ITP (large family / disability). Standard is 6%.
- **Vivienda Habitual** - Check if this will be your primary home.

![Purchase data filled](docs/screenshots/04-purchase-data-filled.png)

---

## Step 5: Add Bank Offers

Click the **+** tab button to add a bank. A new tab named "Banco 1" appears.

Fill in the bank details:

| Field | Description | Example |
|-------|-------------|--------|
| Nombre del Banco | Name of the bank | Banco Santander |
| TIN Base (%) | Base nominal interest rate offered | 2.90 |
| Plazos (anos) | Mortgage terms to evaluate (comma-separated) | 15, 20, 25, 30 |

![Bank offer](docs/screenshots/05-bank-offer-empty.png)

### Adding Bonuses

Banks often offer interest rate reductions (bonuses) in exchange for contracting additional products. Click the **+** button next to "Bonificaciones" to add one.

For each bonus, fill in:

| Field | Description | Example |
|-------|-------------|--------|
| Nombre de Bonificacion | Descriptive name | Domiciliacion de Nomina |
| Categoria | Type of product | Other |
| Reduccion TIN (%) | How much TIN decreases | 0.50 |
| Coste Mensual | Monthly cost of the product | 0 |
| Coste Anual | Annual cost | 0 |
| Coste Unico | Setup or one-time fee | 0 |
| Duracion (anos) | How long the bonus lasts (empty = full term) | - |

Check **Aceptado** if you plan to take this bonus. Check **Obligatorio** if the bank requires it.

![Bank with bonuses](docs/screenshots/06-bank-with-bonuses.png)

### Multiple Banks

You can add as many banks as you want by clicking **+** again. Each bank gets its own tab. This lets you compare offers side by side.

![Second bank added](docs/screenshots/07-second-bank.png)

To remove a bank, click the **X** on its tab.

---

## Step 6: Configure Strategy Parameters

Click the **Estrategia** tab to configure the investment comparison parameters:

| Field | Description | Example |
|-------|-------------|--------|
| Rentabilidad Conservadora (%) | Pessimistic annual investment return | 3 |
| Rentabilidad Base (%) | Expected annual investment return | 6 |
| Rentabilidad Optimista (%) | Best-case annual investment return | 9 |
| Horizonte de Analisis (anos) | Time period for the comparison | 20 |
| Colchon Minimo de Liquidez | Cash reserve you want to keep available | 15,000 |
| Capital Adicional a Preservar | Extra capital not available for investment | 0 |
| Perfil de Riesgo | Your risk tolerance (Conservador / Equilibrado / Agresivo) | Equilibrado |
| Usar Rentabilidad Neta | Whether the return rates are after tax | Yes |

![Strategy parameters](docs/screenshots/08-strategy-params.png)

---

## Step 7: Save the Scenario

Click the **Guardar** button in the top-right corner. A confirmation message "Escenario guardado" will appear. After saving, the **Ejecutar Analisis** button becomes available.

![Scenario saved](docs/screenshots/09-scenario-saved.png)

> You can save at any point and come back later to continue editing.

---

## Step 8: Run Analysis

Click **Ejecutar Analisis**. The system will calculate:
- Sale liquidity (net cash from selling your current property)
- Purchase costs (ITP, notary, entry payment, etc.)
- Mortgage options for each bank and each term
- Bonus cost/benefit evaluation
- Strategy comparison (amortize faster vs. maintain capital)

A "Resultados" tab will appear once the analysis completes.

![Analysis running](docs/screenshots/10-analysis-running.png)

---

## Step 9: Interpret the Results

Click the **Resultados** tab to see the full analysis. The results are divided into sections:

### Venta y Liquidez (Sale & Liquidity Summary)
Shows the net cash available after selling your current property and paying off all debts.

### Costes de Compra (Purchase Costs)
Shows ITP, maximum mortgage, entry payment, total cash needed, remaining liquidity, and whether the operation is viable.

### Bank Comparisons
For each bank, you will see:
- **Recommendation badge**: Recommended / NotWorthIt
- **TIN breakdown**: TIN Base, TIN Max. con Descuento, TIN Real Final
- **Mortgage options table**: For each term (15, 20, 25, 30 years) - monthly payment, total paid, total interest, debt ratio, free cash
- **Bonus evaluations**: Whether each bonus saves or costs you money, with a thumbs-up/down indicator

### Comparacion de Estrategias (Strategy Comparison)
Compares two approaches:
- **Amortizar Mas Rapido**: Use all available cash as entry payment to minimize the mortgage
- **Mantener Capital**: Use standard financing and invest the difference

Shows net differences under conservative, base, and optimistic scenarios, with a weighted recommendation.

![Results overview](docs/screenshots/11-results-overview.png)

---

## Step 10: Return to Scenario List

Click the **back arrow** (top-left) to return to the scenario list. Your scenario will appear with a green checkmark in the "Analizado" column.

![Scenario list with data](docs/screenshots/14-scenario-list-with-data.png)

From here you can:
- **Click a scenario** to reopen and edit it
- **Delete a scenario** using the trash icon
- **Create more scenarios** to compare different properties or conditions

---

## Tips

- **Save often**: You can save partial data and come back later.
- **Compare banks**: Add 2-3 bank offers to see which one gives you the best deal.
- **Try different strategies**: Change the risk profile and return rates to see how the recommendation changes.
- **Adjust the liquidity cushion**: A higher cushion is safer but reduces the capital available for either amortization or investment.
- **Check the debt ratio**: If it exceeds your maximum (typically 35%), the operation may not be viable. Consider a longer mortgage term or a cheaper property.

---

## API Documentation

The backend API is documented with Scalar at **http://localhost:5000/scalar/v1**. You can use it to integrate with other tools or test endpoints directly.
