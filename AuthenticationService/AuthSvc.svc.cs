using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Script.Serialization;
using AuthenticationService.Models;
using AuthenticationService.Data;
using AuthenticationService.Services;

namespace AuthenticationService
{
    public class AuthSvc : IAuthSvc
    {
        #region Test Services
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

        public List<User> TestVal()
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();
            List<User> results = new List<User>();

            foreach (vw_appAuth_ValidateUser auth in dc.vw_appAuth_ValidateUsers)
            {
                results.Add(new User()
                {
                    AppName = auth.AppName,
                    UserName = auth.UserName,
                    FirstName = auth.FirstName,
                    LastName = auth.LastName,
                    HashPassword = auth.Password,
                    UserID= auth.UserID
                });
            }

            return results;
        }
        #endregion

        #region Validate User
        public AuthenticationResults ValidateUser(Stream JSONdataStream)
       {
            AuthenticationResults auth = new AuthenticationResults();
            try
            {
                //Read JSON Stream into a string
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                //Convert to Authentication Results
                JavaScriptSerializer jss = new JavaScriptSerializer();
                LoginUser luser = jss.Deserialize<LoginUser>(JSONdata);
                if (luser == null)
                {
                    //Error: Coudln't deserialize JSON string
                    auth.ReturnCode = 100;
                    auth.ReturnReason = "Unable to deserialize the JSON data.";
                    return auth;
                }

                else
                {
                    AuthenticationServices authenticate = new Services.AuthenticationServices();
                    auth = authenticate.AuthenticateUser(luser);
                }
                return auth;
            }
            catch (Exception ex)
            {
                auth.ReturnCode = 0;
                auth.ReturnReason = ex.Message;
                return auth;
            }
        }
        #endregion  

        #region Users
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
                   returnCode = (int)dc.sp_appAuth_Membership_CreateUser(
                   user.AppName,
                   user.UserName,
                   user.HashPassword,
                   user.Email,
                   ref r);
                   dc.SubmitChanges();
                }

                result.ReturnCode = 1;
                return result;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = 0;
                result.Exception = "An exception occurred: " + ex.Message;
                return result;
            }
        }

        private void UserEmailCheck(string appName, string username, string email)
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();

            foreach (sp_appAuth_ChecksResult checks in dc.sp_appAuth_Checks(appName, username, email))
            {
                returnCode = Convert.ToInt16(checks.ReturnCode);
                returnReason = checks.ReturnReason;
            }
        }

        public wsSQLResult UpdateUser(Stream JSONdataStream)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                //Read JSON Stream into a String..
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // Convert to New User record..
                JavaScriptSerializer jss = new JavaScriptSerializer();
                User user = jss.Deserialize<User>(JSONdata);
                if (user == null)
                {
                    // Error: Couldn't deserialize JSON String
                    result.WasSuccessful = 0;
                    result.Exception = "Unable to deserialize the JSON data.";
                    return result;
                }
                AuthenticationDataContext dc = new AuthenticationDataContext();
                returnCode = (int)dc.sp_appAuth_Membership_UpdateUser(
                user.AppName,
                user.UserName,
                user.Email,
                user.Role,
                user.FirstName,
                user.LastName,
                user.IsApproved,
                user.LastLoginDate);
                dc.SubmitChanges();

                result.ReturnCode = 1;
                return result;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = -1;
                result.Exception = "An exception occurred: " + ex.Message;
                return result;  //Failed
            }
        }

        public wsSQLResult SetPassword(Stream JSONdataStream)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                //Read JSON Stream into a String..
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // Convert to New User record..
                JavaScriptSerializer jss = new JavaScriptSerializer();
                User user = jss.Deserialize<User>(JSONdata);
                if (user == null)
                {
                    // Error: Couldn't deserialize JSON String
                    result.WasSuccessful = 0;
                    result.Exception = "Unable to deserialize the JSON data.";
                    return result;
                }
                AuthenticationDataContext dc = new AuthenticationDataContext();
                returnCode = (int)dc.sp_appAuth_Membership_SetPassword(
                user.AppName,
                user.UserName,
                user.HashPassword);
                dc.SubmitChanges();

                result.ReturnCode = 1;
                return result;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = -1;
                result.Exception = "An exception occurred: " + ex.Message;
                return result;  //Failed
            }
        }
        #endregion

        #region Apps
        public wsSQLResult InsertNewApp(string AppName)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                Guid? r = new Guid?();
                AuthenticationDataContext dc = new AuthenticationDataContext();

                // Run Create Application Stored Procedure
                result.ReturnCode = (int)dc.sp_appAuth_Applications_CreateApplication(
                                   AppName,
                                   ref r);
                dc.SubmitChanges();

                result.WasSuccessful = 1;
                result.Exception = "";
                return result;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = 0;
                result.Exception = ex.Message;
                return result;
            }
        }

        public wsSQLResult UpdateApp(Stream JSONdataStream)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                //Read JSON Stream into a String..
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // Convert to New User record..
                JavaScriptSerializer jss = new JavaScriptSerializer();
                App app = jss.Deserialize<App>(JSONdata);
                if (app == null)
                {
                    // Error: Couldn't deserialize JSON String
                    result.WasSuccessful = 0;
                    result.Exception = "Unable to deserialize the JSON data.";
                    return result;
                }
                AuthenticationDataContext dc = new AuthenticationDataContext();
                appAuth_Application currentApp = dc.appAuth_Applications.Where(a => a.AppID == a.AppID).FirstOrDefault();
                if (currentApp == null)
                {
                    // Couldnt Find User to Update
                    result.WasSuccessful = -3;
                    result.Exception = "Could not find a [appAuth_Application] record with ID: " + app.AppName.ToString();
                    return result;
                }

                // Update Record to SQL Server Table
                currentApp.AppName = app.AppName;
                currentApp.Description = app.Description;

                dc.SubmitChanges();

                result.WasSuccessful = 1;
                result.Exception = "";
                return result;  //Success
            }
            catch (Exception ex)
            {
                result.WasSuccessful = -1;
                result.Exception = "An exception occurred: " + ex.Message;
                return result;  //Failed
            }
        }

        #endregion  

        #region Roles
        public wsSQLResult InsertNewRole(Stream JSONdataStream)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                //Read JSON Stream into a String..
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // Convert to New Role record..
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Role role = jss.Deserialize<Role>(JSONdata);
                if (role == null)
                {
                    // Error: Couldn't deserialize JSON String
                    result.WasSuccessful = 0;
                    result.Exception = "Unable to deserialize the JSON data.";
                    return result;
                }

                Guid? r = new Guid?();
                AuthenticationDataContext dc = new AuthenticationDataContext();

                // Run Create Role Stored Procedure
                result.ReturnCode = (int)dc.sp_appAuth_Roles_CreateRole(
                                   role.RoleName,
                                   role.AppName,
                                   ref r);
                dc.SubmitChanges();

                result.WasSuccessful = 1;
                result.Exception = "";
                return result;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = 0;
                result.Exception = ex.Message;
                return result;
            }
        }

        public wsSQLResult UpdateRole(Stream JSONdataStream)
        {
            wsSQLResult result = new wsSQLResult();
            try
            {
                //Read JSON Stream into a String..
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // Convert to New User record..
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Role role = jss.Deserialize<Role>(JSONdata);
                if (role == null)
                {
                    // Error: Couldn't deserialize JSON String
                    result.WasSuccessful = 0;
                    result.Exception = "Unable to deserialize the JSON data.";
                    return result;
                }
                AuthenticationDataContext dc = new AuthenticationDataContext();
                appAuth_Role currentRole = dc.appAuth_Roles.Where(r => r.RoleID == r.RoleID).FirstOrDefault();
                if (currentRole == null)
                {
                    // Couldnt Find User to Update
                    result.WasSuccessful = -3;
                    result.Exception = "Could not find a [appAuth_Role] record with ID: " + role.RoleName.ToString();
                    return result;
                }

                // Update Record to SQL Server Table
                currentRole.RoleName = role.RoleName;
                currentRole.Description = role.Description;
                dc.SubmitChanges();

                result.WasSuccessful = 1;
                result.Exception = "";
                return result;  //Success
            }
            catch (Exception ex)
            {
                result.WasSuccessful = -1;
                result.Exception = "An exception occurred: " + ex.Message;
                return result;  //Failed
            }
        }

        #endregion

        public int returnCode {get; set;}
        public string returnReason {get; set; }
        
    }
}
