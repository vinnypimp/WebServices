using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using AuthenticationService.Models;
using AuthenticationService.Data;

namespace AuthenticationService.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResults AuthenticateUser(LoginUser luser);
    }

    public class AuthenticationServices : IAuthenticationService
    {

        public AuthenticationResults AuthenticateUser(LoginUser luser)
        {
            InternalUserData userData = new InternalUserData();
            userData.GetUserData(luser.AppName);

            userData = InternalUserData.users.FirstOrDefault(u => u.Username.Equals(luser.UserName)
                && u.HashedPassword.Equals(luser.HashPassword));

            if (userData == null)
                throw new UnauthorizedAccessException("Access denied. Please provide valid credentials.");

            return GetUserInfo(userData.UserID);
        }

        private AuthenticationResults GetUserInfo(Guid UserID)
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();
            AuthenticationResults results = new AuthenticationResults();

            foreach (sp_appAuth_UserInfoResult userInfo in dc.sp_appAuth_UserInfo(UserID))
            {
                results.UserName = userInfo.UserName;
                results.Email = userInfo.Email;
                results.Role = userInfo.RoleName;
                results.FirstName = userInfo.FirstName;
                results.LastName = userInfo.LastName;
                results.LastActivityDate = userInfo.LastActivityDate;
                results.ReturnCode = 1;
                results.ReturnReason = "IsAuthenticated";
             };
            return results;
        }

        private string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            // Use the Hash algorithm to calculate the hash
            HashAlgorithm algorithm = new SHA256Cng();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash to base64 encoded string to be compared to stored password
            return Convert.ToBase64String(hash);
        }

    }

    public class LoginUser
    {
        public string UserName { get; set; }
        public string HashPassword { get; set; }
        public string AppName { get; set; }
    }

    public class InternalUserData
    {
        public static readonly List<InternalUserData> users = new List<InternalUserData>();

        public void GetUserData(string AppName)
        {
            AuthenticationDataContext dc = new AuthenticationDataContext();

            foreach (sp_appAuth_ValidateUserResult userInfo in dc.sp_appAuth_ValidateUser(AppName))
            {
                users.Add(new InternalUserData()
                {
                    UserID = userInfo.UserID,
                    Username = userInfo.UserName,
                    HashedPassword = userInfo.Password
                 });
            }
        }

        public string Username
        {
            get;
            private set;
        }

        public string HashedPassword
        {
            get;
            private set;
        }

        public Guid UserID
        {
            get;
            private set;
        }
    }
}
