namespace SrtsWeb.BusinessLayer.Presenters
{
    using SrtsWeb.BusinessLayer.Concrete;

    /// <summary>
    /// This is a base class to the presenters that is currently being used to hold global variables common to the app.
    /// Not all presenter pages are using this base page.
    /// </summary>
    public class PresenterBase
    {
        public string GeyesSiteCode { get { return Misc.GEYES_SITE_CODE; } }
    }
}