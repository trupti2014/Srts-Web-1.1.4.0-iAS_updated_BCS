using SrtsWeb.Views.Orders;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.BusinessLayer.Presenters.Orders
{
    public sealed class OrderProblemPresenter
    {
        private IOrderProblemView _view;
        //private IOrdersOfStatusRepository _repository;
        //private IOrderRepository _orderRepository;
        //private IIndividualRepository _individualRepository;

        public OrderProblemPresenter(IOrderProblemView view)
        {
            _view = view;
        }

        public void InitView()
        {
        }

        public void GetOverdueOrders()
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            _view.OverdueData = _repository.GetOrdersForOverDueDispenseByClinicCode(_view.mySession.MySite.SiteCode, _view.mySession.MyUserID.ToString());
        }

        public void GetProblemOrders()
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            _view.ProblemData = _repository.GetOrdersWithProblemsByClinicCode(_view.mySession.MyClinicCode, _view.mySession.MyUserID.ToString());
        }

        //public void GetAllPatientInfo()
        //{
        //    _orderRepository = new OrderRepository();
        //    _view.mySession.SelectedOrder = SrtsHelper.ProcessOrderRow(_orderRepository.GetOrderByOrderNumber(_view.mySession.SelectedOrder.OrderNumber, _view.mySession.MyUserID.ToString()).Rows[0]);
        //    PatientEntity pe = new PatientEntity();
        //    DataSet ds = new DataSet();
        //    _individualRepository = new IndividualRepository();
        //    ds = _individualRepository.GetAllPatientInfoByIndividualID(_view.mySession.SelectedOrder.IndividualID_Patient, true, _view.mySession.MyUserID.ToString(), _view.mySession.MySite.SiteCode);
        //    if (ds.Tables[0].Rows.Count >= 1)
        //    {
        //        pe.Individual = SrtsHelper.ProcessIndividualRow(ds.Tables[0].Rows[0]);
        //        pe.IDNumbers = SrtsHelper.ProcessIdentificationNumberTable(ds.Tables[1]);
        //        pe.EMailAddresses = SrtsHelper.ProcessEMailAddressTable(ds.Tables[2]);
        //        pe.PhoneNumbers = SrtsHelper.ProcessPhoneTable(ds.Tables[3]);
        //        pe.Addresses = SrtsHelper.ProcessAddressTable(ds.Tables[4]);
        //        pe.Orders = SrtsHelper.ProcessOrderTable(ds.Tables[5]);
        //        pe.Exams = SrtsHelper.ProcessExamTable(ds.Tables[6]);
        //        pe.Prescriptions = SrtsHelper.ProcessPresciptionTable(ds.Tables[7]);
        //    }
        //    _view.mySession.Patient = pe;
        //    _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;
        //}
    }
}