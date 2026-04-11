import { test, expect } from './fixtures';

test.describe('Scenario 2: Sale Data & Debt Capacity', () => {
  test('enter sale data and debt capacity, then save scenario', async ({ page }) => {
    // Navigate to new scenario
    await page.goto('/scenarios/new');

    // Fill scenario name
    await page.locator('input[placeholder="Scenario name"]').fill('E2E Test - Sale Data');

    // We are on the first tab: "Sale & Liquidity"
    // Fill sale data fields
    const saleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await saleCard.locator('input[type="number"]').nth(0).fill('320000'); // Sale Price
    await saleCard.locator('input[type="number"]').nth(1).fill('3500');   // Sale-Related Costs
    await saleCard.locator('input[type="number"]').nth(2).fill('95000');  // Outstanding Mortgage Debt
    await saleCard.locator('input[type="number"]').nth(3).fill('45000');  // Current Cash Balance
    await saleCard.locator('input[type="number"]').nth(4).fill('1800');   // Municipal Capital Gains Tax
    await saleCard.locator('input[type="number"]').nth(5).fill('500');    // Extraordinary Costs

    // Fill debt capacity fields
    const debtCard = page.locator('mat-card', { hasText: 'Debt Capacity' });
    await debtCard.locator('input[type="number"]').nth(0).fill('3200');   // Monthly Net Salary
    await debtCard.locator('input[type="number"]').nth(1).fill('0');      // Monthly Loan Payments
    await debtCard.locator('input[type="number"]').nth(2).fill('35');     // Max Debt Ratio

    // Save the scenario
    await page.getByRole('button', { name: /save/i }).click();

    // Should show success snackbar and navigate to the saved scenario URL
    await expect(page.locator('simple-snack-bar')).toContainText('Scenario saved', { timeout: 10_000 });

    // URL should now contain a scenario ID (not 'new')
    await expect(page).not.toHaveURL(/\/new$/);
  });
});
