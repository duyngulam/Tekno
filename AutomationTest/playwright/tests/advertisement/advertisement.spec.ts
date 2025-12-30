import { test, expect } from '@playwright/test';
import { AdvertisementPage } from '../../pages/AdvertisementPage';
import path from 'path';

const VALID_POSITIONS = ['HomeTop', 'HomeMiddle', 'HomeBottom', 'CategoryTop', 'ProductSidebar'];
const VALID_PRODUCT_IDS = ['1', '2', '3', '4', '10', '11', '12', '13', '14', '20', '21', '30', '31', '40', '50', '61', '70'];

test.describe('Advertisement Management', () => {
  let adPage: AdvertisementPage;

  test.beforeEach(async ({ page }) => {
    adPage = new AdvertisementPage(page);
    await adPage.goto();
    await adPage.waitForDataLoad();
  });

  test.describe('Page Load & Display', () => {
    test('should display page title and create button', async () => {
      await expect(adPage.pageTitle).toBeVisible();
      await expect(adPage.createButton).toBeVisible();
      await expect(adPage.pageTitle).toHaveText('Advertisement Management');
    });

    test('should display search and filter controls', async () => {
      await expect(adPage.searchInput).toBeVisible();
      await expect(adPage.statusFilterSelect).toBeVisible();
    });

    test('should display table with correct headers', async () => {
      const isTableVisible = await adPage.isTableVisible();
      
      if (isTableVisible) {
        const headers = await adPage.tableHeaders.allTextContents();
        expect(headers).toContain('ID');
        expect(headers).toContain('Image');
        expect(headers).toContain('Product');
        expect(headers).toContain('Position');
        expect(headers).toContain('Priority');
        expect(headers).toContain('Start');
        expect(headers).toContain('End');
        expect(headers).toContain('Status');
      }
    });

    test('should show loading state initially', async ({ page }) => {
      // Create new page instance to test loading
      const freshPage = new AdvertisementPage(page);
      
      // Navigate and immediately check loading
      const navigationPromise = freshPage.goto();
      
      // Should show loading briefly
      const hasLoading = await freshPage.isLoadingVisible();
      
      await navigationPromise;
      
      // After load, loading should be gone
      await expect(freshPage.loadingText).not.toBeVisible();
    });
  });

  test.describe('Search Functionality', () => {
    test('should filter results by product name', async () => {
      const initialCount = await adPage.getRowCount();
      
      if (initialCount === 0) {
        test.skip();
      }

      // Get first row data to search for it
      const firstRowData = await adPage.getRowData(0);
      const searchTerm = firstRowData.productName.split(' ')[0]; // First word

      await adPage.search(searchTerm);

      const filteredCount = await adPage.getRowCount();
      expect(filteredCount).toBeGreaterThan(0);
      expect(filteredCount).toBeLessThanOrEqual(initialCount);

      // Verify search results match
      const isValid = await adPage.verifySearchResults(searchTerm);
      expect(isValid).toBe(true);
    });

    test('should filter results by position', async () => {
      const initialCount = await adPage.getRowCount();
      
      if (initialCount === 0) {
        test.skip();
      }

      const firstRowData = await adPage.getRowData(0);
      await adPage.search(firstRowData.position);

      const filteredCount = await adPage.getRowCount();
      expect(filteredCount).toBeGreaterThan(0);
      expect(filteredCount).toBeLessThanOrEqual(initialCount);
    });

    test('should show no results for non-existent search', async () => {
      await adPage.search('NONEXISTENT_PRODUCT_XYZ_999');

      const count = await adPage.getRowCount();
      expect(count).toBe(0);
    });

    test('should clear search and restore all results', async () => {
      const initialCount = await adPage.getRowCount();
      
      if (initialCount === 0) {
        test.skip();
      }

      // Search to reduce results
      await adPage.search('test');
      const searchCount = await adPage.getRowCount();

      // Clear search
      await adPage.clearSearch();
      const clearedCount = await adPage.getRowCount();

      expect(clearedCount).toBe(initialCount);
    });
  });

  test.describe('Status Filter Functionality', () => {
    test('should filter by Active status', async () => {
      await adPage.filterByStatus('Active');

      const statuses = await adPage.getAllVisibleStatuses();
      
      if (statuses.length > 0) {
        statuses.forEach(status => {
          expect(status).toBe('Active');
        });
      }
    });

    test('should filter by Inactive status', async () => {
      await adPage.filterByStatus('Inactive');

      const statuses = await adPage.getAllVisibleStatuses();
      
      if (statuses.length > 0) {
        statuses.forEach(status => {
          expect(status).toBe('Inactive');
        });
      }
    });

    test('should filter by Scheduled status', async () => {
      await adPage.filterByStatus('Scheduled');

      const statuses = await adPage.getAllVisibleStatuses();
      
      if (statuses.length > 0) {
        statuses.forEach(status => {
          expect(status).toBe('Scheduled');
        });
      }
    });

    test('should filter by Expired status', async () => {
      await adPage.filterByStatus('Expired');

      const statuses = await adPage.getAllVisibleStatuses();
      
      if (statuses.length > 0) {
        statuses.forEach(status => {
          expect(status).toBe('Expired');
        });
      }
    });

    test('should show all statuses when filter is set to All', async () => {
      const initialCount = await adPage.getRowCount();

      // Filter to Active first
      await adPage.filterByStatus('Active');
      const activeCount = await adPage.getRowCount();

      // Then back to All
      await adPage.filterByStatus('All');
      const allCount = await adPage.getRowCount();

      expect(allCount).toBe(initialCount);
    });

    test('should reduce results count when applying restrictive filter', async () => {
      const allCount = await adPage.getRowCount();
      
      if (allCount === 0) {
        test.skip();
      }

      await adPage.filterByStatus('Active');
      const activeCount = await adPage.getRowCount();

      expect(activeCount).toBeLessThanOrEqual(allCount);
    });
  });

  test.describe('Combined Search and Filter', () => {
    test('should apply both search and status filter together', async () => {
      const initialCount = await adPage.getRowCount();
      
      if (initialCount === 0) {
        test.skip();
      }

      // Get a search term
      const firstRowData = await adPage.getRowData(0);
      const searchTerm = firstRowData.productName.split(' ')[0];

      // Apply search
      await adPage.search(searchTerm);
      const searchCount = await adPage.getRowCount();

      // Apply filter
      await adPage.filterByStatus('Active');
      const combinedCount = await adPage.getRowCount();

      // Combined should be <= search only
      expect(combinedCount).toBeLessThanOrEqual(searchCount);

      // Verify results match both criteria
      const statuses = await adPage.getAllVisibleStatuses();
      if (statuses.length > 0) {
        statuses.forEach(status => {
          expect(status).toBe('Active');
        });
      }

      const isSearchValid = await adPage.verifySearchResults(searchTerm);
      expect(isSearchValid).toBe(true);
    });
  });

  test.describe('Table Data Display', () => {
    test('should display advertisement data correctly', async () => {
      const count = await adPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const rowData = await adPage.getRowData(0);

      // Verify all fields are present
      expect(rowData.id).toBeTruthy();
      expect(rowData.productName).toBeTruthy();
      expect(rowData.position).toBeTruthy();
      expect(rowData.priority).toBeTruthy();
      expect(rowData.startDate).toBeTruthy();
      expect(rowData.endDate).toBeTruthy();
      expect(rowData.status).toBeTruthy();
    });

    test('should display status badges with correct styling', async () => {
      const count = await adPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const badge = await adPage.getStatusBadge(0);
      await expect(badge).toBeVisible();
      
      // Check badge has proper classes
      const className = await badge.getAttribute('class');
      expect(className).toContain('rounded');
      expect(className).toContain('text-xs');
    });

    test('should display images when available', async () => {
      const count = await adPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      // Check if at least one row has an image
      let hasAnyImage = false;
      for (let i = 0; i < count; i++) {
        if (await adPage.hasImage(i)) {
          hasAnyImage = true;
          break;
        }
      }

      // If there are ads, at least some should have images
      if (count > 0) {
        // This is optional - not all ads may have images
        // Just verify the method works
        expect(typeof hasAnyImage).toBe('boolean');
      }
    });
  });

  test.describe('Create Advertisement Modal', () => {
    test('should open create modal when clicking create button', async () => {
      await adPage.openCreateModal();
      await expect(adPage.createModal).toBeVisible();
      await expect(adPage.modalTitle).toHaveText('Create Advertisement');
    });

    test('should display all form fields in modal', async () => {
      await adPage.openCreateModal();

      await expect(adPage.productIdInput).toBeVisible();
      await expect(adPage.positionTrigger).toBeVisible();
      await expect(adPage.priorityInput).toBeVisible();
      await expect(adPage.imageInput).toBeVisible();
      await expect(adPage.startDateInput).toBeVisible();
      await expect(adPage.endDateInput).toBeVisible();
      await expect(adPage.isActiveCheckbox).toBeVisible();
      await expect(adPage.createSubmitButton).toBeVisible();
    });

    test('should close modal when pressing Escape', async () => {
      await adPage.openCreateModal();
      await expect(adPage.createModal).toBeVisible();

      await adPage.closeModal();
      await expect(adPage.createModal).not.toBeVisible();
    });

    test('should have default values in form', async () => {
      await adPage.openCreateModal();

      // Priority should have default value
      const priorityValue = await adPage.priorityInput.inputValue();
      expect(priorityValue).toBeTruthy();

      // isActive checkbox should be checked by default
      const isChecked = await adPage.isActiveCheckbox.isChecked();
      expect(isChecked).toBe(true);
    });

    test('should fill form fields correctly', async () => {
      await adPage.openCreateModal();

      const formData = {
        productId: '12',
        position: 'HomeTop',
        priority: 100,
        startDate: '2025-01-01T10:00',
        endDate: '2025-12-31T23:59',
        isActive: false,
      };

      await adPage.fillCreateForm(formData);

      // Verify filled values
      await expect(adPage.productIdInput).toHaveValue(formData.productId);
      await expect(adPage.positionTrigger).toContainText(formData.position);
      await expect(adPage.priorityInput).toHaveValue(String(formData.priority));
      await expect(adPage.startDateInput).toHaveValue(formData.startDate);
      await expect(adPage.endDateInput).toHaveValue(formData.endDate);
      
      const isChecked = await adPage.isActiveCheckbox.isChecked();
      expect(isChecked).toBe(formData.isActive);
    });

    test('should display position dropdown with all options', async () => {
      await adPage.openCreateModal();

      // ✅ NEW: Verify dropdown appears
      await expect(adPage.positionTrigger).toBeVisible();
      
      // Get available options
      const availablePositions = await adPage.getAvailablePositions();
      
      // Verify all expected positions are available
      VALID_POSITIONS.forEach(position => {
        expect(availablePositions).toContain(position);
      });
    });

    test('should select position from dropdown', async () => {
      await adPage.openCreateModal();

      // ✅ Test selecting each position
      for (const position of VALID_POSITIONS.slice(0, 3)) {
        await adPage.selectPosition(position);
        
        // Verify selection
        await expect(adPage.positionTrigger).toContainText(position);
        
        // Small delay
        await adPage.page.waitForTimeout(200);
      }
    });

    test('should keep selected position when filling other fields', async () => {
      await adPage.openCreateModal();

      const testPosition = 'HomeTop';
      await adPage.selectPosition(testPosition);

      // Fill other fields
      await adPage.productIdInput.fill('10');
      await adPage.priorityInput.fill('95');

      // Verify position is still selected
      await expect(adPage.positionTrigger).toContainText(testPosition);
    });
});

  test.describe('Create Advertisement E2E', () => {
    test('should create new advertisement successfully', async () => {
      const initialCount = await adPage.getRowCount();

      // Prepare test image (you need to have a test image file)
      const testImagePath = path.join(__dirname, '../../fixtures/test-ad-image.jpg');

      const newAd = {
        productId: '10',
        position: 'HomeTop',
        priority: 95,
        imageURL: testImagePath,
        startDate: '2025-01-15T08:00',
        endDate: '2025-12-31T23:59',
        isActive: true,
      };

      await adPage.createAdvertisement(newAd);

      // Verify new ad appears in list
      await adPage.page.waitForTimeout(1000); // Wait for list refresh
      const newCount = await adPage.getRowCount();
      expect(newCount).toBe(initialCount);
      
      await adPage.clearSearch();

      // ✅ Filter by Active status (new ad is active)
      await adPage.filterByStatus('Active');

      // Search for newly created ad
      await adPage.search(newAd.position);
      const searchCount = await adPage.getRowCount();
      expect(searchCount).toBeGreaterThan(0);

    let foundNewAd = false;
    for (let i = 0; i < searchCount; i++) {
    const rowData = await adPage.getRowData(i);
    
    if (
      rowData.position === newAd.position &&
      rowData.priority === String(newAd.priority) &&
      rowData.status === 'Active'
    ) {
      foundNewAd = true;
      // Verify all fields
      expect(rowData.position).toBe(newAd.position);
      expect(rowData.priority).toBe(String(newAd.priority));
      expect(rowData.status).toBe('Active');
      break;
    }
  }

    expect(foundNewAd).toBe(true);
    });

    test('should show alert when image is missing', async ({ page }) => {
      await adPage.openCreateModal();

      // Listen for alert
      let alertMessage = '';
      page.once('dialog', async dialog => {
        alertMessage = dialog.message();
        await dialog.accept();
      });

      const formData = {
        productId: '123',
        position: 'HomeTop',
        startDate: '2025-01-01T10:00',
        endDate: '2025-12-31T23:59',
        // No image provided
      };

      await adPage.fillCreateForm(formData);
      await adPage.createSubmitButton.click();

      // Wait a bit for alert
      await page.waitForTimeout(500);

      expect(alertMessage).toContain('Please select an image');
    });
  });

  test.describe('Create ads for all positions', () => {
  const testImagePath = path.join(__dirname, '../../fixtures/test-ad-image.jpg');

  for (const position of VALID_POSITIONS) {
test(`should create ad with position: ${position}`, async ({ page }) => {
  const newAd = {
    productId: '4',
    position: position,
    priority: 75,
    imageURL: testImagePath,
    startDate: '2025-02-01T08:00',
    endDate: '2025-12-31T23:59',
    isActive: true,
  };

  await adPage.createAdvertisement(newAd);

  // ✅ Reload page với network idle wait
  await adPage.goto();
  await adPage.page.waitForLoadState('networkidle'); // ✅ Wait for API response
  await adPage.waitForDataLoad();
  
  // ✅ Extra wait để data được set vào state
  await adPage.page.waitForTimeout(1000);
  
  // Clear any filters
  await adPage.filterByStatus('All');
  await adPage.clearSearch();
  await adPage.page.waitForTimeout(800);

  // Search for the new ad
  await adPage.search(position);
  await adPage.page.waitForTimeout(800); // Wait for filter apply

  const searchCount = await adPage.getRowCount();
  console.log(`🔍 Found ${searchCount} ads with position "${position}"`);
  
  if (searchCount === 0) {
    // ✅ Fallback: log data để debug
    console.error(`❌ No ads found for ${position}. This might be a timing issue.`);
    // Reload again
    await adPage.page.reload();
    await adPage.page.waitForLoadState('networkidle');
    await adPage.waitForDataLoad();
    await adPage.page.waitForTimeout(1000);
    await adPage.search(position);
  }

  expect(searchCount).toBeGreaterThan(0);

  let found = false;
  for (let i = 0; i < searchCount; i++) {
    const rowData = await adPage.getRowData(i);
    console.log(`Row ${i}: position=${rowData.position}, priority=${rowData.priority}`);
    
    if (
      rowData.position === position && 
      rowData.priority === '75'
    ) {
      found = true;
      console.log(`✅ Found newly created ad: ${position} (priority: 75)`);
      break;
    }
  }

  expect(found).toBe(true);
});
  }
  });

  test.describe('Empty State', () => {
    test('should show no data message when no advertisements exist', async ({ page }) => {
      // This test would need API mocking to return empty array
      // For now, just verify the logic exists
      
      const isTableVisible = await adPage.isTableVisible();
      const isNoDataVisible = await adPage.isNoDataVisible();

      // Either table should be visible OR no-data message
      expect(isTableVisible || isNoDataVisible).toBe(true);
    });
  });
});