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

When you first open the app, you will see the scenario list. If no scenarios exist yet, you will see an empty state with a **+ New Scenario** button.

![Empty scenario list](docs/screenshots/01-scenario-list-empty.png)

Click **+ New Scenario** to begin.

---

## Step 2: Create a New Scenario

You will be taken to the scenario editor. Start by typing a descriptive name in the top input field (e.g., "My First Property Analysis").

![New scenario editor](docs/screenshots/02-new-scenario.png)

The editor is organized in **tabs**:
- **Sale & Liquidity** - Your current property sale details
- **Purchase** - New property purchase details
- **Bank tabs** - One tab per bank offer (added dynamically)
- **Strategy** - Investment strategy parameters
- **Results** - Appears after running an analysis

---

## Step 3: Fill Sale & Liquidity Data

In the **Sale & Liquidity** tab, fill in two sections:

### Current Property Sale
| Field | Description | Example |
|-------|-------------|---------|
| Sale Price | Expected sale price of your current property | 320,000 |
| Sale-Related Costs | Agency fees, repairs, or other sale costs | 310,000 |
| Outstanding Mortgage Debt | Remaining mortgage on the property you are selling | 95,000 |
| Current Cash Balance | Your current savings / available cash | 45,000 |
| Municipal Capital Gains Tax | Plusvalia tax (estimate from your municipality) | 1,800 |
| Extraordinary Costs | Any other one-time costs | 0 |

### Debt Capacity
| Field | Description | Example |
|-------|-------------|---------|
| Monthly Net Salary | Your net monthly income | 3,200 |
| Monthly Loan Payments | Existing monthly loan obligations (car, personal) | 800 |
| Max Debt Ratio (%) | Maximum percentage of income for debt servicing | 35 |

![Sale data filled](docs/screenshots/03-sale-data-filled.png)

---

## Step 4: Fill Purchase Data

Click the **Purchase** tab. Fill in the details of the property you want to buy:

| Field | Description | Example |
|-------|-------------|---------|
| Purchase Price | Agreed price for the new property | 280,000 |
| Deed Price | Price registered in the deed (taxable base for ITP) | 280,000 |
| Financeable (%) | Percentage the bank will finance (typically 80%) | 80 |
| Buyer Age | Your age (affects mortgage term limits) | 35 |
| Notary Costs | Estimated notary fees | 900 |
| Administrative Costs | Registration and administrative fees | 400 |
| Appraisal Costs | Property appraisal fee | 350 |
| Agency Costs | Real estate agency fee (if any) | 0 |
| Other Costs | Any additional purchase costs | 0 |

Checkboxes:
- **Apply Reduced ITP** - Check if you qualify for 3% ITP (large family / disability). Standard is 6%.
- **Main Residence** - Check if this will be your primary home.

![Purchase data filled](docs/screenshots/04-purchase-data-filled.png)

---

## Step 5: Add Bank Offers

Click the **+** tab button to add a bank. A new tab named "Bank 1" appears.

Fill in the bank details:

| Field | Description | Example |
|-------|-------------|---------|
| Bank Name | Name of the bank | Banco Santander |
| Base TIN (%) | Base nominal interest rate offered | 2.90 |
| Terms (years) | Mortgage terms to evaluate (comma-separated) | 15, 20, 25, 30 |

![Bank offer](docs/screenshots/05-bank-offer-empty.png)

### Adding Bonuses

Banks often offer interest rate reductions (bonuses) in exchange for contracting additional products. Click the **+** button next to "Bonuses" to add one.

For each bonus, fill in:

| Field | Description | Example |
|-------|-------------|---------|
| Bonus Name | Descriptive name | Payroll Deposit |
| Category | Type of product | Other |
| TIN Reduction (%) | How much TIN decreases | 0.50 |
| Monthly Cost | Monthly cost of the product | 0 |
| Yearly Cost | Annual cost | 0 |
| One-Time Cost | Setup or one-time fee | 0 |
| Duration (years) | How long the bonus lasts (empty = full term) | - |

Check **Accepted** if you plan to take this bonus. Check **Mandatory** if the bank requires it.

![Bank with bonuses](docs/screenshots/06-bank-with-bonuses.png)

### Multiple Banks

You can add as many banks as you want by clicking **+** again. Each bank gets its own tab. This lets you compare offers side by side.

![Second bank added](docs/screenshots/07-second-bank.png)

To remove a bank, click the **X** on its tab.

---

## Step 6: Configure Strategy Parameters

Click the **Strategy** tab to configure the investment comparison parameters:

| Field | Description | Example |
|-------|-------------|---------|
| Conservative Return (%) | Pessimistic annual investment return | 3 |
| Base Return (%) | Expected annual investment return | 6 |
| Optimistic Return (%) | Best-case annual investment return | 9 |
| Analysis Horizon (years) | Time period for the comparison | 20 |
| Minimum Liquidity Cushion | Cash reserve you want to keep available | 15,000 |
| Additional Capital to Preserve | Extra capital not available for investment | 0 |
| Risk Profile | Your risk tolerance (Conservative / Balanced / Aggressive) | Balanced |
| Use Net Returns | Whether the return rates are after tax | Yes |

![Strategy parameters](docs/screenshots/08-strategy-params.png)

---

## Step 7: Save the Scenario

Click the **Save** button in the top-right corner. A confirmation message "Scenario saved" will appear. After saving, the **Run Analysis** button becomes available.

![Scenario saved](docs/screenshots/09-scenario-saved.png)

> You can save at any point and come back later to continue editing.

---

## Step 8: Run Analysis

Click **Run Analysis**. The system will calculate:
- Sale liquidity (net cash from selling your current property)
- Purchase costs (ITP, notary, entry payment, etc.)
- Mortgage options for each bank and each term
- Bonus cost/benefit evaluation
- Strategy comparison (amortize faster vs. maintain capital)

A "Results" tab will appear once the analysis completes.

![Analysis running](docs/screenshots/10-analysis-running.png)

---

## Step 9: Interpret the Results

Click the **Results** tab to see the full analysis. The results are divided into sections:

### Sale & Liquidity Summary
Shows the net cash available after selling your current property and paying off all debts.

### Purchase Costs
Shows ITP, maximum mortgage, entry payment, total cash needed, remaining liquidity, and whether the operation is viable.

### Bank Comparisons
For each bank, you will see:
- **Recommendation badge**: Recommended / NotWorthIt
- **TIN breakdown**: Base TIN, max discounted TIN, final real TIN
- **Mortgage options table**: For each term (15, 20, 25, 30 years) - monthly payment, total paid, total interest, debt ratio, free cash
- **Bonus evaluations**: Whether each bonus saves or costs you money, with a thumbs-up/down indicator

### Strategy Comparison
Compares two approaches:
- **Amortize Faster**: Use all available cash as entry payment to minimize the mortgage
- **Maintain Capital**: Use standard financing and invest the difference

Shows net differences under conservative, base, and optimistic scenarios, with a weighted recommendation.

![Results overview](docs/screenshots/11-results-overview.png)

---

## Step 10: Return to Scenario List

Click the **back arrow** (top-left) to return to the scenario list. Your scenario will appear with a green checkmark in the "Analyzed" column.

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
