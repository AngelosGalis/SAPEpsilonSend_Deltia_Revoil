using System.Collections.Generic;

public class BoEpsilonDocument
{
    #region Public Properties
    public Invoiceheader invoiceHeader { get; set; }
    public List<Paymentmethod> paymentMethods { get; set; }
    public Counterpart counterpart { get; set; }
    public Invoicesummary invoiceSummary { get; set; }
    public Issuer issuer { get; set; }
    public List<Invoicedetail> invoiceDetails { get; set; }
    public string delayedProcessCode { get; set; }
    public List<BoEpsilonMessages> Messages { get; set; }
    public List<taxesTotals> taxesTotals { get; set; }
    #endregion

    public BoEpsilonDocument()
    {
        this.invoiceHeader = new Invoiceheader();
        this.paymentMethods = new List<Paymentmethod>();
        this.counterpart = new Counterpart();
        this.issuer = new Issuer();
        this.invoiceDetails = new List<Invoicedetail>();
        this.invoiceSummary = new Invoicesummary();
        this.Messages = new List<BoEpsilonMessages>();
        this.taxesTotals = new List<taxesTotals>();
    }
}

public class Invoiceheader
{
    #region Public Properties
    public string aa { get; set; }
    public string invoiceTypeDescription { get; set; }
    public string series { get; set; }
    public Formvalues formValues { get; set; }
    public string currency { get; set; }
    public string invoiceType { get; set; }
    public object[] correlatedInvoices { get; set; }
    public string movePurpose { get; set; }
    public string issueDate { get; set; }
    public string dispatchDate { get; set; }
    public string fuelInvoice { get; set; }
    public string exchangeRate { get; set; }
    #endregion
    public Invoiceheader()
    {
        this.formValues = new Formvalues();
    }
}

public class Formvalues
{
    #region Public Properties
    public string deliveryPhone { get; set; }
    public string deliveryStreetNr { get; set; }
    public string deliveryStreet { get; set; }
    public string docNewYpol { get; set; }
    public string deliveryZip { get; set; }
    public string deliveryTitle { get; set; }
    public string docPrevYpol { get; set; }
    public string deliveryPliromi { get; set; }
    public string deliveryCity { get; set; }
    public string deliveryProfession { get; set; }
    public string deliveryFax { get; set; }
    public string deliveryMeso { get; set; }
    public string docRelated { get; set; }
    public string deliveryAfm { get; set; }
    public string deliveryTropos { get; set; }
    public string deliveryApostoli { get; set; }
    public string deliveryDoy { get; set; }
    public string deliveryParadosi { get; set; }
    public string docTime { get; set; }
    public string docComments { get; set; }
    public string deliverySkopos { get; set; }
    #endregion

    public Formvalues()
    { }
}

public class Counterpart
{
    #region Public Properties
    public string city { get; set; }
    public string fax { get; set; }
    public string code { get; set; }
    public string street { get; set; }
    public string name { get; set; }
    public string country { get; set; }
    public string vatNumber { get; set; }
    public string profession { get; set; }
    public string phone { get; set; }
    public string doy { get; set; }
    public string streetNumber { get; set; }
    public string postalCode { get; set; }
    public string email { get; set; }
    #endregion

    public Counterpart()
    { }
}

public class Invoicesummary
{
    #region Public Properties
    public string totalVatAmount { get; set; }
    public string totalNetValue { get; set; }
    public List<Incomeclassification> incomeClassifications { get; set; }
    public string totalValue { get; set; }
    #endregion

    public Invoicesummary()
    {
        this.incomeClassifications = new List<Incomeclassification>();
    }
}

public class Incomeclassification
{
    #region Public Properties
    public string classificationType { get; set; }
    public string amount { get; set; }
    public string classificationCategory { get; set; }
    #endregion

    public Incomeclassification()
    { }
}

public class Issuer
{
    #region Public Properties
    public string city { get; set; }
    public string country { get; set; }
    public string vatNumber { get; set; }
    public string street { get; set; }
    public string streetNumber { get; set; }
    public string postalCode { get; set; }
    #endregion

    public Issuer()
    { }
}

public class Paymentmethod
{
    #region Public Properties
    public string amount { get; set; }
    public string type { get; set; }
    #endregion

    public Paymentmethod()
    { }
}

public class taxesTotals
{
    #region Public Properties
    public string taxType { get; set; }
    public string taxCategory { get; set; }
    public string taxAmount { get; set; }
    #endregion

    public taxesTotals()
    { }
}

public class Invoicedetail
{
    #region Public Properties
    public string fuelCode { get; set; }
    public string quantity15 { get; set; }
    public string vatExemption { get; set; }
    public string classificationCategory { get; set; }
    public string withheldPercentage { get; set; }
    public string entityName { get; set; }
    public string withheldPercentCategory { get; set; }
    public string vatPercent { get; set; }
    public string withheldAmount { get; set; }
    public string netValue { get; set; }
    public string measurementUnit { get; set; }
    public string vatCategory { get; set; }
    public string lineType { get; set; }
    public string lineNumber { get; set; }
    public string vatAmount { get; set; }
    public string withheldDescription { get; set; }
    public string totalValue { get; set; }
    public string incomeClassification { get; set; }
    public string quantity { get; set; }

    public string stampPercentage { get;set;}
    public string stampPercentCategory { get;set;}
    public string stampAmount {get;set;}
    public string stampDescription { get;set;}
    #endregion

    public Invoicedetail()
    { }
}
