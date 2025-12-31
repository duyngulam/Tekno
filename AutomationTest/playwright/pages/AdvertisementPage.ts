import { Page, Locator } from '@playwright/test';

export class AdvertisementPage {
  readonly page: Page;
  
  // Main elements
  readonly pageTitle: Locator;
  readonly createButton: Locator;
  readonly searchInput: Locator;
  readonly statusFilterSelect: Locator;
  readonly loadingText: Locator;
  readonly noDataText: Locator;
  
  // Table elements
  readonly advertisementTable: Locator;
  readonly tableRows: Locator;
  readonly tableHeaders: Locator;
  
  // Modal elements
  readonly createModal: Locator;
  readonly modalTitle: Locator;
  readonly productIdInput: Locator;
  readonly positionTrigger: Locator;
  readonly positionOptions: Locator;
  readonly priorityInput: Locator;
  readonly imageInput: Locator;
  readonly startDateInput: Locator;
  readonly endDateInput: Locator;
  readonly isActiveCheckbox: Locator;
  readonly createSubmitButton: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Main page elements
    this.pageTitle = page.locator('h2:has-text("Advertisement")');
    this.createButton = page.locator('button:has-text("Create Advertisement")');
    this.searchInput = page.locator('input[placeholder*="Search by product name"]');
    this.statusFilterSelect = page.locator('select').filter({ hasText: 'All Status' });
    this.loadingText = page.locator('text=Loading Advertisements...');
    this.noDataText = page.locator('text=No advertisements found');
    
    // Table
    this.advertisementTable = page.locator('table');
    this.tableRows = page.locator('tbody tr');
    this.tableHeaders = page.locator('thead th');
    
    // Modal
    this.createModal = page.locator('[role="dialog"]');
    this.modalTitle = page.locator('h2:has-text("Create Advertisement")');
    this.productIdInput = page.locator('input[placeholder="Enter product ID"]');
    this.positionTrigger = page.locator('button:has-text("Select position")').or(
      page.locator('[role="combobox"]').filter({ hasText: /HomeTop|HomeMiddle|HomeBottom|CategoryTop|ProductSidebar|Select position/i })
    );
    this.positionOptions = page.locator('[role="option"]');
    this.priorityInput = page.locator('input[placeholder*="Priority"]');
    this.imageInput = page.locator('input[type="file"]');
    this.startDateInput = page.locator('input[type="datetime-local"]').first();
    this.endDateInput = page.locator('input[type="datetime-local"]').last();
    this.isActiveCheckbox = page.locator('input#isActive');
    this.createSubmitButton = page.locator('button:has-text("Create Advertisement")').last();
  }

  // Navigation
  async goto() {
    await this.page.goto('/dashboard/advertisement');
    // Wait for key element
    await this.pageTitle.waitFor({ state: 'visible', timeout: 10000 });
  }

  // Wait for data to load
async waitForDataLoad() {
    // Wait for table to be visible
  await this.advertisementTable.waitFor({ 
    state: 'visible', 
    timeout: 10000 
  }).catch(() => {
    // Or no data is fine too
  });
  await this.page.waitForTimeout(500); // Buffer time cho client-side rendering
}

  // Search functionality
  async search(query: string) {
    await this.searchInput.fill(query);
    // Give time for filtering to happen (client-side)
    await this.page.waitForTimeout(500);
  }

  async clearSearch() {
    await this.searchInput.clear();
    await this.page.waitForTimeout(500);
  }

  // Filter functionality
  async filterByStatus(status: 'All' | 'Active' | 'Inactive' | 'Scheduled' | 'Expired') {
    await this.statusFilterSelect.selectOption(status);
    await this.page.waitForTimeout(500);
  }

  // Table operations
  async getRowCount(): Promise<number> {
    try {
      return await this.tableRows.count();
    } catch {
      return 0;
    }
  }

  async getRowByIndex(index: number) {
    return this.tableRows.nth(index);
  }

  async getRowData(index: number) {
    const row = await this.getRowByIndex(index);
    const cells = row.locator('td');
    
    return {
      id: await cells.nth(0).innerText(),
      productName: await cells.nth(2).locator('.font-medium').innerText(),
      productId: await cells.nth(2).locator('.text-xs').innerText(),
      position: await cells.nth(3).innerText(),
      priority: await cells.nth(4).innerText(),
      startDate: await cells.nth(5).innerText(),
      endDate: await cells.nth(6).innerText(),
      status: await cells.nth(7).innerText(),
    };
  }

  async getStatusBadge(index: number) {
    const row = await this.getRowByIndex(index);
    return row.locator('td').nth(7).locator('span');
  }

  async hasImage(index: number): Promise<boolean> {
    const row = await this.getRowByIndex(index);
    const img = row.locator('td').nth(1).locator('img');
    return await img.count() > 0;
  }

  // Modal operations
  async openCreateModal() {
    await this.createButton.click();
    await this.createModal.waitFor({ state: 'visible' });
  }

  async closeModal() {
    // Click outside modal or press Escape
    await this.page.keyboard.press('Escape');
    await this.createModal.waitFor({ state: 'hidden' });
  }

  async selectPosition(position: string) {
    // Click trigger to open dropdown
    await this.positionTrigger.click();
    
    // Wait for options to appear
    await this.page.waitForSelector('[role="option"]', { state: 'visible' });
    
    // Click the option with matching value or text
    await this.page.locator(`[role="option"]:has-text("${position}")`).click();
    
    // Wait for dropdown to close
    await this.page.waitForTimeout(300);
  }

  async fillCreateForm(data: {
    productId: string;
    position: string;
    priority?: number;
    imageURL?: string;
    startDate: string;
    endDate: string;
    isActive?: boolean;
  }) {
    await this.productIdInput.fill(data.productId);
    await this.selectPosition(data.position);
    
    if (data.priority !== undefined) {
      await this.priorityInput.fill(String(data.priority));
    }
    
    if (data.imageURL) {
      await this.imageInput.setInputFiles(data.imageURL);
    }
    
    await this.startDateInput.fill(data.startDate);
    await this.endDateInput.fill(data.endDate);
    
    if (data.isActive !== undefined) {
      const isChecked = await this.isActiveCheckbox.isChecked();
      if (data.isActive !== isChecked) {
        await this.isActiveCheckbox.click();
      }
    }
  }

// AdvertisementPage.ts

async submitCreateForm() {
  // 1. Setup response listener
  const responsePromise = this.page.waitForResponse(
    response => {
      const url = response.url();
      const method = response.request().method();
      return url.includes('/api/admin/advertisements') && method === 'POST';
    },
    { timeout: 15000 }
  );

  // 2. Setup dialog handler
  let dialogShown = false;
  this.page.once('dialog', async (dialog) => {
    dialogShown = true;
    console.log('Dialog message:', dialog.message());
    await dialog.accept();
  });

  // 3. Click submit button
  await this.createSubmitButton.click();

  // 4. Wait for API response
  let apiSuccess = false;
  try {
    const response = await responsePromise;
    apiSuccess = response.ok();
    
    if (!apiSuccess) {
      const body = await response.text();
      console.error('API Error:', response.status(), body);
    }
  } catch (error) {
    console.error('API timeout or error:', error);
  }

  // 5. Wait for dialog if API succeeded
  if (apiSuccess && !dialogShown) {
    await this.page.waitForTimeout(1000); // Give dialog time to appear
  }


// 6. Close modal after successful creation
if (apiSuccess) {
  try {
    await this.createModal.waitFor({ state: 'hidden', timeout: 2000 });
  } catch {
    // Force close with Escape
    await this.page.keyboard.press('Escape');
    await this.page.waitForTimeout(300);
  }
} else {
  throw new Error('Advertisement creation failed - API error');
}

  // 7. Wait for table to refresh  
  await this.page.waitForTimeout(500);
}

  async createAdvertisement(data: {
    productId: string;
    position: string;
    priority?: number;
    imagePath?: string;
    startDate: string;
    endDate: string;
    isActive?: boolean;
  }) {
    await this.openCreateModal();
    await this.fillCreateForm(data);
    await this.submitCreateForm();
  }

  // Validation helpers
  async isTableVisible(): Promise<boolean> {
    return await this.advertisementTable.isVisible();
  }

  async isNoDataVisible(): Promise<boolean> {
    return await this.noDataText.isVisible();
  }

  async isLoadingVisible(): Promise<boolean> {
    try {
      return await this.loadingText.isVisible({ timeout: 1000 });
    } catch {
      return false;
    }
  }

  async getAvailablePositions(): Promise<string[]> {
    await this.positionTrigger.click();
    await this.page.waitForSelector('[role="option"]', { state: 'visible' });
    
    const options = await this.positionOptions.allTextContents();
    
    // Close dropdown
    await this.page.keyboard.press('Escape');
    
    return options;
  }

  // Get all statuses from visible rows
  async getAllVisibleStatuses(): Promise<string[]> {
    const count = await this.getRowCount();
    const statuses: string[] = [];
    
    for (let i = 0; i < count; i++) {
      const badge = await this.getStatusBadge(i);
      statuses.push(await badge.innerText());
    }
    
    return statuses;
  }

  // Search result verification
  async verifySearchResults(searchTerm: string): Promise<boolean> {
    const count = await this.getRowCount();
    
    if (count === 0) return true; // No results is valid
    
    for (let i = 0; i < count; i++) {
      const data = await this.getRowData(i);
      const matchFound = 
        data.productName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        data.position.toLowerCase().includes(searchTerm.toLowerCase()) ||
        data.id.includes(searchTerm);
      
      if (!matchFound) return false;
    }
    
    return true;
  }
}