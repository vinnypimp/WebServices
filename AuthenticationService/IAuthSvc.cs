using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using AuthenticationService.Models;

namespace AuthenticationService
{
    [ServiceContract]
    public interface IAuthSvc
    {
        #region Test Contracts
        [OperationContract]
        [return: MessageParameter(Name = "Test")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "TestAuth")]
        List<AuthenticationResults> TestAuth();

        [OperationContract]
        [return: MessageParameter(Name = "Test")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "TestVal")]
        List<User> TestVal();
        #endregion

        [OperationContract]
        [return:MessageParameter(Name = "Validation")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "ValidateUser")]
        AuthenticationResults ValidateUser(Stream JSONdataStream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "RegisterNewUser")]
        wsSQLResult NewUser(Stream JSONdataStream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "UpdateUser")]
        wsSQLResult UpdateUser(Stream JSONdataStream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "SetPassword")]
        wsSQLResult SetPassword(Stream JSONdataStream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "InsertNewApp/{AppName}")]
        wsSQLResult InsertNewApp(string AppName);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "UpdateApp")]
        wsSQLResult UpdateApp(Stream JSONdataStream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "InsertNewRole")]
        wsSQLResult InsertNewRole(Stream JSONdataStream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "UpdateRole")]
        wsSQLResult UpdateRole(Stream JSONdataStream);
    }
}
