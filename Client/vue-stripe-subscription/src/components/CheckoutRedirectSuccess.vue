<template>
  <h5>Subscription started.</h5>
  <div v-if="isLoading">Loading payment information...</div>
  <div class="text-lg text-success">{{ message }}</div>
  <!-- parse it accorign to application logic -->
  <pre class="mt-3">{{ checkoutsession }}</pre> 
</template>
  
<script>
import axios from 'axios';
import { AppSettings } from '../settings';
export default {
  data() {
    return {
      session: '',
      isLoading: false,
      message: '',
      checkoutsession: null
    };

  },
  computed: {
    currentUser() {
      return this.$store.state.auth.user;
    },
    payment(){
      return this.$store.state.payments.payment;
    }
  },
  mounted() {
    if (!this.currentUser) {
      this.$store.dispatch('auth/logout');
      this.$router.push('/signin');
    }
    else {
      this.getPaymentDetails();
    }
  }, methods: {
    getPaymentDetails() {
      this.isLoading = true;     

      // remove this payment object form store after binding this
      var payload = {
        checkout_session_id: this.payment.payment_intent //stored checkout session id in this property while redirect
      };
      console.log(payload);
      axios.post(new AppSettings().API_URL + "checkout-session", payload, { headers: { Authorization: 'Bearer ' + this.currentUser.token } })
        .then((response) => {
          this.checkoutsession = response.data;
          console.log(this.checkoutsession);
          switch (this.checkoutsession.status) {
            case "complete":
              this.message = "Payment succeeded!";
              break;
            case "expired":
              this.message="Session expried. Try again later";
              break;
            case "Open":
              this.message = "Your payment was not successful, please try again.";
              break;
            default:
              this.message = "Something went wrong.";
              break;
          }
        })
        .catch((errors) => {
          console.log(errors); // Errors
        });
      this.isLoading = false;
      console.log(this.paymentIntent);
    },
  }

};
</script>
