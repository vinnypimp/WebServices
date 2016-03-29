using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using AuthenticationService.Models;

namespace AuthenticationService.Services
{
    [ServiceContract]
    public interface IAuthSvc
    {
        [OperationContract]
        [return: MessageParameter(Name = "Validation")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "validateUser")]
        User ValidateUser(string UserName, string HashedPassword);

    }
}
