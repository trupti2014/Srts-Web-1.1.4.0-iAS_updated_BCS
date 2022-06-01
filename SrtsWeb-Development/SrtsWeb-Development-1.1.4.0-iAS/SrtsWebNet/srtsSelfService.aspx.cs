using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Administration;
using Microsoft.Web.Administration;
using System.Collections.Specialized;
using System.Web.Configuration;
using SrtsWeb.BusinessLayer.Views;
using SrtsWeb.BusinessLayer.Presenters;
using SrtsWeb.Entities;
using System.Web.Security;
using System.Security.Permissions;
using System.Data;

public partial class srtsSelfService : System.Web.UI.Page, IDefaultView
{
    private DefaultPresenter _presenter;

    public srtsSelfService()
    {
        _presenter = new DefaultPresenter(this);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!HttpContext.Current.User.Identity.IsAuthenticated)
        {
            FormsAuthentication.RedirectToLoginPage();
        }
        else
        {
           
            if (mySession == null)
            {
                mySession = new SRTSSession();
            }


            SrtsWeb.Account.CustomProfile profile = SrtsWeb.Account.CustomProfile.GetProfile();

            if (string.IsNullOrEmpty(profile.Personal.SiteCode))
            {
                //Server.Transfer("~/CustomErrors/tempNoAccess.aspx");
            }
            else
            {
                mySession.MyClinicCode = profile.Personal.SiteCode;
            }
            ////put system requirements check here
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                _presenter.InitView(User.Identity.Name);
            }


            //    if (Request.IsSecureConnection)
            //    {

            //        cert = Request.ClientCertificate;
            //        if (cert.IsPresent)
            //            //check for valid cert also
            //        {
            //            if (cert.IsValid)
            //            {
            //                AuthenticateUser(sender, e);
            //                return;
            //            }
            //        }
            //        else
            //        {
            //        //certData.Text = "No client certificate";
            //            //check for username and password
            //        Page.Validate("LoginUserValidationGroup");
            //            if (!Page.IsValid)
            //            {
            //                message.Text = "Please review the error messages.";
            //                return;
            //            }
            //        }

            //    }
        }
    }

    public SRTSSession mySession
    {
        get { return (SRTSSession)Session["SRTSSession"]; }
        set { Session.Add("SRTSSession", value); }
    }

    public DataTable LookupCache
    {
        get { return (DataTable)Cache["SRTSLookUps"]; }
        set { Cache["SRTSLookUps"] = value; }
    }

    protected void SRTSLogin(object sender, EventArgs e)
    {
        //for local testing with iis express
        Response.Redirect("https://localhost:44300/Default.aspx?ReturnUrl=%2f");

        // switch to secure connection
        //ClientScript.RegisterStartupScript(this.GetType(), "SecureConnection", "redirectHttpToHttps()", true);

    }

    public String AnnouncementLinks
    {
        set { }
    }


    //protected void AuthenticateUser(object sender, EventArgs e)
    //{
    //    string username;
    //    username = Request.LogonUserIdentity.Name;
    //    message.Text = "Welcome " + username;
    //    //certData.Text = "Client certificate retrieved";
    //    GetCertificateData(cert);
    //    isSRTSMember();
    //    //do membership role authorization
    //    //create sessionSrtsUser


    //    //pnlLogin.Visible = false;
    //    //pnlSecurityMessage.Visible = false;
    //    }



  

    //private void GetCertificateData(HttpClientCertificate cs) 
    // {

    //       string[] subjectArray = cs.Subject.Split(',');



    //        //Holds the entire contents of the subject line. 
    //        string entireSubjectLine = cs.Subject.ToString();


    //        //gets the total length of the subject line 
    //        int subjectLineLength = entireSubjectLine.Length;


    //        //-10 grabs the start of the 10 digit CAC identifer code for the user. 
    //        int startOfCacIdentifierPosition = subjectLineLength - 10;


    //        string cacIdentifier = entireSubjectLine.Substring(startOfCacIdentifierPosition, 10);


    //        string[] arr = subjectArray[5].Split(' ');
    //        string[] user = arr[1].Split('=');
    //        StringBuilder sb = new StringBuilder();
    //        foreach (string field in user)
    //        {
    //            sb.Append(field);
    //        }
    //        sb.Remove(0, 2);


    //        string str1 = sb.ToString();
    //        string[] sArr1 = str1.Split('.');
    //        string lastName = sArr1[0].ToString();
    //        string firstName = sArr1[1].ToString();
    //        string MI = sArr1[2].ToString();
    //        string Id = cacIdentifier; //10 Digit Unique CAC identifier

    //    //certData.Text += "<br />" + firstName + " " + lastName + "-" + Id;
    //    //certData.Text += "<br />" + entireSubjectLine;
    //        //RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
    //         }



    // private Boolean isSRTSMember()
    // {
    //     //check membership database
    //     Boolean isMember=false;
    //     return isMember;
    // }

    public int? ReadyForDispense
    { get; set; }

    public string AddressDisplay
    {
        get;// { return litAddrDisplay.text; }
        set;// { litAddrDisplay.Text = value; }
    }
    public int? ReadyForCheckin
    {
        get;// { return Convert.ToInt32(litRetrieval.Text); }
        set;// { litRetrieval.Text = value.ToString(); }
    }
    public int? Rejected
    {
        get;// { return Convert.ToInt32(litRejected.Text); }
        set;// { litRejected.Text = value.ToString(); }
    }
    public int? OverDue
    {
        get;// { return Convert.ToInt32(litOverdue.Text); }
        set;// { litOverdue.Text = value.ToString(); }
    }
    public int? AvgDispenseDays
    {
        get;// { return Convert.ToInt32(litAvgDispenseTime.Text); }
        set;// { litAvgDispenseTime.Text = value.ToString(); }
    }
    public string SiteCode { get; set; }
    public string SitePhone { get; set; }

    #region IDefaultView Members


    public int? ReadyForLabCheckin
    {
        get;
        set;
    }

    public int? ReadyForLabDispense
    {
        get;
        set;
    }

    public int? RejectedByLab
    {
        get;
        set;
    }

    public int? LabCancelled
    {
        get;
        set;
    }

    public int? AvgProductionDays
    {
        get;
        set;
    }

    #endregion


}


