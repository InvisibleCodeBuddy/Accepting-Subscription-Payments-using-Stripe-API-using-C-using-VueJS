const selectedPlan = JSON.parse(localStorage.getItem('selectedplan'));
const initialState = selectedPlan
  ? { status: { selected: true, showCheckout:false }, selectedPlan }
  : { status: { selected: false }, selectedPlan: null };

export const plan= {
  namespaced: true,
  state: initialState,  
  actions: {
      setSelectedPlan({commit}, selectedPlan) {              
          commit('setplan', selectedPlan);
      },
      setShowCheckout({commit},show){
        commit('changeShowCheckout', show);
      },
      clearPlan({commit}){
        commit('removePlan');
      }
  },
  mutations: {
      setplan(state, selectedPlan) {
          state.selectedPlan = selectedPlan;
          state.status.selected=true;
          console.log(state.selectedPlan);         
      },
      changeShowCheckout(state, show) {
        state.status.showCheckout = show;             
    },
      removePlan(state){
        state.selectedPlan=null;
        state.status.selected=false;
      }
  }
}
