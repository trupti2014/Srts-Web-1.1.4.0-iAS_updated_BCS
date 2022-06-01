using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class CMSEntity
    {
        public int cmsContentID { get; set; }

        public string cmsContentTitle { get; set; }

        public string cmsContentBody { get; set; }

        public string cmsContentTypeID { get; set; }

        public int cmsContentAuthorID { get; set; }

        public string cmsContentRecipientTypeID { get; set; }

        public int cmsContentRecipientIndividualID { get; set; }

        public string cmsContentRecipientSiteID { get; set; }

        public string cmsContentRecipientGroupID { get; set; }

        public DateTime cmsContentDisplayDate { get; set; }

        public DateTime cmsContentExpireDate { get; set; }

        public DateTime cmsCreatedDate { get; set; }

        public string cmsContentTypeName { get; set; }

        public string cmsContentDescription { get; set; }

        public string cmsRecipientGroupname { get; set; }

        public string cmsRecipientGroupDescription { get; set; }

        public string cmsRecipientTypeName { get; set; }

        public string cmsRecipientTypeDescription { get; set; }

        public string AuthorLastName { get; set; }

        public string AuthorFirstName { get; set; }

        public string AuthorMiddleName { get; set; }

        public string RecipientLastName { get; set; }

        public string RecipientFristName { get; set; }

        public string RecipientMiddleName { get; set; }
    }

    [Serializable]
    public sealed class CmsMessage
    {
        public Int32 cmsContentId { get; set; }

        public String cmsContentTitle { get; set; }

        public String cmsContentBody { get; set; }
    }

    public sealed class CmsSite
    {
        public String SiteName { get; set; }

        public String SiteCode { get; set; }

        public Boolean IsActive { get; set; }

        public String SiteType { get; set; }
    }
}