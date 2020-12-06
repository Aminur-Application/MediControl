<template>
    <div v-if="warningZone">
        are you still with us?
    </div>
</template>

<script>
    export default {
        name: "AutoLogout",



        data: function(){
            return {
                events:['click', 'mousemove', 'mousedown','scroll','keypress','load'],


                warningTimer: null,
                logoutTimer: null,
                warningZone: false,

            }

        },



        mounted(){
            this.events.forEach(function(event) {
                window.addEventListener(event, this.resetTimer);
                
            }, this);
            this.setTimers();
        },

        destroyed(){
            this.events.forEach(function(event) {
                window.removeEventListener(event, this.resetTimer);
                
            }, this);

            this.resetTimer();
        },

        methods:{

            setTimers: function(){
                this.warningTimer = setTimeout(this.warningMessage, 4 * 1000);
                this.logoutTimer = setTimeout(this.logoutUser, 10 * 1000);


                this.warningZone = false;
            },



            warningMessage: function(){
                this.warningZone = true;
                
            },

            logoutUser: function(){
                console.log("logout");
            },

            resetTimer: function() {
                clearTimeout(this.warningTimer);

                this.setTimers();
            }
        }
    }



</script>

<style lang="scss" scoped>

</style>