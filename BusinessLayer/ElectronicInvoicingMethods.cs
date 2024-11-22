using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CommonLibrary.ExceptionHandling;
using SAPEpsilonSend.Enumerations;
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
    public class ElectronicInvoicingMethods
    {
        #region Public Properties
        public List<BoDocument> ListDocuments { get; set; }
        #endregion

        #region Private Properties
        #endregion

        public ElectronicInvoicingMethods()
        { }

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Φόρτωση Δεδομένων για Αποστολή
        /// </summary>
        /// <param name="_enType">Τύπος Αντικειμένου Φόρτωσης</param>
        /// <returns>1 For Success, 0 For Failure</returns>
        public int Load(TransactionTypes _enType, out bool connected)
        {
            int iRetVal = 0;
            try
            {
                this.ListDocuments = new List<BoDocument>();

                LoadClass oLoad = new LoadClass();
                iRetVal = oLoad.Exec(_enType);

                this.ListDocuments = oLoad.ListDocuments;
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
                var a = new Logging("ElectronicInvoicingMethods.Load", ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// Αποστολή Δεδομένων στο API
        /// </summary>
        /// <param name="_enType">Τύπος Αντικειμένου Αποστολής</param>
        /// <returns>1 For Success, 0 For Failure</returns>
        public int Send(TransactionTypes _enType, out bool connected)
        {
            int iRetVal = 0;
            try
            {
                SendClass oSend = new SendClass();
                oSend.ListDocuments = this.ListDocuments;

                iRetVal = oSend.Exec(_enType);
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
                var a = new Logging("ElectronicInvoicingMethods.Send", ex);
            }
            return iRetVal;
        }

        #endregion

        #region Internal Classes
        internal class LoadClass
        {
            #region Public Properties
            public List<BoDocument> ListDocuments { get; set; }
            #endregion

            #region Private Properties

            #endregion

            public LoadClass()
            {
                this.ListDocuments = new List<BoDocument>();
            }

            #region Private Methods
            private int LoadDocuments(string _sSQLBasic)
            {
                int iRetVal = 0;
                try
                {
                    this.ListDocuments = new List<BoDocument>();
                    BoDocument oDocument = null;
                    Invoicedetail oEpsilonDetail = null;
                    Paymentmethod oEpsilonPaymentMethod = null;
                    Invoicesummary oEpsilonInvoiceSummary = null;
                    List<taxesTotals> oEpsilonInvoiceTaxesTotals = null;

                    CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogs\\ConfParams.ini");

                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(_sSQLBasic, Connection.oCompany);



                    while (oRS.EoF == false)
                    {
                        //_sTranCode = oRSClass.Fields.Item("Result").Value.ToString();
                        string newRecordDocnum = oRS.Fields.Item("aa").Value.ToString();
                        BoDocument oResult = this.ListDocuments.Find(oElem => oElem.DocNum == oRS.Fields.Item("aa").Value.ToString());
                        //  oDocument.ObjectCode = oRSClass.Fields.Item("OBJECT_TYPE").Value.ToString();

                        if (oResult != null && !string.IsNullOrEmpty(oResult.DocNum)) //add line 2 document
                        {
                            oEpsilonDetail = new Invoicedetail();
                            //VAT rate 24%
                            oEpsilonDetail.classificationCategory = oRS.Fields.Item("ClassificationCategory").Value.ToString();
                            oEpsilonDetail.entityName = oRS.Fields.Item("entityName").Value.ToString();
                            oEpsilonDetail.incomeClassification = oRS.Fields.Item("incomeClassification").Value.ToString();
                            oEpsilonDetail.lineNumber = oRS.Fields.Item("lineNumber").Value.ToString();
                            oEpsilonDetail.lineType = "income";// oRS.Fields.Item("lineType").Value.ToString();
                            oEpsilonDetail.measurementUnit = oRS.Fields.Item("measurementUnit").Value.ToString(); //allocation table
                            oEpsilonDetail.netValue = oRS.Fields.Item("netValue").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.quantity = oRS.Fields.Item("quantity").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.totalValue = oRS.Fields.Item("totalValue").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.vatAmount = oRS.Fields.Item("vatAmount").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.vatCategory = oRS.Fields.Item("vatCategory").Value.ToString();//allocation table
                            oEpsilonDetail.vatExemption = oRS.Fields.Item("vatExemption").Value.ToString(); //allocation table
                            oEpsilonDetail.vatPercent = oRS.Fields.Item("vatPercent").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.fuelCode = oRS.Fields.Item("fuelCode").Value.ToString();
                            //15-11-2022
                            string quantity15 = oRS.Fields.Item("quantity15").Value.ToString();
                            if (!string.IsNullOrEmpty(quantity15))
                            {
                                oEpsilonDetail.quantity15 = Math.Round(double.Parse(quantity15.ToString()), 2).ToString();
                            }


                            //8-10-2022
                            //αυτό το section θα πάει στα totals γτ δεν μπορούμε να κάνουμε αναγωγή στις γραμμές
                            oEpsilonDetail.withheldAmount = oRS.Fields.Item("withheldAmount").Value.ToString().Replace(",", ".");// oRS.Fields.Item("withheldAmount").Value.ToString();
                            oEpsilonDetail.withheldDescription = oRS.Fields.Item("withheldDescription").Value.ToString();// oRS.Fields.Item("withheldDescription").Value.ToString();
                            oEpsilonDetail.withheldPercentage = oRS.Fields.Item("withheldPercentage").Value.ToString().Replace(",", ".");// oRS.Fields.Item("withheldPercentage").Value.ToString();
                            oEpsilonDetail.withheldPercentCategory = oRS.Fields.Item("withheldPercentCategory").Value.ToString();// oRS.Fields.Item("withheldPercentCategory").Value.ToString();

                            oEpsilonDetail.stampAmount = oRS.Fields.Item("stampDutyAmount").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.stampDescription = oRS.Fields.Item("stampDutyDescr").Value.ToString();
                            oEpsilonDetail.stampPercentage = oRS.Fields.Item("stampDutyPercentage").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.stampPercentCategory = oRS.Fields.Item("stampDutyPercentCategory").Value.ToString(); // oRS.Fields.Item("withheldPercentCategory").Value.ToString();


                            oResult.EpsilonDocument.invoiceDetails.Add(oEpsilonDetail);

                        }
                        else //New Document
                        {
                            string sDocNum = oRS.Fields.Item("aa").Value.ToString();
                            //string sDocNum = "301000159";

                            oDocument = new BoDocument();
                            oDocument.ObjectCode = oRS.Fields.Item("ObjType").Value.ToString();
                            oDocument.EpsilonDocument = new BoEpsilonDocument();

                            oDocument.DocumentType = EligibleSAPObjects.SalesDocuments;
                            oDocument.DocNum = sDocNum;
                            oDocument.DocEntry = oRS.Fields.Item("DocEntry").Value.ToString();

                            if (!string.IsNullOrEmpty(oRS.Fields.Item("E_Mail").Value.ToString()))
                            {
                                BoEpsilonMessages oMessage = new BoEpsilonMessages();
                                oMessage.type = "0";
                                oMessage.recipients = oRS.Fields.Item("E_Mail").Value.ToString();

                                oDocument.EpsilonDocument.Messages = new List<BoEpsilonMessages>();
                                oDocument.EpsilonDocument.Messages.Add(oMessage);

                            }
                            else
                            {
                            }

                            oDocument.EpsilonDocument.invoiceDetails = new List<Invoicedetail>();
                            oDocument.EpsilonDocument.invoiceHeader = new Invoiceheader();
                            oDocument.EpsilonDocument.invoiceSummary = new Invoicesummary();
                            oDocument.EpsilonDocument.issuer = new Issuer();
                            oDocument.EpsilonDocument.paymentMethods = new List<Paymentmethod>();

                            oDocument.EpsilonDocument.invoiceHeader.fuelInvoice = oRS.Fields.Item("fuelInvoice").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.exchangeRate = oRS.Fields.Item("exchangeRate").Value.ToString();

                            oDocument.EpsilonDocument.invoiceHeader.aa = sDocNum;
                            //oDocument.EpsilonDocument.invoiceHeader.correlatedInvoices = "";
                            //oDocument.EpsilonDocument.invoiceHeader.currency = oRS.Fields.Item("currency").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.currency = oRS.Fields.Item("DocCur").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.invoiceType = oRS.Fields.Item("invoiceType").Value.ToString();   //αν είναι ΤΠ / ΤΔΑ (να γίνει mapping) θα δίνω το seriescode
                            oDocument.EpsilonDocument.invoiceHeader.invoiceTypeDescription = oRS.Fields.Item("invoiceTypeDescription").Value.ToString();
                            //2021-11-15T10:33:41Z
                            //2021-11-25


                            //oDocument.EpsilonDocument.invoiceHeader.issueDate = DateTime.Parse("15/11/2021").ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");//oRS.Fields.Item("issueDate").Value.ToString();
                            //oDocument.EpsilonDocument.invoiceHeader.issueDate = DateTime.Parse("15/11/2021").ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff");//oRS.Fields.Item("issueDate").Value.ToString();
                            //oDocument.EpsilonDocument.invoiceHeader.issueDate = "2021-11-15T10:33:41Z";//oRS.Fields.Item("issueDate").Value.ToString();
                            //oDocument.EpsilonDocument.invoiceHeader.issueDate = DateTime.Now.ToString("yyyy-MM-dd");//oRS.Fields.Item("issueDate").Value.ToString();

                            oDocument.EpsilonDocument.invoiceHeader.issueDate = DateTime.Parse(oRS.Fields.Item("issueDate").Value.ToString()).ToString("yyyy-MM-dd");

                            if (!string.IsNullOrEmpty(oRS.Fields.Item("DispatchDate").Value.ToString()))
                            {
                                oDocument.EpsilonDocument.invoiceHeader.dispatchDate = DateTime.Parse(oRS.Fields.Item("DispatchDate").Value.ToString()).ToString("yyyy-MM-dd");//oRS.Fields.Item("issueDate").Value.ToString();
                            }

                            if (!string.IsNullOrEmpty(oRS.Fields.Item("delayedProcessCode").Value.ToString()))
                            {
                                oDocument.EpsilonDocument.delayedProcessCode = oRS.Fields.Item("delayedProcessCode").Value.ToString();
                            }

                            string correlatedInvoices = oRS.Fields.Item("correlatedInvoices").Value.ToString();
                            if (!string.IsNullOrEmpty(correlatedInvoices))
                            {
                                oDocument.EpsilonDocument.invoiceHeader.correlatedInvoices = new string[1];
                                oDocument.EpsilonDocument.invoiceHeader.correlatedInvoices[0] = correlatedInvoices;
                            }

                            oDocument.EpsilonDocument.invoiceHeader.movePurpose = oRS.Fields.Item("movePurpose").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.series = oRS.Fields.Item("series").Value.ToString();

                            oDocument.EpsilonDocument.counterpart = new Counterpart();
                            oDocument.EpsilonDocument.counterpart.city = oRS.Fields.Item("city_2").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.code = oRS.Fields.Item("CounterpartCode").Value.ToString(); //η μεσόγειος επέλεξε να είναι το cardcode
                            oDocument.EpsilonDocument.counterpart.country = oRS.Fields.Item("country_2").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.doy = oRS.Fields.Item("doy").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.email = oRS.Fields.Item("email").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.fax = oRS.Fields.Item("fax").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.name = oRS.Fields.Item("CounterpartName").Value.ToString(); //η μεσόγειος επέλεξε να είναι το cardname
                            oDocument.EpsilonDocument.counterpart.phone = oRS.Fields.Item("phone").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.postalCode = oRS.Fields.Item("postalCode_2").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.profession = oRS.Fields.Item("profession").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.street = oRS.Fields.Item("street_2").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.streetNumber = oRS.Fields.Item("streetNumber_2").Value.ToString();
                            oDocument.EpsilonDocument.counterpart.vatNumber = oRS.Fields.Item("BPvatNumber").Value.ToString();

                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryAfm = oRS.Fields.Item("deliveryAfm").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryApostoli = oRS.Fields.Item("deliveryApostoli").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryCity = oRS.Fields.Item("city_2").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryDoy = oRS.Fields.Item("deliveryDoy").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryFax = oRS.Fields.Item("deliveryFax").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryMeso = oRS.Fields.Item("DeliveryMeso").Value.ToString(); //να μιλήσει σύμβουλος για το τι να βάζει
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryParadosi = oRS.Fields.Item("deliveryParadosi").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryPhone = oRS.Fields.Item("deliveryPhone").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryPliromi = oRS.Fields.Item("deliveryPliromi").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryProfession = oRS.Fields.Item("deliveryProfession").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliverySkopos = oRS.Fields.Item("FormValuesDeliverySkopos").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryStreet = oRS.Fields.Item("deliveryStreetNr").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryStreetNr = oRS.Fields.Item("deliveryStreetNr").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryTitle = oRS.Fields.Item("FormValuesDeliveryTitle").Value.ToString(); //να μιλήσει σύμβουλος για το τι να βάζει
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryTropos = oRS.Fields.Item("deliveryTropos").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.deliveryZip = oRS.Fields.Item("deliveryZip").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.docComments = oRS.Fields.Item("DocComments").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.docNewYpol = oRS.Fields.Item("docNewYpol").Value.ToString().Replace(",", ".");
                            oDocument.EpsilonDocument.invoiceHeader.formValues.docPrevYpol = oRS.Fields.Item("docPrevYpol").Value.ToString().Replace(",", ".");
                            oDocument.EpsilonDocument.invoiceHeader.formValues.docRelated = oRS.Fields.Item("docRelated").Value.ToString();
                            oDocument.EpsilonDocument.invoiceHeader.formValues.docTime = oRS.Fields.Item("docTime").Value.ToString();

                            oDocument.EpsilonDocument.issuer.city = oRS.Fields.Item("city_1").Value.ToString();
                            oDocument.EpsilonDocument.issuer.country = oRS.Fields.Item("country_1").Value.ToString();
                            oDocument.EpsilonDocument.issuer.postalCode = oRS.Fields.Item("postalCode_1").Value.ToString();
                            oDocument.EpsilonDocument.issuer.street = oRS.Fields.Item("street_1").Value.ToString();
                            oDocument.EpsilonDocument.issuer.vatNumber = oRS.Fields.Item("CompanyvatNumber").Value.ToString();
                            oDocument.EpsilonDocument.issuer.streetNumber = oRS.Fields.Item("streetNumber_1").Value.ToString();

                            oEpsilonPaymentMethod = new Paymentmethod();
                            oEpsilonPaymentMethod.amount = oRS.Fields.Item("DocTotal").Value.ToString().Replace(",", "."); //TODO total value -> doctotal (για παρακρατούμενους αρχικά) 20240723
                            oEpsilonPaymentMethod.type = oRS.Fields.Item("PaymentMethodType").Value.ToString();   //mapping με τους κωδ. τις ααδε

                            oDocument.EpsilonDocument.paymentMethods.Add(oEpsilonPaymentMethod);

                            //Logging.WriteToLog("ElectronicInvoicingMethods.LoadClass.GetSummaries", Logging.LogStatus.START);
                            int iResultSummary = this.GetSummaries(oDocument.ObjectCode, oDocument.DocNum, out oEpsilonInvoiceSummary);


                            int iResultTaxesTotals = this.GetTaxes(oDocument.ObjectCode, oDocument.DocEntry, out oEpsilonInvoiceTaxesTotals);


                            //int iResultSummary = this.GetSummaries(oDocument.ObjectCode, "301000150", out oEpsilonInvoiceSummary);
                            //Logging.WriteToLog("ElectronicInvoicingMethods.LoadClass.GetSummaries", Logging.LogStatus.END);

                            if (iResultSummary != 1)
                            {
                                Logging.WriteToLog("Error During Summary Calculation", Logging.LogStatus.ERROR);
                                return 0;
                            }

                            oEpsilonDetail = new Invoicedetail();
                            //VAT rate 24%
                            oEpsilonDetail.classificationCategory = oRS.Fields.Item("ClassificationCategory").Value.ToString();
                            oEpsilonDetail.entityName = oRS.Fields.Item("entityName").Value.ToString();
                            oEpsilonDetail.incomeClassification = oRS.Fields.Item("IncomeClassification").Value.ToString();
                            oEpsilonDetail.lineNumber = oRS.Fields.Item("lineNumber").Value.ToString();
                            oEpsilonDetail.lineType = "income";// oRS.Fields.Item("lineType").Value.ToString();
                            oEpsilonDetail.measurementUnit = oRS.Fields.Item("measurementUnit").Value.ToString(); //allocation table
                            oEpsilonDetail.netValue = oRS.Fields.Item("netValue").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.quantity = oRS.Fields.Item("quantity").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.totalValue = oRS.Fields.Item("totalValue").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.vatAmount = oRS.Fields.Item("vatAmount").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.vatCategory = oRS.Fields.Item("vatCategory").Value.ToString();//allocation table
                            oEpsilonDetail.vatExemption = oRS.Fields.Item("vatExemption").Value.ToString(); //allocation table
                            oEpsilonDetail.vatPercent = oRS.Fields.Item("VatPercent").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.fuelCode = oRS.Fields.Item("FuelCode").Value.ToString();
                            //15-11-2022
                            string quantity15 = oRS.Fields.Item("quantity15").Value.ToString();
                            if (!string.IsNullOrEmpty(quantity15))
                            {
                                oEpsilonDetail.quantity15 = Math.Round(double.Parse(quantity15.ToString()), 2).ToString();
                            }
                            //TODO: Change details 
                            //αυτό το section θα πάει στα totals γτ δεν μπορούμε να κάνουμε αναγωγή στις γραμμές
                            //oEpsilonDetail.withheldAmount = "0";// oRS.Fields.Item("withheldAmount").Value.ToString();
                            //oEpsilonDetail.withheldDescription = "";// oRS.Fields.Item("withheldDescription").Value.ToString();
                            //oEpsilonDetail.withheldPercentage = 0;// oRS.Fields.Item("withheldPercentage").Value.ToString();
                            //oEpsilonDetail.withheldPercentCategory = ""; // oRS.Fields.Item("withheldPercentCategory").Value.ToString();

                            //oEpsilonDetail.stam = "0";// oRS.Fields.Item("withheldAmount").Value.ToString();
                            //oEpsilonDetail.withheldDescription = "";// oRS.Fields.Item("withheldDescription").Value.ToString();
                            //oEpsilonDetail.withheldPercentage = 0;// oRS.Fields.Item("withheldPercentage").Value.ToString();
                            //oEpsilonDetail.withheldPercentCategory = ""; // oRS.Fields.Item("withheldPercentCategory").Value.ToString();

                            oEpsilonDetail.withheldAmount = oRS.Fields.Item("withheldAmount").Value.ToString().Replace(",", ".");// oRS.Fields.Item("withheldAmount").Value.ToString();
                            oEpsilonDetail.withheldDescription = oRS.Fields.Item("withheldDescription").Value.ToString();// oRS.Fields.Item("withheldDescription").Value.ToString();
                            oEpsilonDetail.withheldPercentage = oRS.Fields.Item("withheldPercentage").Value.ToString().Replace(",", ".");// oRS.Fields.Item("withheldPercentage").Value.ToString();
                            oEpsilonDetail.withheldPercentCategory = oRS.Fields.Item("withheldPercentCategory").Value.ToString();// oRS.Fields.Item("withheldPercentCategory").Value.ToString();

                            oEpsilonDetail.stampAmount = oRS.Fields.Item("stampDutyAmount").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.stampDescription = oRS.Fields.Item("stampDutyDescr").Value.ToString();
                            oEpsilonDetail.stampPercentage = oRS.Fields.Item("stampDutyPercentage").Value.ToString().Replace(",", ".");
                            oEpsilonDetail.stampPercentCategory = oRS.Fields.Item("stampDutyPercentCategory").Value.ToString(); // oRS.Fields.Item("withheldPercentCategory").Value.ToString();


                            oDocument.EpsilonDocument.invoiceDetails.Add(oEpsilonDetail);
                            oDocument.EpsilonDocument.taxesTotals = oEpsilonInvoiceTaxesTotals;
                            oDocument.EpsilonDocument.invoiceSummary = oEpsilonInvoiceSummary;

                            this.ListDocuments.Add(oDocument);
                        }
                        oRS.MoveNext();
                    }


                    iRetVal++;
                }
                catch (Exception ex)
                {
                    Logging.WriteToLog("_sSQLBasic=" + _sSQLBasic, Logging.LogStatus.RET_VAL);
                    var a = new Logging("ElectronicInvoicingMethods.LoadClass.LoadDocuments", ex);
                }
                return iRetVal;
            }

            private int GetSummaries(string _sObjectCode, string _sDocNum, out Invoicesummary oSummary)
            {
                oSummary = new Invoicesummary();
                int iRetVal = 0;
                string sSQL = "";
                string sSQLClass = "";
                try
                {
                    Incomeclassification oClassification = null;

                    if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        sSQL = "SELECT " + Environment.NewLine +
                            " \"DocNum\"," + Environment.NewLine +
                            " \"GTOTNetValue\"," + Environment.NewLine +
                            " \"GTOTValue\"," + Environment.NewLine +
                            " \"GTOTVATValue\"" + Environment.NewLine +
                            " FROM ZTKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_DOCUMENT_TOTALS" + Environment.NewLine +
                            " WHERE 1 = 1" + Environment.NewLine +
                            " AND \"ObjType\" = N'" + _sObjectCode + "'" + Environment.NewLine +
                            " AND \"DocNum\" = N'" + _sDocNum + "'";
                    }
                    else
                    {
                        sSQL = "SELECT " + Environment.NewLine +
                            " DocNum," + Environment.NewLine +
                            " GTOTNetValue," + Environment.NewLine +
                            " GTOTValue," + Environment.NewLine +
                            " GTOTVATValue" + Environment.NewLine +
                            " FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_DOCUMENT_TOTALS" + Environment.NewLine +
                            " WHERE 1 = 1" + Environment.NewLine +
                            " AND ObjType = N'" + _sObjectCode + "'" + Environment.NewLine +
                            " AND DocNum = N'" + _sDocNum + "'";
                    }

                    SAPbobsCOM.Recordset oRSTotals = CommonLibrary.Functions.Database.GetRecordSet(sSQL, Connection.oCompany);

                    oSummary.totalNetValue = oRSTotals.Fields.Item("GTOTNetValue").Value.ToString().Replace(",", ".");
                    oSummary.totalValue = oRSTotals.Fields.Item("GTOTValue").Value.ToString().Replace(",", ".");
                    oSummary.totalVatAmount = oRSTotals.Fields.Item("GTOTVATValue").Value.ToString().Replace(",", ".");

                    if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        sSQLClass = "SELECT " + Environment.NewLine +
                            " \"DocNum\"," + Environment.NewLine +
                            " \"ClassificationCategory\"," + Environment.NewLine +
                            " \"IncomeClassification\"," + Environment.NewLine +
                            " \"GTOTNetValue\"," + Environment.NewLine +
                            " \"GTOTValue\"," + Environment.NewLine +
                            " \"GTOTVATValue\"" + Environment.NewLine +
                            " FROM ZTKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_CLASSIFICATION_TOTALS" + Environment.NewLine +
                            " WHERE 1 = 1" + Environment.NewLine +
                            " AND \"ObjType\" = N'" + _sObjectCode + "'" + Environment.NewLine +
                            " AND \"DocNum\" = N'" + _sDocNum + "'";
                    }
                    else
                    {
                        sSQLClass = "SELECT " + Environment.NewLine +
                            " DocNum," + Environment.NewLine +
                            " ClassificationCategory," + Environment.NewLine +
                            " IncomeClassification," + Environment.NewLine +
                            " GTOTNetValue," + Environment.NewLine +
                            " GTOTValue," + Environment.NewLine +
                            " GTOTVATValue" + Environment.NewLine +
                            " FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_CLASSIFICATION_TOTALS" + Environment.NewLine +
                            " WHERE 1 = 1" + Environment.NewLine +
                            " AND ObjType = N'" + _sObjectCode + "'" + Environment.NewLine +
                            " AND DocNum = N'" + _sDocNum + "'";
                    }

                    SAPbobsCOM.Recordset oRSClass = CommonLibrary.Functions.Database.GetRecordSet(sSQLClass, Connection.oCompany);

                    while (oRSClass.EoF == false)
                    {
                        oClassification = new Incomeclassification(); //θα πρέπει να τρέχει ομαδοποίηση βάσει classificationCategory
                        oClassification.amount = oRSClass.Fields.Item("GTOTNetValue").Value.ToString().Replace(",", ".");
                        oClassification.classificationCategory = oRSClass.Fields.Item("IncomeClassification").Value.ToString(); //πίνακας αντιστοίχησης από ααδε
                        oClassification.classificationType = oRSClass.Fields.Item("ClassificationCategory").Value.ToString(); //πίνακας αντιστοίχησης από ααδε

                        oRSClass.MoveNext();
                        oSummary.incomeClassifications.Add(oClassification);
                    }

                    //SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(_sSQL, Connection.oCompany);

                    //while (oRS.EoF == false)
                    //{
                    //    oSummary.totalNetValue = oRS.Fields.Item("").Value.ToString("");
                    //    oSummary.totalValue = oRS.Fields.Item("").Value.ToString("");
                    //    oSummary.totalVatAmount = oRS.Fields.Item("").Value.ToString("");

                    //    oClassification = new Incomeclassification();
                    //    oClassification.amount = oRS.Fields.Item("").Value.ToString();
                    //    oClassification.classificationCategory = oRS.Fields.Item("").Value.ToString();
                    //    oClassification.classificationType = oRS.Fields.Item("").Value.ToString();

                    //    oSummary.incomeClassifications.Add(oClassification);

                    //    oRS.MoveNext();
                    //}

                    iRetVal++;
                }
                catch (Exception ex)
                {
                    Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                    var a = new Logging("ElectronicInvoicingMethods.LoadClass.GetSummaries", ex);
                }
                return iRetVal;
            }

            private int GetTaxes(string _sObjectCode, string _sDocEntry, out List<taxesTotals> oTaxesTotals)
            {
                oTaxesTotals = new List<taxesTotals>();
                taxesTotals oTot = null;

                int iRetVal = 0;
                string sSQL = "";
                string sSQLClass = "";
                try
                {
                    #region old code
                    //taxesTotals oTaxes = null;

                    //if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    //{
                    //    sSQL = "SELECT " + Environment.NewLine +
                    //        " \"DocNum\"," + Environment.NewLine +
                    //        " \"GTOTNetValue\"," + Environment.NewLine +
                    //        " \"GTOTValue\"," + Environment.NewLine +
                    //        " \"GTOTVATValue\"" + Environment.NewLine +
                    //        " FROM ZTKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_DOCUMENT_TOTALS" + Environment.NewLine +
                    //        " WHERE 1 = 1" + Environment.NewLine +
                    //        " AND \"ObjType\" = N'" + _sObjectCode + "'" + Environment.NewLine +
                    //        " AND \"DocNum\" = N'" + _sDocNum + "'";
                    //}
                    //else
                    //{
                    //    sSQL = "SELECT " + Environment.NewLine +
                    //        " DocNum," + Environment.NewLine +
                    //        " GTOTNetValue," + Environment.NewLine +
                    //        " GTOTValue," + Environment.NewLine +
                    //        " GTOTVATValue" + Environment.NewLine +
                    //        " FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_DOCUMENT_TOTALS" + Environment.NewLine +
                    //        " WHERE 1 = 1" + Environment.NewLine +
                    //        " AND ObjType = N'" + _sObjectCode + "'" + Environment.NewLine +
                    //        " AND DocNum = N'" + _sDocNum + "'";
                    //}

                    //SAPbobsCOM.Recordset oRSTotals = CommonLibrary.Functions.Database.GetRecordSet(sSQL, Connection.oCompany);

                    //oTaxes.totalNetValue = oRSTotals.Fields.Item("GTOTNetValue").Value.ToString().Replace(",", ".");
                    //oTaxes.totalValue = oRSTotals.Fields.Item("GTOTValue").Value.ToString().Replace(",", ".");
                    //oTaxes.totalVatAmount = oRSTotals.Fields.Item("GTOTVATValue").Value.ToString().Replace(",", ".");

                    //if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    //{
                    //    sSQLClass = "SELECT " + Environment.NewLine +
                    //        " \"DocNum\"," + Environment.NewLine +
                    //        " \"ClassificationCategory\"," + Environment.NewLine +
                    //        " \"IncomeClassification\"," + Environment.NewLine +
                    //        " \"GTOTNetValue\"," + Environment.NewLine +
                    //        " \"GTOTValue\"," + Environment.NewLine +
                    //        " \"GTOTVATValue\"" + Environment.NewLine +
                    //        " FROM ZTKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_CLASSIFICATION_TOTALS" + Environment.NewLine +
                    //        " WHERE 1 = 1" + Environment.NewLine +
                    //        " AND \"ObjType\" = N'" + _sObjectCode + "'" + Environment.NewLine +
                    //        " AND \"DocNum\" = N'" + _sDocNum + "'";
                    //}
                    //else
                    //{
                    //    sSQLClass = "SELECT " + Environment.NewLine +
                    //        " DocNum," + Environment.NewLine +
                    //        " ClassificationCategory," + Environment.NewLine +
                    //        " IncomeClassification," + Environment.NewLine +
                    //        " GTOTNetValue," + Environment.NewLine +
                    //        " GTOTValue," + Environment.NewLine +
                    //        " GTOTVATValue" + Environment.NewLine +
                    //        " FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_CLASSIFICATION_TOTALS" + Environment.NewLine +
                    //        " WHERE 1 = 1" + Environment.NewLine +
                    //        " AND ObjType = N'" + _sObjectCode + "'" + Environment.NewLine +
                    //        " AND DocNum = N'" + _sDocNum + "'";
                    //}

                    //SAPbobsCOM.Recordset oRSClass = CommonLibrary.Functions.Database.GetRecordSet(sSQLClass, Connection.oCompany);

                    //while (oRSClass.EoF == false)
                    //{
                    //    oTaxes = new Incomeclassification(); //θα πρέπει να τρέχει ομαδοποίηση βάσει classificationCategory
                    //    oClassification.amount = oRSTotals.Fields.Item("GTOTValue").Value.ToString().Replace(",", ".");
                    //    oClassification.classificationCategory = oRSClass.Fields.Item("IncomeClassification").Value.ToString(); //πίνακας αντιστοίχησης από ααδε
                    //    oClassification.classificationType = oRSClass.Fields.Item("ClassificationCategory").Value.ToString(); //πίνακας αντιστοίχησης από ααδε

                    //    oRSClass.MoveNext();
                    //    oSummary.incomeClassifications.Add(oClassification);
                    //}

                    //iRetVal++;
                    #endregion
                    //6-10-2022 Naya -> Add taxes
                    if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        sSQL = "SELECT * FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_TAXES_TOTALS WHERE 1=1 AND \"ObjType\" = '" + _sObjectCode + "' AND \"DocEntry\" = '" + _sDocEntry + "'";
                    }
                    else
                    {
                        sSQL = "SELECT * FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES_TAXES_TOTALS WHERE 1=1 AND ObjType = '" + _sObjectCode + "' AND DocEntry = '" + _sDocEntry + "'";
                    }

                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, Connection.oCompany);

                    while (oRS.EoF == false)
                    {
                        if (oRS.Fields.Item("TAX_CATEGORY").Value.ToString() != "-112")
                        {
                            //oTaxesTotals.taxAmount = oRS.Fields.Item("TAX_AMOUNT").Value.ToString();
                            //oTaxesTotals.taxCategory = oRS.Fields.Item("TAX_CATEGORY").Value.ToString();
                            //oTaxesTotals.taxType = oRS.Fields.Item("TAX_TYPE").Value.ToString();
                            oTot = new taxesTotals();
                            oTot.taxAmount = oRS.Fields.Item("TAX_AMOUNT").Value.ToString();
                            oTot.taxCategory = oRS.Fields.Item("TAX_CATEGORY").Value.ToString();
                            oTot.taxType = oRS.Fields.Item("TAX_TYPE").Value.ToString();

                            oTaxesTotals.Add(oTot);
                        }
                        oRS.MoveNext();
                    }

                    iRetVal++;
                }
                catch (Exception ex)
                {
                    Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                    var a = new Logging("ElectronicInvoicingMethods.LoadClass.GetTaxes", ex);
                }
                return iRetVal;
            }

            private string GetSQL(TransactionTypes _enType)
            {
                string sRetVal = "";
                try
                {
                    switch (_enType)
                    {
                        case TransactionTypes.tt_SalesLoad:
                            //sRetVal = "SELECT top 10 * FROM TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES WHERE 1=1 order by issueDate desc";
                            if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                  sRetVal = "select * from TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES where 1 = 1" + Environment.NewLine +
                                        "and \"mKey\" in (select top 10 distinct \"mKey\" from TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES)";

                            }
                            else
                            {
                                sRetVal = "select* from TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES where 1 = 1" + Environment.NewLine +
                                      "and mKey in (select top 10 distinct mKey from TKA_V_ELECTRONIC_INVOICES_SALES_INVOICES)";
                            }

                            break;
                        case TransactionTypes.tt_JournalEntriesLoad:
                            if (Connection.oCompany.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                sRetVal = "SELECT * FROM ";
                            }
                            else
                            {
                                sRetVal = "SELECT * FROM \"\"";
                            }
                            break;
                        case TransactionTypes.tt_PurchaseLoad:
                            if (Connection.oCompany.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                sRetVal = "SELECT * FROM ";
                            }
                            else
                            {
                                sRetVal = "SELECT * FROM \"\"";
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.LoadClass.GetSQL", ex);
                }
                return sRetVal;
            }

           
            #endregion

            #region Public Methods
            public int Exec(TransactionTypes _enType)
            {
                int iRetVal = 0;
                try
                {
                    string sSQL = "";
                    int iSuccess = 1;
                    int iResult = 0;

                    //Logging.WriteToLog("ElectronicInvoicingMethods.LoadClass.GetSQL", Logging.LogStatus.START);
                    sSQL = this.GetSQL(_enType);
                    //Logging.WriteToLog("ElectronicInvoicingMethods.LoadClass.GetSQL", Logging.LogStatus.END);

                    if (!string.IsNullOrEmpty(sSQL))
                    {
                        //Logging.WriteToLog("ElectronicInvoicingMethods.LoadClass.LoadDocuments", Logging.LogStatus.START);
                        iResult += this.LoadDocuments(sSQL);
                        //Logging.WriteToLog("ElectronicInvoicingMethods.LoadClass.LoadDocuments", Logging.LogStatus.END);
                    }

                    if (iResult == iSuccess)
                    {
                        iRetVal++;
                    }
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.LoadClass.Exec", ex);
                }
                return iRetVal;
            }
            #endregion
        }

        internal class SendClass
        {
            #region Public Properties
            public List<BoDocument> ListDocuments { get; set; }
            #endregion

            #region Private Properties
            private BoTransmission APIConnector { get; set; }
            private string TRANCode { get; set; }
            private string APPICallID { get; set; }
            private string UserSign { get; set; }
            #endregion

            public SendClass()
            {
                this.ListDocuments = new List<BoDocument>();
                this.APIConnector = new BoTransmission("");
            }

            #region Private Methods
            private int PostDocumentProcess()
            {
                int iRetVal = 0;
                try
                {
                    BoEpsilonDocument oEpsilonDocument = null;
                    BoDocument oDocument = null;

                    int iSuccess = this.ListDocuments.Count;
                    int iResult = 0;
                    int iTempResult = 0;
                    int iTempSuccess = 3;

                    for (int i = 0; i < this.ListDocuments.Count; i++)
                    {
                        iTempResult = 0;
                        oDocument = new BoDocument();
                        oDocument = this.ListDocuments[i];

                        int iAlreadyProccessed = BoDAL.ProccessedDocumentExist(oDocument.ObjectCode, oDocument.DocNum,Connection.oCompany);
                        int iLogged = 0;
                        if (iAlreadyProccessed == 0)
                        {
                            iLogged = BoDAL.AddProccessedDocument(Connection.oCompany,oDocument.ObjectCode, oDocument.DocNum, oDocument.DocEntry, "0", "0", "", "");
                        }
                        else
                        {
                            iLogged = 1;
                            string sDocumentID = "";
                            BoDAL.GetDocumentID(oDocument.ObjectCode, oDocument.DocNum, out sDocumentID, Connection.oCompany);

                            oDocument.DocumentID = sDocumentID;
                        }

                        if (iLogged == 0)
                        {
                            Logging.WriteToLog("Cannot Log Document With ObjectCode: " + oDocument.ObjectCode + " And DocNum:" + oDocument.DocNum + "", Logging.LogStatus.START);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(oDocument.DocumentID))
                            {
                                string sDocumentID = "";
                                BoDAL.GetDocumentID(oDocument.ObjectCode, oDocument.DocNum, out sDocumentID, Connection.oCompany);

                                oDocument.DocumentID = sDocumentID;
                            }

                            //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.START);
                            iTempResult += this.LogCall(LogTypes.lg_TranAdd, this.ListDocuments[i]);
                            //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.END);

                            //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.START);
                            iTempResult += this.LogCall(LogTypes.lg_LoadTransactionCode, this.ListDocuments[i]);
                            //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.END);

                            if (iTempResult == 2)
                            {
                                oDocument.TRANCodePK = this.TRANCode;
                            }

                            //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.PostDocument", Logging.LogStatus.START);
                            iTempResult += this.PostDocument(ref oDocument);
                            //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.PostDocument", Logging.LogStatus.END);
                        }
                        if (iTempResult == iTempSuccess)
                        {
                            iResult++;
                        }
                    }

                    if (iResult == iSuccess)
                    {
                        iRetVal++;
                    }
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.SendClass.PostDocumentProcess", ex);
                }
                return iRetVal;
            }

            /// <summary>
            /// Αποστολή Εγγράφου στην Epsilon
            /// </summary>
            /// <param name="_oDocument">Το Bo Παραστατικό</param>
            /// <returns>1 For Success, 0 For Failure</returns>
            private int PostDocument(ref BoDocument _oDocument)
            {
                int iRetVal = 0;
                try
                {
                    BoEpsilonDocumentWrapper oDocumentWrapper = new BoEpsilonDocumentWrapper();
                    BoEpsilonDocumentReply oReply = new BoEpsilonDocumentReply();

                    BoEpsilonDocument oTmpDocument = new BoEpsilonDocument();
                    oTmpDocument = _oDocument.EpsilonDocument;

                    Logging.WriteToLog("ElectronicInvoicingMethods.Prepare", Logging.LogStatus.START);
                    _oDocument.EpsilonDocumentWrapper.Prepare(oTmpDocument, _oDocument.ObjectCode);
                    Logging.WriteToLog("ElectronicInvoicingMethods.Prepare", Logging.LogStatus.END);

                    string sJSON = new JavaScriptSerializer().Serialize(_oDocument.EpsilonDocument);

                    
                    string sPath = "C:\\Program Files\\sap\\HANAServiceLogs\\XML\\XMLIssue\\TEST" + _oDocument.ObjectCode + "_" + _oDocument.DocEntry + ".json";

                    using (StreamWriter sw = File.CreateText(sPath))
                    {
                        sw.WriteLine(sJSON);
                    }

                    _oDocument.JSON2Send = sJSON;
                    oDocumentWrapper = _oDocument.EpsilonDocumentWrapper;

                    Logging.WriteToLog("ElectronicInvoicingMethods.Prepare", Logging.LogStatus.START);
                    this.APIConnector.Issue(oDocumentWrapper, out oReply, this.TRANCode);
                    Logging.WriteToLog("ElectronicInvoicingMethods.Prepare", Logging.LogStatus.END);

                    sJSON = new JavaScriptSerializer().Serialize(oReply);
                    _oDocument.JSONResponse = sJSON;
                    sPath = "C:\\Program Files\\sap\\HANAServiceLogs\\XML\\XMLIssue\\RESPONSE\\" + _oDocument.ObjectCode + "_" + _oDocument.DocEntry + ".json";
                    using (StreamWriter sw = File.CreateText(sPath))
                    {
                        sw.WriteLine(sJSON);
                    }

                    if (string.IsNullOrEmpty(oReply.errorCode)) //Success
                    {
                        BoDAL.UpdateTransactionResult(_oDocument.DocumentID, _oDocument.TRANCodePK, "1", _oDocument.JSONResponse, oReply.documentId, Connection.oCompany);
                        Console.WriteLine("Επιτυχής Αποστολή του Εγγράφου, Τύπου: " + _oDocument.ObjectCode + " με Κωδικό: " + _oDocument.DocNum + "");
                        BoDAL.UpdateEpsilonDocumentCode(_oDocument.DocumentID, oReply.documentId, oReply.qrCode,Connection.oCompany);
                        iRetVal++;
                    }
                    else //Failure
                    {
                        BoDAL.UpdateTransactionResult(_oDocument.DocumentID, _oDocument.TRANCodePK, "0", _oDocument.JSONResponse, oReply.documentId, Connection.oCompany);
                        Console.WriteLine("Σφάλμα Κατά την Αποστολή του Εγγράφου, Τύπου: " + _oDocument.ObjectCode + " με Κωδικό: " + _oDocument.DocNum + "");
                        Logging.WriteToLog("Σφάλμα Κατά την Αποστολή του Εγγράφου, Τύπου: " + _oDocument.ObjectCode + " με Κωδικό: " + _oDocument.DocNum + "", Logging.LogStatus.ERROR);
                    }
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.SendClass.PostDocument", ex);
                }
                return iRetVal;
            }

            /// <summary>
            /// Σύνδεση με το API
            /// </summary>
            /// <returns>1 For Success, 0 For Failure</returns>
            private int Connect2API()
            {
                int iRetVal = 0;
                try
                {
                    this.APIConnector = new BoTransmission(this.APPICallID);
                    iRetVal = this.APIConnector.Login();
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.SendClass.Connect2API", ex);
                }
                return iRetVal;
            }

            /// <summary>
            /// Ενημέρωση SAP Business One Παραστατικού / Ημ. Εγγραφής
            /// </summary>
            /// <param name="_oDocument">Αντικείμενο Bo Παραστατικού</param>
            /// <returns>1 For Success, 0 For Failure</returns>
            private int UpdateSAP(ref BoDocument _oDocument)
            {
                int iRetVal = 0;
                try
                {
                    SAPbobsCOM.Documents oDIDocument = (SAPbobsCOM.Documents)Connection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                    oDIDocument.GetByKey(int.Parse(_oDocument.GetDocEntry()));

                    oDIDocument.EDocNum = "MARK";

                    int iDIResult = oDIDocument.Update();

                    if (iDIResult == 0)
                    {
                        iRetVal++;
                    }
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.SendClass.PostDocument", ex);
                }
                return iRetVal;
            }

            /// <summary>
            /// Καταγραφή στον Πίνακα Κινήσεων
            /// </summary>
            /// <param name="_enType">Τύπος Καταγραφής</param>
            /// <param name="_oDocument">Παραστατικό</param>
            /// <returns>1 For Success, 0 For Failure</returns>
            private int LogCall(LogTypes _enType, BoDocument _oDocument)
            {
                int iRetVal = 0;
                try
                {
                    switch (_enType)
                    {
                        case LogTypes.lg_APICall:
                            BoDAL.LogNewCall(this.UserSign,Connection.oCompany);
                            break;
                        case LogTypes.lg_TranAdd:
                            BoDAL.LogNewTransaction(this.APPICallID, _oDocument, Connection.oCompany);
                            break;
                        case LogTypes.lg_TranUpdate:
                            BoDAL.LogUpdateTransaction( Connection.oCompany);
                            break;
                        case LogTypes.lg_LoadAPICall:
                            string sAPICallID = "";
                            BoDAL.LoadAPICallID(this.UserSign, out sAPICallID, Connection.oCompany);

                            this.APPICallID = sAPICallID;
                            break;
                        case LogTypes.lg_LoadTransactionCode:
                            string sTranCode = "";
                            BoDAL.LoadTransactionCodeID(this.APPICallID, _oDocument.DocNum, Connection.oCompany, out sTranCode);

                            this.TRANCode = sTranCode;
                            break;
                    }
                    iRetVal++;
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.SendClass.LogCall", ex);
                }
                return iRetVal;
            }
            #endregion

            #region Public Methods
            public int Exec(TransactionTypes _enType)
            {
                int iRetVal = 0;
                try
                {
                    CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogs\\ConfParams.ini");

                    int iResult = 0;
                    int iSuccess = 4;
                    this.UserSign = ini.IniReadValue("Default", "B1UserName");

                    //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.START);
                    iResult += this.LogCall(LogTypes.lg_APICall, null);
                    //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.END);

                    //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.START);
                    iResult += this.LogCall(LogTypes.lg_LoadAPICall, null);
                    //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.LogCall", Logging.LogStatus.END);

                    if (iResult == 2)
                    {
                        //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.Connect2API", Logging.LogStatus.START);
                        iResult += this.Connect2API();
                        //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.Connect2API", Logging.LogStatus.END);
                    }

                    if (iResult == 3)
                    {
                        //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.PostDocumentProcess", Logging.LogStatus.START);
                        iResult += this.PostDocumentProcess();
                        //Logging.WriteToLog("ElectronicInvoicingMethods.SendClass.PostDocumentProcess", Logging.LogStatus.END);
                    }

                    if (iSuccess == iResult)
                    {
                        iRetVal++;
                    }
                }
                catch (Exception ex)
                {
                    var a = new Logging("ElectronicInvoicingMethods.SendClass.Exec", ex);
                }
                return iRetVal;
            }
            #endregion
        }
        #endregion
    }
}
