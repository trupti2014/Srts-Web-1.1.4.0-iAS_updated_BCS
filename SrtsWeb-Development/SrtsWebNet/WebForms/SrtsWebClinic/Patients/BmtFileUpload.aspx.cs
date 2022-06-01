using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using SrtsWeb.DataLayer.Repositories;
using System.Data;
using SrtsWebClinic.Patients;
using System.Text;
using System.Web.Security;
using SrtsWeb.Account;

namespace SrtsWebClinic.Patients
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    public partial class BmtFileUpload : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                FormsAuthentication.RedirectToLoginPage();

            var t = new PostBackTrigger();
            t.ControlID = this.btnProcess.UniqueID;
            upFileLoad.Triggers.Add(t);
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            int lineCount = 0;
            StringBuilder errorMessage = new StringBuilder();
            string saveDir = string.Format("{0}{1}", Request.PhysicalApplicationPath, @"App_Data\FileUploads\");

            //if (fUpload.HasFile)
            if (!String.IsNullOrEmpty(this.hfFile.Value))
            {
                //string fileName = fUpload.FileName;
                var fileName = this.hfFile.Value.Substring(this.hfFile.Value.LastIndexOf(@"\") + 1);
                string saveInfo = string.Format("{0}{1}", saveDir, fileName);
                try
                {
                    fUpload.SaveAs(saveInfo);
                    //lblInfo.Text = fUpload.FileName;
                    lblInfo.Text = fileName;
                }
                catch (Exception ex)
                {
                    lblInfo.Text = ex.Message.ToString();
                    return;
                }
                using (StreamReader sr = new StreamReader(saveInfo))
                {
                    string tmpSTr = string.Empty;
                    while ((tmpSTr = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(tmpSTr))
                        {
                            errorMessage.Append(ProcessLine(tmpSTr.Split(',')));
                            lineCount++;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(errorMessage.ToString()))
                {
                    CustomValidator cv = new CustomValidator();
                    cv.IsValid = false;
                    cv.ErrorMessage = errorMessage.ToString();
                    this.Page.Validators.Add(cv);
                    return;
                }
                else
                {
                    lblInfo.Text = string.Format("Processed {0} records", lineCount.ToString());
                }

                //string myFile = fUpload.FileName;

                var myFile = string.Format("{0}.{1}{2}{3}{4}_proc", fileName, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Minute);
                fUpload.SaveAs(string.Format("{0}{1}", saveDir, myFile));
            }
            else
            {
                lblInfo.Text = "Your file was not uploaded, please try again.";
            }
        }

        private string ProcessLine(string[] tmpArray)
        {
            // CSV Format follows:  *****************
            // Firstname,
            // Middlename,
            // Lastname,
            // Date of Birth (MMDDYYYY),
            // Gender(M for male, F for female),
            // Branch of Service(A for Army,F for Air Force,M for Marines,N for Navy,C for Coast Guard),
            // ID Number, 
            // ID Number Type(SSN for social, DIN for DoD ID), 
            // Trainee's Unit, 
            // Base/Post/City, 
            // State (2 letter State code),
            // Country (2 letter Country-Code),
            // Zipcode (5 digit zipcode expressed as 5 digits or 9 digit zipcode expressed as 10 characters #####-####)            
            // UIC

            string eMessage = string.Empty;
            DateTime tmpDOB;
            string bos = string.Empty;
            string gender = string.Empty;

            PatientEntity pe = new PatientEntity();
            pe.Individual = new IndividualEntity();
            pe.IDNumbers = new List<IdentificationNumbersEntity>();
            IdentificationNumbersEntity ine = new IdentificationNumbersEntity();
            pe.Addresses = new List<AddressEntity>();
            AddressEntity ae = new AddressEntity();

            pe.Individual.FirstName = tmpArray[0];
            pe.Individual.MiddleName = tmpArray[1];
            pe.Individual.LastName = tmpArray[2];
            DateTime.TryParse(tmpArray[3], out tmpDOB);
            pe.Individual.DateOfBirth = tmpDOB;
            gender = tmpArray[4];
            bos = tmpArray[5];
            ine.IDNumber = tmpArray[6].Replace("-", "");
            ine.IDNumberType = tmpArray[7];
            ine.IsActive = true;
            ine.ModifiedBy = mySession.MyUserID;
            pe.IDNumbers.Add(ine);
            ae.Address1 = tmpArray[8];
            ae.Address2 = string.Empty;
            ae.Address3 = string.Empty;
            ae.City = tmpArray[9];
            ae.State = tmpArray[10];
            ae.Country = tmpArray[11];
            ae.ZipCode = tmpArray[12];
            ae.UIC = tmpArray[13];
            ae.AddressType = "UNIT";
            ae.IsActive = true;
            ae.ModifiedBy = mySession.MyUserID;
            pe.Addresses.Add(ae);
            pe.Individual.SiteCodeID = mySession.MySite.SiteCode;
            pe.Individual.PersonalType = "PATIENT";
            pe.Individual.EADStopDate = null;
            pe.Individual.IsPOC = false;
            pe.Individual.Comments = string.Empty;
            pe.Individual.TheaterLocationCode = "000000000";
            pe.Individual.IsActive = true;
            pe.Individual.ModifiedBy = mySession.MyUserID;

            string demoGraphic = string.Format("{0}{1}{2}{3}{4}", "E01", bos, "11", gender, "T");
            pe.Individual.Demographic = demoGraphic;

            IIndividualRepository _repository = new IndividualRepository();
            DataSet tmpData = new DataSet();
            tmpData = _repository.InsertIndividual(pe);
            if (string.IsNullOrEmpty(tmpData.Tables[0].Rows[0]["ID"].ToString()))
            {
                eMessage = string.Format("Data Not Inserted for {0}<br />", pe.Individual.LastName);
            }

            return eMessage;
        }

    }
}