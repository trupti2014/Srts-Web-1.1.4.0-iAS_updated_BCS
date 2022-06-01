using Microsoft.Web.Administration;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using SrtsWeb.BusinessLayer.Concrete;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml;
using System.IO;



namespace SrtsWeb.WebForms.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class ManageEventLogTracing : PageBase
    {
        private string strConfigPath;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
                        mySession.ReturnURL = string.Empty;
                    }
                    try
                    {
                        Master.CurrentModuleTitle = string.Empty;
                        mySession.CurrentModule = "Management - Application Event Log Tracing";
                        mySession.CurrentModule_Sub = string.Empty;
                        BuildPageTitle();
                        SetSwitchLevels();         
                    }
                    catch (NullReferenceException)
                    {
                        Response.Redirect(FormsAuthentication.DefaultUrl);
                    }
                }
            }
            else
            {
                BuildPageTitle();
                SetSwitchLevels();   
            }
        }

        private void SetSwitchLevels()
        {
            XDocument docLevels = loadConfigDoc();
            XElement switches = docLevels.Descendants().FirstOrDefault(d => d.Name == "switches");
            var switchLevels = switches.Elements();
            foreach (XElement s in switchLevels)
            {
                string levelName = s.FirstAttribute.Value.ToString();
                int levelValue = int.Parse(s.LastAttribute.Value);
                switch (levelName)
                {
                    case "srtsAdminTraceSwitch":
                        {
                            txtAdmin.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsLoginTraceSwitch":
                        {
                            txtLogin.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsClinicManageTraceSwitch":
                        {
                            txtClinicManagement.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsClinicOrderTraceSwitch":
                        {
                            txtClinicOrders.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsLabManageTraceSwitch":
                        {
                            txtLabManagement.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsLabOrderTraceSwitch":
                        {
                            txtLabOrders.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsRxTraceSwitch":
                        {
                            txtPrescriptions.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsExamTraceSwitch":
                        {
                            txtExams.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                    case "srtsPersonTraceSwitch":
                        {
                            txtPerson.Text = GetSwitchLevelName(levelValue).ToString();
                        }
                        break;
                }
            }


            {
            }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Management - Application Event Log Tracing");
                CurrentModule_Sub(string.Empty);
            }
        }

        private string GetSwitchLevelName(int level)
        {
            string name = string.Empty;
            if (level == 0)
            {
                name = "Off";
            }
            else
            {
            name = string.Format("{0}", Enum.GetName(typeof(TraceEventType), level));
            
            }
            return name;
        }
        
        private static int GetSwitchLevelValue(string enumName)
        {
            int enumValue = 0;
            switch(enumName)
            {
                case "Off":
                    { enumValue = 0; }
                    break;
                case "Error":
                    { enumValue = (int)TraceEventType.Error; }
                    break;
                case "Warning":
                    { enumValue = (int)TraceEventType.Warning; }
                    break;
                case "Information":
                    { enumValue = (int)TraceEventType.Information; }
                    break;
                case "Verbose":
                    { enumValue = (int)TraceEventType.Verbose; }
                    break;
            }
            return enumValue;
        }

        private XDocument loadConfigDoc()
        {
            try
            {
                string baseFileName = "Logging.config";
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var configPath = Path.Combine(appPath, baseFileName);
                var configDoc = XDocument.Load(configPath);
                strConfigPath = configPath.ToString();
                return configDoc;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [System.Web.Services.WebMethod]
        public static void SaveLogLevel(string logName, string newlevel)
        {
            int newSwitchLevel = GetSwitchLevelValue(newlevel);
            string baseFileName = "Logging.config";
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(appPath, baseFileName);
            var configDoc = XDocument.Load(configPath);

            string configSwitchName = GetConfigSwitchName(logName); 
            
            XElement switches = configDoc.Descendants().FirstOrDefault(d => d.Name == "switches");
            {
                var switchName = from c in switches.Elements("add")
                            .Where(c => (string)c.Attribute("name") == configSwitchName)
                                  select c;
                foreach (XElement name in switchName)
                {
                   name.SetAttributeValue("value", newSwitchLevel);
                }
            }
            configDoc.Save(configPath);
        }

        private static string GetConfigSwitchName(string name)
        {
            string switchname = string.Empty;
            switch (name)
            {
                case "Admin Events":
                    {switchname = "srtsAdminTraceSwitch";}
                    break;
                case "Login Events":
                    {switchname = "srtsLoginTraceSwitch";}
                    break;
                case "Clinic Management Events":
                    {switchname = "srtsClinicManageTraceSwitch";}
                    break;
                case "Clinic Orders Events":
                    {switchname = "srtsClinicOrderTraceSwitch";}
                    break;
                case "Lab Management Events":
                    {switchname = "srtsLabManageTraceSwitch";}
                    break;
                case "Lab Orders Events":
                    {switchname = "srtsLabOrderTraceSwitch";}
                    break;
                case "Prescription Events":
                    {switchname = "srtsRxTraceSwitch";}
                    break;
                case "Exam Events":
                    {switchname = "srtsExamTraceSwitch";}
                    break;
                case "Person Events":
                    {switchname = "srtsPersonTraceSwitch";}
                    break;
            }
            return switchname;
        }
    }
}