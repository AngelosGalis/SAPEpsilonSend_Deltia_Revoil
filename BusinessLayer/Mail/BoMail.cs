using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.ExceptionHandling;

namespace SAPEpsilonSend.BusinessLayer
{
    class BoMail
    {
        #region Public Properties
        public string SenderDisplayName { get; set; }
        public string Body { get; set; }
        public string Recepient { get; set; }
        public string Subject { get; set; }
        #endregion  

        public BoMail()
        { }

        public void SendMail()
        {
            try
            {
                #region Digital4u
                var fromAddress = new MailAddress("noreply@innovplanet.eu", this.SenderDisplayName);
                var toAddress = new MailAddress(this.Recepient, this.Recepient);
                string fromPassword = "=?UeB(dZ8~vj";

                var smtp = new SmtpClient
                {
                    Host = "mail.innovplanet.eu",
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = this.Subject,
                    IsBodyHtml = true,
                    Body = this.Body
                })
                {
                    smtp.Send(message);
                }
                #endregion

            }
            catch (Exception ex)
            {
                var a = new Logging("BoMail.SendMail", ex);
            }
        }
    }
}
