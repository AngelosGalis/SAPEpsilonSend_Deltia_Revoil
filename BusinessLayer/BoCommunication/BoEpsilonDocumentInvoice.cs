using System.Collections.Generic;

public class BoEpsilonDocumentInvoice
{
    #region Public Properties
    public BoEpsilonDocument invoice { get; set; }
    #endregion

    public BoEpsilonDocumentInvoice()
    {
        this.invoice = new BoEpsilonDocument();
    }
}

