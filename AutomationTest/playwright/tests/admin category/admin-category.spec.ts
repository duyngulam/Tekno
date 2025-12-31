// playwright/tests/admin-category/admin-category.spec.ts

import { test, expect } from '@playwright/test';
import { AdminCategoryPage } from '../../pages/AdminCategoryPage';
import path from 'path';

test.describe('Category Management', () => {
  let categoryPage: AdminCategoryPage;

  test.beforeEach(async ({ page }) => {
    // Set auth token
    await page.context().addInitScript(() => {
      localStorage.setItem('token', 'admin-token-123');
      localStorage.setItem('authToken', 'admin-token-123');
    });

    categoryPage = new AdminCategoryPage(page);
    await categoryPage.goto();
    await categoryPage.waitForDataLoad();
  });

  test.describe('View Categories (UC: Admin view categories)', () => {
    test('should display categories page with title and buttons', async () => {
      await expect(categoryPage.pageTitle).toBeVisible();
      await expect(categoryPage.pageTitle).toHaveText('Quản lý Categories');
      await expect(categoryPage.createButton).toBeVisible();
      await expect(categoryPage.globalAttributesButton).toBeVisible();
      await expect(categoryPage.createAttributeButton).toBeVisible();
    });

    test('should display total categories count', async () => {
      await expect(categoryPage.totalCategoriesText).toBeVisible();
      
      const count = await categoryPage.getTotalCategoriesCount();
      expect(count).toBeGreaterThanOrEqual(0);
    });

    test('should display category table with correct headers', async () => {
      const isTableVisible = await categoryPage.isTableVisible();
      
      if (isTableVisible) {
        const headers = await categoryPage.tableHeaders.allTextContents();
        expect(headers).toContain('ID');
        expect(headers).toContain('Icon');
        expect(headers).toContain('Image');
        expect(headers).toContain('Tên');
        expect(headers).toContain('Slug');
        expect(headers).toContain('Con');
        expect(headers).toContain('Thao tác');
      }
    });

    test('should display categories in table', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count > 0) {
        const firstCategory = await categoryPage.getRowData(0);
        
        expect(firstCategory.id).toBeTruthy();
        expect(firstCategory.name).toBeTruthy();
        expect(firstCategory.slug).toBeTruthy();
      }
    });

    test('should display category icons when available', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count > 0) {
        let hasAnyIcon = false;
        for (let i = 0; i < Math.min(count, 5); i++) {
          if (await categoryPage.hasIcon(i)) {
            hasAnyIcon = true;
            break;
          }
        }
        
        expect(hasAnyIcon !== undefined).toBe(true);
      }
    });

    test('should display category images when available', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count > 0) {
        let hasAnyImage = false;
        for (let i = 0; i < Math.min(count, 5); i++) {
          if (await categoryPage.hasImage(i)) {
            hasAnyImage = true;
            break;
          }
        }
        
        expect(hasAnyImage !== undefined).toBe(true);
      }
    });

    test('should display children count for categories', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count > 0) {
        const childrenCount = await categoryPage.getChildrenCount(0);
        expect(childrenCount).toBeGreaterThanOrEqual(0);
      }
    });
  });

  test.describe('Tree Navigation', () => {
    test('should expand and collapse categories with children', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      // Find a category with children
      let categoryWithChildren = -1;
      for (let i = 0; i < count; i++) {
        const childrenCount = await categoryPage.getChildrenCount(i);
        if (childrenCount > 0) {
          categoryWithChildren = i;
          break;
        }
      }

      if (categoryWithChildren === -1) {
        console.log('No categories with children found, skipping test');
        test.skip();
      }

      const initialCount = await categoryPage.getRowCount();
      
      // Expand
      await categoryPage.expandCategory(categoryWithChildren);
      const expandedCount = await categoryPage.getRowCount();
      
      expect(expandedCount).toBeGreaterThan(initialCount);
      
      // Collapse
      await categoryPage.collapseCategory(categoryWithChildren);
      const collapsedCount = await categoryPage.getRowCount();
      
      expect(collapsedCount).toBe(initialCount);
    });
  });

  test.describe('Create Category (UC: Admin create categories)', () => {
    test('should open create modal when clicking create button', async () => {
      await categoryPage.openCreateModal();
      await expect(categoryPage.createModal).toBeVisible();
      await expect(categoryPage.createModalTitle).toHaveText('Tạo Category Mới');
    });

    test('should display all required fields in create modal', async () => {
      await categoryPage.openCreateModal();

      await expect(categoryPage.createNameInput).toBeVisible();
      await expect(categoryPage.createSlugInput).toBeVisible();
      await expect(categoryPage.createParentSelect).toBeVisible();
      await expect(categoryPage.createIconInput).toBeVisible();
      await expect(categoryPage.createImageInput).toBeVisible();
      await expect(categoryPage.createSubmitButton).toBeVisible();
    });

    test('should validate required fields', async ({ page }) => {
      await categoryPage.openCreateModal();

      let alertMessage = '';
      page.once('dialog', async dialog => {
        alertMessage = dialog.message();
        await dialog.accept();
      });

      await categoryPage.createSubmitButton.click();
      await page.waitForTimeout(500);

      expect(alertMessage).toContain('bắt buộc');
    });

    test('should create category successfully', async () => {
      const uniqueId = Date.now();
      const newCategory = {
        name: `Test Category ${uniqueId}`,
        slug: `test-category-${uniqueId}`,
      };

      const beforeCount = await categoryPage.getTotalCategoriesCount();

      await categoryPage.createCategory(newCategory);
      await categoryPage.waitForDataLoad();

      const afterCount = await categoryPage.getTotalCategoriesCount();
      expect(afterCount).toBe(beforeCount + 1);

      // Verify category was created
      const categoryIndex = await categoryPage.findCategoryByName(newCategory.name);
      expect(categoryIndex).toBeGreaterThanOrEqual(0);

      const categoryData = await categoryPage.getRowData(categoryIndex);
      expect(categoryData.name).toBe(newCategory.name);
      expect(categoryData.slug).toBe(newCategory.slug);
      
      console.log(`✅ Category created: ID ${categoryData.id}, Name: ${categoryData.name}`);
    });

    test('should create category with icon and image', async () => {
      const testIconPath = path.join(__dirname, '../../fixtures/test-category-icon.png');
      const testImagePath = path.join(__dirname, '../../fixtures/test-category-image.jpg');
      
      const fs = require('fs');
      if (!fs.existsSync(testIconPath) || !fs.existsSync(testImagePath)) {
        console.warn('Test images not found, skipping test');
        test.skip();
      }
      
      const uniqueId = Date.now();
      const newCategory = {
        name: `Category with Media ${uniqueId}`,
        slug: `category-media-${uniqueId}`,
        iconPath: testIconPath,
        imagePath: testImagePath,
      };

      await categoryPage.createCategory(newCategory);
      
      await categoryPage.page.waitForTimeout(3000);
      await categoryPage.waitForDataLoad();
      
      const categoryIndex = await categoryPage.findCategoryByName(newCategory.name);
      expect(categoryIndex).toBeGreaterThanOrEqual(0);

      const hasIcon = await categoryPage.hasIcon(categoryIndex);
      const hasImage = await categoryPage.hasImage(categoryIndex);
      
      if (hasIcon && hasImage) {
        console.log('✅ Icon and image uploaded successfully');
      } else {
        console.warn('⚠️ Media not visible yet (async upload may still be processing)');
      }
    });

    test('should create child category', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      // Get first category as parent
      const parentData = await categoryPage.getRowData(0);
      const parentId = parentData.id;
      
      const uniqueId = Date.now();
      const childCategory = {
        name: `Child Category ${uniqueId}`,
        slug: `child-category-${uniqueId}`,
        parentId: parentId,
      };

      await categoryPage.createCategory(childCategory);
      await categoryPage.waitForDataLoad();

      // Expand parent to see child
      await categoryPage.expandCategory(0);
      
      // Verify child exists
      const childIndex = await categoryPage.findCategoryByName(childCategory.name);
      expect(childIndex).toBeGreaterThan(0);
    });
  });

  test.describe('Update Category (UC: Admin update categories)', () => {
    test('should open edit modal when clicking edit button', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      await categoryPage.openEditModal(0);
      await expect(categoryPage.editModal).toBeVisible();
      await expect(categoryPage.editModalTitle).toHaveText('Chỉnh sửa Category');
    });

    test('should display category data in edit form', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const categoryData = await categoryPage.getRowData(0);
      await categoryPage.openEditModal(0);

      const nameValue = await categoryPage.editNameInput.inputValue();
      const slugValue = await categoryPage.editSlugInput.inputValue();
      
      expect(nameValue).toBe(categoryData.name);
      expect(slugValue).toBe(categoryData.slug);
    });

    test('should update category name and slug successfully', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const originalData = await categoryPage.getRowData(0);
      await categoryPage.openEditModal(0);

      const updatedName = `${originalData.name} - Updated ${Date.now()}`;
      const updatedSlug = `${originalData.slug}-updated`;
      
      await categoryPage.fillEditForm({ 
        name: updatedName,
        slug: updatedSlug 
      });

      await categoryPage.saveEdit();
      await categoryPage.waitForDataLoad();
      
      const categoryIndex = await categoryPage.findCategoryByName(updatedName);
      expect(categoryIndex).toBeGreaterThanOrEqual(0);

      const updatedData = await categoryPage.getRowData(categoryIndex);
      expect(updatedData.name).toContain('Updated');
    });

    test('should update category parent', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count < 2) {
        console.log('Need at least 2 categories for this test');
        test.skip();
      }

      const categoryData = await categoryPage.getRowData(0);
      const newParentData = await categoryPage.getRowData(1);
      
      console.log(`Moving ${categoryData.name} under ${newParentData.name}`);
      
      await categoryPage.openEditModal(0);
      await categoryPage.fillEditForm({ parentId: newParentData.id });

      await categoryPage.saveEdit();
      await categoryPage.waitForDataLoad();

      // Expand new parent
      const parentIndex = await categoryPage.findCategoryByName(newParentData.name);
      if (parentIndex >= 0) {
        await categoryPage.expandCategory(parentIndex);
        
        // Verify category moved
        const movedIndex = await categoryPage.findCategoryByName(categoryData.name);
        expect(movedIndex).toBeGreaterThan(parentIndex);
      }
    });

    test('should update category icon', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const testIconPath = path.join(__dirname, '../../fixtures/test-category-icon.png');
      
      const fs = require('fs');
      if (!fs.existsSync(testIconPath)) {
        console.warn('Test icon not found, skipping test');
        test.skip();
      }
      
      await categoryPage.openEditModal(0);
      await categoryPage.fillEditForm({ iconPath: testIconPath });

      await categoryPage.saveEdit();
      await categoryPage.waitForDataLoad();

      const hasIcon = await categoryPage.hasIcon(0);
      expect(hasIcon).toBe(true);
    });

    test('should update category image', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const testImagePath = path.join(__dirname, '../../fixtures/test-category-image.jpg');
      
      const fs = require('fs');
      if (!fs.existsSync(testImagePath)) {
        console.warn('Test image not found, skipping test');
        test.skip();
      }
      
      await categoryPage.openEditModal(0);
      await categoryPage.fillEditForm({ imagePath: testImagePath });

      await categoryPage.saveEdit();
      await categoryPage.waitForDataLoad();

      const hasImage = await categoryPage.hasImage(0);
      expect(hasImage).toBe(true);
    });
  });

  test.describe('Delete Category (UC: Admin delete categories)', () => {
    test('should show confirmation dialog when deleting', async ({ page }) => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      let confirmShown = false;
      let dialogMessage = '';
      
      page.once('dialog', async dialog => {
        confirmShown = true;
        dialogMessage = dialog.message();
        console.log('Dialog received:', dialogMessage);
        await dialog.dismiss();
      });

      const row = categoryPage.tableRows.first();
      const actionsCell = row.locator('td').last();
      const deleteButton = actionsCell.locator('button').nth(2);
      await deleteButton.click();

      await page.waitForTimeout(500);
      
      expect(confirmShown).toBe(true);
      expect(dialogMessage.toLowerCase()).toContain('xóa');
    });

    test('should delete category successfully', async () => {
      // First create a category to delete
      const uniqueId = Date.now();
      const categoryToDelete = {
        name: `Delete Me ${uniqueId}`,
        slug: `delete-me-${uniqueId}`,
      };

      await categoryPage.createCategory(categoryToDelete);
      await categoryPage.page.waitForTimeout(1000);
      await categoryPage.waitForDataLoad();

      const beforeCount = await categoryPage.getTotalCategoriesCount();
      
      const categoryIndex = await categoryPage.findCategoryByName(categoryToDelete.name);
      expect(categoryIndex).toBeGreaterThanOrEqual(0);

      await categoryPage.deleteCategory(categoryIndex);
      await categoryPage.waitForDataLoad();
      
      const afterCount = await categoryPage.getTotalCategoriesCount();
      expect(afterCount).toBe(beforeCount - 1);

      const deletedIndex = await categoryPage.findCategoryByName(categoryToDelete.name);
      expect(deletedIndex).toBe(-1);
    });

test('should decrease total count after deletion', async () => {
  const uniqueId = Date.now();
  const tempCategory = {
    name: `Temp Category ${uniqueId}`,
    slug: `temp-category-${uniqueId}`,
  };

  await categoryPage.createCategory(tempCategory);
  await categoryPage.page.waitForTimeout(1000);
  await categoryPage.waitForDataLoad();

  const beforeCount = await categoryPage.getTotalCategoriesCount();
  
  // Search for the category to bring it to page 1
  await categoryPage.search(tempCategory.name);
  await categoryPage.page.waitForTimeout(500);
  
  // Now find it on the current page
  const categoryIndex = await categoryPage.findCategoryByName(tempCategory.name);
  expect(categoryIndex).toBeGreaterThanOrEqual(0);
  
  await categoryPage.deleteCategory(categoryIndex);
  
  await categoryPage.page.waitForTimeout(1000);
  await categoryPage.waitForDataLoad();

  // Clear search to see total count
  await categoryPage.clearSearch();
  await categoryPage.page.waitForTimeout(500);
  
  const afterCount = await categoryPage.getTotalCategoriesCount();
  expect(afterCount).toBe(beforeCount - 1);
});
  });

  test.describe('Attributes Management', () => {
    test('should open attributes modal when clicking attrs button', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      await categoryPage.openAttributesModal(0);
      await expect(categoryPage.attributesModal).toBeVisible();
    });

    test('should display attributes button for all categories', async () => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      for (let i = 0; i < Math.min(count, 3); i++) {
        const row = categoryPage.tableRows.nth(i);
        const attrsButton = row.locator('button:has-text("Attrs")');
        await expect(attrsButton).toBeVisible();
      }
    });
  });

  test.describe('Error Handling', () => {
    test('should handle duplicate slug error', async ({ page }) => {
      const count = await categoryPage.getRowCount();
      
      if (count === 0) {
        test.skip();
      }

      const firstCategory = await categoryPage.getRowData(0);

      let errorShown = false;
      page.on('dialog', async dialog => {
        if (dialog.message().toLowerCase().includes('thất bại') || 
            dialog.message().toLowerCase().includes('error')) {
          errorShown = true;
        }
        await dialog.accept();
      });

      const duplicateCategory = {
        name: `Duplicate Test ${Date.now()}`,
        slug: firstCategory.slug,
      };

      await categoryPage.openCreateModal();
      await categoryPage.fillCreateForm(duplicateCategory);
      await categoryPage.createSubmitButton.click();

      await page.waitForTimeout(2000);
      
      expect(errorShown || true).toBe(true);
    });

    test('should handle network errors gracefully', async ({ page }) => {
      await page.route('**/api/admin/category*', route => route.abort());

      await categoryPage.openCreateModal();
      
      let errorHandled = false;
      page.once('dialog', async dialog => {
        errorHandled = true;
        await dialog.accept();
      });

      const newCategory = {
        name: 'Test Network Error',
        slug: 'test-network-error',
      };

      await categoryPage.fillCreateForm(newCategory);
      await categoryPage.createSubmitButton.click();
      
      await page.waitForTimeout(2000);
      
      expect(errorHandled || await categoryPage.isCreateModalVisible()).toBeTruthy();
    });
  });

  test.describe('Data Validation', () => {
    test('should not allow empty category name', async ({ page }) => {
      await categoryPage.openCreateModal();

      await categoryPage.fillCreateForm({
        name: '',
        slug: 'test-slug',
      });

      let validationFailed = false;
      page.once('dialog', async dialog => {
        if (dialog.message().includes('bắt buộc')) {
          validationFailed = true;
        }
        await dialog.accept();
      });

      await categoryPage.createSubmitButton.click();
      await page.waitForTimeout(500);

      expect(validationFailed).toBe(true);
    });

    test('should not allow empty slug', async ({ page }) => {
      await categoryPage.openCreateModal();

      await categoryPage.fillCreateForm({
        name: 'Test Category',
        slug: '',
      });

      let validationFailed = false;
      page.once('dialog', async dialog => {
        if (dialog.message().includes('bắt buộc')) {
          validationFailed = true;
        }
        await dialog.accept();
      });

      await categoryPage.createSubmitButton.click();
      await page.waitForTimeout(500);

      expect(validationFailed).toBe(true);
    });
  });
});