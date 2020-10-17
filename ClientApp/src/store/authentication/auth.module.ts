import { getters } from "./../counter/getters";
import { AuthService } from "./../../services/auth.service";
import jwt_decode from "jwt-decode";
//https://github.com/bezkoder/vue-vuex-jwt-auth

const authService = new AuthService();

let user = JSON.parse(localStorage.getItem("user"));
let initialState = user
  ? { status: { loggedIn: true, expired: false }, user, showModal:false }
  : { status: { loggedIn: false, expired: false }, user: null, showModal:true };

export let auth = {
  namespaced: true,
  state: initialState,
  actions: {
    // tslint:disable-next-line: no-shadowed-variable
    login({ commit }: any, user: { username: any; password: any }) {
      console.log("user : ",user)
      return authService.login(user).then(
        // tslint:disable-next-line: no-shadowed-variable
        (user) => {
          console.log("success :", user);
          commit("loginSuccess", user);
          return Promise.resolve(user);
        },
        (error) => {
          console.log("failure :", user);
          commit("loginFailure");
          return Promise.reject(error);
        }
      );
    },
    logout({ commit }: any) {
      authService.logout();
      commit("logout");
      console.log(user)
    },
    // tslint:disable-next-line: no-shadowed-variable
    register(
      { commit }: any,
      user: { username: any; email: any; password: any }
    ) {
      return authService.register(user).then(
        (response) => {
          commit("registerSuccess");
          return Promise.resolve(response.data);
        },
        (error) => {
          commit("registerFailure");
          return Promise.reject(error);
        }
      );
    },
    inspectToken({ commit }: any) {
      //console.log("valid User :", user )
      if(user){
      const token = user.token;
      //console.log("valid token :", user.expiration)

      
      if (token) {
        const decoded: any = jwt_decode(token);
        const exp = decoded.exp;
        const orig_iat = decoded.orig_iat;
        // if (
        //   token.expiration - Date.now() / 1000 < 1800 &&
        //   Date.now() / 1000 - orig_iat < 628200
        // ) 
        //console.log("inspect token :", user.token)
        if (decoded.exp < Date.now()/1000) 
        {
          console.log("invalid Token")
          commit("invalidToken");
          
        } else {
          // PROMPT USER TO RE-LOGIN, THIS ELSE CLAUSE COVERS THE CONDITION WHERE A TOKEN IS EXPIRED AS WELL
          console.log("Valid Token")
          commit("validToken");
        }
      }
      }
    },
  },
  mutations: {
    // tslint:disable-next-line: no-shadowed-variable
    loginSuccess(
      state: { status: { loggedIn: boolean }; user: any }) {
      user = JSON.parse(localStorage.getItem("user"));
      //console.log(user)
      state.status.loggedIn = true;
      state.user = user;
      
    },
    loginFailure(state: { status: { loggedIn: boolean }; user: null }) {
      state.status.loggedIn = false;
      state.user = null;
    },
    logout(state: { status: { loggedIn: boolean }; user: null }) {
      state.status.loggedIn = false;
      state.user = null;
    },
    registerSuccess(state: { status: { loggedIn: boolean } }) {
      state.status.loggedIn = false;
    },
    registerFailure(state: { status: { loggedIn: boolean } }) {
      state.status.loggedIn = false;
    },
    invalidToken(state: { status: { expired: boolean }}) {
      state.status.expired = true;
      //state.user = null;
    },
    validToken(state: { status: { expired: boolean }}) {
      state.status.expired = false;
      //state.user = user;
    },
    toggleModal(state:{ status : {showModal:boolean}}){
      state.status.showModal = !state.status.showModal;
    }
  },
  getters: {
    isLoggedIn: (
      state: { status: { loggedIn: boolean }; user: any },
      user: any
    ) => state.status.loggedIn,

    authStatus: (
      state: { status: { loggedIn: boolean }; user: any }
    ) => state.user,


    tokenStatus: (state: { status: { expired: boolean } }) =>
      state.status.expired,

    showModal: (state: {status: {showModal: boolean}})=> state.status.showModal
  },
};
