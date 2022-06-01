using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.Account;
using System;

namespace SrtsWeb.Presenters.Account
{
    public class RulesOfBehaviorPresenter
    {
        private IRulesOfBehaviorView _view;

        public RulesOfBehaviorPresenter()
        {
        }

        public RulesOfBehaviorPresenter(IRulesOfBehaviorView _view)
        {
            this._view = _view;
        }

        public void InsertUpdateRulesOfBehaviorDate(string strUserName, DateTime useracceptancedate)
        {
            var repository = new RulesOfBehaviorRepository();
            repository.InsertUpdateRulesOfBehaviorDate(strUserName, useracceptancedate);
        }

        public bool IsRulesOfBehaviorAcceptDateExpired(string strUserName)
        {
            var r = new RulesOfBehaviorRepository();
            var acceptancedate = r.GetRulesOfBehaviorDate(strUserName);
            if (String.IsNullOrEmpty(acceptancedate) || (DateTime.Now - Convert.ToDateTime(acceptancedate)).TotalDays > 365)
                return true;
            else
                return false;
            
        }

        public void GetAllRulesOfBehavior()
        {
            var r = new RulesOfBehaviorRepository();
            var n = r.GetRulesOfBehavior();

            if (n == null) return;
                        
            this._view.RulesOfBehaviorList = n;
        }


    }
}