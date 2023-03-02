<template>
    <img src="../assets/stripe.png" width="200">
    <div class="spinner hidden" id="spinner1"></div>
    <div id="payment-message" class="hidden"></div>
    <form id="payment-form">
        <div id="link-authentication-element">
            <!--Stripe.js injects the Link Authentication Element-->
        </div>
        <div id="payment-element">
            <!--Stripe.js injects the Payment Element-->
        </div>
        <button id="submit" class="hidden">
            <div class="spinner hidden" id="spinner"></div>
            <span id="button-text">Pay now ${{ selectedPlan.price }}</span>
        </button>
    </form>
    <div class='col-md-2 mt-3'>
        <button @click="changePlanClicked" type="button" class='form-control btn btn-default'>Change plan</button>
    </div>
</template>
<script lang="js">
import axios from 'axios';
import { AppSettings } from '../settings';
export default {
    data() {
        return {
            elements: {},
            loading: false,
            stripe: null,
            emailAddress: '',
            clientSecret: '',
            isSubscribed: false,
            subscriptionMessage: null,
            subscriptionId: null
        };
    },
    computed: {
        selectedPlan: function () {
            return this.$store.state.plan.selectedPlan;
        },
        currentUser() {
            return this.$store.state.auth.user;
        },
        showCheckout() {
            return this.$store.state.plan.status.showCheckout;
        }
    },
    mounted() {
        if (!this.currentUser) {
            this.$store.dispatch('auth/logout');
            this.$router.push('/signin');
        }
        else {
            // move it to env or get from azure function     
            document.querySelector("#spinner1").classList.remove("hidden");
            document.querySelector("#submit").classList.add("hidden");
            this.stripe = window.Stripe("pk_test_51MYsOVSA2hIwWxkOO54cGJTVjQZYU5yCIyQVXwWz8230o3jvhtMQaUHw0mo5l4rOl1QBCHxLmM472a2mmXr2AzUN00fputN2QL");
            this.initialize();
        }
    },
    methods: {
        async initialize() {
            var myPlan = JSON.parse(JSON.stringify(this.selectedPlan));
            var checkoutData = {
                priceId: myPlan.id,
                customerId: this.currentUser.user.id
            }
            axios.post(new AppSettings().API_URL + "subscribe", checkoutData, { headers: { Authorization: 'Bearer ' + this.currentUser.token } })
                .then((response) => {
                    console.log(response.data);
                    this.loading = false;
                    this.clientSecret = response.data.clientSecret;
                    this.subscriptionId = response.data.subscriptionId;
                    const appearance = { theme: 'stripe', };
                    const clientSecret = response.data.clientSecret;
                    this.elements = this.stripe.elements({ appearance, clientSecret });
                    const linkAuthenticationElement = this.elements.create("linkAuthentication");
                    linkAuthenticationElement.mount("#link-authentication-element");
                    linkAuthenticationElement.on('change', (event) => {
                        this.emailAddress = event.value.email;
                    });

                    const paymentElementOptions = {
                        layout: "tabs",
                    };

                    const paymentElement = this.elements.create("payment", paymentElementOptions);
                    paymentElement.mount("#payment-element");
                    document
                        .querySelector("#payment-form")
                        .addEventListener("submit", this.handleSubmit);
                    document.querySelector("#submit").classList.remove("hidden");
                    document.querySelector("#spinner1").classList.add("hidden");

                })
                .catch((errors) => {
                    console.log(errors);
                    this.loading = false;
                });

        },

        async handleSubmit(e) {
            e.preventDefault();
            this.setLoading(true);
            const elements = this.elements;
            const { error } = await this.stripe.confirmPayment({
                //`Elements` instance that was used to create the Payment Element
                elements,
                confirmParams: {
                    return_url: "http://localhost:8080/account", // move to api /config
                }
                //,redirect: "if_required"
            });
            if (error) {
                // This point will only be reached if there is an immediate error when
                // confirming the payment. Show error to your customer (for example, payment
                // details incomplete)
                this.showMessage(error.message);
            } else {
                // Your customer will be redirected to your `return_url`. For some payment
                // methods like iDEAL, your customer will be redirected to an intermediate
                // site first to authorize the payment, then redirected to the `return_url`.                
            }
            this.setLoading(false);
        },
        // ------- UI helpers -------

        showMessage(messageText) {
            const messageContainer = document.querySelector("#payment-message");

            messageContainer.classList.remove("hidden");
            messageContainer.textContent = messageText;

            setTimeout(function () {
                messageContainer.classList.add("hidden");
                messageText.textContent = "";
            }, 4000);
        },
        // Show a spinner on payment submission
        setLoading(isLoading) {
            if (isLoading) {
                // Disable the button and show a spinner
                document.querySelector("#submit").disabled = true;
                document.querySelector("#spinner").classList.remove("hidden");
                document.querySelector("#button-text").classList.add("hidden");
            } else {
                document.querySelector("#submit").disabled = false;
                document.querySelector("#spinner").classList.add("hidden");
                document.querySelector("#button-text").classList.remove("hidden");
            }
        },
        changePlanClicked() {              
        this.$store.dispatch("plan/setShowCheckout", false);
        console.log(this.showCheckout);
        this.$emit('changePlanClicked', this.$store.state.plan.status.showCheckout);
    }

    }       
};
</script>
<style scoped>
/* Variables */
* {
    box-sizing: border-box;
}

body {
    font-family: -apple-system, BlinkMacSystemFont, sans-serif;
    font-size: 16px;
    -webkit-font-smoothing: antialiased;
    display: flex;
    justify-content: center;
    align-content: center;
    height: 100vh;
    width: 100vw;
}

form {
    width: 30vw;
    min-width: 500px;
    align-self: center;
    box-shadow: 0px 0px 0px 0.5px rgba(50, 50, 93, 0.1),
        0px 2px 5px 0px rgba(50, 50, 93, 0.1), 0px 1px 1.5px 0px rgba(0, 0, 0, 0.07);
    border-radius: 7px;
    padding: 40px;
}

.hidden {
    display: none;
}

#payment-message {
    color: rgb(105, 115, 134);
    font-size: 16px;
    line-height: 20px;
    padding-top: 12px;
    text-align: center;
}

#payment-element {
    margin-bottom: 24px;
}

/* Buttons and links */
button {
    background: #5469d4;
    font-family: Arial, sans-serif;
    color: #ffffff;
    border-radius: 4px;
    border: 0;
    padding: 12px 16px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    display: block;
    transition: all 0.2s ease;
    box-shadow: 0px 4px 5.5px 0px rgba(0, 0, 0, 0.07);
    width: 100%;
}

.btn-default {
    background: #ccc;
}

button:hover {
    filter: contrast(115%);
}

button:disabled {
    opacity: 0.5;
    cursor: default;
}

/* spinner/processing state, errors */
.spinner,
.spinner:before,
.spinner:after {
    border-radius: 50%;
}

.spinner {
    color: #ffffff;
    font-size: 22px;
    text-indent: -99999px;
    margin: 0px auto;
    position: relative;
    width: 20px;
    height: 20px;
    box-shadow: inset 0 0 0 2px;
    -webkit-transform: translateZ(0);
    -ms-transform: translateZ(0);
    transform: translateZ(0);
}

.spinner:before,
.spinner:after {
    position: absolute;
    content: "";
}

.spinner:before {
    width: 10.4px;
    height: 20.4px;
    background: #5469d4;
    border-radius: 20.4px 0 0 20.4px;
    top: -0.2px;
    left: -0.2px;
    -webkit-transform-origin: 10.4px 10.2px;
    transform-origin: 10.4px 10.2px;
    -webkit-animation: loading 2s infinite ease 1.5s;
    animation: loading 2s infinite ease 1.5s;
}

.spinner:after {
    width: 10.4px;
    height: 10.2px;
    background: #5469d4;
    border-radius: 0 10.2px 10.2px 0;
    top: -0.1px;
    left: 10.2px;
    -webkit-transform-origin: 0px 10.2px;
    transform-origin: 0px 10.2px;
    -webkit-animation: loading 2s infinite ease;
    animation: loading 2s infinite ease;
}

@-webkit-keyframes loading {
    0% {
        -webkit-transform: rotate(0deg);
        transform: rotate(0deg);
    }

    100% {
        -webkit-transform: rotate(360deg);
        transform: rotate(360deg);
    }
}

@keyframes loading {
    0% {
        -webkit-transform: rotate(0deg);
        transform: rotate(0deg);
    }

    100% {
        -webkit-transform: rotate(360deg);
        transform: rotate(360deg);
    }
}

@media only screen and (max-width: 600px) {
    form {
        width: 80vw;
        min-width: initial;
    }
}
</style>
