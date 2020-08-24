
import Vue from 'vue';
import Router from 'vue-router';
import Counter from './views/Counter.vue';
import Home from './views/Home.vue';
import VueRouter from 'vue-router';
import {store} from './store';

Vue.use(Router);

//const router = new VueRouter();

let router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
    {
      path: '/counter',
      name: 'counter',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: Counter,
      meta:{
        requiresAuthentication: true
      }
    },
    {
      path: '/fetch-data',
      name: 'fetch-data',
      component: () => import(/* webpackChunkName: "fetch-data" */ './views/FetchData.vue'),
      meta:{
        requiresAuthentication: true
      }
    },
  ],
});

router.beforeEach((to, from, next) => {
  console.log("im in")
  if(to.matched.some(record => record.meta.requiresAuthentication)) {
    if (store.getters.isLoggedIn) {
      next()
      return
    }
    next('/home') 
  } else {
    next() 
  }
})

export default router;