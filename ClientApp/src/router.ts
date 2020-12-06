import { AuthService } from "./services/auth.service";
import jwt_decode from "jwt-decode";
import Vue from "vue";
import Router from "vue-router";
import Counter from "./views/Counter.vue";
import Home from "./views/Home.vue";
import VueRouter from "vue-router";
import { store } from "./store";

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
        requiresAuthentication: true,
      },
    },
    {
      path: "/fetch-data",
      name: "fetch-data",
      component: () =>
        import(/* webpackChunkName: "fetch-data" */ "./views/FetchData.vue"),
      meta: {
        requiresAuthentication: true,
      },
    },
  ],
});

router.beforeEach((to, from, next) => {
  store.dispatch("auth/inspectToken");
  //console.log(store.getters["auth/inspectToken"]);
  //const user = JSON.parse(localStorage.getItem("user"));
  console.log("status : " ,store.getters["auth/authStatus"]);
  console.log("Token Status : ",store.getters["auth/tokenStatus"])


  if (store.getters["auth/tokenStatus"] === false) {
    //console.log(store.getters["auth/inspectToken"]);
    if (to.matched.some((record) => record.meta.requiresAuthentication)) {
      if (store.getters["auth/isLoggedIn"]) {
        next();
        return;
      }
      next("/");
    } else {
      console.log("no auth req");
      next();
    }
  }
  else{
    //console.log
    // next("/");
    // (store.getters["auth/inspectToken"])
    store.dispatch("auth/logout");
    if(to.path !== '/'){
    console.log("not in login")
    next("/");
    }
  }

  // if(to.path !== '/'){
  //   console.log("not in login")
  //   next("/");
  // }
});

export default router;


// function parseJwt (token) {
//   var base64Url = token.split('.')[1];
//   var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
//   var jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
//       return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
//   }).join(''));

//   return JSON.parse(jsonPayload);
// };

// function fromUTCISOString(s) {
//   var b = s.split(/[-T:\.Z]/i);
//   var n= new Date(Date.UTC(b[0],b[1]-1,b[2],b[3],b[4],b[5]));
//   return n;
// }
