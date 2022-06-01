using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [ParseChildren(false)]
    public partial class ucBoxRight : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public String Title { get; set; }

        public String Header { get; set; }

        protected String Text { get; set; }

        public String LiteralText { set { this.Text += value; } }

        public Image HeaderImage
        {
            get { return (imgHeader); }
            set { imgHeader = value; }
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (obj is LiteralControl)
                this.Text += ((LiteralControl)obj).Text;
            else if (obj is PlaceHolder && !String.IsNullOrEmpty(((PlaceHolder)obj).ID)
                && ((PlaceHolder)obj).ID.Equals("phContent"))
                base.AddParsedSubObject(obj);
            else
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter w = new HtmlTextWriter(sw))
                    {
                        ((Control)obj).RenderControl(w);
                        this.Text += sb.ToString();
                    }
                }
            }
        }
    }
}