import { Component, OnInit } from '@angular/core';
import { FormBuilder,Validators } from '@angular/forms';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  public registerForm = this.formBuilder.group({
    fullname:['',Validators.required],
    email:['',[Validators.email,Validators.required]],
    password:['',Validators.required]
  })
  constructor(private formBuilder:FormBuilder, private userService:UserService) { }

  ngOnInit(): void {
  }

  onSubmit(){
    console.log("on submit")
    let fullname = this.registerForm.controls["fullname"].value;
    let email = this.registerForm.controls["email"].value;
    let password = this.registerForm.controls["password"].value;
    this.userService.register(fullname,email,password).subscribe((data)=>{
      console.log("response",data);
    },error=>{
      console.log("error",error)
    })
    
  }
}
