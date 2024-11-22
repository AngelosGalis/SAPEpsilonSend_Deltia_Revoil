using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPEpsilonSend.BusinessLayer.CommunicationResult
{
    class BoError
    {
        #region Public Properties
        public string ErrorCode { get; set; }
        public string ErrorDescr { get; set; }
        #endregion

        public BoError()
        {
            this.ErrorCode = "";
            this.ErrorDescr = "";
        }
    }
}
