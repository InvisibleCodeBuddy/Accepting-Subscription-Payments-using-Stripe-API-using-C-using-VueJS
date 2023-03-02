using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StripeSubscription.Model;

namespace StripeSubscription.Data
{
    public class UserData
    {
        public static List<User> GetUserList()
        {
            var UserList = new List<User>()
            {
                new User(1,"Fred George","santhi_75@yahoo.com","Test@123","fred","apt #789","street1","City1","LN","US","LN2"),
                new User(2,"Alfred Windson","alfre.wind@gmail.com","Test@123","alfred","apt #789","street1","City1","LN","US","LN2"),
                new User(3,"Edwin George","mayer.striptest@gmail.com","Test@123","edwin","apt #789","street1","City1","LN","US","LN2"),
                new User(4,"Biden Herald","rolan.striptest@gmail.com","Test@123","biden","apt #789","street1","City1","LN","US","LN2"),
                new User(5,"Joe Rand","smith.striptest@gmail.com","Test@123","joe","apt #789","street1","City1","LN","US","LN2"),
                new User(6,"Maddison William","davis.striptest@gmail.com","Test@123","maddison","apt #789","street1","City1","LN","US","LN2"),
                new User(7,"Marry Angel","bassett.striptest@gmail.com","STest@123","marry","apt #789","street1","City1","LN","US","LN2"),
                new User(8,"Wilfred Mathew","morgan.striptest@gmail.com","Test@123","wilfred","apt #789","street1","City1","LN","US","LN2"),
                new User(9,"Clint Gonathan","williams.striptest@gmail.com","Test@123","clint","apt #789","street1","City1","LN","US","LN2"),
                new User(10,"Clara Elizabeth","marry.striptest@gmail.com","Test@123","clara","apt #789","street1","City1","LN","US","LN2"),
                new User(11,"Maria Beauty","Aagel.striptest@gmail.com","TTest@123","maria","apt #789","street1","City1","LN","US","LN2"),
                new User(12,"Deborah Jordan","davis_gord.striptest@gmail.com","Test@123","deborah","apt #789","street1","City1","LN","US","LN2")
            };

            return UserList;
        }
    }
}
