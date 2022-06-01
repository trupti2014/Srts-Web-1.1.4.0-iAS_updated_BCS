using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface ICmsManagerView
    {
        List<CmsMessage> ContentTitles { get; set; }

        Int32 SelectedContentTitleId { get; set; }

        Int32 CurrentAuthorId { get; }
    }
}