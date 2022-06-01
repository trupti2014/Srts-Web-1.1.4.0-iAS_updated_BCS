using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SrtsWeb.JSpecs
{
    public partial class JSpecsMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            closeMenu.ServerClick += new EventHandler(CloseNavigation);
            SetActiveMenuItem();
        }

        /// <summary>
        /// Sets the active menu item based on the active url.
        /// </summary>
        public void SetActiveMenuItem()
        {
            string activePage = Request.RawUrl;

            if(activePage.Contains("JSpecsOrders.aspx"))
            {
                srtsLink.Attributes.Add("style", "display:none");
                ordersLink.Attributes.Add("class", "navigation__menu__item active-link");
            }
            else if(activePage.Contains("JSpecsFAQ.aspx")) {
                srtsLink.Attributes.Add("style", "display:none");
                faqsLink.Attributes.Add("class", "navigation__menu__item active-link");
            }
            else if(activePage.Contains("JSpecsLogin.aspx"))
            {
                ordersLink.Attributes.Add("style", "visibility: hidden");
                faqsLink.Attributes.Add("style", "visibility: hidden");
                logout_button.Attributes.Add("style", "visibility: hidden;");
                newOrderLink.Attributes.Add("style", "visibility: hidden");
            }
            else if (activePage.Contains("JSpecsHomePage.aspx"))
            {
                ordersLink.Attributes.Add("style", "visibility: hidden");
                faqsLink.Attributes.Add("style", "visibility: hidden");
                logout_button.Attributes.Add("style", "visibility: hidden;");
                newOrderLink.Attributes.Add("style", "visibility: hidden");
            }
            else if (activePage.Contains("JSpecsDetails.aspx"))
            {
                srtsLink.Attributes.Add("style", "display:none");
            }
            else if (activePage.Contains("JSpecsNewOrder.aspx"))
            {
                newOrderLink.Attributes.Add("class", "navigation__menu__item active-link");
                srtsLink.Attributes.Add("style", "display:none");
            }
        }

        public void CloseNavigation(object sender, EventArgs e)
        {
            checkBox.Checked = false;
        }

        public HtmlAnchor YourOrdersLink
        {
            get
            {
                return ordersLink;
            }
            set
            {
                ordersLink = value;
            }
        }
    }
}