<template>
  <v-app>
    

      <v-navigation-drawer persistent :mini-variant="miniVariant" :clipped="clipped" v-model="drawer" disable-resize-watcher fixed app>
          <v-list-item two-line :class="miniVariant && 'px-0'" v-if="login">
              <template>


                  <v-list-item-avatar>
                      <img src="https://randomuser.me/api/portraits/men/81.jpg" />
                  </v-list-item-avatar>
                  <v-list-item-content>
                      <v-list-item-title>Application</v-list-item-title>
                      <v-list-item-subtitle>Subtext</v-list-item-subtitle>
                  </v-list-item-content>
              </template>
          </v-list-item>


          <v-list-item two-line :class="miniVariant && 'px-0'" v-if="!login">
              <v-btn block
                     color="primary"
                     dark
                     large
                     @click.stop="showLoginForm=true">
                  Login
              </v-btn>

              <loginForm v-model="showLoginForm" @close="showLoginForm = false" />
          </v-list-item>

          <v-list-item two-line :class="miniVariant && 'px-0'" v-if="!login">
              <v-btn block
                     color="primary"
                     dark
                     large
                     @click.stop="showLoginForm=true">
                    Registration
              </v-btn>

              <loginForm v-model="showLoginForm" @close="showLoginForm = false" />
          </v-list-item>


          <v-divider></v-divider>
          <v-list v-if="!login">
              <v-list-item value="true" v-for="(item, i) in items" :key="i" :to="item.link">
                  <v-list-item-action>
                      <v-icon v-html="item.icon"></v-icon>
                  </v-list-item-action>
                  <v-list-item-content>
                      <v-list-item-title v-text="item.title"></v-list-item-title>
                  </v-list-item-content>
              </v-list-item>
          </v-list>
      </v-navigation-drawer>

    <v-app-bar app :clipped-left="clipped" color="info" dark>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <!-- <v-btn class="d-none d-lg-flex" icon @click.stop="miniVariant = !miniVariant">
        <v-icon v-html="miniVariant ? 'chevron_right' : 'chevron_left'"></v-icon>
      </v-btn> -->
      <!--<v-btn class="d-none d-lg-flex" icon @click.stop="clipped = !clipped">
        <v-icon>web</v-icon>
      </v-btn>-->
      <v-toolbar-title v-text="title"></v-toolbar-title>
      <v-spacer></v-spacer>
    </v-app-bar>

    <v-content>
      <router-view/>
    </v-content>

    <v-footer app>
      <span>&nbsp;Software Ateliers&nbsp;&copy;&nbsp;2020</span>
    </v-footer>

  </v-app>
</template>

<script lang="ts">
import HelloWorld from '@/components/HelloWorld.vue';
import { Component, Vue } from 'vue-property-decorator';
import LoginForm from '@/components/LoginForm.vue';

@Component({
  components: { HelloWorld, LoginForm},
})
export default class App extends Vue {
  private clipped: boolean = false;
  private drawer: boolean = true;
  private miniVariant: boolean = false;
  private right: boolean = true;
  private showLoginForm: boolean = false;
  private login: boolean = false;
  private title: string = 'MediControl';
  private items = [
    { title: 'Home', icon: 'home', link: '/home' },
    { title: 'Counter', icon: 'touch_app', link: '/counter' },
    { title: 'Fetch data', icon: 'get_app', link: '/fetch-data' },
    ];

  //private onResize(event: any) {
  //    //console.log('window has been resized', event)
  //    //this.showLoginForm = false;
  //    //if (window.outerWidth < 1000 && this.showLoginForm == true) {
  //    //    console.log("reset")
  //    //    this.showLoginForm = false
  //    //}
  //}

  //  private mounted() {
  //      window.addEventListener('resize', this.onResize)
  //  }
}
</script>
