import { test, expect } from './fixtures';

test.describe('Scenario 5: Optimal Strategy', () => {
  test('configure strategy params, run analysis, and verify recommendation', async ({ page }) => {
    await page.goto('/scenarios/new');

    // Fill scenario name
    await page.locator('input[placeholder="Scenario name"]').fill('E2E Test - Strategy');

    // Fill sale data (Tab 1)
    const saleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await saleCard.locator('input[type="number"]').nth(0).fill('320000');
    await saleCard.locator('input[type="number"]').nth(2).fill('95000');
    await saleCard.locator('input[type="number"]').nth(3).fill('45000');
    await saleCard.locator('input[type="number"]').nth(4).fill('1800');

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
    await purchaseFields.nth(4).fill('900');
    await purchaseFields.nth(5).fill('400');
    await purchaseFields.nth(6).fill('350');

    // Add a bank (required for analysis to have mortgage data available)
    await page.getByRole('tab').filter({ has: page.locator('mat-icon:text("add")') }).locator('button').click();
    await page.waitForTimeout(500);
    await page.locator('div[role="tab"]', { hasText: /Bank 1/ }).click();
    await page.waitForTimeout(300);

    const bankCard = page.locator('mat-card', { hasText: 'Bank Offer' });
    await bankCard.locator('input').first().fill('Test Bank');
    await bankCard.locator('input[type="number"]').first().fill('2.50');

    // Navigate to Strategy tab
    await page.locator('div[role="tab"]', { hasText: 'Strategy' }).click();
    await page.waitForTimeout(300);

    // Fill strategy parameters
    const strategyCard = page.locator('mat-card', { hasText: 'Strategy Parameters' });
    const strategyFields = strategyCard.locator('input[type="number"]');
    await strategyFields.nth(0).fill('3');   // Conservative return
    await strategyFields.nth(1).fill('6');   // Base return
    await strategyFields.nth(2).fill('9');   // Optimistic return
    await strategyFields.nth(3).fill('20');  // Horizon years
    await strategyFields.nth(4).fill('10000'); // Min liquidity cushion

    // Save the scenario
    await page.getByRole('button', { name: /save/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Scenario saved', { timeout: 10_000 });
    await expect(page.locator('simple-snack-bar')).toBeHidden({ timeout: 5_000 });

    // Run analysis
    await page.getByRole('button', { name: /run analysis/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Analysis complete', { timeout: 20_000 });

    // Navigate to Results tab
    await page.locator('div[role="tab"]', { hasText: 'Results' }).click();
    await page.waitForTimeout(500);

    // Verify strategy comparison section is visible
    await expect(page.locator('mat-card', { hasText: 'Strategy Comparison' })).toBeVisible();

    // Verify the recommendation chip is visible (either AmortizeFaster or MaintainCapital)
    const strategySection = page.locator('mat-card', { hasText: 'Strategy Comparison' });
    await expect(strategySection.locator('mat-chip')).toBeVisible();

    // Verify both strategy columns exist
    await expect(strategySection.getByText('Amortize Faster')).toBeVisible();
    await expect(strategySection.getByText('Maintain Capital', { exact: true })).toBeVisible();

    // Verify net differences section exists
    await expect(strategySection.getByText('Net Differences')).toBeVisible();
  });
});
