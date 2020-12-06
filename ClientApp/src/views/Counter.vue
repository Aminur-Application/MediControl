<template>
  <v-container fluid>
    <!-- <autoLogout v-if="loggedIn"> </autoLogout> -->
    <v-slide-y-transition mode="out-in">
      <v-row>
        <v-col>
          <h1>Counter</h1>
          <p>This is a simple example of a Vue.js component integrated with Vuex</p>
          <Counter/>
        </v-col> 
      </v-row>
    </v-slide-y-transition>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import Counter from '@/components/Counter.vue';
import AutoLogout from '@/components/AutoLogout.vue';
import { store } from "../store";
@Component({
  components: { Counter, AutoLogout },
})
export default class CounterView extends Vue {
  private get loggedIn() {
    if (store.getters["auth/isLoggedIn"]) {
      return true;
    } else {
      return false;
    }
  }


}
</script>


