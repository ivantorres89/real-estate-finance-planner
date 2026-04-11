import { test } from '@playwright/test';
import path from 'path';

const screenshotsDir = path.resolve(__dirname, '../../../docs/screenshots');

test('capture workflow screenshots (dark theme, Spanish)', async ({ page }) => {
  await page.setViewportSize({ width: 1280, height: 800 });

  // Force Spanish locale via localStorage before any navigation
  await page.addInitScript(() => {
    localStorage.setItem('locale', 'es');
  });

  // 01 - Empty scenario list
  await page.goto('/');
  await page.waitForTimeout(1500);
  await page.screenshot({ path: path.join(screenshotsDir, '01-scenario-list-empty.png'), fullPage: true });

  // 02 - Click "Nuevo Escenario"
  await page.getByRole('button', { name: /nuevo escenario/i }).click();
  await page.waitForTimeout(500);
  await page.screenshot({ path: path.join(screenshotsDir, '02-new-scenario.png'), fullPage: true });

  // 03 - Fill scenario name and sale data
  await page.locator('input[placeholder="Nombre del escenario"]').fill('Mi Primer Analisis Inmobiliario');

  const saleCard = page.locator('mat-card', { hasText: 'Venta de Propiedad Actual' });
  await saleCard.locator('input[type="number"]').nth(0).fill('320000');
  await saleCard.locator('input[type="number"]').nth(1).fill('310000');
  await saleCard.locator('input[type="number"]').nth(2).fill('95000');
  await saleCard.locator('input[type="number"]').nth(3).fill('45000');
  await saleCard.locator('input[type="number"]').nth(4).fill('1800');

  const debtCard = page.locator('mat-card', { hasText: 'Capacidad de Endeudamiento' });
  await debtCard.locator('input[type="number"]').nth(0).fill('3200');
  await debtCard.locator('input[type="number"]').nth(1).fill('800');

  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '03-sale-data-filled.png'), fullPage: true });

  // 04 - Purchase tab (Compra)
  await page.locator('div[role="tab"]', { hasText: 'Compra' }).click();
  await page.waitForTimeout(500);

  const purchaseCard = page.locator('mat-card', { hasText: 'Compra de Nueva Propiedad' });
  const pf = purchaseCard.locator('input[type="number"]');
  await pf.nth(0).fill('280000');
  await pf.nth(1).fill('280000');
  await pf.nth(2).fill('80');
  await pf.nth(3).fill('35');
  await pf.nth(4).fill('900');
  await pf.nth(5).fill('400');
  await pf.nth(6).fill('350');

  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '04-purchase-data-filled.png'), fullPage: true });

  // 05 - Add first bank
  await page.getByRole('tab').filter({ has: page.locator('mat-icon:text("add")') }).locator('button').click();
  await page.waitForTimeout(500);
  await page.locator('div[role="tab"]', { hasText: /Banco 1/ }).click();
  await page.waitForTimeout(500);

  const bankCard = page.locator('mat-card', { hasText: 'Oferta Bancaria' });
  await bankCard.locator('input').first().fill('Banco Santander');
  await bankCard.locator('input[type="number"]').first().fill('2.90');

  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '05-bank-offer-empty.png'), fullPage: true });

  // 06 - Add bonuses to bank
  await bankCard.locator('h3 button').click();
  await page.waitForTimeout(300);

  const bonus1 = bankCard.locator('.bonus-card').first();
  await bonus1.locator('input').first().fill('Domiciliacion de Nomina');
  await bonus1.locator('input[type="number"]').nth(0).fill('0.50');
  await bonus1.locator('input[type="number"]').nth(1).fill('0');
  await bonus1.getByText('Aceptado').click();

  await bankCard.locator('h3 button').click();
  await page.waitForTimeout(300);

  const bonus2 = bankCard.locator('.bonus-card').nth(1);
  await bonus2.locator('input').first().fill('Seguro de Hogar');
  await bonus2.locator('input[type="number"]').nth(0).fill('0.30');
  await bonus2.locator('input[type="number"]').nth(1).fill('35');
  await bonus2.getByText('Aceptado').click();

  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '06-bank-with-bonuses.png'), fullPage: true });

  // 07 - Add a second bank
  await page.getByRole('tab').filter({ has: page.locator('mat-icon:text("add")') }).locator('button').click();
  await page.waitForTimeout(500);
  await page.locator('div[role="tab"]', { hasText: /Banco 2/ }).click();
  await page.waitForTimeout(500);

  const bankCard2 = page.locator('mat-card', { hasText: 'Oferta Bancaria' });
  await bankCard2.locator('input').first().fill('BBVA');
  await bankCard2.locator('input[type="number"]').first().fill('3.10');

  await bankCard2.locator('h3 button').click();
  await page.waitForTimeout(300);
  const bbvaBonus = bankCard2.locator('.bonus-card').first();
  await bbvaBonus.locator('input').first().fill('Domiciliacion de Nomina');
  await bbvaBonus.locator('input[type="number"]').nth(0).fill('0.40');
  await bbvaBonus.locator('input[type="number"]').nth(1).fill('0');
  await bbvaBonus.getByText('Aceptado').click();

  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '07-second-bank.png'), fullPage: true });

  // 08 - Strategy tab (Estrategia)
  await page.locator('div[role="tab"]', { hasText: 'Estrategia' }).click();
  await page.waitForTimeout(500);

  const strategyCard = page.locator('mat-card', { hasText: 'Parametros de Estrategia' });
  const sf = strategyCard.locator('input[type="number"]');
  await sf.nth(0).fill('3');
  await sf.nth(1).fill('6');
  await sf.nth(2).fill('9');
  await sf.nth(3).fill('20');
  await sf.nth(4).fill('15000');

  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '08-strategy-params.png'), fullPage: true });

  // 09 - Save scenario (Guardar)
  await page.getByRole('button', { name: /guardar/i }).click();
  await page.waitForTimeout(2500);
  await page.screenshot({ path: path.join(screenshotsDir, '09-scenario-saved.png'), fullPage: true });

  // 10 - Run Analysis (Ejecutar Analisis)
  await page.getByRole('button', { name: /ejecutar an/i }).click();
  await page.waitForTimeout(3000);
  await page.screenshot({ path: path.join(screenshotsDir, '10-analysis-running.png'), fullPage: true });

  // 11 - Results tab (Resultados)
  await page.locator('div[role="tab"]', { hasText: 'Resultados' }).click();
  await page.waitForTimeout(500);
  await page.screenshot({ path: path.join(screenshotsDir, '11-results-overview.png'), fullPage: true });

  // Scroll down to see bank results
  await page.evaluate(() => window.scrollTo(0, 600));
  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '12-results-banks.png'), fullPage: true });

  // Scroll further for strategy
  await page.evaluate(() => window.scrollTo(0, 1200));
  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(screenshotsDir, '13-results-strategy.png'), fullPage: true });

  // 14 - Go back to list
  await page.locator('button[mat-icon-button]').first().click();
  await page.waitForTimeout(1500);
  await page.screenshot({ path: path.join(screenshotsDir, '14-scenario-list-with-data.png'), fullPage: true });
});
