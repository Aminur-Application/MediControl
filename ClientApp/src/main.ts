import 'core-js/stable';
import 'regenerator-runtime/runtime';
import Vue from 'vue';

import './plugins/axios';
import vuetify from './plugins/vuetify';
import VeeValidate from 'vee-validate';
import BootstrapVue from 'bootstrap-vue';
import App from './App.vue';
import router from './router';
import {store} from '@/store/index';
import './registerServiceWorker';
import dateFilter from '@/filters/date.filter';

Vue.config.productionTip = false;

Vue.use(VeeValidate);


Vue.use(BootstrapVue);




Vue.filter('date', dateFilter);

new Vue({
  vuetify,
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');
