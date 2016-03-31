using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Script.Serialization;
using AuthenticationService.Models;
using AuthenticationService.Data;

namespace AuthenticationService
{
    public class AuthSvc : IAuthSvc
    {
        public List<AuthenticationResults> TestAuth()
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();
            List<AuthenticationResults> results = new List<AuthenticationResults>();

            foreach (vw_appAuth_UserInfo auth in dc.vw_appAuth_UserInfos)
            {
                results.Add(new AuthenticationResults()
                {
                    UserName = auth.UserName,
                    FirstName = auth.FirstName,
                    LastName = auth.LastName,
                    Email = auth.Email,
                    Role = auth.RoleName
                });
            }

            return results;
        }

        public AuthenticationResults ValidateUser(string AppName, string UserName, string Password)
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();
            AuthenticationResults results = new AuthenticationResults();

            foreach (appAuth_ValidateUserResult auth in dc.appAuth_ValidateUser(AppName, UserName, Password))
            {
                results.UserName = auth.UserName;
                results.Email = auth.Email;
                results.Role = auth.RoleName;
                results.FirstName = auth.FirstName;
                results.LastName = auth.LastName;
                results.ReturnCode = auth.ReturnCode;
                results.ReturnReason = auth.ReturnReason;
            };

            return results;
        }

        public wsSQLResult NewUser(Stream JSONdataStream)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                //Read JSON Stream into String..
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // Convert to New User Record..
                JavaScriptSerializer jss = new JavaScriptSerializer();
                User user = jss.Deserialize<User>(JSONdata);
                if (user == null)
                {
                    // Error: Couldn't deserialize JSON string
                    result.WasSuccessful = 0;
                    result.Exception = "Unable to deserialize the JSON Data";
                    return result;
                }
                
                // Check if Username or Email Exists
                UserEmailCheck(user.AppName, user.UserName, user.Email);
                if (returnCode == 0)
                {
                   Guid? r = new Guid?();
                   AuthenticationDataContext dc = new AuthenticationDataContext();
                   returnCode = dc.appAuth_Membership_CreateUser(
                   user.AppName,
                   user.UserName,
                   user.HashedPassword,
                   user.Email,
                   DateTime.Now,
                   "User", false, DateTime.Now, 0, ref r);
                   dc.SubmitChanges();
                }

                result.ReturnCode = returnCode;
                return result;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = -1;
                result.Exception = "An exception occurred: " + ex.Message;
                return result;
            }
        }

        private void UserEmailCheck(string appName, string username, string email)
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();

            foreach (appAuth_ChecksResult checks in dc.appAuth_Checks(appName, username, email))
            {
                returnCode = Convert.ToInt16(checks.ReturnCode);
                returnReason = checks.ReturnReason;
            }
        }


        //public string AppName { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
        //public string Email { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string RoleName { get; set; }

        public int returnCode {get; set;}
        public string returnReason {get; set; }
        
    }
}
