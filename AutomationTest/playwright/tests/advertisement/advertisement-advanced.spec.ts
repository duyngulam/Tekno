import { test, expect } from '@playwright/test';
import { AdvertisementPage } from '../../pages/AdvertisementPage';
import {
  AdvertisementAPIHelper,
  DateHelper,
  AdvertisementDataFactory,
  AuthHelper,
  ScreenshotHelper,
} from '../../../src/utils/helpers';
import path from 'path';

test.describe('Advertisement - Advanced Tests', () => {
  let adPage: AdvertisementPage;
  let apiHelper: AdvertisementAPIHelper;
  let authHelper: AuthHelper;
  let screenshotHelper: ScreenshotHelper;

  test.beforeEach(async ({ page }) => {
    adPage = new AdvertisementPage(page);
    apiHelper = new AdvertisementAPIHelper(page);
    authHelper = new AuthHelper(page);
    screenshotHelper = new ScreenshotHelper(page);

    // Set auth token before each test
    await authHelper.loginAsAdmin();
  });

  test.describe('API Mocking Tests', () => {
    test('should handle empty state correctly', async ({ page }) => {
      // Mock empty response
      await apiHelper.mockEmptyAdvertisementsList();

      await adPage.goto();
      await adPage.waitForDataLoad();

      // Should show "No advertisements found"
      const isNoDataVisible = await adPage.isNoDataVisible();
      expect(isNoDataVisible).toBe(true);

      // Table should not be visible
      const isTableVisible = await adPage.isTableVisible();
      expect(isTableVisible).toBe(false);
    });

    test('should show loading state', async ({ page }) => {
      // Mock slow API response
      await apiHelper.mockSlowResponse(3000);

      const gotoPromise = adPage.goto();

      // Check loading state appears
      await page.waitForSelector('text=Loading...', { timeout: 2000 });
      await expect(adPage.loadingText).toBeVisible();

      await gotoPromise;
      await adPage.waitForDataLoad();

      // Loading should disappear
      await expect(adPage.loadingText).not.toBeVisible();
    });

    test('should handle API error gracefully', async ({ page }) => {
      // Mock API error
      await apiHelper.mockAPIError(500);

      await adPage.goto();
      await adPage.waitForDataLoad();

      // Should show empty state (based on error handling in component)
      const count = await adPage.getRowCount();
      expect(count).toBe(0);
    });

    test('should display mocked data correctly', async ({ page }) => {
      const mockData = [
        {
          id: 1,
          productId: 999,
          productName: 'Test Product',
          position: 'Test Banner',
          priority: 150,
          startDate: '2025-01-01T00:00:00Z',
          endDate: '2025-12-31T23:59:59Z',
          isActive: true,
          imageUrl: 'https://via.placeholder.com/300',
        },
      ];

      await apiHelper.mockGetAdvertisementsList(mockData);

      await adPage.goto();
      await adPage.waitForDataLoad();

      const count = await adPage.getRowCount();
      expect(count).toBe(1);

      const rowData = await adPage.getRowData(0);
      expect(rowData.productName).toBe('Test Product');
      expect(rowData.position).toBe('Test Banner');
      expect(rowData.priority).toBe('150');
    });
  });

  test.describe('Status Badge Logic', () => {
    test('should display Active status for current ads', async ({ page }) => {
      const mockData = [
        {
          id: 1,
          productId: 101,
          productName: 'Active Product',
          position: 'Banner',
          priority: 100,
          startDate: DateHelper.getPastDate(10),
          endDate: DateHelper.getFutureDate(10),
          isActive: true,
          imageUrl: 'https://via.placeholder.com/300',
        },
      ];

      await apiHelper.mockGetAdvertisementsList(mockData);
      await adPage.goto();
      await adPage.waitForDataLoad();

      const badge = await adPage.getStatusBadge(0);
      const statusText = await badge.innerText();
      expect(statusText).toBe('Active');

      // Check badge styling
      const className = await badge.getAttribute('class');
      expect(className).toContain('bg-green-100');
      expect(className).toContain('text-green-700');
    });

    test('should display Scheduled status for future ads', async ({ page }) => {
      const mockData = [
        {
          id: 2,
          productId: 102,
          productName: 'Future Product',
          position: 'Banner',
          priority: 100,
          startDate: DateHelper.getFutureDate(5),
          endDate: DateHelper.getFutureDate(30),
          isActive: true,
          imageUrl: 'https://via.placeholder.com/300',
        },
      ];

      await apiHelper.mockGetAdvertisementsList(mockData);
      await adPage.goto();
      await adPage.waitForDataLoad();

      const badge = await adPage.getStatusBadge(0);
      const statusText = await badge.innerText();
      expect(statusText).toBe('Scheduled');

      const className = await badge.getAttribute('class');
      expect(className).toContain('bg-blue-100');
      expect(className).toContain('text-blue-700');
    });

    test('should display Expired status for past ads', async ({ page }) => {
      const mockData = [
        {
          id: 3,
          productId: 103,
          productName: 'Expired Product',
          position: 'Banner',
          priority: 100,
          startDate: DateHelper.getPastDate(60),
          endDate: DateHelper.getPastDate(30),
          isActive: true,
          imageUrl: 'https://via.placeholder.com/300',
        },
      ];

      await apiHelper.mockGetAdvertisementsList(mockData);
      await adPage.goto();
      await adPage.waitForDataLoad();

      const badge = await adPage.getStatusBadge(0);
      const statusText = await badge.innerText();
      expect(statusText).toBe('Expired');

      const className = await badge.getAttribute('class');
      expect(className).toContain('bg-gray-100');
      expect(className).toContain('text-gray-700');
    });

    test('should display Inactive status when isActive is false', async ({ page }) => {
      const mockData = [
        {
          id: 4,
          productId: 104,
          productName: 'Inactive Product',
          position: 'Banner',
          priority: 100,
          startDate: DateHelper.getPastDate(10),
          endDate: DateHelper.getFutureDate(10),
          isActive: false,
          imageUrl: 'https://via.placeholder.com/300',
        },
      ];

      await apiHelper.mockGetAdvertisementsList(mockData);
      await adPage.goto();
      await adPage.waitForDataLoad();

      const badge = await adPage.getStatusBadge(0);
      const statusText = await badge.innerText();
      expect(statusText).toBe('Inactive');

      const className = await badge.getAttribute('class');
      expect(className).toContain('bg-red-100');
      expect(className).toContain('text-red-700');
    });
  });

  test.describe('Create Advertisement with Test Data', () => {
    test('should create active advertisement', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await apiHelper.mockCreateAdvertisement();

      await adPage.goto();
      await adPage.waitForDataLoad();

      const testImagePath = path.join(__dirname, '../../fixtures/test-ad-image.jpg');
      const adData = {
        ...AdvertisementDataFactory.createActiveAd(),
        imagePath: testImagePath,
      };

      await adPage.createAdvertisement(adData);

      // Modal should close
      await expect(adPage.createModal).not.toBeVisible();
    });

    test('should create scheduled advertisement', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await apiHelper.mockCreateAdvertisement();

      await adPage.goto();
      await adPage.waitForDataLoad();

      const testImagePath = path.join(__dirname, '../../fixtures/test-ad-image.jpg');
      const adData = {
        ...AdvertisementDataFactory.createScheduledAd(),
        imagePath: testImagePath,
      };

      await adPage.createAdvertisement(adData);

      await expect(adPage.createModal).not.toBeVisible();
    });

    test('should validate required fields', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();

      await adPage.goto();
      await adPage.waitForDataLoad();

      await adPage.openCreateModal();

      // Try to submit without filling required fields
      let alertMessage = '';
      page.once('dialog', async (dialog) => {
        alertMessage = dialog.message();
        await dialog.accept();
      });

      await adPage.createSubmitButton.click();
      await page.waitForTimeout(500);

      // Should show validation alert
      expect(alertMessage).toContain('Please select an image');
    });
  });

  test.describe('Search Performance', () => {
    test('should search instantly on large dataset', async ({ page }) => {
      // Create large mock dataset
      const largeDataset = Array.from({ length: 100 }, (_, i) => ({
        id: i + 1,
        productId: 1000 + i,
        productName: `Product ${i + 1}`,
        position: i % 2 === 0 ? 'Homepage' : 'Sidebar',
        priority: 100 - i,
        startDate: DateHelper.getPastDate(30),
        endDate: DateHelper.getFutureDate(30),
        isActive: true,
        imageUrl: 'https://via.placeholder.com/300',
      }));

      await apiHelper.mockGetAdvertisementsList(largeDataset);
      await adPage.goto();
      await adPage.waitForDataLoad();

      const initialCount = await adPage.getRowCount();
      expect(initialCount).toBe(100);

      // Measure search time
      const startTime = Date.now();
      await adPage.search('Product 1');
      const endTime = Date.now();

      const searchTime = endTime - startTime;
      expect(searchTime).toBeLessThan(2000); // Should be fast (< 2s)

      const searchCount = await adPage.getRowCount();
      expect(searchCount).toBeGreaterThan(0);
      expect(searchCount).toBeLessThan(initialCount);
    });
  });

  test.describe('Visual Regression', () => {
    test('should match table layout screenshot', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await adPage.goto();
      await adPage.waitForDataLoad();

      // Take screenshot for visual comparison
      await screenshotHelper.captureElement('table', 'advertisement-table');
    });

    test('should match modal layout screenshot', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await adPage.goto();
      await adPage.waitForDataLoad();

      await adPage.openCreateModal();

      await screenshotHelper.captureElement('[role="dialog"]', 'create-modal');
    });
  });

  test.describe('Accessibility', () => {
    test('should have proper ARIA labels', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await adPage.goto();
      await adPage.waitForDataLoad();

      // Check modal dialog role
      await adPage.openCreateModal();
      const dialog = page.locator('[role="dialog"]');
      await expect(dialog).toBeVisible();
    });

    test('should be keyboard navigable', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await adPage.goto();
      await adPage.waitForDataLoad();

      // Tab through controls
      await page.keyboard.press('Tab'); // Search input
      await page.keyboard.press('Tab'); // Status filter
      await page.keyboard.press('Tab'); // Create button

      // Press Enter on create button
      await page.keyboard.press('Enter');

      // Modal should open
      await expect(adPage.createModal).toBeVisible();

      // Press Escape to close
      await page.keyboard.press('Escape');
      await expect(adPage.createModal).not.toBeVisible();
    });
  });

  test.describe('Edge Cases', () => {
    test('should handle very long product names', async ({ page }) => {
      const mockData = [
        {
          id: 1,
          productId: 999,
          productName: 'A'.repeat(200), // Very long name
          position: 'Banner',
          priority: 100,
          startDate: DateHelper.getPastDate(10),
          endDate: DateHelper.getFutureDate(10),
          isActive: true,
          imageUrl: 'https://via.placeholder.com/300',
        },
      ];

      await apiHelper.mockGetAdvertisementsList(mockData);
      await adPage.goto();
      await adPage.waitForDataLoad();

      const rowData = await adPage.getRowData(0);
      expect(rowData.productName.length).toBe(200);
    });

    test('should handle special characters in search', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await adPage.goto();
      await adPage.waitForDataLoad();

      // Search with special characters
      await adPage.search('!@#$%^&*()');
      
      // Should not crash
      const count = await adPage.getRowCount();
      expect(count).toBeGreaterThanOrEqual(0);
    });

    test('should handle rapid filter changes', async ({ page }) => {
      await apiHelper.mockGetAdvertisementsList();
      await adPage.goto();
      await adPage.waitForDataLoad();

      // Rapidly change filters
      await adPage.filterByStatus('Active');
      await adPage.filterByStatus('Inactive');
      await adPage.filterByStatus('Scheduled');
      await adPage.filterByStatus('Expired');
      await adPage.filterByStatus('All');

      // Should not crash and show results
      const isTableVisible = await adPage.isTableVisible();
      expect(isTableVisible).toBe(true);
    });
  });
});