using DataBaseAccessLayer;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;

namespace SrtsWeb.DataLayer.Repositories
{
    public class SitePreferencesRepository
    {
        /// <summary>
        /// A custom repository class to do frame item preference operations.
        /// </summary>
        public class FrameItemsPreferencesRepository : RepositoryBase<SitePrefFrameItemEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public FrameItemsPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Gets a list of frame item preferences for a site.
            /// </summary>
            /// <param name="SiteCode">Site code to get preferences for.</param>
            /// <returns>SitePrefFrameItemEntity list of frame item preferences.</returns>
            public List<SitePrefFrameItemEntity> GetFrameItemPreferences(String SiteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetSitePref_Frames";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Adds/updates a frame item preference for a site.
            /// </summary>
            /// <param name="entity">Frame item preference to add/update.</param>
            /// <param name="modifiedBy">User performing operation.</param>
            /// <returns>Success/failure of operation.</returns>
            public Boolean SetPreferencesToDb(SitePrefFrameItemEntity entity, String modifiedBy)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "InsertSitePref_Frames";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", entity.SiteInfo));
                cmd.Parameters.Add(DAL.GetParamenter("@FrameCode", entity.Frame));
                cmd.Parameters.Add(DAL.GetParamenter("@Color", entity.Color));
                cmd.Parameters.Add(DAL.GetParamenter("@Eye", entity.Eye));
                cmd.Parameters.Add(DAL.GetParamenter("@Bridge", entity.Bridge));
                cmd.Parameters.Add(DAL.GetParamenter("@Temple", entity.Temple));
                cmd.Parameters.Add(DAL.GetParamenter("@Lens_Type", entity.Lens));
                cmd.Parameters.Add(DAL.GetParamenter("@Tint", entity.Tint));
                cmd.Parameters.Add(DAL.GetParamenter("@Material", entity.Material));
                cmd.Parameters.Add(DAL.GetParamenter("@Coatings", entity.Coatings));
                cmd.Parameters.Add(DAL.GetParamenter("@ODSegHeight", entity.OdSegHt));
                cmd.Parameters.Add(DAL.GetParamenter("@OSSegHeight", entity.OsSegHt));
                cmd.Parameters.Add(DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            protected override SitePrefFrameItemEntity FillRecord(System.Data.IDataReader dr)
            {
                var p = new SitePrefFrameItemEntity();
                p.Bridge = dr.AsString("Bridge");
                p.Coatings = dr.AsString("Coatings");
                p.Color = dr.AsString("Color");
                p.Eye = dr.AsString("Eye");
                p.Frame = dr.AsString("Frame");
                p.Lens = dr.AsString("Lens_Type");
                p.Material = dr.AsString("Material");
                p.OdSegHt = dr.AsString("ODSegHeight");
                p.OsSegHt = dr.AsString("OSSegHeight");
                p.SiteInfo = dr.AsString("SiteInfo");
                p.Temple = dr.AsString("Temple");
                p.Tint = dr.AsString("Tint");

                return p;
            }
        }

        /// <summary>
        /// A custom repository class to do Rx preference operations.
        /// </summary>
        public class RxPreferencesRepository : RepositoryBase<SitePrefRxEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public RxPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Get Rx preferences for a site.
            /// </summary>
            /// <param name="siteCode">Site code to get preference for.</param>
            /// <returns>SitePrefRxEntity entity for a site.</returns>
            public SitePrefRxEntity GetRxDefaults(String siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetSitePref_Rx";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));

                var d = GetRecord(cmd);

                return d;
            }

            protected override SitePrefRxEntity FillRecord(IDataReader dr)
            {
                var d = new SitePrefRxEntity();
                d.SiteCode = dr["SiteCode"].ToString();
                d.ProviderId = dr["Provider"].ToNullableInt32();
                d.RxType = dr["RxType"].ToString();
                d.PDDistance = dr["PDDistance"].ToNullableDecimal();
                d.PDNear = dr["PDNear"].ToNullableDecimal();

                return d;
            }

            /// <summary>
            /// Adds a new Rx preference to the database.
            /// </summary>
            /// <param name="spe">Site preference to add.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertSitePrefRx(SitePrefRxEntity spe)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertSitePref_Rx";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", spe.SiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxType", spe.RxType));

                if (!spe.ProviderId.IsNull())
                    cmd.Parameters.Add(this.DAL.GetParamenter("@Provider", spe.ProviderId));

                if (!spe.PDDistance.IsNull())
                    cmd.Parameters.Add(this.DAL.GetParamenter("@PDDistance", spe.PDDistance));

                if (!spe.PDNear.IsNull())
                    cmd.Parameters.Add(this.DAL.GetParamenter("@PDNear", spe.PDNear));

                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", spe.ModifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }
        }

        /// <summary>
        /// A custom repository class to do order preference operations.
        /// </summary>
        public class OrderPreferencesRepository : RepositoryBase<SitePrefOrderEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public OrderPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Gets all frames for a site by priority.
            /// </summary>
            /// <param name="SiteCode">Site code to search with.</param>
            /// <param name="Priority">Priority to narrow down frame list with.</param>
            /// <returns>Dictionary containing frame data for a ddl.</returns>
            public Dictionary<String, String> GetFramesByPriority(String SiteCode, String Priority)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetFramesBySiteBOSPriority";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@Priority", Priority));
                return DAL.GetData(cmd).Rows.Cast<DataRow>().ToList().Select(x => new { Key = x["FrameLongDescription"].ToString(), Value = x["FrameCode"].ToString() }).Distinct().ToDictionary(d => d.Key, d => d.Value);
            }

            /// <summary>
            /// Adds/Updates site preferences to the database.
            /// </summary>
            /// <param name="entity">Site preference entity to add/update.</param>
            /// <param name="modifiedBy">User performing the operation.</param>
            /// <returns>Success/failure of operation.</returns>
            public Boolean SetPreferencesToDb(SitePrefOrderEntity entity, String modifiedBy)
            {
                var good = true;

                // Check for a null priorityframecombo.  If yes then create a blank one and insert the rest of the entity.
                if (entity.PriorityFrameCombo.IsNullOrEmpty())
                {
                    good = DoPrefInsertUpdate(entity, new SitePrefOrderPriorityPreferencesEntity(), modifiedBy);
                }
                else
                {
                    foreach (var x in entity.PriorityFrameCombo)
                    {
                        good = DoPrefInsertUpdate(entity, x, modifiedBy);
                    }
                }
                return good;
            }

            private Boolean DoPrefInsertUpdate(SitePrefOrderEntity entity, SitePrefOrderPriorityPreferencesEntity priority, String modifiedBy)
            {
                var good = false;
                try
                {
                    var cmd = DAL.GetCommandObject();
                    cmd.CommandText = "InsertSitePref_Orders";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", entity.SiteCode));
                    cmd.Parameters.Add(DAL.GetParamenter("@IFrame", entity.InitialLoadFrame));
                    cmd.Parameters.Add(DAL.GetParamenter("@IPriority", entity.InitialLoadPriority));

                    var dm = default(Int32);
                    switch (entity.DistributionMethod)
                    {
                        case "CD":
                            dm = 1;
                            break;
                        case "C2P":
                            dm = 2;
                            break;
                        case "L2P":
                            dm = 3;
                            break;
                        default:
                            dm = 0;
                            break;
                    }
                    cmd.Parameters.Add(DAL.GetParamenter("@DispenseMethod", dm));
                    cmd.Parameters.Add(DAL.GetParamenter("@Lab", entity.Lab.IsNullOrEmpty() ? String.Empty : entity.Lab.Substring(entity.Lab.Length - 6)));
                    cmd.Parameters.Add(DAL.GetParamenter("@ModifiedBy", modifiedBy));

                    cmd.Parameters.Add(DAL.GetParamenter("@Frame", priority.FrameCode ?? String.Empty));
                    cmd.Parameters.Add(DAL.GetParamenter("@Priority", priority.PriorityCode ?? String.Empty));

                    cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                    InsertData(cmd);

                    var p = cmd.Parameters["@Success"] as IDataParameter;
                    good = p.Value.ToInt32().Equals(1);
                }
                catch (Exception ex) { ex.TraceErrorException(); good = false; }

                return good;
            }

            /// <summary>
            /// Gets site preferences for a site.
            /// </summary>
            /// <param name="SiteCode">Site code to get preferences for.</param>
            /// <returns>SitePrefOrderEntity entity of preferences.</returns>
            public SitePrefOrderEntity GetPreferences(String SiteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetSitePref_Orders";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                return GetRecord(cmd);
            }

            protected override SitePrefOrderEntity GetRecord(IDbCommand cmdIn)
            {
                var dt = DAL.GetData(cmdIn);
                return FillRecord(dt) ?? new SitePrefOrderEntity();
            }

            protected override SitePrefOrderEntity FillRecord(DataTable dt)
            {
                if (dt.Rows.Count.Equals(0)) return new SitePrefOrderEntity();
                var a = new SitePrefOrderEntity();
                a.SiteCode = dt.Rows[0]["SiteInfo"].ToString().Substring(0, 6);
                switch (dt.Rows[0]["DispenseMethod"].ToString())
                {
                    case "Clinic Distribution":
                        a.DistributionMethod = "CD";
                        break;
                    case "Clinic Ship To Patient":
                        a.DistributionMethod = "C2P";
                        break;
                    case "Lab Ship To Patient":
                        a.DistributionMethod = "L2P";
                        break;
                }
                var l = dt.Rows[0]["ProdLab"].ToString().Trim();
                a.Lab = l.IsNullOrEmpty() ? String.Empty : String.Format("{0}{1}", l.StartsWith("M") ? 0 : 1, l.Substring(l.Length - 6));
                a.InitialLoadPriority = dt.Rows[0]["IPriority"].ToString().Trim();
                a.InitialLoadFrame = dt.Rows[0]["IFrame"].ToString().Trim();
                a.PriorityFrameCombo = new List<SitePrefOrderPriorityPreferencesEntity>();
                dt.Rows.Cast<DataRow>().ToList().ForEach(x =>
                {
                    a.PriorityFrameCombo.Add(new SitePrefOrderPriorityPreferencesEntity()
                    {
                        FrameCode = x["Frame"].ToString().Trim(),
                        FrameDescription = String.Format("{0} - {1}", x["Frame"].ToString().Trim(), x["FrameDescription"].ToString().Trim()),
                        PriorityCode = x["PriorityCode"].ToString().Trim(),
                        PriorityDescription = x["Priority"].ToString().Trim()
                    });
                });

                return a;
            }
        }

        /// <summary>
        /// A custom repository class to do general preference operations.
        /// </summary>
        public class GeneralPreferencesRepostiory : RepositoryBase<Object>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public GeneralPreferencesRepostiory()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Saves the sort alphabetically flag for a site to the database.
            /// </summary>
            /// <param name="SiteCode">Site code to save flag to.</param>
            /// <param name="AlphaSort">Flag to determine alpha sort.</param>
            /// <returns>Success/failure of insert.</returns>
            public Boolean SetPreferencesToDb(String SiteCode, Boolean AlphaSort)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "InsertSitePref_General";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@AlphaSort", AlphaSort));
                cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Gets alphabetical sort flag for site.
            /// </summary>
            /// <param name="SiteCode">Site code to search in.</param>
            /// <returns>Alpha sort flag.</returns>
            public Boolean GetPreferences(String SiteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetSitePref_General";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                return GetObject(cmd).ToBoolean();
            }

            protected override object GetObject(IDbCommand cmdIn)
            {
                var dt = DAL.GetData(cmdIn);
                return FillRecord(dt);
            }

            protected override object FillRecord(DataTable dt)
            {
                if (dt.IsNull()) return false;
                if (dt.Rows.IsNull() || dt.Rows.Count.Equals(0)) return false;
                return dt.Rows[0]["AlphaSort"];
            }
        }

        /// <summary>
        /// A custom repository class to do lab justification preference operations.
        /// </summary>
        public class LabJustificationRepositoryPreferencesRepository : RepositoryBase<SitePrefLabJustification>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public LabJustificationRepositoryPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Gets the justification preferences for a site by site code.
            /// </summary>
            /// <param name="SiteCode">Site code to search with.</param>
            /// <returns>SitePrefLabJustification object that contains lab justifications.</returns>
            public List<SitePrefLabJustification> GetLabJustifications(String SiteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetSitePref_Justifications";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Adds/Updates justification preferences to the database.
            /// </summary>
            /// <param name="SiteCode">Site code to search with.</param>
            /// <param name="Justifications">Justification object to set in the database.</param>
            /// <returns>Success/failure of operation.</returns>
            public Boolean SetLabJustification(SitePrefLabJustification Justification)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "InsertSitePref_Justifications";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", Justification.SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@Reason", Justification.JustificationReason));
                cmd.Parameters.Add(DAL.GetParamenter("@Justify", Justification.Justification));
                cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Deletes justification preferences to the database.
            /// </summary>
            /// <param name="SiteCode">Site code to search with.</param>
            /// <param name="Reason">Optional reason code to remove otherwise all justification preferences will be removed at a site.</param>
            public void DeleteLabJustifications(String SiteCode, String Reason = null)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "DeleteSitePref_Justifications";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@Rsn", Reason));

                DeleteData(cmd);
            }

            protected override SitePrefLabJustification FillRecord(IDataReader dr)
            {
                var r = new SitePrefLabJustification();
                r.SiteCode = dr.AsString("SiteCode");
                r.Justification = dr.AsString("Justification");
                r.JustificationReason = dr.AsString("Reason");
                return r;
            }
        }


        /// <summary>
        ///  A custom repository class to manage lab mail-to-patient preference operations.
        /// </summary>
        public class LabMailToPatientPreferencesRepository : RepositoryBase<SitePrefLabMTPEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public LabMailToPatientPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }


            /// <summary>
            /// Get lab mail-to-patient preferences for a site.
            /// </summary>
            /// <param name="siteCode">Site code to get preference for.</param>
            /// <returns>SitePrefLabMTPEntity entity for a site.</returns>
            public SitePrefLabMTPEntity GetLabMTPPref(String siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetSitePref_LabMailToPatient";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSite", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                var d = GetRecord(cmd);
                return d;
            }


            public IEnumerable<SitePrefLabMTPEntity> GetClinicStatusforLab(String labsiteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetLMTP";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSite", labsiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                return GetRecords(cmd).ToList();
            }



            /// <summary>
            /// Get lab ship to patient status.
            /// </summary>
            /// <param name="siteCode">Lab Site code to get preference for.</param>
            /// <returns>Wheter or not site is ship to patient.</returns>
            public int GetLabShipToPatient(string siteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetLabShipToPatient";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@SHIPTo", String.Empty, ParameterDirection.Output));
                GetObject(cmd);

                var p = cmd.Parameters["@SHIPTo"] as IDataParameter;
                return p.Value.ToInt32();

                // return GetObject(cmd).ToInt32();
            }


            /// <summary>
            /// Retrieves information for a specific clinic on their labs ability to ship to patient.
            /// </summary>
            /// <param name="siteCode">Clinic Site code to get lab ability for.</param>
            /// <returns>SitePrefLabMTPEntity entity for a site.</returns>
            public SitePrefLabMTPEntity GetLabMTPForClinic(String siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetShipToPatientLabs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@Sitecode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                var d = GetRecord(cmd);
                return d;
            }


            public bool InsertLabNotificationEmailSent(EmailMessageEntity emailnotification)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertLMTPEmails";
                cmd.Parameters.Add(this.DAL.GetParamenter("@Clinic", emailnotification.ClinicSiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicAdmin", emailnotification.EmailRecipient));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailAddress", emailnotification.EmailAddress));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", emailnotification.LabSiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailVerbiage", emailnotification.EmailBody));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailType", emailnotification.EmailType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                cmd.Parameters.Add(this.DAL.GetParamenter("@TempID", 0, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }



            protected override SitePrefLabMTPEntity FillRecord(IDataReader dr)
            {
                var d = new SitePrefLabMTPEntity();
                d.SiteCode = dr["LabSiteCode"].ToString();
                d.ClinicSiteCode = Extenders.HasColumn(dr, "ClinicSiteCode") ? dr["ClinicSiteCode"].ToString() : null;
                d.IsLabMailToPatient = Extenders.HasColumn(dr, "LabMailToPatient") ? dr["LabMailToPatient"].ToString() : null;
                d.ClinicActionRequired = Extenders.HasColumn(dr, "ActionRequired") ? dr["ActionRequired"].ToString() : null;
                d.IsCapabilityOn = Extenders.HasColumn(dr, "LMTP_AllClinics") ? dr["LMTP_AllClinics"].ToString() : null;
                d.StatusReason = Extenders.HasColumn(dr, "StatusReason") ? dr["StatusReason"].ToString() : null;
                d.Capacity = Extenders.HasColumn(dr, "OrderCapacity") ? dr["OrderCapacity"].ToInt32() : 0;
                if (Extenders.HasColumn(dr, "StopDate")) d.StopDate = dr["StopDate"].ToDateTime();
                if (Extenders.HasColumn(dr, "AnticipatedEndDate")) d.AnticipatedRestartDate = dr["AnticipatedEndDate"].ToDateTime();
                if (Extenders.HasColumn(dr, "StartDate")) d.StartDate = dr["StartDate"].ToDateTime();
                if (Extenders.HasColumn(dr, "StopDate")) d.StopDate = dr["StopDate"].ToDateTime();
                if (Extenders.HasColumn(dr, "EffectiveDate")) d.EffectiveDate = dr["EffectiveDate"].ToDateTime();
                if (Extenders.HasColumn(dr, "Comments")) d.Comments = dr["Comments"].ToString();
                if (Extenders.HasColumn(dr, "Comment")) d.Comments = dr["Comment"].ToString();

                d.ModifiedBy = dr["ModifiedBy"].ToString();
                return d;
            }




            /// <summary>
            /// Adds a new lab mail-to-patient preference to the database.
            /// </summary>
            /// <param name="mtp">Site preference to add.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertSitePref_LabMTP(SitePrefLabMTPEntity mtp)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "IUSitePref_LabMailToPatient";
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", mtp.SiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LMTP", mtp.IsLabMailToPatient));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LMTP_All", mtp.IsCapabilityOn));
                cmd.Parameters.Add(this.DAL.GetParamenter("@StatusReason", mtp.StatusReason));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Comments", mtp.Comments));
                if (!mtp.Capacity.IsNull())
                    cmd.Parameters.Add(this.DAL.GetParamenter("@CapacityNbr", mtp.Capacity));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", mtp.ModifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@StartDate", mtp.StartDate == null ? (object)DBNull.Value : mtp.StartDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@StopDate", mtp.StopDate == null ? (object)DBNull.Value : mtp.StopDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@AnticipatedEndDate", mtp.AnticipatedRestartDate == null ? (object)DBNull.Value : mtp.AnticipatedRestartDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DateLastModified", DateTime.Now));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Adds a new lab mail-to-patient preference to the database.
            /// </summary>
            /// <param name="mtp">Site preference to add.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertUpdateClinicStatus(string clinicsitecode, string labsitecode, string actionrequired, DateTime? effectivedate, [Optional] string comment)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "IULabClinicMailToPatient";
                cmd.Parameters.Add(this.DAL.GetParamenter("@Lab", labsitecode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Clinic", clinicsitecode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Action", actionrequired));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Comment", comment));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", Globals.ModifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DateLastModified", DateTime.Now));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EffectiveDate", effectivedate == null ? (object)DBNull.Value : effectivedate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            protected override object GetObject(IDbCommand cmdIn)
            {
                var dt = DAL.GetData(cmdIn);
                return FillRecord(dt);
            }
        }


        /// <summary>
        /// A custom repository class to do Rx preference operations.
        /// </summary>
        //public class ClinicGroupsPreferencesRepository : RepositoryBase<SitePrefClinicGroupsEntity>
        //{
        //    /// <summary>
        //    /// Default ctor.
        //    /// </summary>
        //    public ClinicGroupsPreferencesRepository()
        //        : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        //    { }

        //    /// <summary>
        //    /// Get Group preferences for a Clinic.
        //    /// </summary>
        //    /// <returns>SitePrefClinicGroupsEntity entity for a site.</returns>
        //    public bool InsertClinicGroup(SitePrefClinicGroupsEntity ClinicGroup)
        //    {
        //        var cmd = this.DAL.GetCommandObject();
        //        cmd.CommandText = "IUClinicOrderGroup";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSite", ClinicGroup.ClinicSite));
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@GroupName", ClinicGroup.GroupName));
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@GroupDesc", ClinicGroup.GroupDesc));
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", ClinicGroup.IsActive));
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
        //        InsertData(cmd);

        //        var p = cmd.Parameters["@Success"] as IDataParameter;
        //        return p.Value.ToInt32().Equals(1);
        //    }

        //    public bool ActivateAllClinicGroups(SitePrefClinicGroupsEntity AllClinicGroups)
        //    {
        //        var cmd = this.DAL.GetCommandObject();
        //        cmd.CommandText = "EnableAllClinicGroups";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", AllClinicGroups.ClinicSite));
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@Activate", AllClinicGroups.IsActive));

        //        cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
        //        InsertData(cmd);

        //        var p = cmd.Parameters["@Success"] as IDataParameter;
        //        return p.Value.ToInt32().Equals(1);
        //    }

        //    protected override SitePrefClinicGroupsEntity FillRecord(IDataReader dr)
        //    {
        //        var d = new SitePrefClinicGroupsEntity();
        //        d.ClinicSite = dr["ClinicSiteCode"].ToString();
        //        d.GroupName = dr["GroupName"].ToString();
        //        d.GroupDesc = dr["GroupDesc"].ToString();
        //        d.IsActive = dr["IsActive"].ToBoolean();

        //        return d;
        //    }

        //    public List<SitePrefClinicGroupsEntity> GetClinicGroupsDefaults(String ClinicSite)
        //    {
        //        var cmd = this.DAL.GetCommandObject();
        //        cmd.CommandText = "GetClinicGrouping";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", ClinicSite));

        //        return GetRecords(cmd).ToList();
        //    }
        //}
        /// <summary>
        ///  A custom repository class to manage lab routing orders preference operations.
        /// </summary>
        public class RoutingOrdersPreferencesRepository : RepositoryBase<SitePrefRoutingOrdersEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public RoutingOrdersPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }


            /// <summary>
            /// Gets the current lab capacity for a site.
            /// </summary>
            /// <param name="siteCode">Site to get current lab capacity for.</param>
            /// <returns>SitePrefRoutingOrdersEntity object.</returns>
            public SitePrefRoutingOrdersEntity GetCurrentLabCapacityBySiteCode(string siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetSitePref_RoutingOrders";
                cmd.Parameters.Add(this.DAL.GetParamenter("@HowMany", "Last"));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                var d = GetRecord(cmd);
                return d;
            }

            /// <summary>
            /// Gets a list of lab capacity history for a site.
            /// </summary>
            /// <param name="siteCode">Site to get lab capacity history for.</param>
            /// <returns>SitePrefRoutingOrdersEntity list.</returns>
            public List<SitePrefRoutingOrdersEntity> GetLabCapacityHistoryBySiteCode(string siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetSitePref_RoutingOrders";
                cmd.Parameters.Add(this.DAL.GetParamenter("@HowMany", "All"));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                return GetRecords(cmd).ToList();
            }


            protected override SitePrefRoutingOrdersEntity FillRecord(System.Data.IDataReader dr)
            {
                var roe = new SitePrefRoutingOrdersEntity();
                roe.SiteCode = dr.AsString("LabSiteCode");
                roe.Capacity = dr.ToInt32("CapacityNbr");
                roe.DateLastModified = dr.ToDateTime("DateLastModified");
                roe.ModifiedBy = dr.AsString("ModifiedBy");
                roe.PDO = dr.ToBoolean("PatientDirectedOrders");
                return roe;
            }

            /// <summary>
            /// Adds a new lab routing orders preference to the database.
            /// </summary>
            /// <param name="o">SitePrefRoutingOrdersEntity to add.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertSitePref_RoutingOrders(SitePrefRoutingOrdersEntity o)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "IUSitePref_LabRoutingOrders";
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", o.SiteCode));
                if (!o.Capacity.IsNull())
                    cmd.Parameters.Add(this.DAL.GetParamenter("@CapacityNbr", o.Capacity));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientDirectedOrders", o.PDO));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", o.ModifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DateLastModified", DateTime.Now));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            protected override object GetObject(IDbCommand cmdIn)
            {
                var dt = DAL.GetData(cmdIn);
                return FillRecord(dt);
            }

        }

        /// <summary>
        /// A custom repository class to do shipping preference operations.
        /// </summary>
        public class ShippingPreferencesRepository : RepositoryBase<Object>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public ShippingPreferencesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Saves the shipping preferences for a site to the database.
            /// </summary>
            /// <param name="SiteCode">Site code to save shipping provider to.</param>
            /// <param name="Shipper">Shipping Provider.</param>
            /// <returns>Success/failure of insert.</returns>
            public Boolean SetShippingPrefToDb(String SiteCode, String Shipper)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "IUSitePref_GeneralShipper";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@Shipper", Shipper));
                cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Gets shipping preferences for site.
            /// </summary>
            /// <param name="SiteCode">Site code to search in.</param>
            /// <returns>Shipping preference</returns>
            public string GetShippingPreferences(String SiteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetDefaultShipper";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return GetObject(cmd).ToString();
            }

            protected override object GetObject(IDbCommand cmdIn)
            {
                var dt = DAL.GetData(cmdIn);
                return FillRecord(dt);
            }

            protected override object FillRecord(DataTable dt)
            {
                if (dt.IsNull()) return false;
                if (dt.Rows.IsNull() || dt.Rows.Count.Equals(0)) return false;
                return dt.Rows[0]["Shipper"];
            }
        }


    }
}


   // }
//}

