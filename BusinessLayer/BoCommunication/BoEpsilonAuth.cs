using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPEpsilonSend.BusinessLayer
{
    public class BoEpsilonAuth
    {
        #region Public Properties
        public string jwt { get; set; }
        public string jwtExpiration { get; set; }
        public string jwtRefreshToken { get; set; }
        public string jwtRefreshTokenExpiration { get; set; }
        public string itemIdentifier { get; set; }
        public string itemFamilyIdentifier { get; set; }
        public string appIdentifier { get; set; }
        public string url1 { get; set; }
        public string url2 { get; set; }
        #endregion

        public BoEpsilonAuth()
        { }
    }
}
