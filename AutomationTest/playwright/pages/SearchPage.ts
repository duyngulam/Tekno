import { Page, Locator } from '@playwright/test';

export class SearchPage {
  readonly page: Page;
  readonly searchInput: Locator;
  readonly searchButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.searchInput = page.locator('input[name="q"], input[type="search"], input[placeholder*="Search"], input[aria-label*="Search"]');
    this.searchButton = page.locator('button:has-text("Search"), button[aria-label*="search"], button#search-btn');
  }

  async goto() {
    // Prefer a dedicated search route if present, otherwise go to products
    await this.page.goto('/products');
    await this.page.waitForLoadState('networkidle');
  }

  async search(query: string) {
    // Try native search input first
    if (await this.searchInput.count() > 0) {
      await this.searchInput.fill(query);
      if (await this.searchButton.count() > 0) {
        await this.searchButton.click();
      } else {
        // press Enter if no explicit button
        await this.searchInput.press('Enter');
      }
      await this.page.waitForLoadState('networkidle');
      return;
    }

    // fallback: navigate to products with keyword query param (app may support it)
    const url = `/products?keyword=${encodeURIComponent(query)}`;
    await this.page.goto(url);
    await this.page.waitForLoadState('networkidle');
  }
}
