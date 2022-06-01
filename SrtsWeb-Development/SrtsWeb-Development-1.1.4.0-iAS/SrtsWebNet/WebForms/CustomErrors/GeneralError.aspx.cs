using SrtsWeb.Base;

namespace SrtsWeb.CustomErrors
{
    public partial class GeneralError : PageBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            CurrentModule("SRTS Web - An Error Has Occured!");
        }
    }
}