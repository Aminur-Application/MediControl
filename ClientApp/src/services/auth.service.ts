import axios from "axios";
import { BehaviorSubject, Observable } from "rxjs";
import { User } from "@/models/user";
import { map } from "rxjs/operators";

// onst API_URL = "http://localhost:5000/api/login";

const API_URL = "http://localhost:5001/api/";

export class AuthService {

  // constructor(){
  //   this.userSubject = new BehaviorSubject<User>(null!);
  //   this.user = this.userSubject.asObservable();
  // }

  public get userValue(): User {
    return this.userSubject;
  }
  // let userSubject: BehaviorSubject<User>;
  // let user: Observable<User>;
  //private userSubject: BehaviorSubject<User>;
  public userSubject = new User();


  private refreshTokenTimeout: any;



  public async login(user: { username: any; password: any; }) {
    const response = await axios
           .post(API_URL + "login", {
               username: user.username,
               password: user.password
           }, {withCredentials: false});
    if (response.data) {
      this.userSubject = response.data;
      this.startRefreshTokenTimer();
      console.log("login jwt" , this.userSubject)
      localStorage.setItem("user", JSON.stringify(response.data));
    } 
    return response.data;
  }

  public logout() {
    const response = axios
    .post(API_URL + "revoke-token" ,{}, {withCredentials: false});
    this.stopRefreshTokenTimer();
    this.userSubject = new User();
    localStorage.removeItem("user");
  }

  public register(user: { username: any; email: any; password: any; }) {
    return axios.post(API_URL + "signup", {
      username: user.username,
      email: user.email,
      password: user.password
    });
  }

  public async refreshToken(){
    const response = await axios
    .post(API_URL + "refresh-Token", {}, { withCredentials: true });
    if (response.data) {    
        this.userSubject = response.data;
        this.startRefreshTokenTimer();
        console.log("refreshing" , this.userSubject)
        localStorage.setItem("user", JSON.stringify(response.data));
    }

    return response.data;
  }

  private startRefreshTokenTimer() {
      // parse json object from base64 encoded jwt token
      //console.log("checking refreshtokentimer", this.userValue.jwtToken)
      const jwtToken = JSON.parse(atob(this.userValue.jwtToken!.split(".")[1]));

      // set a timeout to refresh the token a minute before it expires
      const expires = new Date(jwtToken.exp * 1000);
      const timeout = expires.getTime() - Date.now() - (60 * 1000);

      console.log("starrefreshtokentimer expires", expires)
      console.log("starrefreshtokentimer timeout", timeout )
      this.refreshTokenTimeout = setTimeout(() => this.refreshToken(), timeout);
  }

  private stopRefreshTokenTimer() {
      clearTimeout(this.refreshTokenTimeout);
  }

}

// https://github.com/bezkoder/vue-vuex-jwt-auth

