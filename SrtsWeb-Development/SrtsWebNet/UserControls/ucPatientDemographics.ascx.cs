using SrtsWeb.ExtendersHelpers;
using SrtsWeb.CustomErrors;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Security.Permissions;
using System.Web.UI;

namespace SrtsWeb.UserControls
{
    [ParseChildren(false)]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucPatientDemographics : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (mySession == null || mySession.Patient == null || mySession.Patient.Individual == null || mySession.Patient.IDNumbers == null) return;

                tbClinicName.Text = GetSiteInfo().SiteCombination;
                litBranch.Text = mySession.Patient.Individual.Demographic.ToBOSValue();
                litRank.Text = mySession.Patient.Individual.Demographic.ToRankValue();
                litStatus.Text = mySession.Patient.Individual.Demographic.ToPatientStatusValue();
                litID.Text = mySession.Patient.IDNumbers[0].IDNumberFilter;
                litPriority.Text = mySession.Patient.Individual.Demographic.ToOrderPriorityValue();
                litDOB.Text = mySession.Patient.Individual.DateOfBirth != null ? mySession.Patient.Individual.DateOfBirth.Value.ToShortDateString() : string.Empty;
                litName.Text = mySession.Patient.Individual.NameLFMi;
                litGender.Text = mySession.Patient.Individual.Demographic.ToGenderValue();
                tbCommentsView.Text = mySession.Patient.Individual.Comments;
                litPatientNameHeader.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
                litEAD.Text = mySession.Patient.Individual.EADStopDate != null &&
                    mySession.Patient.Individual.EADStopDate.Value.Equals(Convert.ToDateTime("01/01/1900")) ?
                    string.Empty : mySession.Patient.Individual.EADStopDate.ToStringMil();
                litTheater.Text = mySession.Patient.Individual.TheaterLocationCode == string.Empty ? "N/A" : mySession.Patient.Individual.TheaterLocationCode;
            }
            catch (NullReferenceException ex)
            {
                ExceptionUtility.LogException(ex, "Error in ucPatientDemographics on Page_Load, null reference exception..");
            }
            finally
            {
            }
        }

        protected SiteCodeEntity GetSiteInfo()
        {
            var siteCodeRepository = new SiteRepository.SiteCodeRepository();
            try
            {
                return siteCodeRepository.GetSiteBySiteID(mySession.Patient.Individual.SiteCodeID)[0];
            }
            catch (NullReferenceException ex)
            {
                ExceptionUtility.LogException(ex, "Error in ucPatientDemographics on Page_Load, null reference exception..");
                return null;
            }
        }

        new protected void btnEditPatient_Click(object sender, EventArgs e)
        {
            mySession.AddOrEdit = "EDIT";
            mySession.TempID = mySession.Patient.Individual.ID;
            Response.Redirect("~/WebForms/SrtsWebClinic/Patients/PatientManagementEdit.aspx");
        }
    }
}