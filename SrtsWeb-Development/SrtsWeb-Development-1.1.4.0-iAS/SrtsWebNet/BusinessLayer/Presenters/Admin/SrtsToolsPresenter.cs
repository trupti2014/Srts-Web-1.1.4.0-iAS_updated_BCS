using SrtsWeb.Views.Admin;

namespace SrtsWeb.BusinessLayer.Presenters.Admin
{
    public sealed class SrtsToolsPresenter
    {
        private ISrtsToolsView _view;

        public SrtsToolsPresenter(ISrtsToolsView view)
        {
            _view = view;
        }

        public void InitView()
        {
        }

        public void InitViewSOT()
        {
        }

        public void InitViewGZT()
        {
        }
    }
}