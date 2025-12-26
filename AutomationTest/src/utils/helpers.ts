// src/utils/helpers.ts

import { Page } from '@playwright/test';

/**
 * Mock API Helper for Advertisement Tests
 */
export class AdvertisementAPIHelper {
  constructor(private page: Page) {}

  async mockGetAdvertisementsList(data?: any[]) {
    const defaultData = [
      {
        id: 1,
        productId: 101,
        productName: 'Laptop Dell XPS 15',
        position: 'Homepage Banner',
        priority: 100,
        startDate: '2025-01-01T00:00:00Z',
        endDate: '2025-12-31T23:59:59Z',
        isActive: true,
        imageUrl: 'https://via.placeholder.com/300x150',
      },
    ];

    await this.page.route('**/api/admin/advertisements', async (route) => {
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

  async mockEmptyAdvertisementsList() {
    await this.mockGetAdvertisementsList([]);
  }

  async mockCreateAdvertisement() {
    await this.page.route('**/api/admin/advertisements', async (route) => {
      if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 201,
          contentType: 'application/json',
          body: JSON.stringify({
            success: true,
            data: {
              id: Date.now(),
              message: 'Advertisement created successfully',
            },
          }),
        });
      }
    });
  }

  async mockAPIError(statusCode: number = 500) {
    await this.page.route('**/api/admin/advertisements', async (route) => {
      await route.fulfill({
        status: statusCode,
        contentType: 'application/json',
        body: JSON.stringify({
          error: 'Internal Server Error',
        }),
      });
    });
  }

  async mockSlowResponse(delayMs: number = 2000) {
    await this.page.route('**/api/admin/advertisements', async (route) => {
      await new Promise((resolve) => setTimeout(resolve, delayMs));
      await route.continue();
    });
  }
}

export class DateHelper {
  static toDateTimeLocal(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  static getFutureDate(daysFromNow: number): string {
    const date = new Date();
    date.setDate(date.getDate() + daysFromNow);
    return this.toDateTimeLocal(date);
  }

  static getPastDate(daysAgo: number): string {
    const date = new Date();
    date.setDate(date.getDate() - daysAgo);
    return this.toDateTimeLocal(date);
  }

  static getActiveDateRange(): { start: string; end: string } {
    return {
      start: this.getPastDate(30),
      end: this.getFutureDate(30),
    };
  }

  static getScheduledDateRange(): { start: string; end: string } {
    return {
      start: this.getFutureDate(5),
      end: this.getFutureDate(35),
    };
  }

  static getExpiredDateRange(): { start: string; end: string } {
    return {
      start: this.getPastDate(60),
      end: this.getPastDate(30),
    };
  }
}

export class AdvertisementDataFactory {
  static createActiveAd(overrides?: any) {
    const dates = DateHelper.getActiveDateRange();
    return {
      productId: '101',
      position: 'Homepage Banner',
      priority: 100,
      startDate: dates.start,
      endDate: dates.end,
      isActive: true,
      ...overrides,
    };
  }

  static createScheduledAd(overrides?: any) {
    const dates = DateHelper.getScheduledDateRange();
    return {
      productId: '102',
      position: 'Sidebar Banner',
      priority: 90,
      startDate: dates.start,
      endDate: dates.end,
      isActive: true,
      ...overrides,
    };
  }

  static createExpiredAd(overrides?: any) {
    const dates = DateHelper.getExpiredDateRange();
    return {
      productId: '103',
      position: 'Footer Banner',
      priority: 80,
      startDate: dates.start,
      endDate: dates.end,
      isActive: true,
      ...overrides,
    };
  }

  static createInactiveAd(overrides?: any) {
    const dates = DateHelper.getActiveDateRange();
    return {
      productId: '104',
      position: 'Popup Banner',
      priority: 70,
      startDate: dates.start,
      endDate: dates.end,
      isActive: false,
      ...overrides,
    };
  }
}

export class AuthHelper {
  constructor(private page: Page) {}

  async setAuthToken(token: string = 'mock-jwt-token') {
    await this.page.evaluate((t) => {
      localStorage.setItem('token', t);
      localStorage.setItem('authToken', t);
    }, token);
  }

  async clearAuth() {
    await this.page.evaluate(() => {
      localStorage.removeItem('token');
      localStorage.removeItem('authToken');
    });
  }

  async loginAsAdmin() {
    await this.setAuthToken('admin-token-123');
  }
}

export class ScreenshotHelper {
  constructor(private page: Page) {}

  async capture(name: string) {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    await this.page.screenshot({
      path: `test-results/screenshots/${name}-${timestamp}.png`,
      fullPage: true,
    });
  }

  async captureElement(selector: string, name: string) {
    const element = this.page.locator(selector);
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    await element.screenshot({
      path: `test-results/screenshots/${name}-${timestamp}.png`,
    });
  }
}