import axios from 'axios';

const API_URL = "http://localhost:5000/api/login";

export class AuthService {
   public async login(user: { username: any; password: any; }) {
    const response = await axios
           .post(API_URL, {
               username: user.username,
               password: user.password
           });
    if (response.data.token) {
           localStorage.setItem("user", JSON.stringify(response.data));
       }
    return response.data;
  }

  public logout() {
    localStorage.removeItem("user");
  }

  public register(user: { username: any; email: any; password: any; }) {
    return axios.post(API_URL + "signup", {
      username: user.username,
      email: user.email,
      password: user.password
    });
  }
}

//https://github.com/bezkoder/vue-vuex-jwt-auth

