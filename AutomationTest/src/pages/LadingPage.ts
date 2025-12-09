import BasePage from '../core/BasePage';
import { By, until } from 'selenium-webdriver';

export default class LandingPage extends BasePage {
  searchInput = By.css('input[placeholder="Search"]');
  // header login button that opens a modal (supports Vietnamese 'Đăng nhập' and English 'Login')
  headerLoginButton = By.xpath('//header//button[contains(normalize-space(.),"Đăng nhập") or contains(normalize-space(.),"Login")]');
  // generic modal locator (role=dialog or common modal classes/ids)
  loginModal = By.css('[role="dialog"], .login-modal, #loginModal');

  async openLoginModal() {
    await this.click(this.headerLoginButton);
    // wait until modal dialog is present
    await this.driver.wait(until.elementLocated(this.loginModal), 5000);
  }

  async goToLogin() {
    // open login modal from header
    await this.openLoginModal();
  }
}