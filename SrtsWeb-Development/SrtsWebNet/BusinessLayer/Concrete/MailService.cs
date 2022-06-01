using SrtsWeb.BusinessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class to perform email operations.
    /// </summary>
    public class MailService : IMailService
    {
        /// <summary>
        /// Default Ctor
        /// </summary>
        public MailService()
        {
        }

        /// <summary>
        /// Sends an email based on specified arguments.
        /// </summary>
        /// <param name="body">Email message contents.</param>
        /// <param name="fromEmail">Senders emaill address.</param>
        /// <param name="toEmailAddress">Email recipient list.</param>
        /// <param name="subject">Subject line of email.</param>
        public void SendEmail(String body, String fromEmail, List<String> toEmailAddress, String subject)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail);
                mail.Subject = subject;
                mail.Body = body;

                toEmailAddress.ForEach(x => mail.To.Add(x));

                using (var _server = new SmtpClient())
                {
                    _server.Send(mail);
                }
            }
        }
    }
}