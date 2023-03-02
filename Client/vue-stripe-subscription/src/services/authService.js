import axios from 'axios';
import {AppSettings} from '../settings'

export class AuthService {    
  appSettings = new AppSettings();
    async authenticateUser(user) {
    return axios
      .post(this.appSettings.API_URL + 'auth', {
        username: user.username,
        password: user.password
      })
      .then(response => {
        if (response.data.token) {
          localStorage.setItem('user', JSON.stringify(response.data));
        }
        return response.data;
      });
  }

  signOut() {
    localStorage.removeItem('user');
  }

  getCurrentUser() {   
    let user = JSON.parse(localStorage.getItem('user'));
    return user 
    }
  
}

