using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthenticationService.Models
{
    [DataContract]
    [Serializable]
        public class AuthenticationResults
        {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime LastLoginDate { get; set; }

        [DataMember]
        public DateTime LastActivityDate { get; set; }

        [DataMember]
        public int ReturnCode { get; set; }

        [DataMember]
        public string ReturnReason { get; set; }

        }
}