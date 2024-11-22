using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPEpsilonSend.Enumerations
{
    public enum TransactionTypes
    {
        tt_SalesLoad,
        tt_SalesSend,
        tt_SalesUpdate,
        tt_PurchaseLoad,
        tt_PurchaseSend,
        tt_PurchaseUpdate,
        tt_JournalEntriesLoad,
        tt_JournalEntriesSend,
        tt_JournalEntriesUpdate
    }

    public enum LogTypes
    {
        lg_APICall,
        lg_LoadAPICall,
        lg_TranAdd,
        lg_TranUpdate,
        lg_LoadTransactionCode
    }

    public enum EligibleSAPObjects
    {
        JournalEntry,
        SalesDocuments,
        PurchaseDocuments
    }

    public enum XMLFile
    {
        xmlIssue,
        xmlLogin,
        xmlRefresh,
        xmlGetReply
    }

    public enum HTTPReq { hPost, hPut, hNone }
    public enum ProcessResult { prSUCCESS, prFAILURE, NONE }
}
