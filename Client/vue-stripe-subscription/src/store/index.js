import { createStore } from "vuex";
import { auth } from "./auth.module";
import { plan } from "./plan.module";
import { payments } from "./payments.module";
export const store = createStore({
  modules: {
    auth,plan,payments
  },
});

