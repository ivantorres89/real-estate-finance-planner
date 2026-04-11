import { test, expect } from './fixtures';

test.describe('Scenario 4: Bank Comparison', () => {
  test('add bank with bonuses, toggle bonuses, save and run analysis', async ({ page }) => {
    await page.goto('/scenarios/new');

    // Fill scenario name
    await page.locator('input[placeholder="Scenario name"]').fill('E2E Test - Banks');

    // Fill minimal sale data (Tab 1)
    const saleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await saleCard.locator('input[type="number"]').nth(0).fill('320000');
    await saleCard.locator('input[type="number"]').nth(2).fill('95000');
    await saleCard.locator('input[type="number"]').nth(3).fill('45000');

    const debtCard = page.locator('mat-card', { hasText: 'Debt Capacity' });
    await debtCard.locator('input[type="number"]').nth(0).fill('3200');

    // Fill purchase data (Tab 2)
    await page.locator('div[role="tab"]', { hasText: 'Purchase' }).click();
    await page.waitForTimeout(300);

    const purchaseCard = page.locator('mat-card', { hasText: 'New Property Purchase' });
    const purchaseFields = purchaseCard.locator('input[type="number"]');
    await purchaseFields.nth(0).fill('280000');
    await purchaseFields.nth(1).fill('280000');
    await purchaseFields.nth(2).fill('80');
    await purchaseFields.nth(3).fill('35');

    // Add a bank by clicking the + tab button
    await page.getByRole('tab').filter({ has: page.locator('mat-icon:text("add")') }).locator('button').click();
    await page.waitForTimeout(500);

    // Click on the newly created bank tab (should be "Bank 1")
    await page.locator('div[role="tab"]', { hasText: /Bank 1/ }).click();
    await page.waitForTimeout(300);

    // Fill bank details
    const bankCard = page.locator('mat-card', { hasText: 'Bank Offer' });
    await bankCard.locator('input').first().fill('Banco Santander');

    // Fill base TIN
    const baseTinInput = bankCard.locator('input[type="number"]').first();
    await baseTinInput.fill('2.90');

    // Add a bonus
    await bankCard.locator('h3 button').click();
    await page.waitForTimeout(300);

    // Fill bonus details
    const bonusCard = bankCard.locator('.bonus-card').first();
    // Bonus name
    await bonusCard.locator('input').first().fill('Payroll deposit');

    // TIN reduction
    const bonusNumberInputs = bonusCard.locator('input[type="number"]');
    await bonusNumberInputs.nth(0).fill('0.50'); // TIN reduction

    // Toggle "Accepted" checkbox on the bonus
    await bonusCard.getByText('Accepted').click();

    // Add a second bonus
    await bankCard.locator('h3 button').click();
    await page.waitForTimeout(300);

    const secondBonus = bankCard.locator('.bonus-card').nth(1);
    await secondBonus.locator('input').first().fill('Home Insurance');
    const secondBonusNumbers = secondBonus.locator('input[type="number"]');
    await secondBonusNumbers.nth(0).fill('0.30'); // TIN reduction
    await secondBonusNumbers.nth(1).fill('35');   // Monthly cost

    // Accept the second bonus
    await secondBonus.getByText('Accepted').click();

    // Save the scenario
    await page.getByRole('button', { name: /save/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Scenario saved', { timeout: 10_000 });
    await expect(page.locator('simple-snack-bar')).toBeHidden({ timeout: 5_000 });

    // Run analysis
    await page.getByRole('button', { name: /run analysis/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Analysis complete', { timeout: 20_000 });

    // Verify Results tab appeared
    await page.locator('div[role="tab"]', { hasText: 'Results' }).click();
    await page.waitForTimeout(500);

    // Verify bank result card exists
    await expect(page.locator('mat-card', { hasText: 'Banco Santander' })).toBeVisible();
  });
});
