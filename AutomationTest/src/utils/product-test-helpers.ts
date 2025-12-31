// playwright/utils/product-test-helpers.ts

import { Page } from '@playwright/test';

/**
 * Product Test Data Factory
 */
export class ProductDataFactory {
  static createBasicProduct(overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Test Product ${uniqueId}`,
      slug: `test-product-${uniqueId}`,
      basePrice: 10000000,
      discount: 10,
      overview: 'Automated test product',
      ...overrides,
    };
  }

  static createProductWithImage(overrides?: any) {
    return {
      ...this.createBasicProduct(),
      imagePath: 'path/to/image.jpg',
      ...overrides,
    };
  }

  static createProductForDeletion(overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Delete Me ${uniqueId}`,
      slug: `delete-me-${uniqueId}`,
      basePrice: 1000000,
      ...overrides,
    };
  }
}

/**
 * Product API Helper for mocking
 */
export class ProductAPIHelper {
  constructor(private page: Page) {}

  async mockGetProducts(data?: any[]) {
    const defaultData = [
      {
        id: 1,
        name: 'MacBook Pro M3',
        slug: 'macbook-pro-m3',
        brandName: 'Apple',
        categoryName: 'Laptops',
        basePrice: 45000000,
        discountPercent: 5,
        finalPrice: 42750000,
        status: 'Active',
        overview: 'Premium laptop',
        images: [{
          id: 1,
          imageUrl: 'https://via.placeholder.com/300',
          isPrimary: true,
        }],
      },
      {
        id: 2,
        name: 'Dell XPS 15',
        slug: 'dell-xps-15',
        brandName: 'Dell',
        categoryName: 'Laptops',
        basePrice: 35000000,
        discountPercent: 10,
        finalPrice: 31500000,
        status: 'Active',
        overview: 'Business laptop',
        images: [],
      },
    ];

    await this.page.route('**/api/admin/products', async (route) => {
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

  async mockCreateProduct() {
    await this.page.route('**/api/admin/products', async (route) => {
      if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 201,
          contentType: 'application/json',
          body: JSON.stringify({
            success: true,
            data: { id: Date.now() },
          }),
        });
      }
    });
  }

  async mockUpdateProduct() {
    await this.page.route('**/api/admin/products/*', async (route) => {
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

  async mockDeleteProduct() {
    await this.page.route('**/api/admin/products/*', async (route) => {
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

  async mockAPIError(statusCode: number = 500) {
    await this.page.route('**/api/admin/products*', async (route) => {
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
 * Screenshot Helper
 */
export class ProductScreenshotHelper {
  constructor(private page: Page) {}

  async captureTableState(name: string) {
    await this.page.screenshot({
      path: `test-results/screenshots/products-${name}-${Date.now()}.png`,
      fullPage: true,
    });
  }

  async captureModal(name: string) {
    await this.page.locator('.fixed .bg-white').screenshot({
      path: `test-results/screenshots/product-modal-${name}-${Date.now()}.png`,
    });
  }
}

/**
 * Wait Helper
 */
export class WaitHelper {
  static async waitForTableRefresh(page: Page) {
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
  }

  static async waitForModalClose(page: Page) {
    await page.waitForSelector('.fixed .bg-white', { state: 'hidden', timeout: 10000 });
  }

  static async waitForDialogAndAccept(page: Page) {
    return new Promise<string>((resolve) => {
      page.once('dialog', async (dialog) => {
        const message = dialog.message();
        await dialog.accept();
        resolve(message);
      });
    });
  }
}