// playwright/utils/brand-test-helpers.ts

import { Page } from '@playwright/test';

/**
 * Brand Test Data Factory
 */
export class BrandDataFactory {
  static createBasicBrand(overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Test Brand ${uniqueId}`,
      slug: `test-brand-${uniqueId}`,
      country: 'Vietnam',
      ...overrides,
    };
  }

  static createBrandWithLogo(logoPath: string, overrides?: any) {
    return {
      ...this.createBasicBrand(),
      imagePath: logoPath,
      ...overrides,
    };
  }

  static createBrandForDeletion(overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Delete Me ${uniqueId}`,
      slug: `delete-me-${uniqueId}`,
      country: 'Test',
      ...overrides,
    };
  }

  static createInternationalBrand(country: string, overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `${country} Brand ${uniqueId}`,
      slug: `${country.toLowerCase()}-brand-${uniqueId}`,
      country,
      ...overrides,
    };
  }
}

/**
 * Brand API Helper for mocking
 */
export class BrandAPIHelper {
  constructor(private page: Page) {}

  async mockGetBrands(data?: any[]) {
    const defaultData = [
      {
        id: '1',
        name: 'Apple',
        slug: 'apple',
        country: 'USA',
        logoPath: 'C:\\Users\\NAT\\Tekno\\AutomationTest\\playwright\\fixtures\\test-brand-logo.jpg',
      },
      {
        id: '2',
        name: 'Samsung',
        slug: 'samsung',
        country: 'South Korea',
        logoPath: 'C:\\Users\\NAT\\Tekno\\AutomationTest\\playwright\\fixtures\\test-brand-logo.jpg',
      },
      {
        id: '3',
        name: 'Dell',
        slug: 'dell',
        country: 'USA',
        logoPath: null,
      },
    ];

    await this.page.route('**/api/admin/brand*', async (route) => {
      if (route.request().method() === 'GET') {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify({
            data: {
              data: data || defaultData,
            },
          }),
        });
      }
    });
  }

  async mockCreateBrand() {
    await this.page.route('**/api/admin/brand', async (route) => {
      if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 201,
          contentType: 'application/json',
          body: JSON.stringify({
            success: true,
            data: { id: Date.now().toString() },
          }),
        });
      }
    });
  }

  async mockUpdateBrand() {
    await this.page.route('**/api/admin/brand*', async (route) => {
      if (route.request().method() === 'PUT' || route.request().method() === 'PATCH') {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify({
            success: true,
          }),
        });
      }
    });
  }

  async mockDeleteBrand() {
    await this.page.route('**/api/admin/brand/*', async (route) => {
      if (route.request().method() === 'DELETE') {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify({
            success: true,
          }),
        });
      }
    });
  }

  async mockDuplicateSlugError() {
    await this.page.route('**/api/admin/brand', async (route) => {
      if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 400,
          contentType: 'application/json',
          body: JSON.stringify({
            error: 'Slug already exists',
          }),
        });
      }
    });
  }

  async mockAPIError(statusCode: number = 500) {
    await this.page.route('**/api/admin/brand*', async (route) => {
      await route.fulfill({
        status: statusCode,
        contentType: 'application/json',
        body: JSON.stringify({
          error: 'Internal Server Error',
        }),
      });
    });
  }
}

/**
 * Brand Screenshot Helper
 */
export class BrandScreenshotHelper {
  constructor(private page: Page) {}

  async captureTableState(name: string) {
    await this.page.screenshot({
      path: `test-results/screenshots/brands-${name}-${Date.now()}.png`,
      fullPage: true,
    });
  }

  async captureModal(name: string) {
    await this.page.locator('div[role="dialog"]').screenshot({
      path: `test-results/screenshots/brand-modal-${name}-${Date.now()}.png`,
    });
  }

  async captureLogoPreview(rowIndex: number) {
    const row = this.page.locator('tbody tr').nth(rowIndex);
    const logoCell = row.locator('td').nth(1);
    
    await logoCell.screenshot({
      path: `test-results/screenshots/brand-logo-${Date.now()}.png`,
    });
  }
}

/**
 * Brand Wait Helper
 */
export class BrandWaitHelper {
  static async waitForTableRefresh(page: Page) {
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
  }

  static async waitForModalClose(page: Page) {
    await page.waitForSelector('div[role="dialog"]', { state: 'hidden', timeout: 10000 });
  }

  static async waitForDialogAndAccept(page: Page): Promise<string> {
    return new Promise<string>((resolve) => {
      page.once('dialog', async (dialog) => {
        const message = dialog.message();
        await dialog.accept();
        resolve(message);
      });
    });
  }

  static async waitForDialogAndDismiss(page: Page): Promise<string> {
    return new Promise<string>((resolve) => {
      page.once('dialog', async (dialog) => {
        const message = dialog.message();
        await dialog.dismiss();
        resolve(message);
      });
    });
  }
}

/**
 * Brand Validation Helper
 */
export class BrandValidationHelper {
  static isValidSlug(slug: string): boolean {
    // Slug should be lowercase, alphanumeric with hyphens
    const slugRegex = /^[a-z0-9]+(?:-[a-z0-9]+)*$/;
    return slugRegex.test(slug);
  }

  static isValidBrandName(name: string): boolean {
    return name.trim().length > 0 && name.length <= 100;
  }

  static isValidCountry(country: string): boolean {
    return country.length <= 50;
  }

  static generateSlugFromName(name: string): string {
    return name
      .toLowerCase()
      .trim()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/^-+|-+$/g, '');
  }
}

/**
 * Brand Assertion Helper
 */
export class BrandAssertionHelper {
  static assertBrandDataMatches(
    actual: { id: string; name: string; country: string },
    expected: { name: string; country?: string }
  ): boolean {
    if (actual.name !== expected.name) {
      console.error(`Name mismatch: ${actual.name} !== ${expected.name}`);
      return false;
    }

    if (expected.country && actual.country !== expected.country) {
      console.error(`Country mismatch: ${actual.country} !== ${expected.country}`);
      return false;
    }

    return true;
  }

  static assertBrandCreated(
    beforeCount: number,
    afterCount: number
  ): boolean {
    if (afterCount !== beforeCount + 1) {
      console.error(`Brand count mismatch: expected ${beforeCount + 1}, got ${afterCount}`);
      return false;
    }
    return true;
  }

  static assertBrandDeleted(
    beforeCount: number,
    afterCount: number
  ): boolean {
    if (afterCount !== beforeCount - 1) {
      console.error(`Brand count mismatch: expected ${beforeCount - 1}, got ${afterCount}`);
      return false;
    }
    return true;
  }
}