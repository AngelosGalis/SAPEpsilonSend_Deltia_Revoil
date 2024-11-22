using SAPEpsilonSend.Enumerations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.ExceptionHandling;

namespace SAPEpsilonSend.BusinessLayer
{
    public class BoDocument
    {
        #region Public Properties
        public string DocNum { get; set; }
        public string TRANCodePK { get; set; }
        public string ObjectCode { get; set; }
        public string DocEntry { get; set; }
        public EligibleSAPObjects DocumentType { get; set; }
        public BoEpsilonDocument EpsilonDocument { get; set; }
        public BoEpsilonDocumentWrapper EpsilonDocumentWrapper { get; set; }
        public string JSON2Send { get; set; }
        public string JSONResponse { get; set; }
        public string QRString { get; set; }
        public string Mark { get; set; }
        public string DocumentID { get; set; }
        public string EpsilonDocumentID { get; set; }
        public BoEpsilonDocumentReply Reply { get; set; }
        public CommunicationResult.BoResult Result { get; set; }

        #endregion

        public BoDocument()
        {
            this.EpsilonDocument = new BoEpsilonDocument();
            this.EpsilonDocumentWrapper = new BoEpsilonDocumentWrapper();
            this.Reply = new BoEpsilonDocumentReply();
            this.Result = new CommunicationResult.BoResult();
        }

        #region Public Methods
        public string GetDocEntry()
        {
            string sRetVal = "";
            string sSQL = "";
            try
            {
                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    if (this.ObjectCode == "13")
                    {
                        sSQL = "SELECT \"DocEntry\" FROM \"OINV\" WHERE 1=1 AND \"DocNum\" = '" + this.DocNum + "'";
                    }

                    if (this.ObjectCode == "14")
                    {
                        sSQL = "SELECT \"DocEntry\" FROM \"ORIN\" WHERE 1=1 AND \"DocNum\" = '" + this.DocNum + "'";
                    }
                }
                else
                {
                    if (this.ObjectCode == "13")
                    {
                        sSQL = "SELECT DocEntry FROM OINV WHERE 1=1 AND DocNum='" + this.DocNum + "'";
                    }

                    if (this.ObjectCode == "14")
                    {
                        sSQL = "SELECT DocEntry FROM ORIN WHERE 1=1 AND DocNum='" + this.DocNum + "'";
                    }
                }

                sRetVal = CommonLibrary.Functions.Database.ReturnDBValues(sSQL, "DocEntry", Connection.oCompany).ToString();
            }
            catch (Exception ex)
            {
                var a = new Logging("sSQL=" + sSQL, ex);
            }
            return sRetVal;
        }
        #endregion  

        #region Private Properties
        #endregion
    }
}
