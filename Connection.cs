using System;
using SAPbobsCOM;
using CommonLibrary.ExceptionHandling;
using System.Configuration;

namespace SAPEpsilonSend
{
    public class Connection
    {
        #region Public Properties
        public static SAPbobsCOM.Company oCompany { get; set; }
        public static SAPbobsCOM.BoYesNoEnum Connected { get; set; }
        #endregion

        public Connection()
        {
            Logging.WriteToLog("Connection.Connect", Logging.LogStatus.START);
            this.Connect();
            Logging.WriteToLog("Connection.Connect", Logging.LogStatus.END);
        }

        public void SapConnection()
        {
            Logging.WriteToLog("SapConnection.Connect", Logging.LogStatus.START);
            this.Connect();
            Logging.WriteToLog("SapConnection.Connect", Logging.LogStatus.END);
        }

        #region Private Methods
        private int Connect()
        {
            int iRetVal = 0;
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                oCompany = new SAPbobsCOM.Company();
                //oCompany.Server = "10.0.1.105:30015";
                oCompany.Server = ini.IniReadValue("Default", "ServerIP");

                switch (ini.IniReadValue("Default", "DatabaseVersion"))
                {
                    case "MSSQL2017":
                        oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2017;
                        break;
                    case "MSSQL2019":
                        oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2019;
                        break;
                    case "HANA":
                        oCompany.DbServerType = BoDataServerTypes.dst_HANADB;
                        break;
                }


                oCompany.UseTrusted = false;
                oCompany.DbUserName = ini.IniReadValue("Default", "DbUserName");
                oCompany.DbPassword = ini.IniReadValue("Default", "DbPassword");
                oCompany.CompanyDB = ini.IniReadValue("Default", "Database");
                oCompany.UserName = ini.IniReadValue("Default", "B1UserName");
                oCompany.Password = ini.IniReadValue("Default", "B1Password");
                oCompany.LicenseServer = ini.IniReadValue("Default", "LicenseServer");

                if (oCompany.Connect() == 0)
                {
                    Console.WriteLine("Connected successfully to DB: " + oCompany.CompanyDB);
                    Connected = BoYesNoEnum.tYES;
                    iRetVal++;
                }
                else
                {
                    Connected = BoYesNoEnum.tNO;
                    Console.WriteLine("Connection Failed because: " + oCompany.GetLastErrorDescription());
                    int nErr;
                    string sErrMsg;
                    oCompany.GetLastError(out nErr, out sErrMsg);

                    Logging.WriteToLog("Invalid Connection", Logging.LogStatus.RET_VAL);
                    Logging.WriteToLog("DI Error: " + nErr.ToString() + " / " + sErrMsg, Logging.LogStatus.RET_VAL);
                }
            }
            catch (Exception ex)
            {
                var a = new Logging("SapConnection.Connect", ex);
            }
            return iRetVal;


        }
        #endregion
    }
}
