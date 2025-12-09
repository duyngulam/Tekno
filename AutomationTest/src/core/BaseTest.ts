import { WebDriver } from 'selenium-webdriver';
import { createDriver } from '../config/driverConfig';
import { TEST_CONFIG } from '../config/testConfig';

export abstract class BaseTest {
  protected driver!: WebDriver;

  async setup() {
    this.driver = await createDriver(TEST_CONFIG.defaultBrowser, TEST_CONFIG.headless as boolean);
    await this.driver.get(TEST_CONFIG.baseUrl);
  }

  async teardown() {
    if (this.driver) await this.driver.quit();
  }
}