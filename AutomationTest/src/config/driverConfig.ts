import { Builder, WebDriver } from 'selenium-webdriver';
import * as chrome from 'selenium-webdriver/chrome';
import * as firefox from 'selenium-webdriver/firefox';
import path from 'path';

export type BrowserName = 'chrome' | 'firefox';

const driversDir = path.resolve(__dirname, '../../drivers');

export async function createDriver(browser: BrowserName, headless = false): Promise<WebDriver> {
  let builder: Builder = new Builder();

  if (browser === 'chrome') {
    const options = new chrome.Options();
    if (headless) options.addArguments('--headless=new', '--window-size=1920,1080');
    builder = builder.forBrowser('chrome').setChromeOptions(options);
  } else {
    const options = new firefox.Options();
    if (headless) options.addArguments('-headless');
    const geckoPath = path.join(driversDir, 'geckodriver.exe');
    try {
      const service = new firefox.ServiceBuilder(geckoPath);
      builder = builder.setFirefoxService(service);
    } catch (e) {
      // ignore if service builder not available
    }
    builder = builder.forBrowser('firefox').setFirefoxOptions(options);
  }

  const driver = await builder.build();
  try {
    await driver.manage().window().setRect({ width: 1920, height: 1080 });
  } catch (e) {
    // some drivers may not support setRect
  }

  return driver;
}