<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
      <a class="navbar-brand" href="#"> Vue Stripe Subscriptions </a>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item">
            <router-link class="nav-link" active-class="active" to="/">
              Home
            </router-link>
          </li>

          <li v-if="!currUser">
            <router-link class="nav-link" active-class="active" to="/signin">
              Sign In
            </router-link>
          </li>

          <li v-if="currUser">
            <router-link class="nav-link" active-class="active" to="/account">
              {{ currUser.user.name }}
            </router-link>
          </li>

          <li v-if="currUser">
            <button class="btn btn-outline-secondary" @click="logOut">
              Log out
            </button>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script lang="js">
export default {
  data() {
  },
  computed: {
    currUser() {      
      return this.$store.state.auth.user;
    }
  },
  methods: {
    logOut() {
      console.log("from nav");
      this.$store.dispatch('auth/logout');
      this.$router.push('/signin');
    },
  },
};
</script>