import { Component } from '@angular/core';
import { EntityContextProvider, SecurityService, GuidUtils, SyncService, ClientAppProfile } from './bit';
import { __await } from 'tslib';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  title = 'ToDoLine';

  public async test() {
    const userName = prompt("userName?");
    const password = prompt("password?");
    const context = await EntityContextProvider.getContext<ToDoLineContext>("ToDoLine");
    await context.userRegistration.register(new ToDoLine.Dto.UserRegistrationDto({ UserName: userName, Password: password }));
    await SecurityService.loginWithCredentials(userName, password, "ToDoLine", "secret");
  }
}
