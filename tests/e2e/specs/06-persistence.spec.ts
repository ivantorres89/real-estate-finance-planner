import { test, expect } from './fixtures';

test.describe('Scenario 6: Persistence', () => {
  const scenarioName = `E2E Persistence ${Date.now()}`;

  test('create, save, navigate away, and verify persistence', async ({ page }) => {
    // Step 1: Create and save a scenario
    await page.goto('/scenarios/new');
    await page.locator('input[placeholder="Scenario name"]').fill(scenarioName);

    // Fill some data so it is a meaningful scenario
    const saleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await saleCard.locator('input[type="number"]').nth(0).fill('250000');
    await saleCard.locator('input[type="number"]').nth(3).fill('20000');

    const debtCard = page.locator('mat-card', { hasText: 'Debt Capacity' });
    await debtCard.locator('input[type="number"]').nth(0).fill('2800');

    // Save
    await page.getByRole('button', { name: /save/i }).click();
    await expect(page.locator('simple-snack-bar')).toContainText('Scenario saved', { timeout: 10_000 });
    await page.waitForTimeout(1_000);

    // Capture the URL (contains the scenario ID)
    const savedUrl = page.url();
    expect(savedUrl).not.toContain('/new');

    // Step 2: Navigate back to the list
    const listResponse = page.waitForResponse(resp => resp.url().includes('/api/scenarios') && resp.request().method() === 'GET');
    await page.goto('/');
    await listResponse;

    // Verify the scenario appears in the list
    await expect(page.locator('table')).toBeVisible({ timeout: 15_000 });
    await expect(page.locator('td', { hasText: scenarioName })).toBeVisible();

    // Step 3: Click to reopen the scenario
    await page.locator('tr', { hasText: scenarioName }).click();
    await page.waitForTimeout(1_000);

    // Verify we are on the correct URL and data was loaded
    await expect(page).toHaveURL(savedUrl);
    await expect(page.locator('input[placeholder="Scenario name"]')).toHaveValue(scenarioName);

    // Verify the sale price was persisted
    const reloadedSaleCard = page.locator('mat-card', { hasText: 'Current Property Sale' });
    await expect(reloadedSaleCard.locator('input[type="number"]').nth(0)).toHaveValue('250000');
  });

  test('verify scenario exists via API', async ({ request }) => {
    const backendUrl = process.env.BACKEND_URL || 'http://localhost:5000';

    // List all scenarios
    const response = await request.get(`${backendUrl}/api/scenarios`);
    expect(response.ok()).toBeTruthy();

    const scenarios = await response.json();
    expect(Array.isArray(scenarios)).toBeTruthy();

    // At least one scenario should exist from previous tests
    expect(scenarios.length).toBeGreaterThan(0);
  });
});
