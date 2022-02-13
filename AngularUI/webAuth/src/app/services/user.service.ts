import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ResponseModel } from '../Models/responseModel';
import {map} from 'rxjs/operators';
import { ResponseCode } from '../enums/responseCode';
import { User } from '../Models/user';
import { Constants } from '../Helper/constants';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly baseURL:string="https://localhost:5001/api/user/"

  constructor( private httpClient:HttpClient) { }

  //login
  public login(email:string, password:string)
  {
    const body={
      Email : email,
      Password : password
    }
    return this.httpClient.post<ResponseModel>(this.baseURL+"Login",body);
  }
  
  //register
  public register(fullname:string,email:string, password:string)
  {
    const body={
      FullName : fullname,
      Email : email,
      Password : password
    }
    return this.httpClient.post<ResponseModel>(this.baseURL+"RegisterUser",body);
  }

  //getalluser to user-management
  public getAllUser()
  {
    let userInfo=JSON.parse(localStorage.getItem("userInfo"));
    const headers =new HttpHeaders({
      'Authorization':`Bearer ${userInfo?.token}`
    });
    return this.httpClient.get<ResponseModel>(this.baseURL+"GetAllUser",{headers:headers}).pipe(map(res=>{
      let userList=new Array<User>();
      if(res.ResponseCode==ResponseCode.OK)
      {
        if(res.DataSet)
        {
          res.DataSet.map((x:User)=>{
            userList.push(new User(x.fullName,x.email,x.userName));
          })
        }
      }
      return userList;
    }));
  }

}
