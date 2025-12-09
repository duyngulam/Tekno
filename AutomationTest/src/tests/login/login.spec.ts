import { expect } from 'chai';
import { describe, it, beforeEach, afterEach } from 'mocha';
import { By, until } from 'selenium-webdriver';
import LoginPage from '../../pages/LoginPage';
import LandingPage from '../../pages/LadingPage';
import { BaseTest } from '../../core/BaseTest';

class LoginTest extends BaseTest {
  // reuse setup/teardown
}

const test = new LoginTest();

describe('Login', function () {
  this.timeout(120000);

  beforeEach(async () => {
    await test.setup();
  });

  afterEach(async () => {
    await test.teardown();
  });

  it('should navigate to login and fail with wrong creds', async () => {
    const landing = new LandingPage(test['driver']);
    await landing.goToLogin();

    const login = new LoginPage(test['driver']);
    await login.login('wrong@example.com', 'badpassword');

    // assert some error shows
    const err = await login.getText(By.css('.error'));
    expect(err).to.exist;
  });

  it('should login successfully with valid credentials', async () => {
    const landing = new LandingPage(test['driver']);
    await landing.goToLogin();

    const login = new LoginPage(test['driver']);
    await login.login('customer@Tekno.com', 'customer123');

    // wait for success toast/modal message (Vietnamese/English)
    const successToast = await test['driver'].wait(
      until.elementLocated(By.xpath("//*[contains(normalize-space(.),'Đăng nhập thành công') or contains(normalize-space(.),'Login successful') or contains(normalize-space(.),'successfully')]")),
      10000
    );

    // click OK on the alert/toast if an OK button exists
    try {
      const okBtn = await successToast.findElement(By.xpath(".//button[normalize-space(.)='OK' or normalize-space(.)='Ok' or contains(normalize-space(.),'OK') or contains(normalize-space(.),'Ok')]"));
      await okBtn.click();
    } catch (err) {
      // ignore if no OK button present
    }

    // wait for header user indicator (avatar/name/email) to appear
    const userIndicator = await test['driver'].wait(
      until.elementLocated(By.xpath("//header//*[contains(@class,'avatar') or contains(@class,'user') or contains(@class,'profile') or contains(normalize-space(.),'@') or contains(normalize-space(.),'Trường') or contains(normalize-space(.),'customer@Tekno.com')]")),
      10000
    );

    expect(userIndicator).to.exist;
  });

});