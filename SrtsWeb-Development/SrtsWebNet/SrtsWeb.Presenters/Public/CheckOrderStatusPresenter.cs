using SrtsWeb.BusinessLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Views.Public;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.Presenters.Public
{
    public class CheckOrderStatusPresenter
    {
        private ICheckOrderStatusView v;
        public CheckOrderStatusPresenter(ICheckOrderStatusView view)
        {
            this.v = view;
        }

        public Boolean GetIndividualInfo()
        {
            var idT = this.v.IdNumber.Length.Equals(9) ? "SSN" : "DIN";

            var inR = new IdentificationNumbersRepository();
            var lpID = inR.GetIdentificationNumberByIDNumber(this.v.IdNumber, idT, "CheckOrderStatus");

            if (lpID.IsNullOrEmpty()) return false;

            var pId = lpID.FirstOrDefault(x => x.IDNumber == this.v.IdNumber).IndividualID;

            var r = new CheckOrderStatusRepository();
            // if user is in srts web via the entered id number then use it.
            var o = r.GetPatientOrdersAndStatuses(pId);

            if (o.IsNullOrEmpty())
            {
                //************************************ DMDC *************************************************

                //Get other ID Type
                var oIdType = idT.Equals("SSN") ? "DIN" : "SSN";

                // if i is null then go to DMDC
                var ds = new DmdcService();
                var de = idT.Equals("SSN") ?
                    (this.v.IdNumber.StartsWith("9") ? ds.DoDmdcByFsId(this.v.IdNumber) : ds.DoDmdcBySsn(this.v.IdNumber)) :
                    ds.DoDmdcByDodId(this.v.IdNumber);


                try
                {
                    // attempt to get the patient data by the other id number (if available) different from the original
                    var idN = idT.Equals("SSN") ?
                        de.ToList()[0]._DmdcIdentifier.FirstOrDefault(x => x.PnIdType == "D").PnId :
                        de.ToList()[0]._DmdcIdentifier.FirstOrDefault(x => x.PnIdType == "S").PnId;

                    // Get the person id
                    lpID = inR.GetIdentificationNumberByIDNumber(idN, oIdType, "CheckOrderStatus");

                    if (lpID.IsNullOrEmpty()) return false;

                    pId = lpID.FirstOrDefault(x => x.IDNumber == idN).IndividualID;

                    o = r.GetPatientOrdersAndStatuses(pId);

                    if (o.IsNullOrEmpty()) return false;

                }
                catch
                {
                    return false;
                }
            }

            this.v.PatientStatuses = o;

            // get person information
            var indRepository = new IndividualRepository();
            var ind = indRepository.GetIndividualByIDNumberIDNumberType(this.v.IdNumber, idT, "CheckOrderStatus");
            this.v.PatientInfo = ind;
            return true;
        }

        //public void GetStatus(String OrderNumber)
        //{
        //    // Get the order status of the most recent order
        //    var osR = new OrderStateRepository.OrderStatusRepository();
        //    var os = osR.GetOrderStateByOrderNumber(OrderNumber);

        //    if (os.IsNullOrEmpty()) return;

        //    this.v.OrderStatus = os.Where(x => x.IsActive == true).ToList();
        //}
    }
}
