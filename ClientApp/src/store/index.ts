
import Vue from "vue";
import Vuex from "vuex";
import { auth } from "./authentication/auth.module";

//https://github.com/bezkoder/vue-vuex-jwt-auth

Vue.use(Vuex);

export const store = new Vuex.Store({
    modules: {
        auth
    }
});
