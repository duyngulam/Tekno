import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: 'playwright/tests',
  timeout: 30_000,
  expect: { timeout: 5000 },
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    headless: true,
    viewport: { width: 1280, height: 720 },
    actionTimeout: 0,
    baseURL: process.env.TEST_BASE_URL || 'http://localhost:3000'
  },
  projects: [
    { name: 'chromium', use: { browserName: 'chromium' } }
  ]
});
