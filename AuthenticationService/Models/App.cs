using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthenticationService.Models
{
    [DataContract]
    [Serializable]
        public class App
        {

        [DataMember]
        public Guid AppID { get; set; }

        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public string Description { get; set; }

        }
}