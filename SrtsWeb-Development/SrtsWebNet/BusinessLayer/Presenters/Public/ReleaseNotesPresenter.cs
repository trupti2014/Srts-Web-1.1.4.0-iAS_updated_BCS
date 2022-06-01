using SrtsWeb.Views.Public;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.BusinessLayer.Presenters.Public
{
    public class ReleaseNotesPresenter
    {
        private IReleaseNotesView _view;

        public ReleaseNotesPresenter(IReleaseNotesView _view)
        {
            this._view = _view;
        }

        public void GetAllReleaseNotes()
        {
            var r = new ReleaseNotesRepository();
            var n = r.GetReleaseNotes();

            // reverse the order so the newest release is at the begining.

            if (n == null) return;

            n.Reverse();
            this._view.ReleaseNoteList = n;
        }
    }
}