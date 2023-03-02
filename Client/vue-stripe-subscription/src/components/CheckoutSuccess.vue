<template> 
  <h5>Subscription started.</h5>
  <div v-if="isLoading">Loading payment information...</div>
  <div class="text-lg text-success">{{ message }}</div>
 
  <!-- parse it accorign to application logic -->
  <pre class="mt-3">{{ paymentIntent }}</pre> 
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
      paymentIntent: null
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
  }, 
  methods: {
    getPaymentDetails() {
      this.isLoading = true;     

      // remove this payment object form store after binding this
      var payload = {
        payment_intent_id: this.payment.payment_intent
      };
      console.log(payload);
      axios.post(new AppSettings().API_URL + "payment-intent", payload, { headers: { Authorization: 'Bearer ' + this.currentUser.token } })
        .then((response) => {
          this.paymentIntent = response.data;
          console.log(this.paymentIntent);
          switch (this.paymentIntent.status) {
            case "succeeded":
              this.message = "Payment succeeded!";
              break;
            case "processing":
              this.message="Your payment is processing.";
              break;
            case "requires_payment_method":
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
    }
  }

};
</script>
