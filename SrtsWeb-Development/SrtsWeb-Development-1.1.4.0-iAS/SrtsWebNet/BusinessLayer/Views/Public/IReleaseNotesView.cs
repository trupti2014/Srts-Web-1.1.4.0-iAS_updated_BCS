using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Public
{
    public interface IReleaseNotesView
    {
        List<ReleaseNote> ReleaseNoteList { get; set; }
    }
}