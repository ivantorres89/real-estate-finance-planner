import { test, expect } from './fixtures';

test.describe('Scenario 1: Initial Load', () => {
  test('application opens and scenario list loads', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('mat-card-title')).toContainText('Real Estate Finance Planner');
  });

  test('backend health endpoint responds', async ({ request }) => {
    const backendUrl = process.env.BACKEND_URL || 'http://localhost:5000';
    const response = await request.get(`${backendUrl}/health`);
    expect(response.ok()).toBeTruthy();
    const body = await response.json();
    expect(body.status).toBe('Healthy');
  });

  test('backend API scenarios endpoint responds', async ({ request }) => {
    const backendUrl = process.env.BACKEND_URL || 'http://localhost:5000';
    const response = await request.get(`${backendUrl}/api/scenarios`);
    expect(response.ok()).toBeTruthy();
    const data = await response.json();
    expect(Array.isArray(data)).toBeTruthy();
  });

  test('can navigate to new scenario form', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: /new scenario/i }).click();
    await expect(page).toHaveURL(/\/scenarios\/new/);
    await expect(page.locator('input[placeholder="Scenario name"]')).toBeVisible();
  });
});
