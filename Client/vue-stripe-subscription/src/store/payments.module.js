const payment = JSON.parse(localStorage.getItem('payment'));
const initialState = payment
  ? { status: { success: true}, payment }
  : { status: { success: false }, payment: null };

export const payments= {
  namespaced: true,
  state: initialState,  
  actions: {
      setPaymentSuccess({commit}, payment) {              
          commit('setpayment', payment);
      },
      clearPayment({commit}){
        commit('removePayment');
      }
  },
  mutations: {
      setpayment(state, payment) {
          state.payment = payment;
          state.status.success=true;
          console.log(state.payment);         
      },
      removePayment(state){
        state.payment=null;
        state.status.success=false;
      }
  }
}
