using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class that implements from RepositoryBase to perform data operations for address data.
    /// </summary>

    //public sealed class EmailMEssageRepository : RepositoryBase<EmailMessageEntity>, IEmailMessageRepository
    public sealed class EmailMEssageRepository : RepositoryBase<EmailMessageEntity>
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public EmailMEssageRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }


        /// <summary>
        /// Gets an email message type by emailtype.
        /// </summary>
        /// <param name="emailmessagetype">Message type to search for.</param>
        /// <returns>Email Message by type.</returns>
        public EmailMessageEntity GetEmailMessageByType(string emailtype)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetResource_EmailMessage";
            cmd.Parameters.Add(this.DAL.GetParamenter("@EmailType", emailtype));
            var d = GetRecord(cmd);
            return d;
        }


        /// <summary>
        /// Gets a list of all email message types
        /// </summary>
        /// <returns>List of email messages.</returns>
        public List<EmailMessageEntity> GetAllEmailMesageTypes()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetResource_EmailMessage";
            cmd.Parameters.Add(this.DAL.GetParamenter("@EmailType", null));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a admin users' email addreses for a clinic 
        /// </summary>
        /// <param SiteCode="clinicsitecode">Clinic to search with.</param>
        /// <returns>List of admin users' email addresses for a clinic.</returns>
        public List<EmailMessageEntity> GetAdminEmailAddressesforClinic(string clinicsitecode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetClinicAdminEmailAddresses";
            cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", clinicsitecode));
            cmd.Parameters.Add(DAL.GetParamenter("@Success", string.Empty, ParameterDirection.Output));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets all users' email addreses for a clinic 
        /// </summary>
        /// <param SiteCode="clinicsitecode">Clinic to search with.</param>
        /// <returns>List of all users' email addresses for a clinic.</returns>
        public List<EmailMessageEntity> GetUserEmailAddressesforClinic(string clinicsitecode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetClinicUserEmailAddresses";
            cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", clinicsitecode));
            cmd.Parameters.Add(DAL.GetParamenter("@Success", string.Empty, ParameterDirection.Output));
            var p = cmd.Parameters["@Success"] as IDataParameter;
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Inserts an email message into email messages resoures table
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public EmailMessageEntity InsertEmailMessage(EmailMessageEntity emailmessage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Inserts an email message into email messages resources table
        /// </summary>
        /// <returns></returns>
        public EmailMessageEntity UpdateEMailMessage(EmailMessageEntity emailmessage)
        {
            throw new System.NotImplementedException();
        }

        protected override EmailMessageEntity FillRecord(System.Data.IDataReader dr)
        {
            var emailmessage = new EmailMessageEntity();
            emailmessage.EmailType = Extenders.HasColumn(dr, "EmailType") ? dr["EmailType"].ToString() : null;
            emailmessage.MessagePart1 = Extenders.HasColumn(dr, "MsgPart1") ? dr["MsgPart1"].ToString() : null;
            emailmessage.MessagePart2 = Extenders.HasColumn(dr, "MsgPart2") ? dr["MsgPart2"].ToString() : null;
            emailmessage.MessagePart3 = Extenders.HasColumn(dr, "MsgPart3") ? dr["MsgPart3"].ToString() : null;
            emailmessage.MessagePart4 = Extenders.HasColumn(dr, "MsgPart4") ? dr["MsgPart4"].ToString() : null;
            emailmessage.EmailAddress = Extenders.HasColumn(dr, "Email") ? dr["Email"].ToString() : null;
            emailmessage.EmailRecipient = Extenders.HasColumn(dr, "LowerUsername") ? dr["LowerUsername"].ToString() : null;
            return emailmessage;
        }
    }
}