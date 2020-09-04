import { AuthService } from "./services/auth.service";

import Vue from "vue";
import Router from "vue-router";
import Counter from "./views/Counter.vue";
import Home from "./views/Home.vue";
import VueRouter from "vue-router";
import {store} from "./store";
import jwt_decode from "jwt-decode";

Vue.use(Router);

// const router = new VueRouter();

const router = new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      name: "home",
      component: Home,
    },
    {
      path: "/counter",
      name: "counter",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: Counter,
      meta: {
        requiresAuthentication: true
      }
    },
    {
      path: "/fetch-data",
      name: "fetch-data",
      component: () => import(/* webpackChunkName: "fetch-data" */ "./views/FetchData.vue"),
      meta: {
        requiresAuthentication: true
      }
    },
  ],
});

router.beforeEach((to, from, next) => {
  // console.log(store.getters['auth/isLoggedIn'])
  const user = JSON.parse(localStorage.getItem("user"));
  console.log("here i am 1");
  const token = user;




  let utcstart = fromUTCISOString(user.expiration)
  let todaysdate = new Date(Date.now())

  console.log("local storage date "  ,utcstart)
  console.log("current time ", todaysdate)

  if(todaysdate.getTime() > utcstart.getTime()){
    console.log("token expired");
    localStorage.clear();
  }

 // console.log(user.exp)

  if (to.matched.some((record) => record.meta.requiresAuthentication)) {
    if (store.getters["auth/isLoggedIn"]) {
      next();
      return;
    }
    next("/home");
  } else {
    next();
  }
});

export default router;

function fromUTCISOString(s) {
  var b = s.split(/[-T:\.Z]/i);
  var n= new Date(Date.UTC(b[0],b[1]-1,b[2],b[3],b[4],b[5]));
  return n;
}