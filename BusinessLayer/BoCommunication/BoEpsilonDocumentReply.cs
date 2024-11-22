using System;

public class BoEpsilonDocumentReply
{
    #region Public Properties
    public string authenticationCode { get; set; }
    public string qrCode { get; set; }
    public string documentId { get; set; }
    public string extSystemId { get; set; }
    public string uid { get; set; }
    public string mark { get; set; }
    public string timeStamp { get; set; }
    public string status { get; set; }
    public string pdfUploaded { get; set; }
    public string pdfFileUrl { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public string errorSeverity { get; set; }
    #endregion

    public BoEpsilonDocumentReply()
    { }
}
