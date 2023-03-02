using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeSubscription.Model
{
    public class User
    {
        public int Id { get; set; }       
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Apartment { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }

        public User() { }
       public User(int id, string name, string email, string password, string userName, 
            string apartment, string street, string city, string state, string country, string zip)
        {
            Id = id;
            Name = name;
            Email = email;
            UserName = userName;
            Password = password;
            Apartment = apartment;
            Street = street;
            City = city;
            State = state;
            Country = country;
            Zip = zip;
        }

    }

    public class UserCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthData
    {
        public User User { get; set; }
        public string Token { get; set; }
    }
}
