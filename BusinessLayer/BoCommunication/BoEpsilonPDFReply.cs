using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPEpsilonSend.BusinessLayer
{
    public class BoEpsilonPDFReply
    {
        #region Public Properties
        public string uploadUrl { get; set; }
        public string status { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        #endregion

        public BoEpsilonPDFReply()
        { }
    }
}
