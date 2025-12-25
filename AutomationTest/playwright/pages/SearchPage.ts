import { Page, Locator } from '@playwright/test';

export class SearchPage {
  readonly page: Page;
  readonly searchInput: Locator;
  readonly searchButton: Locator;
  readonly results: Locator;
  readonly filterPanel: Locator;

  constructor(page: Page) {
    this.page = page;
    // Flexible selectors - adjust to your app's markup
    this.searchInput = page.locator('input[name="q"], input[aria-label="Search"], #search-input');
    this.searchButton = page.locator('button[type="submit"], #search-btn, button:has-text("Search")');
    this.results = page.locator('.search-result, .result-item, .product-item');
    this.filterPanel = page.locator('.filters, .filter-panel');
  }

  async goto() {
    await this.page.goto('/search');
  }

  async search(query: string) {
    await this.searchInput.fill(query);
    await this.searchButton.click();
    // wait for results or loading to finish
    await this.page.waitForLoadState('networkidle');
  }

  async applyFilter(labelText: string) {
    // Try checkbox or select filter by visible label
    const checkbox = this.page.locator(`label:has-text("${labelText}") input[type=checkbox]");
    try {
      await checkbox.first().check({ force: true });
      await this.page.waitForLoadState('networkidle');
      return;
    } catch (e) {
      // fallback: click a filter button/link
      const btn = this.page.locator(`button:has-text("${labelText}"), a:has-text("${labelText}")`);
      await btn.first().click();
      await this.page.waitForLoadState('networkidle');
    }
  }

  async resultsCount() {
    return await this.results.count();
  }

  async firstResultTitle() {
    const el = this.results.first();
    try {
      return await el.innerText();
    } catch (e) {
      return '';
    }
  }
}
