import { test as base } from '@playwright/test';

/**
 * Poll a URL until it returns a 2xx response or timeout is reached.
 */
export async function waitForService(url: string, timeoutMs = 60_000, intervalMs = 2_000): Promise<void> {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    try {
      const res = await fetch(url);
      if (res.ok) return;
    } catch {
      // service not ready yet
    }
    await new Promise((r) => setTimeout(r, intervalMs));
  }
  throw new Error(`Service at ${url} did not become ready within ${timeoutMs}ms`);
}

/**
 * Extended test fixture that ensures the stack is healthy before running tests.
 */
export const test = base.extend<{ ensureStackReady: void }>({
  ensureStackReady: [
    async ({}, use) => {
      const backendUrl = process.env.BACKEND_URL || 'http://localhost:5000';
      const frontendUrl = process.env.BASE_URL || 'http://localhost:4200';

      await waitForService(`${backendUrl}/health`, 90_000);
      await waitForService(frontendUrl, 90_000);

      await use();
    },
    { auto: true },
  ],
});

export { expect } from '@playwright/test';
