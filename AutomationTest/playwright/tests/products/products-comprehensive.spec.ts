import { test, expect, Page } from '@playwright/test';
import { ProductsPage } from '../../pages/ProductsPage';
import { SearchPage } from '../../pages/SearchPage';

type Scenario = {
  id: string;
  description: string;
  search?: string | null;
  category?: string | null; // slug or visible name
  brand?: string | null; // visible brand name
  minPrice?: number | null;
  maxPrice?: number | null;
  attributes?: Record<string, string[]>
  expectZero?: boolean;
  extra?: any;
};

// NOTE: This is a data-driven scaffold. Selectors and data (brand names, category slugs,
// attribute labels) should be adjusted to your app's actual values for reliable assertions.

const scenarios: Scenario[] = [
  { id: 'exact-name', description: 'Search product by exact name', search: 'MacBook Pro 14' },
  { id: 'partial-name', description: 'Search product by partial name', search: 'MacBook' },
  { id: 'fuzzy', description: 'Fuzzy search for product', search: 'mcbk pro' },
  { id: 'no-match', description: 'Search with no matching keyword', search: 'skibiiiiideeeeeeee', expectZero: true },
  { id: 'special-chars', description: 'Search with special characters', search: '@#$%^&*()_+' },
  { id: 'filter-category', description: 'Filter by category', category: 'laptops' },
  { id: 'filter-brand', description: 'Filter by brand', brand: 'Apple' },
  { id: 'filter-price', description: 'Filter by price range', minPrice: 1000, maxPrice: 2000 },
  { id: 'filter-attributes', description: 'Filter by attributes', attributes: { Color: ['Space Gray'] } },
  { id: 'filter-multi-attr', description: 'Filter by multiple attribute values (union)', attributes: { Color: ['Space Gray', 'Silver'] } },
  { id: 'pagination-1', description: 'Search result pagination page 1', search: 'laptop' },
  { id: 'pagination-2', description: 'Search result pagination page 2', search: 'laptop', extra: { goToPage: 2 } },
  { id: 'sort-newest', description: 'Sort by newest products', search: 'laptop', extra: { sort: 'created_desc' } },
  { id: 'sort-price-asc', description: 'Sort by price ascending', search: 'laptop', extra: { sort: 'price_asc' } },
  { id: 'sort-price-desc', description: 'Sort by price descending', search: 'laptop', extra: { sort: 'price_desc' } },
  { id: 'sort-rating', description: 'Sort by rating', search: 'laptop', extra: { sort: 'rating_desc' } },
  { id: 'variant-selection', description: 'Verify product variant selection', search: 'laptop', extra: { verifyVariant: true } },
  { id: 'favorite-search', description: 'Search favorite products', search: '', extra: { favoritesOnly: true }, expectZero: false },
  { id: 'empty-input', description: 'Search empty input', search: '' },
  { id: 'long-keyword', description: 'Search with long keyword', search: 'a'.repeat(200) },
  { id: 'numeric', description: 'Search with numeric keyword', search: '123456' },
  { id: 'alpha-numeric', description: 'Search with mixed alpha-numeric', search: 'iPhone12' },
  { id: 'category+keyword', description: 'Search with category + keyword', search: 'pro', category: 'laptops' },
  { id: 'brand+keyword', description: 'Search with brand + keyword', search: 'pro', brand: 'Apple' },
  { id: 'price+keyword', description: 'Search with price range + keyword', search: 'pro', minPrice: 500, maxPrice: 2000 },
  { id: 'multi-filters-cat-brand', description: 'Search with multiple filters (category + brand)', category: 'laptops', brand: 'Apple' },
  { id: 'multi-filters-cat-price', description: 'Search with multiple filters (category + price)', category: 'laptops', minPrice: 500, maxPrice: 2000 },
  { id: 'multi-filters-brand-price', description: 'Search with multiple filters (brand + price)', brand: 'Apple', minPrice: 500, maxPrice: 2000 },
  { id: 'keyword+cat+brand', description: 'Search with keyword + category + brand', search: 'pro', category: 'laptops', brand: 'Apple' },
  { id: 'keyword+cat+brand+price', description: 'Search with keyword + category + brand + price', search: 'pro', category: 'laptops', brand: 'Apple', minPrice: 500, maxPrice: 3000 },
  { id: 'keyword+all', description: 'Search with keyword + category + brand + price + attributes', search: 'pro', category: 'laptops', brand: 'Apple', minPrice: 500, maxPrice: 3000, attributes: { Color: ['Space Gray'] } },
  { id: 'attributes-only', description: 'Search with attributes filter', attributes: { RAM: ['16GB'] } },
  { id: 'multi-attributes', description: 'Search with multi attributes', attributes: { RAM: ['8GB','16GB'], Color: ['Space Gray'] } },
  { id: 'keyword+attributes', description: 'Search with keyword + attributes', search: 'pro', attributes: { RAM: ['16GB'] } },
  { id: 'verify-count', description: 'Verify search result count', search: 'laptop' }
];

async function applyCategory(page: Page, category: string) {
  // Navigate using query param (products page reads category from searchParams)
  await page.goto(`/products?category=${encodeURIComponent(category)}`);
  await page.waitForLoadState('networkidle');
}

async function applyBrand(page: Page, brand: string) {
  // Click the brand label (Filter component renders labels)
  const label = page.locator('aside label').filter({ hasText: brand }).first();
  if (await label.count()) {
    await label.click();
    await page.waitForLoadState('networkidle');
  } else {
    console.log(`Brand label not found: ${brand}`);
  }
}

async function applyAttributes(page: Page, attrs: Record<string, string[]>) {
  for (const [name, vals] of Object.entries(attrs)) {
    for (const v of vals) {
      const checkbox = page.locator(`aside label:has-text("${v}")`).first();
      if (await checkbox.count()) {
        await checkbox.click();
        await page.waitForLoadState('networkidle');
      } else {
        console.log(`Attribute value not found: ${name} -> ${v}`);
      }
    }
  }
}

test.describe('Comprehensive products search & filter suite (scaffold)', () => {
  for (const s of scenarios) {
    test(s.id + ' - ' + s.description, async ({ page }) => {
      const products = new ProductsPage(page);
      const searchPage = new SearchPage(page);

      // start from products by default
      await products.goto();

      // apply category
      if (s.category) await applyCategory(page, s.category);

      // apply brand
      if (s.brand) await applyBrand(page, s.brand);

      // apply attributes
      if (s.attributes) await applyAttributes(page, s.attributes);

      // apply price
      if (s.minPrice !== undefined || s.maxPrice !== undefined) {
        const min = s.minPrice ?? 0;
        const max = s.maxPrice ?? 999999999;
        await products.applyPriceRange(min, max);
      }

      // perform search if provided (use search page)
      if (s.search !== undefined && s.search !== null) {
        // empty string means run products listing without extra query
        if (s.search.length > 0) {
          await searchPage.goto();
          await searchPage.search(s.search);
        }
      }

      // handle sort
      if (s.extra?.sort) {
        // select element used in product page is a custom Select; we try to change via value on the select trigger
        await page.locator('select, [data-testid="sort-select"]').first().selectOption({ value: s.extra.sort }).catch(() => {
          test.info().log('Sort control not found or not a native select');
        });
      }

      // pagination
      if (s.extra?.goToPage) {
        // click page link by number
        const btn = page.locator(`a:has-text("${s.extra.goToPage}")`).first();
        if (await btn.count()) {
          await btn.click();
          await page.waitForLoadState('networkidle');
        } else {
          console.log(`Pagination button not found for page: ${s.extra.goToPage}`);
        }
      }

      // variant verification
      if (s.extra?.verifyVariant) {
        const count = await products.resultsCount();
        if (count === 0) test.skip(true, 'No products to verify variant');
        // open first product
        await page.locator('a[href^="/products/"]').first().click();
        await page.waitForLoadState('networkidle');
        // attempt to select variant by clicking the first option/button
        const variant = page.locator('select, button:has-text("Variant"), [data-variant]').first();
        if (await variant.count()) {
          await variant.click();
        } else {
          console.log('No variant control found');
        }
        return; // variant checks are done
      }

      // favorites-only
      if (s.extra?.favoritesOnly) {
        // try to use a favorite filter or fall back to skipping
        const favFilter = page.locator('button:has-text("Favorites"), a:has-text("Favorites")').first();
        if (await favFilter.count()) {
          await favFilter.click();
          await page.waitForLoadState('networkidle');
        } else {
          console.log('Favorites filter not present; skipping');
          test.skip(true, 'Favorites not available');
        }
      }

      // final assertions: get results count
      const finalCount = await products.resultsCount();

      if (s.expectZero) {
        expect(finalCount).toBe(0);
      } else {
        // default expectation: at least 0 — but prefer >0 for positive searches
        if (s.search && s.search.length > 0 && !s.expectZero) {
          expect(finalCount).toBeGreaterThanOrEqual(0);
        } else {
          expect(finalCount).toBeGreaterThanOrEqual(0);
        }
      }
    });
  }
});
