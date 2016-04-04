﻿using System;
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
        public string HashPassword { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public bool IsApproved { get; set; }

        [DataMember]
        public DateTime LastLoginDate { get; set; }

        }
}