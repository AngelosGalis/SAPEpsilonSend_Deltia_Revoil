using CommonLibrary.ExceptionHandling;
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

namespace SAPEpsilonSend.BusinessLayer
{
    public static class BoDAL
    {
        #region Public Methods
        public static int LogNewCall(string _sUserSign, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_CALLS_INSERT\"('" + _sUserSign + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_CALLS_INSERT", oConnection))
                        {
                            oCommand.CommandTimeout = 0;
                            oCommand.Parameters.Add(new SqlParameter("@USERSIGN", "" + _sUserSign + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }

                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LogNewCall", ex);
            }
            return iRetVal;
        }
        public static int LogNewToken(BoEpsilonAuth _oAuth, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_PARAMS_INSERT\"(" +
                        "'" + _oAuth.jwt + "'," +
                        "'" + _oAuth.jwtRefreshToken + "'," +
                        "'" + _oAuth.url1 + "'," +
                        "'" + _oAuth.url2 + "'," +
                        "'" + _oAuth.jwtExpiration + "'," +
                        "'" + _oAuth.jwtRefreshTokenExpiration
                        + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_PARAMS_INSERT", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            SqlParameter oParam = null;

                            oCommand.Parameters.Add(new SqlParameter("@JWT", "" + _oAuth.jwt + ""));
                            oCommand.Parameters.Add(new SqlParameter("@JWT_REFRESH_TOKEN", "" + _oAuth.jwtRefreshToken + ""));
                            oCommand.Parameters.Add(new SqlParameter("@URL1", "" + _oAuth.url1 + ""));
                            oCommand.Parameters.Add(new SqlParameter("@URL2", "" + _oAuth.url2 + ""));


                            oParam = new SqlParameter("@JWT_EXPIRATION", System.Data.SqlDbType.DateTime);
                            //oParam.Value = DateTime.Parse(_oAuth.jwtExpiration + "").ToString("yyyy-MM-dd");
                            oParam.Value = DateTime.Parse(_oAuth.jwtExpiration + "");
                            oCommand.Parameters.Add(oParam);

                            oParam = new SqlParameter("@JWT_REFRESH_TOKEN_EXPIRATION", System.Data.SqlDbType.DateTime);
                            //oParam.Value = DateTime.Parse(_oAuth.jwtRefreshTokenExpiration + "").ToString("yyyy-MM-dd");
                            oParam.Value = DateTime.Parse(_oAuth.jwtRefreshTokenExpiration + "");
                            oCommand.Parameters.Add(oParam);


                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }

                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LogNewCall", ex);
            }
            return iRetVal;
        }
        public static int LogNewTransaction(string _sCALL_ID, BoDocument _oDocument, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                string tranPK = "0";
                if (!string.IsNullOrEmpty(_oDocument.TRANCodePK))
                {
                    tranPK = _oDocument.TRANCodePK;
                }
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_TRANSACTIONS_INSERT\"(" +
                        "'" + _sCALL_ID + "'," +
                        "'" + "13" + "'," +
                        "'" + _oDocument.DocNum + "'," +
                        "'" + _oDocument.JSON2Send + "'," +
                        "'" + _oDocument.JSONResponse + "'," +
                        "'" + tranPK + "'," +
                        "'" + "0" + "'," +
                        "'" + _oDocument.QRString + "'," +
                        "'" + _oDocument.Mark + "'," +
                        "'" + _oDocument.DocumentID
                        + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_TRANSACTIONS_INSERT", oConnection))
                        {
                            oCommand.CommandTimeout = 0;
                            oCommand.Parameters.Add(new SqlParameter("@CALL_ID", "" + _sCALL_ID + ""));
                            oCommand.Parameters.Add(new SqlParameter("@OBJECT_TYPE", "13"));
                            oCommand.Parameters.Add(new SqlParameter("@RECORD_ENTRY", "" + _oDocument.DocNum + ""));
                            oCommand.Parameters.Add(new SqlParameter("@JSON_SEND_URL", "" + _oDocument.JSON2Send + ""));
                            oCommand.Parameters.Add(new SqlParameter("@JSON_REPLY_URL", "" + _oDocument.JSONResponse + ""));
                            oCommand.Parameters.Add(new SqlParameter("@TRAN_RESULT", "" + _oDocument.TRANCodePK + ""));
                            oCommand.Parameters.Add(new SqlParameter("@SAP_UPDATED", "0"));
                            oCommand.Parameters.Add(new SqlParameter("@QR_CODE", "" + _oDocument.QRString + ""));
                            oCommand.Parameters.Add(new SqlParameter("@MARK", "" + _oDocument.Mark + ""));
                            oCommand.Parameters.Add(new SqlParameter("@DOCUMENT_ID", "" + _oDocument.DocumentID + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LogNewTransaction", ex);
            }
            return iRetVal;
        }
        public static int LogUpdateTransaction(SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {

                }
                else
                {

                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LogUpdateTransaction", ex);
            }
            return iRetVal;
        }
        public static int LoadTransactionCodeID(string _sCallID, string _sDocNum, SAPbobsCOM.Company CompanyConnection, out string _sTranCode)
        {
            int iRetVal = 0;
            string sSQL = "";
            _sTranCode = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");
                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "SELECT \"F_ELIV_GET_TRANSACTION_CALL_CODE\"(" +
                        "'" + _sCallID + "'," +
                        "'" + _sDocNum
                        + "') AS RESULT FROM DUMMY;";
                    _sTranCode = CommonLibrary.Functions.Database.ReturnDBValues(sSQL, "RESULT" ,CompanyConnection).ToString();
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        sSQL = "SELECT  Result = dbo.[F_ELIV_GET_TRANSACTION_CALL_CODE](@CALL_ID, @DOCNUM)";
                        DataTable dtRet = new DataTable();
                        using (SqlDataAdapter oSQLAdapter = new SqlDataAdapter(sSQL, oConnection))
                        {
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@CALL_ID", "" + _sCallID + "");
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@DOCNUM", "" + _sDocNum + "");
                            oSQLAdapter.SelectCommand.CommandTimeout = 0;
                            //SQLAdapter.SelectCommand.CommandType = CommandType.;
                            oSQLAdapter.Fill(dtRet);
                        }

                        foreach (DataRow dtRow in dtRet.Rows)
                        {
                            _sTranCode = dtRow["Result"].ToString();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LoadAPICallID", ex);
            }
            return iRetVal;
        }
        public static int LoadAPICallID(string _sUserName, out string _sAPICallID, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            _sAPICallID = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "SELECT \"F_ELIV_GET_TRANS_CODE\"('" + _sUserName + "') AS RESULT FROM DUMMY;";
                    _sAPICallID = CommonLibrary.Functions.Database.ReturnDBValues(sSQL, "RESULT", CompanyConnection).ToString();
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        sSQL = "SELECT  Result = dbo.[F_ELIV_GET_TRANS_CODE](@USERNAME)";
                        DataTable dtRet = new DataTable();
                        using (SqlDataAdapter oSQLAdapter = new SqlDataAdapter(sSQL, oConnection))
                        {
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@USERNAME", "" + _sUserName + "");
                            oSQLAdapter.SelectCommand.CommandTimeout = 0;
                            //SQLAdapter.SelectCommand.CommandType = CommandType.;
                            oSQLAdapter.Fill(dtRet);
                        }

                        foreach (DataRow dtRow in dtRet.Rows)
                        {
                            _sAPICallID = dtRow["Result"].ToString();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LoadAPICallID", ex);
            }
            return iRetVal;
        }
        public static int LogEpsilonDocumentReply(SAPbobsCOM.Company CompanyConnection, string _sAPICallID, string _sauthenticationCode, string _sQRCode, string _sdocumentId, string _sextSystemId, string _suid, string _smark, string _sstatus, string _spdfUploaded, string _spdfFileUrl, string _serrorCode, string _serrorMessage, string _serrorSeverity)
        {
            int iRetVal = 0;
            string sSQL = "";
            _sAPICallID = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_EPSILON_REPLY_INSERT\"(" +
                        "'" + _sAPICallID + "'," +
                        "'" + _sauthenticationCode + "'," +
                        "'" + _sQRCode + "'," +
                        "'" + _sdocumentId + "'," +
                        "'" + _sextSystemId + "'," +
                        "'" + _suid + "'," +
                        "'" + _smark + "'," +
                        "'" + _sstatus + "'," +
                        "'" + _spdfUploaded + "'," +
                        "'" + _spdfFileUrl + "'," +
                        "'" + _serrorCode + "'," +
                        "'" + _serrorMessage + "'," +
                        "'" + _serrorSeverity
                        + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_EPSILON_REPLY_INSERT", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@APICallID", "" + _sAPICallID + ""));
                            oCommand.Parameters.Add(new SqlParameter("@authenticationCode", "" + _sauthenticationCode + ""));
                            oCommand.Parameters.Add(new SqlParameter("@QRCode", "" + _sQRCode + ""));
                            oCommand.Parameters.Add(new SqlParameter("@documentId", "" + _sdocumentId + ""));
                            oCommand.Parameters.Add(new SqlParameter("@extSystemId", "" + _sextSystemId + ""));
                            oCommand.Parameters.Add(new SqlParameter("@uid", "" + _suid + ""));
                            oCommand.Parameters.Add(new SqlParameter("@mark", "" + _smark + ""));
                            oCommand.Parameters.Add(new SqlParameter("@status", "" + _sstatus + ""));
                            oCommand.Parameters.Add(new SqlParameter("@pdfUploaded", "" + _spdfUploaded + ""));
                            oCommand.Parameters.Add(new SqlParameter("@pdfFileUrl", "" + _spdfFileUrl + ""));
                            oCommand.Parameters.Add(new SqlParameter("@errorCode", "" + _serrorCode + ""));
                            oCommand.Parameters.Add(new SqlParameter("@errorMessage", "" + _serrorMessage + ""));
                            oCommand.Parameters.Add(new SqlParameter("@errorSeverity", "" + _serrorSeverity + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LogEpsilonDocumentReply", ex);
            }
            return iRetVal;
        }
        public static int UpdateTransactionSETSendJSON(string _sTranCodePK, string _sJSON, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");
                string json = _sJSON.Substring(0, Math.Min(_sJSON.Length, 3999));
                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "CALL \"ELIV_TRANSACTIONS_UPDATE_SEND_JSON\"(" +
                        "'" + _sTranCodePK + "'," +
                        "'" + json
                        + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_TRANSACTIONS_UPDATE_SEND_JSON", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@AA", "" + _sTranCodePK + ""));
                            oCommand.Parameters.Add(new SqlParameter("@JSON_SEND_URL", "" + _sJSON + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.LogEpsilonDocumentReply", ex);
            }
            return iRetVal;
        }
        public static int AddProccessedDocument(SAPbobsCOM.Company CompanyConnection, string _sOBJECT_TYPE, string _sRECORD_ENTRY, string _sDocEntry, string _sTRAN_RESULT, string _sSAP_UPDATED, string _sQR_CODE, string _sMARK)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_DOCUMENT_INSERT\"(" +
                        "'" + _sOBJECT_TYPE + "'," +
                        "'" + _sRECORD_ENTRY + "'," +
                        "'" + _sDocEntry + "'," +
                        "'" + _sTRAN_RESULT + "'," +
                        "'" + _sSAP_UPDATED + "'," +
                        "'" + _sQR_CODE + "'," +
                        "'" + _sMARK
                        + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_DOCUMENT_INSERT", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@OBJECT_TYPE", "" + _sOBJECT_TYPE + ""));
                            oCommand.Parameters.Add(new SqlParameter("@RECORD_ENTRY", "" + _sRECORD_ENTRY + ""));
                            oCommand.Parameters.Add(new SqlParameter("@DOC_ENTRY", "" + _sDocEntry + ""));
                            oCommand.Parameters.Add(new SqlParameter("@TRAN_RESULT", "" + _sTRAN_RESULT + ""));
                            oCommand.Parameters.Add(new SqlParameter("@SAP_UPDATED", "" + _sSAP_UPDATED + ""));
                            oCommand.Parameters.Add(new SqlParameter("@QR_CODE", "" + _sQR_CODE + ""));
                            oCommand.Parameters.Add(new SqlParameter("@MARK", "" + _sMARK + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.AddProccessedDocument", ex);
            }
            return iRetVal;
        }
        public static int UpdateTransactionResult(string _sDocumentID, string _sTransactionCode, string _sTRAN_RESULT, string _sJSONReply, string _sEpsilonDocumentID, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_UPDATE_TRANSACTION_STATUS\"(" +
                       "'" + _sDocumentID + "'," +
                       "'" + _sTransactionCode + "'," +
                       "'" + _sTRAN_RESULT + "'," +
                       "'" + _sJSONReply + "'," +
                       "'" + _sEpsilonDocumentID
                       + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_UPDATE_TRANSACTION_STATUS", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@DOCUMENT_ID", "" + _sDocumentID + ""));
                            oCommand.Parameters.Add(new SqlParameter("@TRAN_ID", "" + _sTransactionCode + ""));
                            oCommand.Parameters.Add(new SqlParameter("@TRAN_RESULT", "" + _sTRAN_RESULT + ""));
                            oCommand.Parameters.Add(new SqlParameter("@JSON_REPLY", "" + _sJSONReply + ""));
                            oCommand.Parameters.Add(new SqlParameter("@EPSILON_DOCUMENT_ID", "" + _sEpsilonDocumentID + ""));


                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.UpdateTransactionResult", ex);
            }
            return iRetVal;
        }
        public static int UpdateEpsilonDocumentCode(string _sDocumentID, string _sEpsilonDocumentID, string _sQRCode, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_UPDATE_DOCUMENT__EPSILON_DOCUMENT\"(" +
                      "'" + _sDocumentID + "'," +
                      "'" + _sEpsilonDocumentID + "'," +
                      "'" + _sQRCode
                      + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_UPDATE_DOCUMENT__EPSILON_DOCUMENT", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@DOCUMENT_PK", "" + _sDocumentID + ""));
                            oCommand.Parameters.Add(new SqlParameter("@DOCUMENT_ID", "" + _sEpsilonDocumentID + ""));
                            oCommand.Parameters.Add(new SqlParameter("@QRCODE", "" + _sQRCode + ""));


                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.UpdateTransactionResult", ex);
            }
            return iRetVal;
        }
        public static int UpdateDocumentData(string _sEpsilonDocumentID, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "call \"ELIV_UPDATE_DOCUMENT_SEEK_4_MARK\"('" + _sEpsilonDocumentID + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_UPDATE_DOCUMENT_SEEK_4_MARK", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@DOCUMENT_ID", "" + _sEpsilonDocumentID + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.UpdateDocumentData", ex);
            }
            return iRetVal;
        }
        public static int UpdateDocumentSETSAPUpdate(string _sDocumentID, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");
                string DBVersion = ini.IniReadValue("Default", "DatabaseVersion");
                if (DBVersion.Equals("HANA"))
                {
                    sSQL = "call \"ELIV_UPDATE_DOCUMENT__SAP_UPDATED\"('" + _sDocumentID + "')";
                    SAPbobsCOM.Recordset oRS = CommonLibrary.Functions.Database.GetRecordSet(sSQL, CompanyConnection);
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        using (SqlCommand oCommand = new SqlCommand("[dbo].ELIV_UPDATE_DOCUMENT__SAP_UPDATED", oConnection))
                        {
                            oCommand.CommandTimeout = 0;

                            oCommand.Parameters.Add(new SqlParameter("@DOCUMENT_ID", "" + _sDocumentID + ""));

                            oCommand.CommandType = CommandType.StoredProcedure;

                            oCommand.ExecuteScalar();
                        }
                    }
                }
                iRetVal++;
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.UpdateDocumentSETSAPUpdate", ex);
            }
            return iRetVal;
        }

        public static int ProccessedDocumentExist(string _sOBJECT_TYPE, string _sRECORD_ENTRY, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "SELECT \"F_ELIV_GET_DOCUMENT_EXISTANCE\"(" +
                        "'" + _sOBJECT_TYPE + "'," +
                        "'" + _sRECORD_ENTRY
                        + "') AS RESULT FROM DUMMY;";
                    iRetVal = int.Parse(CommonLibrary.Functions.Database.ReturnDBValues(sSQL, "RESULT", CompanyConnection).ToString());
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        sSQL = "SELECT  Result = dbo.[F_ELIV_GET_DOCUMENT_EXISTANCE](@OBJECT_TYPE, @RECORD_ENTRY)";
                        DataTable dtRet = new DataTable();
                        using (SqlDataAdapter oSQLAdapter = new SqlDataAdapter(sSQL, oConnection))
                        {
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@OBJECT_TYPE", "" + _sOBJECT_TYPE + "");
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@RECORD_ENTRY", "" + _sRECORD_ENTRY + "");
                            oSQLAdapter.SelectCommand.CommandTimeout = 0;
                            //SQLAdapter.SelectCommand.CommandType = CommandType.;
                            oSQLAdapter.Fill(dtRet);
                        }

                        foreach (DataRow dtRow in dtRet.Rows)
                        {
                            iRetVal = int.Parse(dtRow["Result"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.ProccessedDocumentExist", ex);
            }
            return iRetVal;
        }
        public static int GetDocumentID(string _sOBJECT_TYPE, string _sRECORD_ENTRY, out string _sDocumentPK, SAPbobsCOM.Company CompanyConnection)
        {
            int iRetVal = 0;
            string sSQL = "";
            _sDocumentPK = "";
            try
            {
                CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

                if (Connection.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    sSQL = "SELECT \"F_ELIV_GET_DOCUMENT_PK\"(" +
                       "'" + _sOBJECT_TYPE + "'," +
                       "'" + _sRECORD_ENTRY
                       + "') AS RESULT FROM DUMMY;";
                    _sDocumentPK = CommonLibrary.Functions.Database.ReturnDBValues(sSQL, "RESULT", CompanyConnection).ToString();
                }
                else
                {
                    using (SqlConnection oConnection = new SqlConnection(ini.IniReadValue("Default", "MSSQLConnectionString")))
                    {
                        oConnection.Open();

                        sSQL = "SELECT  Result = dbo.[F_ELIV_GET_DOCUMENT_PK](@OBJECT_TYPE, @RECORD_ENTRY)";
                        DataTable dtRet = new DataTable();
                        using (SqlDataAdapter oSQLAdapter = new SqlDataAdapter(sSQL, oConnection))
                        {
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@OBJECT_TYPE", "" + _sOBJECT_TYPE + "");
                            oSQLAdapter.SelectCommand.Parameters.AddWithValue("@RECORD_ENTRY", "" + _sRECORD_ENTRY + "");
                            oSQLAdapter.SelectCommand.CommandTimeout = 0;
                            //SQLAdapter.SelectCommand.CommandType = CommandType.;
                            oSQLAdapter.Fill(dtRet);
                        }

                        foreach (DataRow dtRow in dtRet.Rows)
                        {
                            _sDocumentPK = dtRow["Result"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteToLog("sSQL=" + sSQL, Logging.LogStatus.RET_VAL);
                var a = new Logging("BoDAL.GetDocumentID", ex);
            }
            return iRetVal;
        }
        #endregion  
    }
}
