using System;
using System.Collections.Generic;
using System.Data;
using SrtsWeb.Entities;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface ICmsManagerView
    {
        List<CmsMessage> ContentTitles { get; set; }

        Int32 SelectedContentTitleId { get; set; }

        Int32 CurrentAuthorId { get; }
    }
}