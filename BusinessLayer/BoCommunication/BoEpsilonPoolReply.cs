using System;
public class BoEpsilonPoolReply
{
    public object authenticationCode { get; set; }
    public string qrCode { get; set; }
    public string documentId { get; set; }
    public string extSystemId { get; set; }
    public object uid { get; set; }
    public int mark { get; set; }
    public DateTime timeStamp { get; set; }
    public int status { get; set; }
    public bool pdfUploaded { get; set; }
    public object pdfFileUrl { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public int errorSeverity { get; set; }
}
