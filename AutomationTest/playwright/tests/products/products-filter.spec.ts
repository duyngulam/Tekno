import { test, expect } from '@playwright/test';
import { ProductsPage } from '../../pages/ProductsPage';

test.describe('Products - search & filters', () => {
  test('brand filter and price filter narrow results', async ({ page }) => {
    const products = new ProductsPage(page);
    await products.goto();

    const initialCount = await products.resultsCount();
    expect(initialCount).toBeGreaterThan(0);

    const firstTitle = await products.firstResultTitle();
    expect(firstTitle.length).toBeGreaterThan(0);

    // apply first brand filter
    await products.applyFirstBrandFilter();
    const afterBrandCount = await products.resultsCount();
    expect(afterBrandCount).toBeLessThanOrEqual(initialCount);

    // try a restrictive price range to further reduce results
    await products.applyPriceRange(0, 1000); // adjust as needed for your data
    const afterPriceCount = await products.resultsCount();
    expect(afterPriceCount).toBeLessThanOrEqual(afterBrandCount);
  });
});
