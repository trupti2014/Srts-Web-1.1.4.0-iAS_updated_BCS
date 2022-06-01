using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.Admin;
using System;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class CmsManagerPresenter
    {
        private ICmsManagerView _view;

        public CmsManagerPresenter(ICmsManagerView view)
        {
            this._view = view;
        }

        public void InitView()
        {
            FillDropDowns();
        }

        private void FillDropDowns()
        {
            var _repository = new CMSRepository.CmsMessageRepository();
            this._view.ContentTitles = _repository.GetCMS_ContentByAuthorId(this._view.CurrentAuthorId);
        }

        public void DeleteContent(Int32 contentId)
        {
            var _repository = new CMSRepository.CmsEntityRepository();
            _repository.DeleteMessage(contentId);
        }
    }
}