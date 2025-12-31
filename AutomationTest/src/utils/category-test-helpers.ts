// playwright/utils/category-test-helpers.ts

import { Page } from '@playwright/test';

/**
 * Category Test Data Factory
 */
export class CategoryDataFactory {
  static createBasicCategory(overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Test Category ${uniqueId}`,
      slug: `test-category-${uniqueId}`,
      ...overrides,
    };
  }

  static createCategoryWithMedia(iconPath: string, imagePath: string, overrides?: any) {
    return {
      ...this.createBasicCategory(),
      iconPath,
      imagePath,
      ...overrides,
    };
  }

  static createChildCategory(parentId: string, overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Child Category ${uniqueId}`,
      slug: `child-category-${uniqueId}`,
      parentId,
      ...overrides,
    };
  }

  static createCategoryForDeletion(overrides?: any) {
    const uniqueId = Date.now();
    return {
      name: `Delete Me ${uniqueId}`,
      slug: `delete-me-${uniqueId}`,
      ...overrides,
    };
  }

  static createCategoryHierarchy(depth: number = 3) {
    const categories: { name: string; slug: string }[] = [];
    for (let i = 0; i < depth; i++) {
      const uniqueId = Date.now() + i;
      categories.push({
        name: `Level ${i + 1} Category ${uniqueId}`,
        slug: `level-${i + 1}-category-${uniqueId}`,
      });
    }
    return categories;
  }
}

/**
 * Category API Helper for mocking
 */
export class CategoryAPIHelper {
  constructor(private page: Page) {}

  async mockGetCategories(data?: any[]) {
    const defaultData = [
      {
        id: 1,
        name: 'Electronics',
        slug: 'electronics',
        iconPath: 'https://via.placeholder.com/50',
        imageUrl: 'https://via.placeholder.com/200',
        isActive: true,
        subCategories: [
          {
            id: 11,
            name: 'Smartphones',
            slug: 'smartphones',
            parentId: 1,
            isActive: true,
            subCategories: [],
          },
          {
            id: 12,
            name: 'Laptops',
            slug: 'laptops',
            parentId: 1,
            isActive: true,
            subCategories: [],
          },
        ],
      },
      {
        id: 2,
        name: 'Fashion',
        slug: 'fashion',
        iconPath: null,
        imageUrl: null,
        isActive: true,
        subCategories: [],
      },
    ];

    await this.page.route('**/api/admin/category/tree*', async (route) => {
      if (route.request().method() === 'GET') {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(data || defaultData),
        });
      }
    });
  }

  async mockCreateCategory() {
    await this.page.route('**/api/admin/category', async (route) => {
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

  async mockUpdateCategory() {
    await this.page.route('**/api/admin/category*', async (route) => {
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

  async mockDeleteCategory() {
    await this.page.route('**/api/admin/category/*', async (route) => {
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
    await this.page.route('**/api/admin/category', async (route) => {
      if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 400,
          contentType: 'application/json',
          body: JSON.stringify({
            error: 'Slug đã tồn tại',
          }),
        });
      }
    });
  }

  async mockAPIError(statusCode: number = 500) {
    await this.page.route('**/api/admin/category*', async (route) => {
      await route.fulfill({
        status: statusCode,
        contentType: 'application/json',
        body: JSON.stringify({
          error: 'Internal Server Error',
        }),
      });
    });
  }

  async mockGetAttributes(categoryId: number, attributes?: any[]) {
    const defaultAttributes = [
      {
        id: 1,
        name: 'Color',
        type: 'select',
        values: ['Red', 'Blue', 'Green'],
      },
      {
        id: 2,
        name: 'Size',
        type: 'select',
        values: ['S', 'M', 'L', 'XL'],
      },
    ];

    await this.page.route(`**/api/admin/category/${categoryId}/attributes*`, async (route) => {
      if (route.request().method() === 'GET') {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(attributes || defaultAttributes),
        });
      }
    });
  }
}

/**
 * Category Screenshot Helper
 */
export class CategoryScreenshotHelper {
  constructor(private page: Page) {}

  async captureTreeState(name: string) {
    await this.page.screenshot({
      path: `test-results/screenshots/category-tree-${name}-${Date.now()}.png`,
      fullPage: true,
    });
  }

  async captureModal(name: string) {
    await this.page.locator('div[role="dialog"]').screenshot({
      path: `test-results/screenshots/category-modal-${name}-${Date.now()}.png`,
    });
  }

  async captureIconPreview(rowIndex: number) {
    const row = this.page.locator('tbody tr').nth(rowIndex);
    const iconCell = row.locator('td').nth(2);
    
    await iconCell.screenshot({
      path: `test-results/screenshots/category-icon-${Date.now()}.png`,
    });
  }

  async captureImagePreview(rowIndex: number) {
    const row = this.page.locator('tbody tr').nth(rowIndex);
    const imageCell = row.locator('td').nth(3);
    
    await imageCell.screenshot({
      path: `test-results/screenshots/category-image-${Date.now()}.png`,
    });
  }
}

/**
 * Category Wait Helper
 */
export class CategoryWaitHelper {
  static async waitForTreeRefresh(page: Page) {
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

  static async waitForExpansion(page: Page) {
    await page.waitForTimeout(300);
  }
}

/**
 * Category Validation Helper
 */
export class CategoryValidationHelper {
  static isValidSlug(slug: string): boolean {
    const slugRegex = /^[a-z0-9]+(?:-[a-z0-9]+)*$/;
    return slugRegex.test(slug);
  }

  static isValidCategoryName(name: string): boolean {
    return name.trim().length > 0 && name.length <= 200;
  }

  static generateSlugFromName(name: string): string {
    return name
      .toLowerCase()
      .trim()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/Đ/g, 'd')
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/^-+|-+$/g, '');
  }

  static isValidHierarchy(parentId: number | null, childId: number): boolean {
    // Prevent circular references
    return parentId !== childId;
  }
}

/**
 * Category Tree Helper
 */
export class CategoryTreeHelper {
  static flattenTree(tree: any[]): any[] {
    const result: any[] = [];
    
    const traverse = (nodes: any[], depth: number = 0) => {
      nodes.forEach((node) => {
        result.push({ ...node, depth });
        if (node.subCategories && node.subCategories.length > 0) {
          traverse(node.subCategories, depth + 1);
        }
      });
    };
    
    traverse(tree);
    return result;
  }

  static findCategoryById(tree: any[], id: number): any | null {
    for (const node of tree) {
      if (node.id === id) return node;
      if (node.subCategories) {
        const found = this.findCategoryById(node.subCategories, id);
        if (found) return found;
      }
    }
    return null;
  }

  static findCategoryBySlug(tree: any[], slug: string): any | null {
    for (const node of tree) {
      if (node.slug === slug) return node;
      if (node.subCategories) {
        const found = this.findCategoryBySlug(node.subCategories, slug);
        if (found) return found;
      }
    }
    return null;
  }

  static countTotalCategories(tree: any[]): number {
    let count = 0;
    
    const traverse = (nodes: any[]) => {
      nodes.forEach((node) => {
        count++;
        if (node.subCategories) {
          traverse(node.subCategories);
        }
      });
    };
    
    traverse(tree);
    return count;
  }

  static getMaxDepth(tree: any[]): number {
    let maxDepth = 0;
    
    const traverse = (nodes: any[], depth: number) => {
      if (depth > maxDepth) maxDepth = depth;
      nodes.forEach((node) => {
        if (node.subCategories) {
          traverse(node.subCategories, depth + 1);
        }
      });
    };
    
    traverse(tree, 0);
    return maxDepth;
  }
}

/**
 * Category Assertion Helper
 */
export class CategoryAssertionHelper {
  static assertCategoryDataMatches(
    actual: { id: string; name: string; slug: string },
    expected: { name: string; slug: string }
  ): boolean {
    if (actual.name !== expected.name) {
      console.error(`Name mismatch: ${actual.name} !== ${expected.name}`);
      return false;
    }

    if (actual.slug !== expected.slug) {
      console.error(`Slug mismatch: ${actual.slug} !== ${expected.slug}`);
      return false;
    }

    return true;
  }

  static assertCategoryCreated(
    beforeCount: number,
    afterCount: number
  ): boolean {
    if (afterCount !== beforeCount + 1) {
      console.error(`Category count mismatch: expected ${beforeCount + 1}, got ${afterCount}`);
      return false;
    }
    return true;
  }

  static assertCategoryDeleted(
    beforeCount: number,
    afterCount: number
  ): boolean {
    if (afterCount !== beforeCount - 1) {
      console.error(`Category count mismatch: expected ${beforeCount - 1}, got ${afterCount}`);
      return false;
    }
    return true;
  }

  static assertIsChildOf(
    tree: any[],
    childId: number,
    parentId: number
  ): boolean {
    const parent = CategoryTreeHelper.findCategoryById(tree, parentId);
    if (!parent || !parent.subCategories) return false;

    return parent.subCategories.some((child: any) => child.id === childId);
  }

  static assertHierarchyDepth(
    tree: any[],
    maxDepth: number
  ): boolean {
    const actualDepth = CategoryTreeHelper.getMaxDepth(tree);
    if (actualDepth > maxDepth) {
      console.error(`Hierarchy too deep: ${actualDepth} > ${maxDepth}`);
      return false;
    }
    return true;
  }
}

/**
 * Category Attributes Helper
 */
export class CategoryAttributesHelper {
  static async verifyAttributesModalOpen(page: Page): Promise<boolean> {
    const modal = page.locator('div[role="dialog"]').filter({ hasText: 'Quản lý Attribute' });
    return await modal.isVisible();
  }

  static async getAttributeCount(page: Page): Promise<number> {
    const attributeRows = page.locator('div[role="dialog"] tbody tr');
    return await attributeRows.count();
  }

  static createAttributeData(overrides?: any) {
    return {
      name: `Attribute ${Date.now()}`,
      type: 'select',
      values: ['Value1', 'Value2', 'Value3'],
      ...overrides,
    };
  }
}