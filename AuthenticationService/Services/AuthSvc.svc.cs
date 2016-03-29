using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AuthenticationService.Models;
using AuthenticationService.Data;

namespace AuthenticationService.Services
{
    public class AuthSvc : IAuthSvc
    {
        public User ValidateUser()
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();
            User results = new User();

            foreach (appAuth_ValidateUserResult user in dc.appAuth_ValidateUser(AppName, UserName, Password))
            {
                results.UserName = user.UserName;
                results.Email = user.Email;
                results.Role = user.RoleName;
                //results.FirstName
                //results.LastName
            };

            return results;
        }

        public string AppName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
