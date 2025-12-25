import { test, expect } from '@playwright/test';
import { SearchPage } from '../../pages/SearchPage';

test.describe('Search & Filter', () => {
  test('search returns results and filter reduces or narrows results', async ({ page }) => {
    const search = new SearchPage(page);
    await search.goto();

    const query = 'laptop';
    await search.search(query);

    // basic assertion: at least one result appears
    const initialCount = await search.resultsCount();
    expect(initialCount).toBeGreaterThan(0);

    const firstTitle = await search.firstResultTitle();
    expect(firstTitle.toLowerCase()).toContain(query.split(' ')[0]);

    // apply a filter (adjust label to a real filter in your app)
    // e.g., 'In Stock', 'Price: Low to High', 'Brand X'
    const filterLabel = 'In Stock';
    await search.applyFilter(filterLabel);

    const afterFilterCount = await search.resultsCount();
    // after applying a restrictive filter, the count should be <= initial
    expect(afterFilterCount).toBeLessThanOrEqual(initialCount);
  });
});
