using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthenticationService.Models
{
    [DataContract]
    [Serializable]
        public class User
        {

        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string HashedPassword { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
        }
}