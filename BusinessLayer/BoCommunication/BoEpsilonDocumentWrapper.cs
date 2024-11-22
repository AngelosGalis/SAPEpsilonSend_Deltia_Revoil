using System;
using System.Web.Script.Serialization;
using CommonLibrary.ExceptionHandling;

public class BoEpsilonDocumentWrapper
{
    #region Public Properties
    public string externalSystemId { get; set; }
    public string source { get; set; }
    public string templateIdentifier { get; set; }
    #endregion
    public BoEpsilonDocumentWrapper()
    { }

    #region Public Methods
    public int Prepare(BoEpsilonDocument _oDocument, string _sObjectCode)
    {
        int iRetVal = 0;
        try
        {
            CommonLibrary.Ini.IniFile ini = new CommonLibrary.Ini.IniFile("C:\\Program Files\\sap\\HANAServiceLogsDA\\ConfParams.ini");

            //this.externalSystemId = _oDocument.invoiceHeader.aa;
            this.externalSystemId = _sObjectCode + "_" + _oDocument.invoiceHeader.aa;
            this.templateIdentifier = ini.IniReadValue("Default", "Template");

            BoEpsilonDocumentInvoice oInvoice = new BoEpsilonDocumentInvoice();
            oInvoice.invoice = _oDocument;

            //object q = _oDocument;
            object q = oInvoice;
            this.source = new JavaScriptSerializer().Serialize(q);

            iRetVal++;
        }
        catch (Exception ex)
        {
            var a = new Logging("BoEpsilonDocumentWrapper.Prepare", ex);
        }
        return iRetVal;
    }
    #endregion
}

