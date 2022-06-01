using SrtsWeb.Views.Reporting;
using System.Data;

namespace SrtsWeb.BusinessLayer.Presenters.Reporting
{
    public sealed class SiteCodeInfoPresenter
    {
        private ISiteCodeView _view;

        public SiteCodeInfoPresenter(ISiteCodeView view)
        {
            _view = view;
        }

        public void GetAllSites()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("_repository.GetAllSites");
            if (dt.Rows.Count > 0)
            {
                dt.TableName = "SiteCodeData";
                ds.Tables.Add(dt);
                _view.SiteCodeData = ds;
            }
        }
    }
}