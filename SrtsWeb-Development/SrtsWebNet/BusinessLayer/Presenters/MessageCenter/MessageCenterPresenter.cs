using SrtsWeb.Views.MessageCenter;
using SrtsWeb.DataLayer.Repositories;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.MessageCenter
{
    public sealed class MessageCenterPresenter
    {
        private IMEssageCenterView _view;

        public MessageCenterPresenter(IMEssageCenterView view)
        { this._view = view; }

        public void InitView()
        {
            GetMessages();
        }

        public void GetMessages()
        {
            if (this._view.mySession.MySite == null) return;

            var _repository = new CMSRepository.CmsEntityRepository();

            var l = _repository.GetCms_MessageCenter_ApplicationAnnouncement_Content(
                "C002",
                this._view.mySession.MySite.SiteCode,
                this._view.mySession.MySite.SiteType.ToLower().Equals("clinic") ? "R002" : "R003",
                _view.mySession.MyIndividualID);

            l.AddRange(_repository.GetCms_MessageCenter_ApplicationAnnouncement_Content(
                "C001",
                this._view.mySession.MySite.SiteCode,
                this._view.mySession.MySite.SiteType.ToLower().Equals("clinic") ? "R002" : "R003",
                _view.mySession.MyIndividualID));

            l.AddRange(_repository.GetMessageByContentTypeID("C000"));

            this._view.Messages = l.OrderByDescending(x => x.cmsCreatedDate).ToList();
        }
    }
}