// playwright/pages/AdminBrandPage.ts

import { Page, Locator, Dialog, expect } from '@playwright/test';

export class AdminBrandPage {
  readonly page: Page;
  
  // Main elements
  readonly pageTitle: Locator;
  readonly createButton: Locator;
  readonly searchInput: Locator;
  readonly loadingText: Locator;
  
  // Table elements
  readonly brandTable: Locator;
  readonly tableRows: Locator;
  readonly tableHeaders: Locator;
  
  // Create Modal
  readonly createModal: Locator;
  readonly createModalTitle: Locator;
  readonly createNameInput: Locator;
  readonly createSlugInput: Locator;
  readonly createCountryInput: Locator;
  readonly createImageInput: Locator;
  readonly createSubmitButton: Locator;
  
  // Edit Modal
  readonly editModal: Locator;
  readonly editModalTitle: Locator;
  readonly editNameInput: Locator;
  readonly editSlugInput: Locator;
  readonly editCountryInput: Locator;
  readonly editImageInput: Locator;
  readonly editUpdateButton: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Main elements
    this.pageTitle = page.locator('h2:has-text("Brands")');
    this.createButton = page.locator('button:has-text("Create Brand")');
    this.searchInput = page.locator('input[placeholder*="Search brands"]');
    this.loadingText = page.locator('text=Loading...');
    
    // Table
    this.brandTable = page.locator('table');
    this.tableRows = page.locator('tbody tr');
    this.tableHeaders = page.locator('thead th');
    
    // Create Modal
    this.createModal = page.locator('div[role="dialog"]').filter({ hasText: 'Create Brand' });
    this.createModalTitle = this.createModal.locator('h2:has-text("Create Brand")');
    this.createNameInput = this.createModal.locator('input').first();
    this.createSlugInput = this.createModal.locator('input').nth(1);
    this.createCountryInput = this.createModal.locator('input').nth(2);
    this.createImageInput = this.createModal.locator('input[type="file"]');
    this.createSubmitButton = this.createModal.locator('button:has-text("Create Brand")');
    
    // Edit Modal
    this.editModal = page.locator('div[role="dialog"]').filter({ hasText: 'Edit Brand' });
    this.editModalTitle = this.editModal.locator('h2:has-text("Edit Brand")');
    this.editNameInput = this.editModal.locator('input').first();
    this.editSlugInput = this.editModal.locator('input').nth(1);
    this.editCountryInput = this.editModal.locator('input').nth(2);
    this.editImageInput = this.editModal.locator('input[type="file"]');
    this.editUpdateButton = this.editModal.locator('button:has-text("Update Brand")');
  }

  async goto() {
    await this.page.goto('/dashboard/brand');
    await this.page.waitForLoadState('networkidle');
  }

  async waitForDataLoad() {
    try {
      await this.page.waitForLoadState('networkidle', { timeout: 10000 });
    } catch (error) {
      console.warn('Network did not become idle within 10s, continuing anyway');
    }
    await this.page.waitForTimeout(1000);
  }

  // Search
  async search(query: string) {
    await this.searchInput.fill(query);
    await this.page.waitForTimeout(500);
  }

  async clearSearch() {
    await this.searchInput.clear();
    await this.page.waitForTimeout(500);
  }

  // Table operations
  async getRowCount(): Promise<number> {
    try {
      await this.tableRows.first().waitFor({ state: 'visible', timeout: 5000 });
      return await this.tableRows.count();
    } catch {
      return 0;
    }
  }

  async getRowData(index: number) {
    const row = this.tableRows.nth(index);
    const cells = row.locator('td');
    
    return {
      id: await cells.nth(0).innerText(),
      name: await cells.nth(2).innerText(),
      country: await cells.nth(3).innerText(),
    };
  }

  async hasLogo(index: number): Promise<boolean> {
    const row = this.tableRows.nth(index);
    const logoCell = row.locator('td').nth(1);
    const img = logoCell.locator('img');
    
    try {
      // Wait for image to load (up to 10 seconds)
      await img.waitFor({ state: 'visible', timeout: 10000 });
      return await img.count() > 0;
    } catch {
      return false;
    }
  }

  // Create operations
  async openCreateModal() {
    await this.createButton.click();
    await this.createModal.waitFor({ state: 'visible' });
  }

  async fillCreateForm(data: {
    name: string;
    slug: string;
    country?: string;
    imagePath?: string;
  }) {
    await this.createNameInput.fill(data.name);
    await this.createSlugInput.fill(data.slug);
    
    if (data.country) {
      await this.createCountryInput.fill(data.country);
    }
    
    if (data.imagePath) {
      await this.createImageInput.setInputFiles(data.imagePath);
    }
  }

  async submitCreate() {
    // Handle alert if any
    this.page.once('dialog', dialog => dialog.accept());
    
    await this.createSubmitButton.click();
    
    // Wait for modal to close
    await this.createModal.waitFor({ state: 'hidden', timeout: 15000 });
    
    // Wait for table refresh
    await this.page.waitForLoadState('networkidle');
  }

  async createBrand(data: {
    name: string;
    slug: string;
    country?: string;
    imagePath?: string;
  }) {
    await this.openCreateModal();
    await this.fillCreateForm(data);
    await this.submitCreate();
  }

  // Edit operations
  async openEditModal(rowIndex: number) {
    const row = this.tableRows.nth(rowIndex);
    const editButton = row.locator('button').first(); // Actions component edit button
    
    await editButton.click();
    await this.editModal.waitFor({ state: 'visible' });
  }

  async fillEditForm(data: {
    name?: string;
    slug?: string;
    country?: string;
    imagePath?: string;
  }) {
    if (data.name !== undefined) {
      await this.editNameInput.clear();
      await this.editNameInput.fill(data.name);
    }
    
    if (data.slug !== undefined) {
      await this.editSlugInput.clear();
      await this.editSlugInput.fill(data.slug);
    }
    
    if (data.country !== undefined) {
      await this.editCountryInput.clear();
      await this.editCountryInput.fill(data.country);
    }
    
    if (data.imagePath) {
      await this.editImageInput.setInputFiles(data.imagePath);
    }
  }

  async saveEdit() {
    // Handle any alert/dialog
    this.page.once('dialog', async dialog => {
      console.log('Edit Dialog:', dialog.message());
      await dialog.accept();
    });

    await this.editUpdateButton.click();

    // Wait for API to complete
    await this.page.waitForLoadState('networkidle');
    
    // Wait for modal to close
    try {
      await this.editModal.waitFor({ state: 'hidden', timeout: 10000 });
    } catch (error) {
      console.log('Modal did not close automatically, attempting manual close');
      // Try pressing Escape
      await this.page.keyboard.press('Escape');
      await this.page.waitForTimeout(500);
    }

    // Final wait for table refresh
    await this.page.waitForLoadState('networkidle');
    await this.page.waitForTimeout(1000);
  }

  // Delete operations
async deleteBrand(rowIndex: number) {
  const row = this.tableRows.nth(rowIndex);
  const deleteButton = row.locator('button').nth(1);

  const dialogPromise = this.page.waitForEvent('dialog', { timeout: 3000 })
    .then(dialog => dialog.accept())
    .catch(() => {}); // nếu không có dialog thì không fail

  await deleteButton.click();
  await dialogPromise;

  await this.page.waitForLoadState('networkidle');
  await expect(row).toHaveCount(0);
}



  // Validation
  async isTableVisible(): Promise<boolean> {
    return await this.brandTable.isVisible();
  }

  async isCreateModalVisible(): Promise<boolean> {
    return await this.createModal.isVisible();
  }

  async isEditModalVisible(): Promise<boolean> {
    return await this.editModal.isVisible();
  }

  async verifySearchResults(searchTerm: string): Promise<boolean> {
    const count = await this.getRowCount();
    if (count === 0) return true;
    
    for (let i = 0; i < count; i++) {
      const data = await this.getRowData(i);
      const matchFound = 
        data.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        data.country.toLowerCase().includes(searchTerm.toLowerCase());
      
      if (!matchFound) return false;
    }
    
    return true;
  }
}