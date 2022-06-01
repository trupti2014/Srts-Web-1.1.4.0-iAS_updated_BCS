using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IReleaseNotesRepository
    {
        List<ReleaseNote> GetReleaseNotes();
    }
}