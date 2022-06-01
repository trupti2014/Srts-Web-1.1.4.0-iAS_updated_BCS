using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SrtsWeb.Presenters.Admin
{
    public class SitePreferencesPresenter : IDisposable
    {
        private ISitePreferencesView v;
        private SiteRepository.SiteCodeRepository r;
        private Boolean disposed = false;
        public SitePreferencesPresenter(ISitePreferencesView view)
        {
            this.v = view;
        }

        public void GetSites()
        {
            r = new SiteRepository.SiteCodeRepository();
            this.v.SiteCodes = r.GetAllSites();
        }


        public int GetLabShipToPatient(string sitecode)
        {
            var r = new SitePreferencesRepository.LabMailToPatientPreferencesRepository();
            var result = r.GetLabShipToPatient(sitecode);
            return result;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if (v != null)
                    v = null;
                if (r != null)
                    r = null;
            }
            disposed = true;
        }


        public class FrameItemsPreferencesPresenter : IDisposable
        {
            private Boolean disposed = false;
            private IFrameItemsPreferencesView v;
            private FrameRepository fr;
            private FrameItemsRepository fir;
            private FrameItemsDefaultsRepository fidr;
            private SitePreferencesRepository.FrameItemsPreferencesRepository r;

            public FrameItemsPreferencesPresenter(IFrameItemsPreferencesView view)
            {
                this.v = view;
            }

            public void GetFrameList()
            {
                fr = new FrameRepository();
                this.v.FrameList = fr.GetAllFrames();
            }

            public void GetAllFrameswithPreferences()
            {
                fr = new FrameRepository();
                this.v.FrameList = fr.GetAllFrameswithPreferences();
            }

            public void GetFrameItemsList()
            {
                fir = new FrameItemsRepository();
                var l = fir.GetFrameItemByFrameCode(this.v.Frame);

                this.v.ColorList = l.Where(x => x.TypeEntry.ToLower() == "color").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.EyeList = l.Where(x => x.TypeEntry.ToLower() == "eye").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.BridgeList = l.Where(x => x.TypeEntry.ToLower() == "bridge").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.TempleList = l.Where(x => x.TypeEntry.ToLower() == "temple").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.LensList = l.Where(x => x.TypeEntry.ToLower() == "lens_type").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.TintList = l.Where(x => x.TypeEntry.ToLower() == "tint").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.CoatingList = l.Where(x => x.TypeEntry.ToLower() == "coating").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                this.v.MaterialList = l.Where(x => x.TypeEntry.ToLower() == "material").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            }

            public void GetGlobalPreferencesList()
            {
                fidr = new FrameItemsDefaultsRepository();
                this.v.FrameItemDefault = fidr.GetFrameItemsDefaults(this.v.Frame);
            }

            public void GetSitePreferences()
            {
                r = new SitePreferencesRepository.FrameItemsPreferencesRepository();
                this.v.FrameItemPreferenceList = r.GetFrameItemPreferences(this.v.SiteCode);
            }

            public void SetDefaultsForFrame()
            {
                var p = this.v.FrameItemPreference.Clone();

                // BRIDGE
                this.v.Bridge = !p.Bridge.IsNullOrEmpty() ? p.Bridge : !this.v.FrameItemDefault.DefaultBridge.IsNullOrEmpty() ? this.v.FrameItemDefault.DefaultBridge : "G";

                // COLOR - Global BLK
                this.v.Color = !p.Color.IsNullOrEmpty() ? p.Color : this.v.ColorList.Any(x => x.Value == "BLK") ? "BLK" : "G";

                // EYE
                this.v.Eye = !p.Eye.IsNullOrEmpty() ? p.Eye : !this.v.FrameItemDefault.DefaultEyeSize.IsNullOrEmpty() ? this.v.FrameItemDefault.DefaultEyeSize : "G";

                // LENS - Global SVC
                this.v.Lens = !p.Lens.IsNullOrEmpty() ? p.Lens : this.v.LensList.Any(x => x.Value == "SVD") ? "SVD" : "G";

                // MATERIAL - Global if frame is UPLC then POLY else PLAS
                this.v.Material = !p.Material.IsNullOrEmpty() ? p.Material : this.v.Frame.Equals("UPLC") ? "POLY" : this.v.MaterialList.Any(x => x.Value == "PLAS") ? "PLAS" : "G";

                // TEMPLE
                this.v.Temple = !p.Temple.IsNullOrEmpty() ? p.Temple : !this.v.FrameItemDefault.DefaultTemple.IsNullOrEmpty() ? this.v.FrameItemDefault.DefaultTemple : "G";

                // TINT - Global CL
                this.v.Tint = !p.Tint.IsNullOrEmpty() ? p.Tint : this.v.TintList.Any(x => x.Value == "CL") ? "CL" : "G";

                // COATING
                this.v.Coating = !p.Coatings.IsNullOrEmpty() ? p.Coatings : String.Empty;

                // OD SEG HT
                this.v.OdSegHt = !p.OdSegHt.IsNullOrEmpty() ? p.OdSegHt : String.Empty;

                // OS SEG HT
                this.v.OsSegHt = !p.OsSegHt.IsNullOrEmpty() ? p.OsSegHt : String.Empty;
            }

            public Boolean SetPreferencesToDb()
            {
                var p = new SitePrefFrameItemEntity();
                p.Bridge = this.v.Bridge.Equals("X") ? String.Empty : this.v.Bridge;
                p.Color = this.v.Color.Equals("X") ? String.Empty : this.v.Color;
                //p.Coatings = this.v.Coating.Equals("X") ? String.Empty : this.v.Coating;
                p.Eye = this.v.Eye.Equals("X") ? String.Empty : this.v.Eye;
                p.Lens = this.v.Lens.Equals("X") ? String.Empty : this.v.Lens;
                p.Material = this.v.Material.Equals("X") ? String.Empty : this.v.Material;
                p.OdSegHt = this.v.OdSegHt;
                p.OsSegHt = this.v.OsSegHt;
                p.Temple = this.v.Temple.Equals("X") ? String.Empty : this.v.Temple;
                p.Tint = this.v.Tint.Equals("X") ? String.Empty : this.v.Tint;
                p.Coatings = this.v.Coating;
                p.SiteInfo = this.v.SiteCode;
                p.Frame = this.v.Frame;

                r = new SitePreferencesRepository.FrameItemsPreferencesRepository();
                var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
                return r.SetPreferencesToDb(p, ModifiedBy);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                    if (fr != null)
                        fr = null;
                    if (fir != null)
                        fir = null;
                    if (fidr != null)
                        fidr = null;
                }
                disposed = true;
            }
        }

        public class PrescriptionPreferencesPresenter : IDisposable
        {
            private Boolean disposed = false;
            private IPrescriptionPreferencesView v;
            private IndividualRepository ir;
            private SitePreferencesRepository.RxPreferencesRepository r;

            public PrescriptionPreferencesPresenter(IPrescriptionPreferencesView view)
            {
                this.v = view;
            }

            public void InitView()
            {
                GetProviderList();
                var d = GetRxDefaults();
                if (d.IsNull())
                {
                    this.v.SitePrefsRX = new Entities.SitePrefRxEntity();
                    this.v.PDDistance = null;
                    this.v.PDNear = null;
                    this.v.RxType = "FTW";
                    this.v.SitePrefsRX.RxType = "FTW";
                    this.v.SitePrefsRX.SiteCode = this.v.mySession.MyClinicCode;
                    //this.v.PDDistance = 63;
                    //this.v.PDNear = 60;
                    return;
                }
                this.v.SitePrefsRX = d;
                this.v.Provider = d.ProviderId;
                this.v.RxType = d.RxType;
                this.v.PDDistance = d.PDDistance;
                this.v.PDNear = d.PDNear;
            }

            public void FillView()
            {
                var d = GetRxDefaults();
                if (d.IsNull()) return;

                this.v.Provider = d.ProviderId;
                this.v.RxType = d.RxType;
                this.v.PDDistance = d.PDDistance;
                this.v.PDNear = d.PDNear;
            }

            private void GetProviderList()
            {
                ir = new IndividualRepository();
                var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
                this.v.ProviderList = ir.GetIndividualBySiteCodeAndPersonalType(this.v.SiteCode, "PROVIDER", ModifiedBy);
            }

            public SitePrefRxEntity GetRxDefaults()
            {
                r = new SitePreferencesRepository.RxPreferencesRepository();
                return r.GetRxDefaults(this.v.SiteCode);
            }

            public Boolean UpdateRxDefaults(SitePrefRxEntity spe)
            {
                spe.ModifiedBy = string.IsNullOrEmpty(spe.ModifiedBy) ? Globals.ModifiedBy : spe.ModifiedBy;
                r = new SitePreferencesRepository.RxPreferencesRepository();
                return r.InsertSitePrefRx(spe);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                    if (ir != null)
                        ir = null;
                }
                disposed = true;
            }
        }

        public class OrderPreferencesPresenter : IDisposable
        {
            private Boolean disposed = false;
            private IOrderPreferencesView v;
            private SitePreferencesRepository.OrderPreferencesRepository r;

            public OrderPreferencesPresenter(IOrderPreferencesView view)
            {
                this.v = view;
            }

            public void GetPriorityList()
            {
                var p = this.v.LookupCache.Where(x => x.Code.ToLower() == "orderprioritytype").ToList();
                this.v.InitialLoadPriorityList = p;
                this.v.PriorityPriorityList = p;
            }

            public void SetFrameListToSelect()
            {
                var l = new Dictionary<String, String>();
                this.v.InitialLoadFrameList = l;
                this.v.PriorityFrameList = l;
            }

            public IDictionary<String, String> GetFramesByPriority(String Priority)
            {
                r = new SitePreferencesRepository.OrderPreferencesRepository();
                return r.GetFramesByPriority(this.v.SiteCode, Priority);
            }

            public void GetLabList()
            {
                var d = new Dictionary<String, String>();

                // Get lab list.
                var s = this.v.SiteCodes.FirstOrDefault(x => x.SiteCode == this.v.SiteCode);

                d.Add(String.Format("{0} - {1}", "MV", s.MultiPrimary), String.Format("0{0}", s.MultiPrimary));
                d.Add(String.Format("{0} - {1}", "SV", s.SinglePrimary), String.Format("1{0}", s.SinglePrimary));

                this.v.LabList = d;
            }

            public void GetDistributionMethodList()
            {
                var d = new Dictionary<String, String>();

                d.Add("Clinic Distribution", "CD");
                d.Add("Clinic Ship to Patient", "C2P");
                d.Add("Lab Ship to Patient", "L2P");

                this.v.DistributionMethodList = d;
            }

            public void GetPreferences()
            {
                r = new SitePreferencesRepository.OrderPreferencesRepository();
                this.v.OrderPreferences = r.GetPreferences(this.v.SiteCode);
            }

            public Boolean SetPreferencesToDb()
            {
                r = new SitePreferencesRepository.OrderPreferencesRepository();
                var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
                return r.SetPreferencesToDb(this.v.OrderPreferences, ModifiedBy);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                }
                disposed = true;
            }
        }

        public class GeneralPreferencesPresenter : IDisposable
        {
            private ISitePreferencesGeneral v;
            private Boolean disposed = false;
            private SitePreferencesRepository.GeneralPreferencesRepostiory r;

            public GeneralPreferencesPresenter(ISitePreferencesGeneral view) { this.v = view; }

            public void GetPreferences()
            {
                r = new SitePreferencesRepository.GeneralPreferencesRepostiory();
                this.v.LabelSortedAlpha = r.GetPreferences(this.v.SiteCode);
            }

            public bool SetPreferencesToDb()
            {
                r = new SitePreferencesRepository.GeneralPreferencesRepostiory();
                return r.SetPreferencesToDb(this.v.SiteCode, this.v.LabelSortedAlpha);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                }
                disposed = true;
            }
        }

        public class ShippingPreferencesPresenter : IDisposable
        {
            private ISitePreferencesShipping v;
            private Boolean disposed = false;
            private SitePreferencesRepository.ShippingPreferencesRepository r;

            public ShippingPreferencesPresenter(ISitePreferencesShipping view) { this.v = view; }

            public void InitView()
            {
                this.v.ShippingProviderList = GetShippingProviders();
                this.v.Shipper = GetShippingPreferences();
            }

            public List<LookupTableEntity> GetShippingProviders()
            {
                var l = new LookupRepository();
                return l.GetLookupsByType("Shippers");
            }

            public string GetShippingPreferences()
            {
                r = new SitePreferencesRepository.ShippingPreferencesRepository();
                return r.GetShippingPreferences(this.v.SiteCode);
            }

            public bool SetShippingPreferences()
            {
                r = new SitePreferencesRepository.ShippingPreferencesRepository();
                return r.SetShippingPrefToDb(this.v.SiteCode, this.v.Shipper);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                }
                disposed = true;
            }
        }

        public class LabParametersPresenter : IDisposable
        {
            private Boolean disposed = false;
            private ISitePreferencesLabParametersView v;
            private FabricationParametersRepository r;
            private LabParametersRepository lpr;
            private FrameItemsRepository fir;

            public LabParametersPresenter(ISitePreferencesLabParametersView view)
            {
                this.v = view;
            }

            public void InitView()
            {
                FillLabParameters();
                FillLensLabParameters();
            }

            public void FillLabParameters()
            {
                r = new FabricationParametersRepository();
                fir = new FrameItemsRepository();
                var l = fir.GetFrameItems();
                               
                v.FabricationParameterData = r.GetAllParametersBySiteCode(v.SiteCode);
                v.LensMaterial = l.Where(x => x.TypeEntry.ToLower() == "material").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }

            public void FillLensLabParameters()
            {
                LabParametersRepository labParmRep = new LabParametersRepository();
                var lp = labParmRep.GetLabParametersBySiteCode(v.SiteCode);
                if (lp.Count > 0)
                {
                    v.MaxPrism = lp.SingleOrDefault().MaxPrism;
                    v.MaxDecentrationPlus = lp.SingleOrDefault().MaxDecentrationPlus;
                    v.MaxDecentrationMinus = lp.SingleOrDefault().MaxDecentrationMinus;
                }
            }  
            
            public bool HasLensLabParameters()
            {
                LabParametersRepository labParmRep = new LabParametersRepository();
                var lp = labParmRep.GetLabParametersBySiteCode(v.SiteCode);
                return lp.Count > 0;
                    
            }


            public bool InsertParameter()
            {
                var pe = new FabricationParameterEntitiy();

                pe.Material = v.Material;
                pe.Cylinder = v.Cylinder;
                pe.MaxPlus = v.MaxPlus;
                pe.MaxMinus = v.MaxMinus;
                pe.IsStocked = v.IsStocked.ToBoolean();
                pe.SiteCode = v.SiteCode;
                pe.ModifiedBy = v.mySession.ModifiedBy;
                pe.CapabilityType = v.CapabilityType;
                

                r = new FabricationParametersRepository();
                return r.InsertParameter(pe);
            }

            public bool UpdateParameter(int id)
            {
                var pe = new FabricationParameterEntitiy();

                pe.ID = id;
                pe.Material = v.Material;
                pe.Cylinder = v.Cylinder;
                pe.MaxPlus = v.MaxPlus;
                pe.MaxMinus = v.MaxMinus;
                pe.IsStocked = v.IsStocked.ToBoolean();
                pe.SiteCode = v.SiteCode;
                pe.ModifiedBy = v.mySession.ModifiedBy;
                pe.IsActive = true;
                pe.CapabilityType = v.CapabilityType;

                r = new FabricationParametersRepository();
                return r.UpdateParameter(pe);
            }

            public void DeleteParameter(int id)
            {
                r = new FabricationParametersRepository();
                r.DeleteParameter(id);
            }

            public bool InsertUpdateLabParameter(string id)
            {
                var lp = new LabParameterEntity();

                lp.SiteCode = v.SiteCode;
                lp.MaxPrism = v.MaxPrism;
                lp.MaxDecentrationPlus = v.MaxDecentrationPlus;
                lp.MaxDecentrationMinus = v.MaxDecentrationMinus;
                lp.ModifiedBy = v.mySession.ModifiedBy;

                lpr = new LabParametersRepository();
                return lpr.InsertUpdateLabParameter(lp);
            }

            //public bool UpdateLabParameter(string id)
            //{
            //    var lp = new LabParameterEntity();

            //    lp.SiteCode = v.SiteCode;
            //    lp.MaxPrism = v.MaxPrism;
            //    lp.MaxDecentrationPlus = v.MaxDecentrationPlus;
            //    lp.MaxDecentrationMinus = v.MaxDecentrationMinus;
            //    lp.ModifiedBy = v.mySession.ModifiedBy;

            //    lpr = new LabParametersRepository();
            //    return lpr.UpdateLabParameter(lp);
            //}


            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                    if (fir != null)
                        fir = null;
                }
                disposed = true;
            }
        }

        public class LabJustificationPresenter : IDisposable
        {
            private Boolean disposed = false;
            private ISitePreferencesLabJustification v;
            private SitePreferencesRepository.LabJustificationRepositoryPreferencesRepository r;

            public LabJustificationPresenter(ISitePreferencesLabJustification view)
            {
                this.v = view;
            }

            public void InitView()
            {
                r = new SitePreferencesRepository.LabJustificationRepositoryPreferencesRepository();
                var j = r.GetLabJustifications(this.v.SiteCode);
                if (j.IsNullOrEmpty())
                {
                    j = new List<SitePrefLabJustification>();
                    // Add "empty" objects, everything is filled out except the justification
                    j.Add(new SitePrefLabJustification() { JustificationReason = "redirect", SiteCode = this.v.SiteCode });
                    j.Add(new SitePrefLabJustification() { JustificationReason = "reject", SiteCode = this.v.SiteCode });
                }
                var h = j.GetObjectHash();
                this.v.Justifications = j;
                this.v.JustificationHash = h;
            }

            public bool SetLabJustifications()
            {
                var good = true;
                this.v.Justifications.ForEach(x =>
                {
                    if (!x.Justification.IsNullOrEmpty())
                    {
                        r = new SitePreferencesRepository.LabJustificationRepositoryPreferencesRepository();
                        if (!r.SetLabJustification(x))
                            good = false;
                    }
                    else //Aldela: added these two lines
                    {
                        r = new SitePreferencesRepository.LabJustificationRepositoryPreferencesRepository();
                        r.DeleteLabJustifications(this.v.SiteCode, x.JustificationReason);//reason);
                    }
                });

                return good;
            }

            public void DeleteLabJustifications(String reason = null)
            {
                r = new SitePreferencesRepository.LabJustificationRepositoryPreferencesRepository();
                // There is no success/fail return from the delete SP so the assumption is always success.
                r.DeleteLabJustifications(this.v.SiteCode, reason);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                    if (r != null)
                        r = null;
                }
                disposed = true;
            }
        }

        public class LabMailToPatientPresenter : IDisposable
        {
            private Boolean disposed = false;
            private ISitePreferencesLabMailToPatientView v;
            private SitePreferencesRepository.LabMailToPatientPreferencesRepository m;
            public LabMailToPatientPresenter(ISitePreferencesLabMailToPatientView view)
            {
                this.v = view;
            }

            public SitePrefLabMTPEntity InitView()
            {
                v.EmailNotification = new List<EmailMessageEntity>();
                var mySession = System.Web.HttpContext.Current.Session["SRTSSession"] as SRTSSession;
                v.SitePrefLabMTPEntity = new SitePrefLabMTPEntity();
                var d = GetLabMTPPref();
                if (d.IsNull())
                {
                    v.SitePrefLabMTPEntity.SiteCode = this.v.mySession.MySite.SiteCode;
                    v.SitePrefLabMTPEntity.ClinicSiteCode = "";
                    v.SitePrefLabMTPEntity.ClinicActionRequired = "";
                    v.SitePrefLabMTPEntity.ListofClinicActionRequired = null;
                    v.SitePrefLabMTPEntity.IsCapabilityOn = "false";
                    v.SitePrefLabMTPEntity.Capacity = 0;
                    v.SitePrefLabMTPEntity.StatusReason = "NoCapacity";
                    v.SitePrefLabMTPEntity.Comments = "";
                    v.SitePrefLabMTPEntity.StartDate = null;
                    v.SitePrefLabMTPEntity.StopDate = null;
                    v.SitePrefLabMTPEntity.AnticipatedRestartDate = null;
                }
                else
                {
                    v.SitePrefLabMTPEntity = d;
                }
                v.SitePrefLabMTPEntity.IsLabMailToPatient = mySession.MySite.ShipToPatientLab.ToString();
                SetJavascriptLabMTPEntity(v.SitePrefLabMTPEntity);
                GetClinicsforLabDisabled(v.SitePrefLabMTPEntity.SiteCode);
                GetClinicsforLab(v.SitePrefLabMTPEntity.SiteCode);



                return this.v.SitePrefLabMTPEntity;
            }

            public void SetJavascriptLabMTPEntity(SitePrefLabMTPEntity mtpentity)
            {
                string[] entityproperties = null;
            }



            public void GetClinicsforLab(string sitecode)
            {
                var s = new SiteRepository.SiteCodeRepository();
                v.Clinics = s.GetAllSites().Where(x => x.SiteType == "CLINIC" && x.MultiPrimary == sitecode ||
                                                        x.MultiSecondary == sitecode ||
                                                        x.SinglePrimary == sitecode ||
                                                        x.SingleSecondary == sitecode).ToList();
            }




            public void GetClinicsforLabDisabled(string labsitecode)
            {
                m = new SitePreferencesRepository.LabMailToPatientPreferencesRepository();
                v.DisabledClinics = m.GetClinicStatusforLab(labsitecode);
            }



            public SitePrefLabMTPEntity GetLabMTPPref()
            {
                m = new SitePreferencesRepository.LabMailToPatientPreferencesRepository();
                return m.GetLabMTPPref(this.v.mySession.MySite.SiteCode);
            }

          
             


            public Boolean InsertUpdateLabMTPPref(SitePrefLabMTPEntity mtp)
            {
                mtp.ModifiedBy = string.IsNullOrEmpty(mtp.ModifiedBy) ? Globals.ModifiedBy : mtp.ModifiedBy;
                if (mtp.StartDate < DateTime.Now)
                {
                    mtp.StartDate = null;
                }
                m = new SitePreferencesRepository.LabMailToPatientPreferencesRepository();

                return m.InsertSitePref_LabMTP(mtp);
            }



            public Boolean InsertUpdateClinicStatus(string clinicSiteCode, string actionRequired, DateTime? effectiveDate, [Optional] string comment)
            {
                try
                {
                    m = new SitePreferencesRepository.LabMailToPatientPreferencesRepository();
                    return m.InsertUpdateClinicStatus(clinicSiteCode, this.v.mySession.MySite.SiteCode, actionRequired, effectiveDate, comment);
                }
                catch (Exception ex)
                {
                    ex.Source = "SitePreferencesPresenter.cs - InsertUpdateClinicStatus()";
                    ex.LogException(String.Format("An error occurred in - {0}, {1}, {2}", ex.Source, ex.Message, ex.InnerException));
                    return false;
                }
            }




            public Boolean InsertLabNotificationEmailSent(EmailMessageEntity emailnotification)
            {
                m = new SitePreferencesRepository.LabMailToPatientPreferencesRepository();
                return m.InsertLabNotificationEmailSent(emailnotification);
            }



            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                }
                disposed = true;
            }


        }


        //public class ClinicGroupsPreferencesPresenter : IDisposable
        //{
        //    private Boolean disposed = false;
        //    private IClinicGroupsView v;
        //    private SitePreferencesRepository.ClinicGroupsPreferencesRepository r;

        //    public ClinicGroupsPreferencesPresenter(IClinicGroupsView view)
        //    {
        //        this.v = view;
        //    }

        //    public void InitView()
        //    {

        //    }

        //    public void FillView()
        //    {

        //    }

        //    public List<SitePrefClinicGroupsEntity> GetClinicGroupsDefaults()
        //    {
        //        r = new SitePreferencesRepository.ClinicGroupsPreferencesRepository();
        //        return r.GetClinicGroupsDefaults(this.v.mySession.MySite.SiteCode);
        //    }

        //    public bool InsertClinicGroup(SitePrefClinicGroupsEntity group)
        //    {
        //        SitePrefClinicGroupsEntity spc = new SitePrefClinicGroupsEntity();
        //        r = new SitePreferencesRepository.ClinicGroupsPreferencesRepository();
        //        var result = r.InsertClinicGroup(group);
        //        return result;
        //    }

        //    public bool ActivateAllClinicGroups(SitePrefClinicGroupsEntity groups)
        //    {
        //        SitePrefClinicGroupsEntity spc = new SitePrefClinicGroupsEntity();
        //        r = new SitePreferencesRepository.ClinicGroupsPreferencesRepository();
        //        var result = r.ActivateAllClinicGroups(groups);
        //        return result;
        //    }

        //    public void Dispose()
        //    {
        //        Dispose(true);
        //        GC.SuppressFinalize(this);
        //    }

        //    protected virtual void Dispose(Boolean disposing)
        //    {
        //        if (disposed) return;
        //        if (disposing)
        //        {
        //            if (v != null)
        //                v = null;
        //            if (r != null)
        //                r = null;
        //        }
        //        disposed = true;
        //    }
        //}

        public class RoutingOrdersPresenter : IDisposable
        {
            private Boolean disposed = false;
            private ISitePreferencesRoutingOrdersView v;
            private SitePreferencesRepository.RoutingOrdersPreferencesRepository rop;

            public RoutingOrdersPresenter(ISitePreferencesRoutingOrdersView view)
            {
                this.v = view;
            }

            public void InitView()
            {
                FillCurrentLabCapacity();
                FillLabCapacityHistory();
            }

            public void FillCurrentLabCapacity()
            {
                rop = new SitePreferencesRepository.RoutingOrdersPreferencesRepository();
                var lp = rop.GetCurrentLabCapacityBySiteCode(v.SiteCode);
                if (lp != null)
                {
                    v.Capacity = lp.Capacity;
                    v.PDO = lp.PDO;
                }
            }

            public void FillLabCapacityHistory()
            {
                rop = new SitePreferencesRepository.RoutingOrdersPreferencesRepository();
                v.LabCapacityHistoryData = rop.GetLabCapacityHistoryBySiteCode(v.SiteCode);
            }

            public bool HasCurrentLabCapacity()
            {
                rop = new SitePreferencesRepository.RoutingOrdersPreferencesRepository();
                var lc = rop.GetLabCapacityHistoryBySiteCode(v.SiteCode);
                return lc.Count > 0;
            }


            public bool InsertRoutingOrdersPref()
            {
                var roe = new SitePrefRoutingOrdersEntity();

                roe.SiteCode = v.SiteCode;
                roe.Capacity = v.Capacity;
                roe.PDO = v.PDO;
                roe.ModifiedBy = v.mySession.ModifiedBy;

                rop = new SitePreferencesRepository.RoutingOrdersPreferencesRepository();
                return rop.InsertSitePref_RoutingOrders(roe);
            }


            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                }
                disposed = true;
            }
        }

    }
}