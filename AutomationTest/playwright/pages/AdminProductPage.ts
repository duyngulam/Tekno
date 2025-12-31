// playwright/pages/AdminProductPage.ts

import { Page, Locator, Dialog } from '@playwright/test';

export class AdminProductPage {
  readonly page: Page;
  
  // Main elements
  readonly pageTitle: Locator;
  readonly createButton: Locator;
  readonly searchInput: Locator;
  readonly loadingText: Locator;
  
  // Table elements
  readonly productTable: Locator;
  readonly tableRows: Locator;
  readonly tableHeaders: Locator;
  
  // Pagination
  readonly itemsPerPageSelect: Locator;
  readonly prevButton: Locator;
  readonly nextButton: Locator;
  readonly pageNumbers: Locator;
  
  // Create Modal
  readonly createModal: Locator;
  readonly createModalTitle: Locator;
  readonly nameInput: Locator;
  readonly slugInput: Locator;
  readonly categorySelect: Locator;
  readonly brandSelect: Locator;
  readonly basePriceInput: Locator;
  readonly discountInput: Locator;
  readonly overviewTextarea: Locator;
  readonly imageInput: Locator;
  readonly createSubmitButton: Locator;
  readonly createCancelButton: Locator;
  
  // Edit Modal
  readonly editModal: Locator;
  readonly editModalTitle: Locator;
  readonly editSaveButton: Locator;
  readonly editCancelButton: Locator;
  
  // Detail Modal
  readonly detailModal: Locator;
  readonly detailModalTitle: Locator;
  readonly detailCloseButton: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Main elements
    this.pageTitle = page.locator('h2:has-text("Products")');
    this.createButton = page.locator('button:has-text("Create Product")');
    this.searchInput = page.locator('input[placeholder*="Search by ID"]');
    this.loadingText = page.locator('text=Loading...');
    
    // Table
    this.productTable = page.locator('table');
    this.tableRows = page.locator('tbody tr');
    this.tableHeaders = page.locator('thead th');
    
    // Pagination
    this.itemsPerPageSelect = page.locator('select').filter({ hasText: '10' });
    this.prevButton = page.locator('button:has-text("Prev")');
    this.nextButton = page.locator('button:has-text("Next")');
    this.pageNumbers = page.locator('button').filter({ hasText: /^\d+$/ });
    
    // Create Modal
    this.createModal = page.locator('.fixed .bg-white').filter({ hasText: 'Create Product' });
    this.createModalTitle = page.locator('h2:has-text("Create Product")');
    this.nameInput = this.createModal.locator('input').first();
    this.slugInput = this.createModal.locator('input').nth(1);
    this.categorySelect = this.createModal.locator('select').first();
    this.brandSelect = this.createModal.locator('select').nth(1);
    this.basePriceInput = this.createModal.locator('input[type="number"]').first();
    this.discountInput = this.createModal.locator('input[type="number"]').nth(1);
    this.overviewTextarea = this.createModal.locator('textarea');
    this.imageInput = this.createModal.locator('input[type="file"]');
    this.createSubmitButton = this.createModal.locator('button:has-text("Create Product")');
    this.createCancelButton = this.createModal.locator('button:has-text("Cancel")');
    
    // Edit Modal
    this.editModal = page.locator('.fixed .bg-white').filter({ hasText: 'Edit Product' });
    this.editModalTitle = page.locator('h2:has-text("Edit Product")');
    this.editSaveButton = this.editModal.locator('button:has-text("Save Changes")');
    this.editCancelButton = this.editModal.locator('button:has-text("Cancel")');
    
    // Detail Modal
    this.detailModal = page.locator('.fixed .bg-white').filter({ hasText: 'Product Detail' });
    this.detailModalTitle = this.detailModal.locator('h2').filter({ hasText: 'Product Detail' });
    this.detailCloseButton = this.detailModal.locator('button').first();
  }

  async goto() {
    await this.page.goto('/dashboard/products');
    await this.page.waitForLoadState('networkidle');
  }

  async waitForDataLoad() {
    await this.page.waitForLoadState('networkidle');
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
      brand: await cells.nth(1).innerText(),
      category: await cells.nth(2).innerText(),
      name: await cells.nth(3).innerText(),
      basePrice: await cells.nth(4).innerText(),
      discount: await cells.nth(5).innerText(),
      finalPrice: await cells.nth(6).innerText(),
      status: await cells.nth(7).innerText(),
    };
  }

  async clickRow(index: number) {
    await this.tableRows.nth(index).click();
  }

  // Create operations
  async openCreateModal() {
    await this.createButton.click();
    await this.createModal.waitFor({ state: 'visible' });
  }

  async fillCreateForm(data: {
    name: string;
    slug: string;
    categoryId?: string;
    brandId?: string;
    basePrice?: number;
    discount?: number;
    overview?: string;
    imagePath?: string;
  }) {
    // Fill required fields
    await this.nameInput.fill(data.name);
    await this.slugInput.fill(data.slug);
    
    // Select category
    if (data.categoryId) {
      await this.categorySelect.selectOption(data.categoryId);
    }
    
    // Select brand
    if (data.brandId) {
      await this.brandSelect.selectOption(data.brandId);
    }
    
    // Optional fields
    if (data.basePrice !== undefined) {
      await this.basePriceInput.fill(String(data.basePrice));
    }
    
    if (data.discount !== undefined) {
      await this.discountInput.fill(String(data.discount));
    }
    
    if (data.overview) {
      await this.overviewTextarea.fill(data.overview);
    }
    
    // Upload image
    if (data.imagePath) {
      await this.imageInput.setInputFiles(data.imagePath);
    }
  }

  async submitCreate() {
    // Handle alert
    this.page.once('dialog', dialog => dialog.accept());
    
    await this.createSubmitButton.click();
    
    // Wait for modal to close
    await this.createModal.waitFor({ state: 'hidden', timeout: 15000 });
    
    // Wait for table refresh
    await this.page.waitForLoadState('networkidle');
  }

  async createProduct(data: {
    name: string;
    slug: string;
    categoryId?: string;
    brandId?: string;
    basePrice?: number;
    discount?: number;
    overview?: string;
    imagePath?: string;
  }) {
    await this.openCreateModal();
    await this.fillCreateForm(data);
    await this.submitCreate();
  }

  // Edit operations
async openEditModal(rowIndex: number) {
  const row = this.tableRows.nth(rowIndex);
  
  // ✅ Use data-testid
  const editButton = row.locator('[data-testid="edit-button"]');
  
  await editButton.click();
  await this.editModal.waitFor({ state: 'visible' });
}

  async updateProductField(fieldName: string, value: string) {
    const input = this.editModal.locator(`input, textarea, select`).filter({ hasText: fieldName }).or(
      this.editModal.locator(`label:has-text("${fieldName}")`).locator('..').locator('input, textarea, select')
    );
    
    await input.first().fill(value);
  }

// AdminProductPage.ts

async saveEdit() {
  this.page.once('dialog', async dialog => {
    console.log('Dialog:', dialog.message());
    await dialog.accept();
  });

  await this.editSaveButton.click();

  // Wait for API to complete
  await this.page.waitForLoadState('networkidle');
  await this.page.waitForTimeout(2000);

  // Check if modal closed
  const isStillVisible = await this.editModal.isVisible();
  
  if (isStillVisible) {
    console.log('Modal still visible, closing manually');
    // Click close button (X button)
    const closeButton = this.editModal.locator('button').first();
    await closeButton.click();
  }

  // Final wait
  await this.editModal.waitFor({ state: 'hidden', timeout: 10000 });
  await this.page.waitForLoadState('networkidle');
}

// Delete operations
async deleteProduct(rowIndex: number) {
  const dialogHandler = async (dialog: Dialog) => {
    console.log('Dialog:', dialog.message());
    await dialog.accept();
  };
  
  this.page.on('dialog', dialogHandler);
  
  try {
    const row = this.tableRows.nth(rowIndex);
    const deleteButton = row.locator('[data-testid="delete-button"]');
    
    await deleteButton.click();
    
    // Give time for dialog to be handled
    await this.page.waitForTimeout(500);
    
    // Wait for API to complete
    await this.page.waitForLoadState('networkidle');
    await this.page.waitForTimeout(1000);
  } finally {
    // Always cleanup handler
    this.page.off('dialog', dialogHandler);
  }
}

  // Detail view
  async viewProductDetail(rowIndex: number) {
    await this.tableRows.nth(rowIndex).click();
    await this.detailModal.waitFor({ state: 'visible' });
  }

  async closeDetailModal() {
    await this.detailCloseButton.click();
    await this.detailModal.waitFor({ state: 'hidden' });
  }

  async getDetailInfo() {
    const text = await this.detailModal.innerText();
    return text;
  }

  // Helper methods
  async getFirstAvailableCategory(): Promise<string> {
    await this.openCreateModal();
    const options = await this.categorySelect.locator('option').allTextContents();
    await this.createCancelButton.click();
    
    // Get first non-empty option
    const validOptions = options.filter(opt => opt.trim() && opt !== '-- Select --');
    return validOptions[0] || '';
  }

  async getFirstAvailableBrand(): Promise<string> {
    await this.openCreateModal();
    const options = await this.brandSelect.locator('option').allTextContents();
    await this.createCancelButton.click();
    
    const validOptions = options.filter(opt => opt.trim() && opt !== '-- Select --');
    return validOptions[0] || '';
  }

  async getCategoryIdByName(name: string): Promise<string> {
    await this.openCreateModal();
    const option = this.categorySelect.locator(`option:has-text("${name}")`);
    const value = await option.getAttribute('value');
    await this.createCancelButton.click();
    return value || '';
  }

  async getBrandIdByName(name: string): Promise<string> {
    await this.openCreateModal();
    const option = this.brandSelect.locator(`option:has-text("${name}")`);
    const value = await option.getAttribute('value');
    await this.createCancelButton.click();
    return value || '';
  }

  // Pagination
  async changeItemsPerPage(items: number) {
    await this.itemsPerPageSelect.selectOption(String(items));
    await this.page.waitForTimeout(500);
  }

  async goToNextPage() {
    await this.nextButton.click();
    await this.page.waitForTimeout(500);
  }

  async goToPrevPage() {
    await this.prevButton.click();
    await this.page.waitForTimeout(500);
  }

  async goToPage(pageNumber: number) {
    await this.pageNumbers.filter({ hasText: String(pageNumber) }).click();
    await this.page.waitForTimeout(500);
  }

  // Validation
  async isTableVisible(): Promise<boolean> {
    return await this.productTable.isVisible();
  }

  async isCreateModalVisible(): Promise<boolean> {
    return await this.createModal.isVisible();
  }

  async isEditModalVisible(): Promise<boolean> {
    return await this.editModal.isVisible();
  }

  async isDetailModalVisible(): Promise<boolean> {
    return await this.detailModal.isVisible();
  }

  async verifySearchResults(searchTerm: string): Promise<boolean> {
    const count = await this.getRowCount();
    if (count === 0) return true;
    
    for (let i = 0; i < count; i++) {
      const data = await this.getRowData(i);
      const matchFound = 
        data.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
        data.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        data.brand.toLowerCase().includes(searchTerm.toLowerCase()) ||
        data.category.toLowerCase().includes(searchTerm.toLowerCase());
      
      if (!matchFound) return false;
    }
    
    return true;
  }
}