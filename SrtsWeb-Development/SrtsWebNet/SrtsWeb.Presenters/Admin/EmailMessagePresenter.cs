using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public class EmailMessagePresenter : IDisposable
    {

        private IEmailMessageView view;
        private EmailMEssageRepository repository;
        private Boolean disposed = false;
        public EmailMessagePresenter(IEmailMessageView v)
        {
            view = v;
            //repository = r;
        }

        public EmailMessagePresenter()
        {

            repository = new EmailMEssageRepository();
        }

        public EmailMessageEntity GetEmailMessageByType(string type)
        {
            EmailMessageEntity eme = new EmailMessageEntity();
            try
            {
                eme = repository.GetEmailMessageByType(type);
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
            }
            return eme;
        }

        public List<EmailMessageEntity> GetAllEmailMessageTypes()
        {
            List<EmailMessageEntity> eme = new List<EmailMessageEntity>();
            try
            {
                eme = repository.GetAllEmailMesageTypes();
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
            }
            return eme;
        }



        public List<string> GetAdminEmailAddresses(string sitecode)
        {
            List<EmailMessageEntity> entities = new List<EmailMessageEntity>();
            List<string> emailaddresses = new List<string>();
            entities = repository.GetAdminEmailAddressesforClinic(sitecode);
            emailaddresses = (from a in entities
                               select a.EmailAddress + "," + a.EmailRecipient).ToList();
            return emailaddresses;
        }

        public List<string> GetUserEmailAddresses(string sitecode)
        {
            List<EmailMessageEntity> entities = new List<EmailMessageEntity>();
            List<string> emailaddresses = new List<string>();
            entities = repository.GetUserEmailAddressesforClinic(sitecode);
            emailaddresses = (from a in entities
                              select a.EmailAddress + "," + a.EmailRecipient).ToList();
            return emailaddresses;
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if (view != null)
                    view = null;
            }
            disposed = true;
        }
    }
}