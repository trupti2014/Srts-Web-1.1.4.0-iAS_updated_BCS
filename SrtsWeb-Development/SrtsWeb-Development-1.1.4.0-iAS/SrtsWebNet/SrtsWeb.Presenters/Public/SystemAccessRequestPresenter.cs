using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.Public;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace SrtsWeb.Presenters.Public
{
    public class SystemAccessRequestPresenter
    {
        private ISystemAccessRequest v;

        public SystemAccessRequestPresenter(ISystemAccessRequest view)
        { this.v = view; }

        public void InitView()
        {
            var r = new SiteRepository.SiteCodeRepository();
            this.v.SiteList = r.GetAllSites();
        }

        public bool SendEmail(IMailService mailService)
        {
            try
            {
                var sb = new StringBuilder();
                var fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                var toEmail = new List<String> { ConfigurationManager.AppSettings["GscEmail"], ConfigurationManager.AppSettings["SrtsTeamEmail"] };
                var subject = "SRTSWeb System Access Request";
                sb.AppendLine("When you submit a SRTSWeb system access request, please include the following:");
                sb.AppendLine();
                sb.AppendLine("Name:");
                sb.AppendLine(String.IsNullOrEmpty(this.v.RequesterTitle) ? this.v.RequesterName : String.Format("{0} {1}", this.v.RequesterTitle, this.v.RequesterName));
                sb.AppendLine("Phone Number (including area code):");
                sb.AppendLine(this.v.PhoneNumber);
                sb.AppendLine("E-mail Address:");
                sb.AppendLine(this.v.Email);
                sb.AppendLine("Site:");
                sb.AppendLine(this.v.SiteCode);
                sb.AppendLine("Detailed description of the request:  SRTSWeb System Access Request");
                sb.AppendLine();
                sb.AppendLine("Note: Do not include Personally Identifiable Information (PII).");
                sb.AppendLine();
                sb.AppendLine("Global Service Center: Please enter ticket and assign to DHA|DHA-CSPMO|CSPMO SRTS");

                mailService.SendEmail(sb.ToString(), fromEmail, toEmail, subject);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Mail Service Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }
    }
}