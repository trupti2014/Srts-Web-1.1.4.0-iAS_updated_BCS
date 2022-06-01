using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.JSpecs;
using SrtsWeb.Views.JSpecs;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JSpecs.Forms
{
    public partial class JSpecsFAQ : PageBaseJSpecs, IJSpecsFAQView
    {
        private JSpecsFAQPresenter _presenter;

        public JSpecsFAQ()
        {
            _presenter = new JSpecsFAQPresenter(this);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["userInfo"] != null)
            {
                //JSpecsSession userInfo = (JSpecsSession)Session["userInfo"];
                //if (userInfo.Patient.Orders == null)
                //{
                //   // this.Master.YourOrdersLink.Attributes.Add("style", "display:none");
                //}
            }
            else
            {
                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
            }

        }
    }
}