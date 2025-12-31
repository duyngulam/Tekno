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
    this.priceMinInput = page.locator('input[placeholder*="min"]');
    this.priceMaxInput = page.locator('input[placeholder*="max"]');
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

  async applyBrand(brandName: string) {
    const lbl = this.filterAside.locator(`label:has-text("${brandName}")`).first();
    if (await lbl.count()) {
      await lbl.scrollIntoViewIfNeeded();
      await lbl.click();
      await this.page.waitForLoadState('networkidle');
    } else {
      // fallback: try to click by text anywhere in aside
      const alt = this.page.locator(`aside >> text=${brandName}`).first();
      if (await alt.count()) {
        await alt.click();
        await this.page.waitForLoadState('networkidle');
      }
    }
  }

  async applyAttribute(attrName: string, value: string) {
    // find attribute accordion by attrName then value checkbox label
    const checkbox = this.filterAside.locator(`div:has-text("${attrName}")`).locator(`label:has-text("${value}")`).first();
    if (await checkbox.count()) {
      await checkbox.scrollIntoViewIfNeeded();
      await checkbox.click();
      await this.page.waitForLoadState('networkidle');
    } else {
      const any = this.page.locator(`aside label:has-text("${value}")`).first();
      if (await any.count()) {
        await any.click();
        await this.page.waitForLoadState('networkidle');
      }
    }
  }

  async getNoResultsText(): Promise<string | null> {
    const el = this.page.locator('text=No Product Available').first();
    if (await el.count()) return (await el.innerText()).trim();
    return null;
  }

  async getTotalRecords(): Promise<number> {
    const el = this.page.locator('p:has-text("Showing") span').first();
    if (await el.count()) {
      const txt = (await el.innerText()).replace(/[.,]/g, '').trim();
      const n = parseInt(txt, 10);
      return Number.isNaN(n) ? 0 : n;
    }
    return 0;
  }

  async goToPage(n: number) {
    const btn = this.page.locator(`a:has-text("${n}")`).first();
    if (await btn.count()) {
      await btn.click();
      await this.page.waitForLoadState('networkidle');
    }
  }

  async openFirstProduct() {
    const first = this.productLinks.first();
    if (await first.count()) {
      await first.click();
      await this.page.waitForLoadState('networkidle');
    }
  }

  async searchKeyword(keyword: string) {
    const searchInput = this.page.locator('input[placeholder="Search products…"]');
    await searchInput.fill(keyword);
    await this.page.waitForLoadState('networkidle');
  }

  async applySortBy(optionText: string) {
    // Find the combobox trigger (Radix renders a button[role=combobox])
    const trigger = this.page.locator('button[role="combobox"]').first();
    if (await trigger.count() === 0) {
      // fallback: any element with role combobox
      const byRole = this.page.getByRole('combobox').first();
      await byRole.click().catch(() => {});
    } else {
      await trigger.click();
    }

    // If Radix uses aria-controls, open the popup container by ID and click inside it
    try {
      const usedTrigger = (await trigger.count()) ? trigger : this.page.getByRole('combobox').first();
      const controls = await usedTrigger.getAttribute('aria-controls');
      if (controls) {
        const popup = this.page.locator(`#${controls}`);
        await popup.waitFor({ state: 'visible', timeout: 2000 });
        const opt = popup.locator(`text=${optionText}`).first();
        if (await opt.count()) {
          await opt.click();
          await this.page.waitForLoadState('networkidle');
          return;
        }
      }
    } catch (e) {
      // ignore and fallback
    }

    // fallback: try ARIA option role or global text
    const optionByRole = this.page.getByRole('option', { name: optionText }).first();
    if (await optionByRole.count()) {
      await optionByRole.click().catch(() => {});
    } else {
      const optText = this.page.locator(`text=${optionText}`).first();
      if (await optText.count()) await optText.click().catch(() => {});
    }

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
