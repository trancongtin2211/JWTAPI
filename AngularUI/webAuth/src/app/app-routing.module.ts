import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserManagementComponent } from './user-management/user-management.component';
// import { AuthGuardService } from './guards/auth.service';
import { AllUserManagementComponent } from './all-user-management/all-user-management.component';

const routes: Routes = [
  {path:'',redirectTo:'login',pathMatch:'full'},
  {path:"register",component:RegisterComponent},
  {path:"login",component:LoginComponent},
  {path:"user-management",component:UserManagementComponent}
  // {path: "all-user-management",component:AllUserManagementComponent,canActivate:[AuthGuardService]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
