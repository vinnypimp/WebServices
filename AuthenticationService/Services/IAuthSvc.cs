using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
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
        AuthenticationResults ValidateUser(string AppName, string UserName, string HashedPassword);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "registerNewUser")]
        wsSQLResult NewUser(Stream JSONdataStream);
    }
}
