export const TEST_CONFIG = {
  baseUrl: process.env.TEST_BASE_URL || 'http://localhost:3000', // Next.js dev server
  apiUrl: process.env.API_URL || 'http://localhost:5000', // .NET backend
  defaultBrowser: (process.env.TEST_BROWSER as any) || 'chrome',
  headless: process.env.HEADLESS === 'true' || false,
  timeout: 120000
};