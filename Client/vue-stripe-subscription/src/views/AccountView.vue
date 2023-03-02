<template>
    <default-layout>
        <h1>Account Profile!</h1>
        <hr />       
        <div v-if="isLoading">Loading account information...</div>
        <PriceSelection v-else-if="!SuccessPayment && !checkoutSuccess"/>     
        <CheckoutSuccess v-else-if="SuccessPayment" />  
        <CheckoutRedirectSuccess v-else-if="checkout_session_id!='' && checkout_session_id!=undefined" />  
    </default-layout>
</template>

<script>
import DefaultLayout from "@/layouts/DefaultLayout";
import PriceSelection from "@/components/PriceSelection";
import CheckoutSuccess from "@/components/CheckoutSuccess";
import CheckoutRedirectSuccess from "@/components/CheckoutRedirectSuccess";
export default {
  components: {
    DefaultLayout,
    PriceSelection,
    CheckoutSuccess,  
    CheckoutRedirectSuccess   
  },

  data() {
    return {
      isLoading: false,     
      payment_intent:'',
      payment_intent_client_secret:'',
      redirect_status:'',  
      checkout_session_id:'',  // redirect method      
    };
  },
  computed: {
    currentUser() {     
      return this.$store.state.auth.user;
    },
    SuccessPayment(){
      return this.payment_intent!=undefined && this.payment_intent_client_secret !=undefined && this.redirect_status !=undefined && this.payment_intent!=''&& this.payment_intent_client_secret !='' && this.redirect_status !=''
    },
    checkoutSuccess(){
      return this.checkout_session_id!='' && this.checkout_session_id!=undefined;
    },
    showProducts(){
      return  !this.SuccessPayment && !this.checkoutSuccess;
    }
    
  },
  created(){
    this.payment_intent = this.$route.query.payment_intent;  
    this.payment_intent_client_secret = this.$route.query.payment_intent_client_secret;  
    this.redirect_status = this.$route.query.redirect_status;

    // for redirect method
    this.checkout_session_id=this.$route.query.checkout_session_id;  

    /////
    
    if(this.payment_intent!='' && this.payment_intent!=undefined){
    var payment={
      payment_intent:this.payment_intent,
      payment_intent_client_secret:this.payment_intent_client_secret,
      redirect_status:this.redirect_status

    }
      localStorage.removeItem('payment');
      localStorage.setItem('payment', JSON.stringify(payment));
      this.$store.dispatch("payments/setPaymentSuccess", payment);
  }
  // to test checkout redirect to stripe
  else if (this.checkout_session_id!='' && this.checkout_session_id!=undefined){

    var checkout={
      payment_intent:this.checkout_session_id,
      payment_intent_client_secret:'',
      redirect_status:''

    }
      localStorage.removeItem('payment');
      localStorage.setItem('payment', JSON.stringify(checkout));
      this.$store.dispatch("payments/setPaymentSuccess", checkout);

  }
  

  },
  mounted() {      
    if (!this.currentUser) {
      this.$store.dispatch('auth/logout');
      this.$router.push('/signin');
    }
  }
 };
</script>
