import { WebDriver, By, until, WebElement } from 'selenium-webdriver';

export default abstract class BasePage {
  protected driver: WebDriver;

  constructor(driver: WebDriver) {
    this.driver = driver;
  }

  async find(locator: any): Promise<WebElement> {
    await this.driver.wait(until.elementLocated(locator), 10000);
    return this.driver.findElement(locator);
  }

  async click(locator: any) {
    const el = await this.find(locator);
    await el.click();
  }

  async type(locator: any, text: string) {
    const el = await this.find(locator);
    await el.clear();
    await el.sendKeys(text);
  }

  async getText(locator: any) {
    const el = await this.find(locator);
    return el.getText();
  }
}