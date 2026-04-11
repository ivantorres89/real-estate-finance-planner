import { test, expect } from './fixtures';

test.describe('Scenario 7: Re-analysis with changed inputs', () => {
  test('re-running analysis after changing deed price uses updated values', async ({ page }) => {
    await page.goto('/scenarios/new');

    // Fill scenario name
    await page.locator('input[placeholder="Scenario name"]').fill('E2E Test - Reanalysis');

    // Fill sale data
    const saleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await saleCard.locator('input[type="number"]').nth(0).fill('320000');
    await saleCard.locator('input[type="number"]').nth(2).fill('95000');
    await saleCard.locator('input[type="number"]').nth(3).fill('45000');
    await saleCard.locator('input[type="number"]').nth(4).fill('1800');

    const debtCard = page.locator('mat-card', { hasText: 'Debt Capacity' });
    await debtCard.locator('input[type="number"]').nth(0).fill('3200');

    // Fill purchase data
    await page.locator('div[role="tab"]', { hasText: 'Purchase' }).click();
    await page.waitForTimeout(300);

    const purchaseCard = page.locator('mat-card', { hasText: 'New Property Purchase' });
    const pf = purchaseCard.locator('input[type="number"]');
    await pf.nth(0).fill('280000'); // Purchase price
    await pf.nth(1).fill('280000'); // Deed price (same as purchase)
    await pf.nth(2).fill('80');     // Financeable %
    await pf.nth(3).fill('35');     // Age
    await pf.nth(4).fill('900');    // Notary
    await pf.nth(5).fill('400');    // Admin
    await pf.nth(6).fill('350');    // Appraisal

    // Add a bank
    await page.getByRole('tab').filter({ has: page.locator('mat-icon:text("add")') }).locator('button').click();
    await page.waitForTimeout(500);
    await page.locator('div[role="tab"]', { hasText: /Bank 1/ }).click();
    await page.waitForTimeout(300);

    const bankCard = page.locator('mat-card', { hasText: 'Bank Offer' });
    await bankCard.locator('input').first().fill('Test Bank');
    await bankCard.locator('input[type="number"]').first().fill('2.50');

    // Fill strategy params
    await page.locator('div[role="tab"]', { hasText: 'Strategy' }).click();
    await page.waitForTimeout(300);
    const strategyCard = page.locator('mat-card', { hasText: 'Strategy Parameters' });
    const sf = strategyCard.locator('input[type="number"]');
    await sf.nth(0).fill('3');
    await sf.nth(1).fill('6');
    await sf.nth(2).fill('9');
    await sf.nth(3).fill('20');
    await sf.nth(4).fill('10000');

    // Save
    await page.getByRole('button', { name: /save/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Scenario saved', { timeout: 10_000 });
    await expect(page.locator('simple-snack-bar')).toBeHidden({ timeout: 5_000 });

    // First analysis - deed price = 280000, 80% => maxMortgage = 224000
    await page.getByRole('button', { name: /run analysis/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Analysis complete', { timeout: 20_000 });

    await page.locator('div[role="tab"]', { hasText: 'Results' }).click();
    await page.waitForTimeout(500);

    const purchaseCostsCard = page.locator('app-analysis-results');
    const maxMortgageText = purchaseCostsCard.getByText(/224[.,]000/);
    await expect(maxMortgageText.first()).toBeVisible();

    // Now change the deed price from 280000 to 200000 WITHOUT manually saving
    await page.locator('div[role="tab"]', { hasText: 'Purchase' }).click();
    await page.waitForTimeout(300);

    // Clear and change deed price (second number input in purchase card)
    const deedPriceInput = purchaseCard.locator('input[type="number"]').nth(1);
    await deedPriceInput.fill('200000');

    // Re-run analysis (should auto-save, then analyze with new data)
    await page.getByRole('button', { name: /run analysis/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Analysis complete', { timeout: 20_000 });

    // Go to results - maxMortgage should be 80% of 200000 = 160000
    await page.locator('div[role="tab"]', { hasText: 'Results' }).click();
    await page.waitForTimeout(500);

    const newMaxMortgage = purchaseCostsCard.getByText(/160[.,]000/);
    await expect(newMaxMortgage.first()).toBeVisible();
  });
});
