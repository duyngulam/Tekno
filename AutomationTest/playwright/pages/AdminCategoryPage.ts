// playwright/pages/AdminCategoryPage.ts

import { Page, Locator, expect } from '@playwright/test';

export class AdminCategoryPage {
  readonly page: Page;
  
  // Main elements
  readonly pageTitle: Locator;
  readonly totalCategoriesText: Locator;
  readonly createButton: Locator;
  readonly globalAttributesButton: Locator;
  readonly createAttributeButton: Locator;
  readonly loadingIndicator: Locator;
  
  // Table elements
  readonly categoryTable: Locator;
  readonly tableRows: Locator;
  readonly tableHeaders: Locator;
  
  // Create Modal
  readonly createModal: Locator;
  readonly createModalTitle: Locator;
  readonly createNameInput: Locator;
  readonly createSlugInput: Locator;
  readonly createParentSelect: Locator;
  readonly createIconInput: Locator;
  readonly createImageInput: Locator;
  readonly createSubmitButton: Locator;
  readonly createCancelButton: Locator;
  
  // Edit Modal
  readonly editModal: Locator;
  readonly editModalTitle: Locator;
  readonly editNameInput: Locator;
  readonly editSlugInput: Locator;
  readonly editParentSelect: Locator;
  readonly editIconInput: Locator;
  readonly editImageInput: Locator;
  readonly editSaveButton: Locator;
  readonly editCancelButton: Locator;
  
  // Attributes Modal
  readonly attributesModal: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Main elements
    this.pageTitle = page.locator('h1:has-text("Quản lý Categories")');
    this.totalCategoriesText = page.locator('text=/Tổng số: \\d+ categories/');
    this.createButton = page.locator('button:has-text("Tạo Category")');
    this.globalAttributesButton = page.locator('button:has-text("Global Attributes")');
    this.createAttributeButton = page.locator('button:has-text("Create Attribute")');
    this.loadingIndicator = page.locator('text=Đang tải...');
    
    // Table
    this.categoryTable = page.locator('table');
    this.tableRows = page.locator('tbody tr');
    this.tableHeaders = page.locator('thead th');
    
    // Create Modal
    this.createModal = page.locator('div[role="dialog"]').filter({ hasText: 'Tạo Category Mới' });
    this.createModalTitle = this.createModal.locator('h2:has-text("Tạo Category Mới")');
    this.createNameInput = this.createModal.locator('input').first();
    this.createSlugInput = this.createModal.locator('input').nth(1);
    this.createParentSelect = this.createModal.locator('select');
    this.createIconInput = this.createModal.locator('input[type="file"]').first();
    this.createImageInput = this.createModal.locator('input[type="file"]').nth(1);
    this.createSubmitButton = this.createModal.locator('button:has-text("Tạo Category")');
    this.createCancelButton = this.createModal.locator('button:has-text("Hủy")');
    
    // Edit Modal
    this.editModal = page.locator('div[role="dialog"]').filter({ hasText: 'Chỉnh sửa Category' });
    this.editModalTitle = this.editModal.locator('h2:has-text("Chỉnh sửa Category")');
    this.editNameInput = this.editModal.locator('input').first();
    this.editSlugInput = this.editModal.locator('input').nth(1);
    this.editParentSelect = this.editModal.locator('select');
    this.editIconInput = this.editModal.locator('input[type="file"]').first();
    this.editImageInput = this.editModal.locator('input[type="file"]').nth(1);
    this.editSaveButton = this.editModal.locator('button:has-text("Lưu thay đổi")');
    this.editCancelButton = this.editModal.locator('button:has-text("Hủy")');
    
    // Attributes Modal
    this.attributesModal = page.locator('div[role="dialog"]').filter({ hasText: 'Quản lý Attribute' });
  }

  async goto() {
    await this.page.goto('/dashboard/category');
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

  // Tree expansion operations
  async expandCategory(rowIndex: number) {
    const row = this.tableRows.nth(rowIndex);
    const expandButton = row.locator('button').first();
    
    // Check if it has chevron icon (has children)
    const hasChevron = await expandButton.locator('svg').count() > 0;
    
    if (hasChevron) {
      await expandButton.click();
      await this.page.waitForTimeout(300);
    }
  }

  async collapseCategory(rowIndex: number) {
    const row = this.tableRows.nth(rowIndex);
    const expandButton = row.locator('button').first();
    
    const hasChevron = await expandButton.locator('svg').count() > 0;
    
    if (hasChevron) {
      await expandButton.click();
      await this.page.waitForTimeout(300);
    }
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
  
  // Remove "#" prefix from ID
  const idText = await cells.nth(1).innerText();
  const cleanId = idText.replace('#', '').trim();
  
  return {
    id: cleanId,  // Return "2" instead of "#2"
    name: await cells.nth(4).innerText(),
    slug: await cells.nth(5).innerText(),
    childrenCount: await cells.nth(6).innerText(),
  };
}

  async hasIcon(index: number): Promise<boolean> {
    const row = this.tableRows.nth(index);
    const iconCell = row.locator('td').nth(2);
    const img = iconCell.locator('img');
    
    try {
      await img.waitFor({ state: 'visible', timeout: 5000 });
      return await img.count() > 0;
    } catch {
      return false;
    }
  }

  async hasImage(index: number): Promise<boolean> {
    const row = this.tableRows.nth(index);
    const imageCell = row.locator('td').nth(3);
    const img = imageCell.locator('img');
    
    try {
      await img.waitFor({ state: 'visible', timeout: 5000 });
      return await img.count() > 0;
    } catch {
      return false;
    }
  }

  async getChildrenCount(index: number): Promise<number> {
    const row = this.tableRows.nth(index);
    const childrenCell = row.locator('td').nth(6);
    const text = await childrenCell.innerText();
    return parseInt(text.trim()) || 0;
  }

  // Create operations
  async openCreateModal() {
    await this.createButton.click();
    await this.createModal.waitFor({ state: 'visible' });
  }

  async fillCreateForm(data: {
    name: string;
    slug: string;
    parentId?: string;
    iconPath?: string;
    imagePath?: string;
  }) {
    await this.createNameInput.fill(data.name);
    await this.createSlugInput.fill(data.slug);
    
    if (data.parentId) {
      await this.createParentSelect.selectOption(data.parentId);
    }
    
    if (data.iconPath) {
      await this.createIconInput.setInputFiles(data.iconPath);
    }
    
    if (data.imagePath) {
      await this.createImageInput.setInputFiles(data.imagePath);
    }
  }

  async submitCreate() {
    this.page.once('dialog', dialog => dialog.accept());
    
    await this.createSubmitButton.click();
    
    await this.createModal.waitFor({ state: 'hidden', timeout: 15000 });
    await this.page.waitForLoadState('networkidle');
  }

  async createCategory(data: {
    name: string;
    slug: string;
    parentId?: string;
    iconPath?: string;
    imagePath?: string;
  }) {
    await this.openCreateModal();
    await this.fillCreateForm(data);
    await this.submitCreate();
  }

  // Edit operations
  async openEditModal(rowIndex: number) {
    const row = this.tableRows.nth(rowIndex);
    const actionsCell = row.locator('td').last();
    const editButton = actionsCell.locator('button').nth(1);
    
    await editButton.click();
    await this.editModal.waitFor({ state: 'visible' });
  }

  async fillEditForm(data: {
    name?: string;
    slug?: string;
    parentId?: string;
    iconPath?: string;
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
    
    if (data.parentId !== undefined) {
      await this.editParentSelect.selectOption(data.parentId);
    }
    
    if (data.iconPath) {
      await this.editIconInput.setInputFiles(data.iconPath);
    }
    
    if (data.imagePath) {
      await this.editImageInput.setInputFiles(data.imagePath);
    }
  }

  async saveEdit() {
    this.page.once('dialog', async dialog => {
      console.log('Edit Dialog:', dialog.message());
      await dialog.accept();
    });

    await this.editSaveButton.click();

    await this.page.waitForLoadState('networkidle');
    
    try {
      await this.editModal.waitFor({ state: 'hidden', timeout: 10000 });
    } catch (error) {
      console.log('Modal did not close automatically, attempting manual close');
      await this.page.keyboard.press('Escape');
      await this.page.waitForTimeout(500);
    }

    await this.page.waitForLoadState('networkidle');
    await this.page.waitForTimeout(1000);
  }

  // Delete operations

async search(searchTerm: string): Promise<void> {
  const searchInput = this.page.locator('input[placeholder*="Tìm theo"]');
  await searchInput.waitFor({ state: 'visible', timeout: 5000 });
  await searchInput.fill(searchTerm);
  await searchInput.press('Enter');
  await this.page.waitForLoadState('networkidle');
}

async clearSearch(): Promise<void> {
  const searchInput = this.page.locator('input[placeholder*="Tìm theo"]');
  await searchInput.clear();
  await searchInput.press('Enter');
  await this.page.waitForLoadState('networkidle');
}

  async deleteCategory(rowIndex: number) {
    const row = this.tableRows.nth(rowIndex);
    const actionsCell = row.locator('td').last();
    const deleteButton = actionsCell.locator('button').nth(2);

    const dialogPromise = this.page.waitForEvent('dialog', { timeout: 3000 })
      .then(dialog => dialog.accept())
      .catch(() => {});

    await deleteButton.click();
    await dialogPromise;

    await this.page.waitForLoadState('networkidle');
    await this.page.waitForTimeout(1000);
  }

  // Attributes operations
  async openAttributesModal(rowIndex: number) {
    const row = this.tableRows.nth(rowIndex);
    const actionsCell = row.locator('td').last();
    const attrsButton = actionsCell.locator('button:has-text("Attrs")');
    
    await attrsButton.click();
    await this.attributesModal.waitFor({ state: 'visible' });
  }

  // Validation
  async isTableVisible(): Promise<boolean> {
    return await this.categoryTable.isVisible();
  }

  async isCreateModalVisible(): Promise<boolean> {
    return await this.createModal.isVisible();
  }

  async isEditModalVisible(): Promise<boolean> {
    return await this.editModal.isVisible();
  }

  async isAttributesModalVisible(): Promise<boolean> {
    return await this.attributesModal.isVisible();
  }

  async getTotalCategoriesCount(): Promise<number> {
    const text = await this.totalCategoriesText.innerText();
    const match = text.match(/(\d+)/);
    return match ? parseInt(match[1]) : 0;
  }

  async findCategoryByName(name: string): Promise<number> {
    const count = await this.getRowCount();
    
    for (let i = 0; i < count; i++) {
      const data = await this.getRowData(i);
      if (data.name === name) {
        return i;
      }
    }
    
    return -1;
  }
}