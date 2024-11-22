using SAPEpsilonSend.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPEpsilonSend.BusinessLayer.CommunicationResult
{
    public class BoResult
    {
        #region Public Properties
        public string ErrorCode { get; set; }
        public string ErrorDescr { get; set; }
        public ProcessResult Result{ get; set; }
        #endregion

        public BoResult()
        {
            this.ErrorCode = "";
            this.ErrorDescr = "";
            this.Result = ProcessResult.NONE;
        }
    }
}
