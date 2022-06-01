using SrtsWeb.Account;
using SrtsWebTrainingAdmin.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace SrtsWebTrainingAdmin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ClassMaintenance : Page, IClassMaintenanceView
    {
        private ClassMaintenancePresenter _presenter;
        private Dictionary<String, Int32> _nameId;

        public ClassMaintenance()
        {
            _presenter = new ClassMaintenancePresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
            if (!IsPostBack)
            {
            }
            lblFeedback.Text = string.Empty;
        }

        public string ClassID
        {
            get { return tbClassName.Text; }
            set { tbClassName.Text = value; }
        }

        //public string DeleteClassID
        //{
        //    get { return tbDeleteClassID.Text; }
        //    set { tbDeleteClassID.Text = value; }
        //}

        //protected void btnDeleteClass_Click(object sender, EventArgs e)
        //{
        //    lblFeedback.Text = string.Empty;
        //    if (!string.IsNullOrEmpty(tbDeleteClassID.Text))
        //    {
        //        _presenter.DeleteClass();
        //        DeleteUsers();
        //        lblFeedback.Text = string.Format("Deleted students and sites for class {0}", DeleteClassID);
        //    }
        //    else
        //    {
        //        lblFeedback.Text = "Please enter a class id of 5 characters or less to delete";
        //    }
        //}

        //public void DeleteUsers()
        //{
        //    string _userName = string.Empty;
        //    _userName = string.Format("{0}INSTCA", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    _userName = string.Format("{0}INSTCT", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    _userName = string.Format("{0}INSTCC", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    _userName = string.Format("{0}INSTLA", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    _userName = string.Format("{0}INSTLT", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    _userName = string.Format("{0}INSTLC", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    _userName = string.Format("{0}INSTLM", DeleteClassID);
        //    Membership.DeleteUser(_userName, true);
        //    for (int x = 1; x <= 35; x++)
        //    {
        //        _userName = string.Format("{0}{1}CA", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //        _userName = string.Format("{0}{1}CT", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //        _userName = string.Format("{0}{1}CC", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //        _userName = string.Format("{0}{1}LA", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //        _userName = string.Format("{0}{1}LT", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //        _userName = string.Format("{0}{1}LC", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //        _userName = string.Format("{0}{1}LM", DeleteClassID, x.ToString().PadLeft(2, '0'));
        //        Membership.DeleteUser(_userName, true);
        //    }
        //}

        protected void btnCreateClass_Click(object sender, EventArgs e)
        {
            lblFeedback.Text = string.Empty;
            if (!string.IsNullOrEmpty(tbClassName.Text) && tbClassName.Text.Trim().Length <= 5)
            {
                if (!_presenter.IsClassNameAvailable(tbClassName.Text))
                {
                    lblFeedback.Text = string.Format("The class name {0} is already in use, please use another name.", tbClassName.Text);
                    return;
                }

                _presenter.AddSites();
                this._nameId = _presenter.CreateStudents();
                CreateStudents();
                lblFeedback.Text = string.Format("Complete creation of SiteCodes and Students for class {0}", ClassID);
            }
            else
            {
                lblFeedback.Text = "Please enter a class id of 5 characters or less to start";
            }
        }

        public void CreateStudents()
        {
            string _password = "1234!@#$abcdABCD";
            string _userName = string.Empty;
            string _question = "What is one plus one?  Spell it out.";
            string _answer = "two";
            MembershipCreateStatus stat;

            _userName = string.Format("{0}INSTCA", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTCA@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}C", ClassID));
            AddRole(_userName, "ClinicAdmin");

            _userName = string.Format("{0}INSTCT", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTCT@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}C", ClassID));
            AddRole(_userName, "ClinicTech");

            _userName = string.Format("{0}INSTCC", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTCC@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}C", ClassID));
            AddRole(_userName, "ClinicClerk");

            _userName = string.Format("{0}INSTLA", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTLA@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}L", ClassID));
            AddRole(_userName, "LabAdmin");

            _userName = string.Format("{0}INSTLT", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTLT@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}L", ClassID));
            AddRole(_userName, "LabTech");

            _userName = string.Format("{0}INSTLC", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTLC@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}L", ClassID));
            AddRole(_userName, "LabClerk");

            _userName = string.Format("{0}INSTLM", ClassID);
            Membership.CreateUser(
                _userName,
                _password,
                string.Format("{0}INSTLM@mail.mil", _userName),
                _question,
                _answer,
                true, out stat);
            CreateProfile(_userName, string.Format("{0}L", ClassID));
            AddRole(_userName, "LabMail");

            for (int x = 1; x <= 35; x++)
            {
                _userName = string.Format("{0}{1}CA", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}1@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "C"));
                AddRole(_userName, "ClinicAdmin");

                _userName = string.Format("{0}{1}CT", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}1@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "C"));
                AddRole(_userName, "ClinicTech");

                _userName = string.Format("{0}{1}CC", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}2@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "C"));
                AddRole(_userName, "ClinicClerk");

                _userName = string.Format("{0}{1}LA", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}3@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "L"));
                AddRole(_userName, "LabAdmin");

                _userName = string.Format("{0}{1}LT", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}3@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "L"));
                AddRole(_userName, "LabTech");

                _userName = string.Format("{0}{1}LC", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}4@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "L"));
                AddRole(_userName, "LabClerk");

                _userName = string.Format("{0}{1}LM", ClassID, x.ToString().PadLeft(2, '0'));
                Membership.CreateUser(
                    _userName,
                    _password,
                    string.Format("{0}5@mail.mil", _userName),
                    _question,
                    _answer,
                true, out stat);
                CreateProfile(_userName, string.Format("{0}{1}", ClassID, "L"));
                AddRole(_userName, "LabMail");
            }
        }

        private void CreateProfile(string _name, string _siteCode)
        {
            // Get the individual id from the individual table based on user name
            SrtsWeb.DataLayer.Repositories.IIndividualRepository ir = new SrtsWeb.DataLayer.Repositories.IndividualRepository();
            var ie = this._nameId[_name]; //ir.GetIndividualIdByUserName(_name);

            CustomProfile userProfile = CustomProfile.GetProfile(_name);
            userProfile.Personal.IndividualId = ie;
            userProfile.Personal.FirstName = _name;
            userProfile.Personal.MiddleName = _name;
            userProfile.Personal.Lastname = _name;
            userProfile.Personal.SiteCode = _siteCode;
            try
            {
                // Save the new profile information
                userProfile.Save();
                userProfile = null;
            }
            catch (Exception ex)
            {
                this.lblFeedback.Text = string.Format("There was an error trying to load this profile - {0}", ex.Message);
            }
        }

        private void AddRole(string _userName, string _role)
        {
            try
            {
                Roles.AddUserToRole(_userName, _role);
            }
            catch (Exception ex)
            {
                this.lblFeedback.Text = string.Format("There was an error trying to load this profile - {0}", ex.Message);
            }
        }
    }
}