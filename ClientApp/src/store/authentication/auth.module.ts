import { getters } from './../counter/getters';
import { AuthService } from "./../../services/auth.service";
//https://github.com/bezkoder/vue-vuex-jwt-auth

const authService = new AuthService();

const user = JSON.parse(localStorage.getItem("user") || "{}");
const initialState = user
  ? { status: { loggedIn: true }, user }
  : { status: { loggedIn: false }, user: null };

export const auth = {
  namespaced: true,
  state: initialState,
  actions: {
    // tslint:disable-next-line: no-shadowed-variable
    login({ commit }: any, user: { username: any; password: any; }) {
      return authService.login(user).then(
        // tslint:disable-next-line: no-shadowed-variable
        (user) => {
          commit("loginSuccess", user);
          return Promise.resolve(user);
        },
        (error) => {
          commit("loginFailure");
          return Promise.reject(error);
        }
      );
    },
    logout({ commit }: any){
      authService.logout();
      commit("logout");
    },
    // tslint:disable-next-line: no-shadowed-variable
    register({ commit }: any, user: { username: any; email: any; password: any; }) {
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
    }
  },
  mutations: {
    // tslint:disable-next-line: no-shadowed-variable
    loginSuccess(state: { status: { loggedIn: boolean; }; user: any; }, user: any) {
      state.status.loggedIn = true;
      state.user = user;
    },
    loginFailure(state: { status: { loggedIn: boolean; }; user: null; }) {
      state.status.loggedIn = false;
      state.user = null;
    },
    logout(state: { status: { loggedIn: boolean; }; user: null; }) {
      state.status.loggedIn = false;
      state.user = null;
    },
    registerSuccess(state: { status: { loggedIn: boolean; }; }) {
      state.status.loggedIn = false;
    },
    registerFailure(state: { status: { loggedIn: boolean; }; }) {
      state.status.loggedIn = false;
    }
  },
  getters : {
    isLoggedIn: (state: { token: any; }) => !!state.token,
    authStatus: (state: { status: any; }) => state.status,
  }
  
};
