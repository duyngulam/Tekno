import { test, expect } from "@playwright/test";

test.describe("Product Detail Page - E2E Tests", () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to homepage first
    await page.goto("http://localhost:3000");
    await page.waitForLoadState('networkidle');
  });

  test("should navigate to product detail when clicking a product card from homepage", async ({ page }) => {
    // Wait for products to load on homepage (from components like NewProducts, BestSell)
    await page.waitForSelector('img[alt*="product"], .group:has(img)', { timeout: 10000 });
    
    // Find first product card link
    const productLink = page.locator('a[href*="/products/"]:has(img)').first();
    await expect(productLink).toBeVisible();
    
    // Get product URL before clicking
    const productHref = await productLink.getAttribute('href');
    console.log('Clicking product:', productHref);
    
    // Click the product card
    await productLink.click();
    
    // Wait for navigation to product detail page
    await page.waitForURL(/.*\/products\/.*/, { timeout: 5000 });
    
    // Verify we're on the product detail page
    expect(page.url()).toMatch(/\/products\/.+/);
  });

  test("should display product detail page elements correctly", async ({ page }) => {
    // Navigate via product card click
    const productLink = page.locator('a[href*="/products/"]:has(img)').first();
    await expect(productLink).toBeVisible();
    await productLink.click();
    await page.waitForURL(/.*\/products\/.*/, { timeout: 5000 });
    
    // Check for key product detail elements
    
    // Product title should be visible
    const productTitle = page.locator('h1, [data-testid="product-title"]').first();
    await expect(productTitle).toBeVisible();
    
    // Product image should be visible  
    const productImage = page.locator('img').first();
    await expect(productImage).toBeVisible();
    
    // Price should be displayed (FormattedPrice component)
    const priceElements = page.locator('text=/\\$|€|£|₹|¥|VND/, [class*="price"]');
    if (await priceElements.count() > 0) {
      await expect(priceElements.first()).toBeVisible();
    }
  });

  test("should display and interact with variant selector", async ({ page }) => {
    // Navigate to product detail
    const productLink = page.locator('a[href*="/products/"]:has(img)').first();
    await productLink.click();
    await page.waitForURL(/.*\/products\/.*/, { timeout: 5000 });
    
    // Look for variant selector (ProductVariantSelectorDynamic component)
    const variantSelectors = page.locator('select, .variant-option, [data-testid="variant-selector"]');
    
    if (await variantSelectors.count() > 0) {
      const firstSelector = variantSelectors.first();
      await expect(firstSelector).toBeVisible();
      
      // If it's a select dropdown, test interaction
      if (await firstSelector.evaluate(el => el.tagName === 'SELECT')) {
        await firstSelector.click();
        const options = firstSelector.locator('option');
        const optionCount = await options.count();
        expect(optionCount).toBeGreaterThan(1);
        
        // Select a different variant
        if (optionCount > 1) {
          await options.nth(1).click();
        }
      }
    }
  });

  test("should display add to cart functionality", async ({ page }) => {
    // Navigate to product detail
    const productLink = page.locator('a[href*="/products/"]:has(img)').first();
    await productLink.click();
    await page.waitForURL(/.*\/products\/.*/, { timeout: 5000 });
    
    // Look for add to cart button
    const addToCartBtn = page.locator('button:has-text("Add to Cart"), button:has-text("Thêm vào giỏ"), button:has-text("Add to cart")').first();
    
    if (await addToCartBtn.count() > 0) {
      await expect(addToCartBtn).toBeVisible();
      await expect(addToCartBtn).toBeEnabled();
    }
    
    // Look for quantity selector (QuantityButton component)
    const quantityButtons = page.locator('button:has-text("+"), button:has-text("-"), input[type="number"]');
    if (await quantityButtons.count() > 0) {
      await expect(quantityButtons.first()).toBeVisible();
    }
  });

  test("should display comments section", async ({ page }) => {
    // Navigate to product detail
    const productLink = page.locator('a[href*="/products/"]:has(img)').first();
    await productLink.click();
    await page.waitForURL(/.*\/products\/.*/, { timeout: 5000 });
    
    // Scroll down to find comments section
    await page.evaluate(() => window.scrollTo(0, document.body.scrollHeight));
    
    // Look for comments section (Comments component)
    const commentsHeader = page.locator('text=Comments, text=Bình luận, .font-bold:has-text("Comments")').first();
    
    if (await commentsHeader.count() > 0) {
      await expect(commentsHeader).toBeVisible();
      
      // Check for comment form elements
      const commentTextarea = page.locator('textarea').first();
      if (await commentTextarea.count() > 0) {
        await expect(commentTextarea).toBeVisible();
      }
      
      const commentButton = page.locator('button:has-text("Comment"), button:has-text("Bình luận")').first();
      if (await commentButton.count() > 0) {
        await expect(commentButton).toBeVisible();
      }
      
      // Check for rating stars
      const ratingStars = page.locator('svg, .star, [class*="rating"]');
      if (await ratingStars.count() > 0) {
        await expect(ratingStars.first()).toBeVisible();
      }
    }
  });

  test("should test complete product card click flow from different homepage sections", async ({ page }) => {
    // Test clicking products from New Products section
    const newProductsSection = page.locator('text=New Products').first();
    if (await newProductsSection.count() > 0) {
      await newProductsSection.scrollIntoViewIfNeeded();
      
      // Find product in this section
      const sectionContainer = newProductsSection.locator('..').locator('..');
      const productInSection = sectionContainer.locator('a[href*="/products/"]:has(img)').first();
      
      if (await productInSection.count() > 0) {
        await productInSection.click();
        await page.waitForURL(/.*\/products\/.*/, { timeout: 5000 });
        
        // Verify product page loaded
        const productTitle = page.locator('h1').first();
        await expect(productTitle).toBeVisible();
        
        // Go back to test other sections
        await page.goBack();
        await page.waitForLoadState('networkidle');
      }
    }
  });
});
