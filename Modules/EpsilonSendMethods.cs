using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SAPEpsilonSend.BusinessLayer;
//using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Threading;
using CommonLibrary.ExceptionHandling;
using SAPEpsilonSend.Enumerations;
using SAPbobsCOM;

namespace SAPEpsilonSend.Modules
{
    class EpsilonSendMethods
    {


        #region Private Variables
        private Thread _thread;
        #endregion

        #region Private Properties
        private SAPbobsCOM.Company CompanyConnection { get; set; }
        private List<string> RunningMinutes { get; set; }
        private int StopService { get; set; }
        #endregion

        public static Settings1 settings = new SAPEpsilonSend.Settings1();

        public EpsilonSendMethods()
        { }

        public void Start()
        {
            int milliseconds = 30000;
            Thread.Sleep(milliseconds);

            //this.SendMail("Service Is Started!");

            _thread = new Thread(new ThreadStart(Execute));
            _thread.Start();
        }

        public void Stop()
        {
            this.SendMail("Service Is Stopped!");
            Logging.WriteToLog("Service Stopped", Logging.LogStatus.ERROR);

            if (_thread != null)
            {
                if (CompanyConnection != null)
                {
                    CompanyConnection.Disconnect();
                }

                _thread.Abort();
                _thread.Join();
            }
        }

        public void Execute()
        {
            try
            {
                int iResult = 0;

              
                Logging.WriteToLog("SERVICE STARTING", Logging.LogStatus.START);
                this.ManageSAPConnection();
              
                while (true)
                {

                    if (Connection.oCompany != null && Connection.oCompany.Connected == true)
                    {
                        iResult = 0;
                        ElectronicInvoicingMethods oLoad = new ElectronicInvoicingMethods();
                        //Logging.WriteToLog("EpsilonSendMethods.Load", Logging.LogStatus.START);
                        bool connected=true;
                        iResult = oLoad.Load(TransactionTypes.tt_SalesLoad,out connected);
                       // Logging.WriteToLog("EpsilonSendMethods.Load", Logging.LogStatus.END);

                        if (iResult == 1 && oLoad.ListDocuments.Count > 0)
                        {
                           // Logging.WriteToLog("EpsilonSendMethods.Send", Logging.LogStatus.START);
                            iResult += oLoad.Send(TransactionTypes.tt_SalesSend, out connected);
                           // Logging.WriteToLog("EpsilonSendMethods.Send", Logging.LogStatus.END);
                        }
                        if (connected == false)
                        {
                            this.ManageSAPConnection();
                        }
                    }
                    else
                    {
                        this.ManageSAPConnection();
                    }
                }
            }
            catch (Exception ex)
            {
                var a = new Logging("Execute", ex);
            }
        }


        private void ManageSAPConnection()
        {
            while (Connection.oCompany == null || (Connection.oCompany != null && Connection.oCompany.Connected == false))
            {
                Logging.WriteToLog("EpsilonSendMethods.ConnectDI", Logging.LogStatus.START);
                this.ConnectDI();
                Logging.WriteToLog("EpsilonSendMethods.ConnectDI", Logging.LogStatus.END);
                if (Connection.oCompany == null || (Connection.oCompany != null && Connection.oCompany.Connected == false))
                {
                    Logging.WriteToLog("Could not connect to SAP. Going to sleep", Logging.LogStatus.START);
                    Thread.Sleep(300000);
                    Logging.WriteToLog("Waking Up", Logging.LogStatus.END);
                }
            }
        }

        public void ConnectDI()
        {
            try
            {
                Connection oConnection = new Connection();
            }
            catch (Exception ex)
            {
                var a = new Logging("Main.ConnectDI", ex);
            }
        }

        public void SendMail(string _sSubject)
        {
            try
            {
                List<string> ListRecepients = new List<string>();

                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");
                BoMail oMail = new BoMail();

                string sRecepients = ini.IniReadValue("Default", "MailReceiver");
                ListRecepients = sRecepients.Split(';').ToList();

                oMail.SenderDisplayName = ini.IniReadValue("Default", "MailSenderDisplayName") + " Send";
                oMail.Subject = _sSubject;

                for (int i = 0; i < ListRecepients.Count; i++)
                {
                    oMail.Recepient = ListRecepients[i];
                    oMail.SendMail();
                }
            }
            catch (Exception ex)
            {
                var a = new Logging("SendMail", ex);
            }
        }
    }
}
