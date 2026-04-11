import { test, expect } from './fixtures';

test.describe('Scenario 3: Purchase Simulation', () => {
  test('create scenario with purchase data and verify save', async ({ page }) => {
    await page.goto('/scenarios/new');

    // Fill scenario name
    await page.locator('input[placeholder="Scenario name"]').fill('E2E Test - Purchase');

    // Fill minimal sale data first (Tab 1)
    const saleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await saleCard.locator('input[type="number"]').nth(0).fill('300000'); // Sale Price
    await saleCard.locator('input[type="number"]').nth(2).fill('80000');  // Outstanding Mortgage
    await saleCard.locator('input[type="number"]').nth(3).fill('30000'); // Cash Balance

    const debtCard = page.locator('mat-card', { hasText: 'Debt Capacity' });
    await debtCard.locator('input[type="number"]').nth(0).fill('3000'); // Monthly salary

    // Navigate to Purchase tab
    await page.locator('div[role="tab"]', { hasText: 'Purchase' }).click();
    await page.waitForTimeout(300);

    // Fill purchase data
    const purchaseCard = page.locator('mat-card', { hasText: 'New Property Purchase' });
    const fields = purchaseCard.locator('input[type="number"]');
    await fields.nth(0).fill('280000'); // Purchase Price
    await fields.nth(1).fill('260000'); // Deed Price
    await fields.nth(2).fill('80');     // Financeable %
    await fields.nth(3).fill('35');     // Buyer Age
    await fields.nth(4).fill('900');    // Notary
    await fields.nth(5).fill('400');    // Administrative
    await fields.nth(6).fill('350');    // Appraisal
    await fields.nth(7).fill('0');      // Agency
    await fields.nth(8).fill('200');    // Other

    // Toggle reduced ITP checkbox
    const reducedItpCheckbox = purchaseCard.getByText('Apply Reduced ITP');
    await reducedItpCheckbox.click();

    // Save the scenario
    await page.getByRole('button', { name: /save/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Scenario saved', { timeout: 10_000 });
    await expect(page).not.toHaveURL(/\/new$/);
  });
});
