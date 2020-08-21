<template>
    <v-dialog v-model="show" persistent max-width="350">
      <v-card class="elevation-12">
              <v-toolbar
                color="primary"
                dark
                flat
              >
                <v-toolbar-title>Login form</v-toolbar-title>
                <v-spacer></v-spacer>
                
              </v-toolbar>
              <v-card-text>
                <v-form>
                  <v-text-field
                    label="Login"
                    name="login"
                    prepend-icon="mdi-account"
                    type="text"
                  ></v-text-field>

                  <v-text-field
                    id="password"
                    label="Password"
                    name="password"
                    prepend-icon="mdi-lock"
                    type="password"
                  ></v-text-field>
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="primary" @click.stop="close">Login</v-btn>
              </v-card-actions>
            </v-card>
         
    </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';

@Component
export default class LoginForm extends Vue{
  @Prop()
  private value: boolean;



    private get show(){
        console.log(this.value)
        return this.value;
    }
    
    private close(){
        this.$emit('close');
    }
  
}
</script>

<style lang="scss" scoped>

</style>