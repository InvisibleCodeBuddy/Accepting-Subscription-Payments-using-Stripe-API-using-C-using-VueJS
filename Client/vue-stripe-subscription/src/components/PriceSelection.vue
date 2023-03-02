<template>
  <div class="row" v-if="!showCheckout">
    <div class="col mt-2">      
      <h2>Available Subscription Plans</h2> 
      <div class="mb-2">Select your appropriate plan and click on subscribe</div>
      <div v-for="(product, index) in products" 
            :key="index">            
          <div class="form-check">
            <input
              type="radio"
              class="form-check-input"
              v-model="selectedPrice"
              :value="product.id"
              :id="product.id" />            
            <label :for="product.id" class="form-check-label">
              <!--  map all the currencies used in plans to display it correctly-->
              <span class="text-lg">{{ product.planName }}</span>     {{ product.currency=='usd' ? '$' :''}}{{ product.price.toFixed(2) }} per   {{ product.billingInterval }}
            </label>
          </div>       
      </div>
      <button
        class="btn btn-primary mt-3"
        @click="createSub"
        :disabled="!selectedPrice || isLoading" >
        {{ isLoading ? "Loading..." : "Create subscription" }}
      </button>
      <hr>
      <div> Click below button to test stripe checkout redirect</div>
      <button
        class="btn btn-primary mt-3"
        @click="createSubRedirect"
        :disabled="!selectedPrice || isLoading" >
        {{ isLoading ? "Loading..." : "Create subscription - redirect to stripe" }}
      </button>
    </div>
  </div> 
  <CheckOut v-if="showCheckout" v-on:changePlanClicked="updatePlan"/>  
  
</template>
<script lang="js">
//import {ProductService} from '../services/productService';
import axios from 'axios';
import {AppSettings} from '../settings';
import CheckOut from "@/components/CheckOut";
import { ref } from 'vue'

export default {
  components: {   
     CheckOut    
  },
  data() {
    return {
      products: [],
      componentKey: ref(0),
      selectedPrice: null,
      isLoading: false,
      showCheckout:false,      
      selectedPlan:null,
      renderComponent: true, 
      
    };
  },
  computed: {
    currentUser() { 
      return this.$store.state.auth.user;     
    }
  },
  mounted() {    
    if (this.currentUser==null) {   
      this.$store.dispatch('auth/logout');
      this.$router.push('/signin');
    }
    else{
        this.fetchProducts();
    }    
  },
 
  methods: {
    fetchProducts() {   
      this.products= [];       
        axios.get(new AppSettings().API_URL+ "products",{ headers: { Authorization: 'Bearer ' + this.currentUser.token }})
                .then((response) => {                                   
                    this.products = response.data;                   
                    console.log(this.products);
                })
                .catch((errors) => {
                    console.log(errors); // Errors
                });   
    },
    createSub(){
      this.selectedPlan=null;
      this.showCheckout=true;   
      this.$store.dispatch("plan/setShowCheckout", this.showCheckout);
      var plan = this.products.filter(product => product.id == this.selectedPrice);
      this.selectedPlan=plan[0];
      // better use vue's passing object from parent to child here
      localStorage.removeItem('selectedplan');
      localStorage.setItem('selectedplan', JSON.stringify(this.selectedPlan));
      this.$store.dispatch("plan/setSelectedPlan", this.selectedPlan);
      
    },      
    createSubRedirect(){
      this.selectedPlan=null;    
      this.$store.dispatch("plan/setShowCheckout", this.showCheckout);
      var plan = this.products.filter(product => product.id == this.selectedPrice);
      this.selectedPlan=plan[0];
      // better use vue's passing object from parent to child here
      localStorage.removeItem('selectedplan');
      localStorage.setItem('selectedplan', JSON.stringify(this.selectedPlan));
      this.$store.dispatch("plan/setSelectedPlan", this.selectedPlan);
      var myPlan = JSON.parse(JSON.stringify(this.selectedPlan));           
            var checkoutData={
                priceId: myPlan.id,
                customerId: this.currentUser.user.id,
                stripeToken: "test",
            }
            axios.post(new AppSettings().API_URL + "checkout", checkoutData, { headers: { Authorization: 'Bearer ' + this.currentUser.token } })
                .then((response) => {
                    console.log(response.data);                   
                    this.loading = false;
                    const  url  = response.data.url;
                    console.log(url);
                    window.location.href = url;
                })
                .catch((errors) => {
                    console.log(errors);
                    this.loading = false;
                });
      
    },      
    updatePlan(){
      this.showCheckout=this.$store.state.plan.status.showCheckout;       
      //this.$store.dispatch("plan/setShowCheckout", this.showCheckout);
    }   
  }
 };
</script>
