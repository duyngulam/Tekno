import { Page, Locator } from '@playwright/test';

export class ProductsPage {
  readonly page: Page;
  readonly productLinks: Locator;
  readonly filterAside: Locator;
  readonly priceMinInput: Locator;
  readonly priceMaxInput: Locator;

  constructor(page: Page) {
    this.page = page;
    this.productLinks = page.locator('a[href^="/products/"]');
    this.filterAside = page.locator('aside');
    this.priceMinInput = page.locator('input[placeholder="min"]');
    this.priceMaxInput = page.locator('input[placeholder="max"]');
  }

  async goto() {
    await this.page.goto('/products');
    await this.page.waitForLoadState('networkidle');
  }

  async resultsCount(): Promise<number> {
    return await this.productLinks.count();
  }

  async firstResultTitle(): Promise<string> {
    const first = this.productLinks.first();
    try {
      return (await first.innerText()).toLowerCase();
    } catch (e) {
      return '';
    }
  }

  async applyFirstBrandFilter() {
    // Click the first label inside the filter aside (brand list is first accordion)
    const label = this.filterAside.locator('label').first();
    await label.scrollIntoViewIfNeeded();
    await label.click();
    await this.page.waitForLoadState('networkidle');
  }
  async applySortBy(optionText: string) {
    const sortSelect = this.page.getByRole('combobox', { name: 'Sort by' });
    await sortSelect.selectOption({ label: optionText });
    await this.page.waitForLoadState('networkidle');
  }

  async applyPriceRange(min: number, max: number) {
    await this.priceMinInput.fill(String(min));
    await this.priceMaxInput.fill(String(max));
    // trigger change events if any
    await this.priceMaxInput.press('Tab');
    await this.page.waitForLoadState('networkidle');
  }
}
