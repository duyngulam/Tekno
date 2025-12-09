import BasePage from '../core/BasePage';
import { By } from 'selenium-webdriver';

export default class LoginPage extends BasePage {
  email = By.css('input[name="email"]');
  password = By.css('input[name="password"]');
  submit = By.css('button[type="submit"]');

  async login(email: string, password: string) {
    await this.type(this.email, email);
    await this.type(this.password, password);
    await this.click(this.submit);
  }
}