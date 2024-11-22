using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPEpsilonSend.BusinessLayer
{
    public class BoEpsilonPDFUpload
    {
        #region Public Properties
        public string externalSystemId { get; set; }
        public string documentId { get; set; }
        public string fileName { get; set; }
        public string fileSize { get; set; }
        #endregion

        public BoEpsilonPDFUpload()
        { }
    }
}
