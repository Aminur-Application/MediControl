<template>
  <v-container>
    <v-dialog v-model="show" persistent max-width="350">
      <v-card class="elevation-12">
        <v-toolbar color="primary" dark flat>
          <v-toolbar-title>Login form</v-toolbar-title>
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text>
          <v-form>
            <v-text-field
              label="email"
              v-validate="'required'"
              v-model="user.email"
              prepend-icon="mdi-account"
              type="text"
              name="email"
            ></v-text-field>
            <div
              v-if="errors.has('email')"
              class="alert alert-danger"
              role="alert"
            >
              Username is required!
            </div>

            <v-text-field
              id="password"
              label="Password"
              v-model="user.password"
              v-validate="'required'"
              prepend-icon="mdi-lock"
              type="password"
              name="password"
            ></v-text-field>
            <div
              v-if="errors.has('password')"
              class="alert alert-danger"
              role="alert"
            >
              Password is required!
            </div>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
<!-- @click.stop="close" -->
          <v-btn color="primary" @click.stop="handleLogin">
            <span
              v-show="loading"
              class="spinner-border spinner-border-sm"
            ></span>
            Login</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import {User} from '../models/user';


@Component
export default class LoginForm extends Vue{
  [x: string]: any;
  @Prop()
  private value: boolean;

  private user: User = {
      username: "",
      password: "",
      email: "",
  };

  private message: string;

  private loading: boolean = false;



    private get show(){
        console.log(this.value)
        return this.value;
    }

    private close(){
        this.$emit('close');
    }
    private handleLogin() {
      this.loading = true;
      console.log("testing")
      this.$validator.validateAll().then(isValid => {
        if (!isValid) {
            console.log("not valid")
          this.loading = false;
          return;
        }
        if (this.user.email && this.user.password) {
            console.log("valid email & password")
          this.$store.dispatch('auth/login', this.user).then(
            () => {
              // this.$router.push('/home');
              this.$emit('close');
            },
            error => {
                console.log(" not valid email & password")
              this.loading = false;
              this.message =
                (error.response && error.response.data) ||
                error.message ||
                error.toString();
            }
          );
        }
      })
    }
};

</script>

<style lang="scss" scoped></style>
    