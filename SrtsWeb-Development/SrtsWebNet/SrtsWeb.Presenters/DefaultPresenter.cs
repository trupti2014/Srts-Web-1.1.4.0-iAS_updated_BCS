using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views;
using System;
using System.Data;
using System.Diagnostics;

namespace SrtsWeb.Presenters
{
    public sealed class DefaultPresenter
    {
        private IDefaultView _view;

        public DefaultPresenter(IDefaultView view)
        {
            _view = view;
        }

        public void InitView(string userName)
        {
            InitMySession(userName);
        }

        public void InitMySession(string userName)
        {
            var _repository = new SiteRepository.SiteCodeRepository();
            var _individualRepository = new IndividualRepository();

            _view.mySession.MyIndividualID = _individualRepository.GetIndividualIdByUserName(userName);

            if (!String.IsNullOrEmpty(_view.mySession.MyClinicCode))
                _view.mySession.MySite = _repository.GetSiteBySiteID(_view.mySession.MyClinicCode)[0];
 
            _view.mySession.MyClinicCode = _view.mySession.MySite.SiteCode;
            _view.mySession.AddOrEdit = string.Empty;
            _view.mySession.Patient = new PatientEntity();
            _view.mySession.ReturnURL = string.Empty;
            _view.mySession.SelectedExam = new ExamEntity();
            _view.mySession.SelectedOrder = new OrderEntity();
            _view.mySession.SelectedPatientID = 0;
            _view.mySession.TempID = 0;
            _view.mySession.tempOrderID = string.Empty;
            _view.mySession.CurrentModule = "My SRTSweb Dashboard";
            _view.mySession.CurrentModule_Sub = string.Empty;
        }

        public void GetSummary()
        {
            if (_view.mySession != null)
            {
                var _orderRepository = new OrderRepository();
                DataSet ds = new DataSet();
                try
                {
                    switch (_view.mySession.MySite.SiteType)
                    {
                        case "CLINIC":
                            ds = _orderRepository.GetClinicSummary(_view.mySession.MySite.SiteCode);
                            _view.Pending = 0;
                            _view.ReadyForCheckin = 0;
                            _view.ReadyForDispense = 0;
                            _view.AvgDispenseDays = 0;
                            _view.OverDue = 0;
                            _view.Rejected = 0;

                            if (ds.Tables[0].Rows.Count > 0)
                                _view.ReadyForCheckin = ds.Tables[0].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[1].Rows.Count > 0)
                                _view.Rejected = ds.Tables[1].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[2].Rows.Count > 0)
                                _view.OverDue = ds.Tables[2].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[3].Rows.Count > 0)
                                _view.AvgDispenseDays = ds.Tables[3].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[4].Rows.Count > 0)
                                _view.ReadyForDispense = ds.Tables[4].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[5].Rows.Count > 0)
                                _view.Pending = ds.Tables[5].Rows[0][1].ToNullableInt32();
                            break;

                        case "LAB":
                            ds = _orderRepository.GetLabSummary(_view.mySession.MySite.SiteCode);
                            _view.ReadyForLabCheckin = 0;
                            _view.ReadyForLabDispense = 0;
                            _view.AvgProductionDays = 0;
                            _view.HoldForStockOrders = 0;

                            if (ds.Tables[0].Rows.Count > 0)
                                _view.ReadyForLabCheckin = ds.Tables[0].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[1].Rows.Count > 0)
                                _view.AvgProductionDays = ds.Tables[1].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[2].Rows.Count > 0)
                                _view.ReadyForLabDispense = ds.Tables[2].Rows[0][1].ToNullableInt32();

                            if (ds.Tables[3].Rows.Count > 0)
                                _view.HoldForStockOrders = ds.Tables[3].Rows[0][1].ToNullableInt32();

                            break;
                    }
                }
                catch { }
            }
        }

        public void GetAnnouncements()
        {
            SrtsWeb.BusinessLayer.Abstract.IMessageService m = new SrtsWeb.BusinessLayer.Concrete.MessageService();

            var siteTypeId = this._view.mySession.MySite.SiteType.ToLower().Equals("lab") ? "R003" : "R002";

            var l = m.GetApplicationAnnouncements(
                this._view.mySession.MySite.SiteCode,
                siteTypeId,
                this._view.mySession.MyIndividualID);

            l.AddRange(m.GetMessageCenterMessages(
                this._view.mySession.MySite.SiteCode,
                siteTypeId,
                this._view.mySession.MyIndividualID));

            l.AddRange(m.GetPublicAnnouncements());
            var sb = new System.Text.StringBuilder();

            sb.Append("<ul>");
            foreach (var s in l)
                sb.AppendFormat("<li class='announcement'><a href=\"/WebForms/SrtsMessageCenter/MessageCenter.aspx?tId={0}\">{1}</a></li>",
                    s.cmsContentId,
                    s.cmsContentTitle);
            sb.Append("</ul>");

            this._view.AnnouncementLinks = sb.ToString();
        }

        public String GetGuideFileName(string role)
        {
            string fn = "";

            switch (role)
            {
                case "CT":
                    fn = "ClinicTechGuide.docx";
                    break;

                case "CA":
                    fn = "ClinicAdminGuide.docx";
                    break;

                case "LT":
                    fn = "LabTechGuide.docx";
                    break;

                case "LA":
                    fn = "LabAdminGuide.docx";
                    break;

                default:
                    fn = "none";
                    break;
            }
            return fn;
        }
    }
}