using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CommonLibrary.ExceptionHandling;
using SAPEpsilonSend.Enumerations;
using Newtonsoft.Json;
using RestSharp;

namespace SAPEpsilonSend.BusinessLayer
{
    public class BoTransmission
    {
        #region Public Properties
        #endregion

        #region Private Properties
        private RestClient Client { get; set; }
        private RestRequest Request { get; set; }
        private BoEpsilonAuth Auth { get; set; }
        private string APICallID { get; set; }
        #endregion

        public BoTransmission(string _sApiCallID)
        {
            this.Auth = new BoEpsilonAuth();
            this.APICallID = _sApiCallID;
        }

        #region Public Methods
        /// <summary>
        /// Σύνδεση στην Υπηρεσία
        /// </summary>
        /// <returns>1 For Success, 0 For Failure</returns>
        public int Login()
        {
            int iRetVal = 0;
            try
            {
                if (this.Check4Login() > 0) //Υπάρχει Account και άρα κάνω έλεγχο για το αν έχει λήξει ή όχι
                {
                    if (!string.IsNullOrEmpty(this.Auth.jwt))
                    {
                        if (DateTime.Now < DateTime.Parse(this.Auth.jwtRefreshTokenExpiration))
                        {
                            //DoNothing
                            iRetVal++;
                        }
                        else
                        {
                            Logging.WriteToLog("BoTransmission.RefreshToken", Logging.LogStatus.START);
                            this.RefreshToken();
                            Logging.WriteToLog("BoTransmission.RefreshToken", Logging.LogStatus.END);
                        }
                    }
                }
                else
                {
                    Logging.WriteToLog("BoTransmission.LoginProcess", Logging.LogStatus.START);
                    iRetVal = this.LoginProcess();
                    Logging.WriteToLog("BoTransmission.LoginProcess", Logging.LogStatus.END);
                }
            }
            catch (Exception ex)
            {
                var a = new Logging("BoTransmission.Login", ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// Έλεγχος Αν Είναι Ενεργό το Connection
        /// </summary>
        /// <returns>1 For Active, 0 For Non Active</returns>
        public int IsConnectionActive()
        {
            int iRetVal = 0;
            try { }
            catch (Exception ex)
            {
                var a = new Logging("BoTransmission.IsConnectionActive", ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// Άνανέωση Token
        /// </summary>
        /// <returns>1 For Success, 0 For Failure</returns>
        public int RefreshToken()
        {
            int iRetVal = 0;
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                string sEndPoint = ini.IniReadValue("Default", "EndPointRefreshToken");

                BoEpsilonRefreshToken oRefresh = new BoEpsilonRefreshToken();
                oRefresh.refreshToken = ini.IniReadValue("Default", "EpsilonEmailAccount");
                oRefresh.token = ini.IniReadValue("Default", "EpsilonPassWord");

                object q = oRefresh;

                this.Client = new RestClient(sEndPoint);
                this.Client.Timeout = -1;
                this.Request = new RestRequest(Method.POST);
                this.Request.AddHeader("Content-Type", "application/json");

                string sJSONBody = new JavaScriptSerializer().Serialize(q);

                this.Request.AddParameter("application/json", sJSONBody, ParameterType.RequestBody);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IRestResponse response = this.Client.Execute(this.Request);
                Console.WriteLine(response.Content);
            }
            catch (Exception ex)
            {
                var a = new Logging("BoTransmission.RefreshToken", ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// Αποστολή Παραστατικού
        /// </summary>
        /// <param name="_oWrapper"> Το Παραστατικό που θα Αποσταλλεί</param>
        /// <param name="_oReply"> Η Απάντηση από το API</param>
        /// <param name="_sTranCode">Transaction Code</param>
        /// <returns>1 For Success, 0 for Failure</returns>
        public int Issue(BoEpsilonDocumentWrapper _oWrapper, out BoEpsilonDocumentReply _oReply, string _sTranCode)
        {
            int iRetVal = 0;
            _oReply = new BoEpsilonDocumentReply();
            try
            {
                //string sEndPoint = ConfigurationManager.AppSettings["EndPointPostDocuments"];
                string sEndPoint = this.Auth.url1;
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");
                sEndPoint += ini.IniReadValue("Default", "EndPointIssueDocument").Replace("qqq", "");

                object q = _oWrapper;

                this.Client = new RestClient(sEndPoint);
                this.Client.Timeout = -1;
                this.Request = new RestRequest(Method.POST);
                this.Request.AddHeader("Content-Type", "application/json");
                this.Request.AddHeader("Authorization", "Bearer " + this.Auth.jwt);

                string sJSONBody = new JavaScriptSerializer().Serialize(q);
                this.Request.AddParameter("application/json", sJSONBody, ParameterType.RequestBody);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //BoDAL.UpdateTransactionSETSendJSON(this.APICallID, sJSONBody);
                BoDAL.UpdateTransactionSETSendJSON(_sTranCode, sJSONBody, Connection.oCompany);


                IRestResponse oResponse = this.Client.Execute(this.Request);

                if (oResponse.StatusCode == HttpStatusCode.OK)
                {
                    _oReply = JsonConvert.DeserializeObject<BoEpsilonDocumentReply>(oResponse.Content);

                    BoDAL.LogEpsilonDocumentReply(Connection.oCompany, this.APICallID,
                        _oReply.authenticationCode,
                        _oReply.qrCode,
                        _oReply.documentId,
                        _oReply.extSystemId,
                        _oReply.uid,
                        _oReply.mark,
                        _oReply.status,
                        _oReply.pdfUploaded,
                        _oReply.pdfFileUrl,
                        _oReply.errorCode,
                        _oReply.errorMessage,
                        _oReply.errorSeverity
                        );
                }

                Logging.WriteToLog("BoTransmission.LogXML", Logging.LogStatus.START);
                this.LogXML(XMLFile.xmlIssue, oResponse.Content);
                Logging.WriteToLog("BoTransmission.LogXML", Logging.LogStatus.END);
            }
            catch (Exception ex)
            {
                var a = new Logging("BoTransmission.Issue", ex);
            }
            return iRetVal;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Έλεγχος Αν Υπάρχουν Δεδομένα
        /// </summary>
        /// <returns>1 For Success, 0 For Failure</returns>
        private int Check4Login()
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "SELECT COUNT(*) AS RESULT FROM ELIV_PARAMS";
                }
                else
                {
                    sSQL = "SELECT COUNT(*) AS RESULT FROM ELIV_PARAMS";
                }
                SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, Connection.oCompany);

                while (oRS.EoF == false)
                {
                    iRetVal = int.Parse(oRS.Fields.Item("Result").Value.ToString());
                    oRS.MoveNext();
                }

                if (iRetVal > 0)
                {
                    if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        sSQL = "SELECT TOP 1 *  FROM ELIV_PARAMS ORDER BY AA DESC";
                    }
                    else
                    {
                        sSQL = "SELECT TOP 1 * FROM ELIV_PARAMS ORDER BY AA DESC";
                    }
                    SAPbobsCOM.Recordset oRSParams = CommonLibrary.Functions.Database.GetRecordSet(sSQL, Connection.oCompany);

                    while (oRSParams.EoF == false)
                    {
                        this.Auth = new BoEpsilonAuth();
                        this.Auth.jwt = oRSParams.Fields.Item("JWT").Value.ToString();
                        this.Auth.jwtExpiration = oRSParams.Fields.Item("JWT_EXPIRATION").Value.ToString();
                        this.Auth.jwtRefreshToken = oRSParams.Fields.Item("JWT_REFRESH_TOKEN").Value.ToString();
                        this.Auth.jwtRefreshTokenExpiration = oRSParams.Fields.Item("JWT_REFRESH_TOKEN_EXPIRATION").Value.ToString();
                        this.Auth.url1 = oRSParams.Fields.Item("URL1").Value.ToString();
                        this.Auth.url2 = oRSParams.Fields.Item("URL2").Value.ToString();
                        oRSParams.MoveNext();
                    }

                    if (DateTime.Parse(this.Auth.jwtExpiration) < DateTime.Now)
                    {
                        //this.RefreshToken();
                        Logging.WriteToLog("BoTransmission.LoginProcess", Logging.LogStatus.START);
                        iRetVal = this.LoginProcess();
                        Logging.WriteToLog("BoTransmission.LoginProcess", Logging.LogStatus.END);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoTransmission.Check4Login", ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// Σύνδεση στην Υπηρεσία
        /// </summary>
        /// <returns>1 For Success, 0 For Failure</returns>
        private int LoginProcess()
        {
            int iRetVal = 0;
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");
                string sEndPoint = ini.IniReadValue("Default", "EndPointLogin");

                BoEpsilonLogin oLogin = new BoEpsilonLogin();
                oLogin.email = ini.IniReadValue("Default", "EpsilonEmailAccount");
                oLogin.password = ini.IniReadValue("Default", "EpsilonPassWord");
                oLogin.subscriptionKey = ini.IniReadValue("Default", "EpsilonSubscriptionKey");
                object q = oLogin;

                this.Client = new RestClient(sEndPoint);
                this.Client.Timeout = -1;
                this.Request = new RestRequest(Method.POST);
                this.Request.AddHeader("Content-Type", "application/json");

                string sJSONBody = new JavaScriptSerializer().Serialize(q);
                this.Request.AddParameter("application/json", sJSONBody, ParameterType.RequestBody);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IRestResponse oResponse = this.Client.Execute(this.Request);

                if (oResponse.StatusCode == HttpStatusCode.OK)
                {
                    this.Auth = JsonConvert.DeserializeObject<BoEpsilonAuth>(oResponse.Content);

                    Logging.WriteToLog("BoDAL.LogNewToken", Logging.LogStatus.START);
                    BoDAL.LogNewToken(this.Auth, Connection.oCompany);
                    Logging.WriteToLog("BoDAL.LogNewToken", Logging.LogStatus.END);

                    iRetVal++;
                }

                Logging.WriteToLog("BoTransmission.LogXML", Logging.LogStatus.START);
                this.LogXML(XMLFile.xmlLogin, oResponse.Content);
                Logging.WriteToLog("BoTransmission.LogXML", Logging.LogStatus.END);
            }
            catch (Exception ex)
            {
                var a = new Logging("BoTransmission.LoginProcess", ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// ΑΠοθήκευση XML Αρχείου
        /// </summary>
        /// <param name="_enType">Τύπος Αρχείου</param>
        /// <param name="_sContent">Περιεχόμενο Αρχείου</param>
        /// <returns>1 For Success, 0 For Failure</returns>
        private int LogXML(XMLFile _enType, string _sContent)
        {
            int iRetVal = 0;
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                string sSuffix = "";
                string sPath = "";
                string sDestination = "";
                if (ini.IniReadValue("Default", "SaveXMLResponse") == "1")
                {
                    switch (_enType)
                    {
                        case XMLFile.xmlIssue:
                            sSuffix = "Issue_" + this.APICallID + "";
                            sPath = ini.IniReadValue("Default", "XMLPathIssue");
                            break;
                        case XMLFile.xmlLogin:
                            sSuffix = "Login_" + this.APICallID + "";
                            sPath = ini.IniReadValue("Default", "XMLPathLogin");
                            break;
                        case XMLFile.xmlRefresh:
                            sSuffix = "Refresh_" + this.APICallID + "";
                            sPath = ini.IniReadValue("Default", "XMLPathRefresh");
                            break;
                        case XMLFile.xmlGetReply:
                            sSuffix = "GetReply_" + this.APICallID + "";
                            sPath = ini.IniReadValue("Default", "XMLPathGET");
                            break;
                    }

                    sDestination = sPath + sSuffix + ".xml";

                    using (StreamWriter sw = File.CreateText(sDestination))
                    {
                        sw.WriteLine(_sContent);
                    }
                }
            }
            catch (Exception ex)
            {
                var a = new Logging("BoTransmission.LoginProcess", ex);
            }
            return iRetVal;
        }

        #endregion
    }
}
