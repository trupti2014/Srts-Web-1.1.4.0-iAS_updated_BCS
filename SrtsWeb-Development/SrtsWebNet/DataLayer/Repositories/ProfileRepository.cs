using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web.Security;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class use for custom user profile operations.
    /// </summary>
    public class ProfileRepository : RepositoryBase<UserProfile>, IProfileRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public ProfileRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets a users profile by user name.
        /// </summary>
        /// <param name="userName">User name to search with.</param>
        /// <returns>Users profile.</returns>
        public UserProfile GetProfile(string userName)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "GetProfileInfo";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@LowerUserName", userName, System.Data.ParameterDirection.Input));
            return GetRecord(cmd);
        }

        protected override UserProfile FillRecord(System.Data.IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var p = new UserProfile();

            p.IndividualId = dr.ToInt32("IndividualID", c);
            p.LastName = dr.AsString("LastName", c);
            p.MiddleName = dr.AsString("MiddleName", c);
            p.FirstName = dr.AsString("FirstName", c);
            p.PrimarySite = dr.AsString("PrimarySiteCode", c);
            p.SiteCode = dr.AsString("LoggedInSiteCode", c);
            p.UserName = dr.AsString("LowerUserName", c);
            p.IsCmsUser = dr.ToBoolean("IsCMSUser", c);
            var a = dr.AsString("AvailableSiteList", c);
            var al = a.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var asl = new List<ProfileSiteEntity>();
            try
            {
                al.ForEach(x =>
                    {
                        var i = x.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        asl.Add(new ProfileSiteEntity() { SiteCode = i[0], Approved = i[1].Equals("1") });
                    }
                );
                p.AvailableSiteList = asl;

            }
            catch (Exception ex)
            {
                Exception exx = new Exception();
                exx.LogException("In ProfileRepository - UserProfile FillRecord - " + ex.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exx);
            }
            return p;
        }

        /// <summary>
        /// Saves the currently logged in site code for a user.
        /// </summary>
        /// <param name="userName">User name for profile to modify.</param>
        /// <param name="siteCode">Site code to add to profile.</param>
        public void SaveLoggedInSite(String userName, String siteCode)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "UpdateLoggedInSite";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@LoweredUserName", userName, System.Data.ParameterDirection.Input));
            cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode, System.Data.ParameterDirection.Input));
            UpdateData(cmd);
        }

        /// <summary>
        /// Saves the primary site code for a user.  This is associated with their primary duty station.
        /// </summary>
        /// <param name="profile">Profile to update.</param>
        public void SavePrimarySite(UserProfile profile)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "InsertProfilePrimarySiteUnion";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@lowerusername", profile.UserName.ToLower(), System.Data.ParameterDirection.Input));
            cmd.Parameters.Add(DAL.GetParamenter("@IndividualID", profile.IndividualId, System.Data.ParameterDirection.Input));
            cmd.Parameters.Add(DAL.GetParamenter("@IsCMSUser", profile.IsCmsUser, System.Data.ParameterDirection.Input));
            cmd.Parameters.Add(DAL.GetParamenter("@PrimarySiteCode", profile.PrimarySite, System.Data.ParameterDirection.Input));
            UpdateData(cmd);
        }

        /// <summary>
        /// Saves the sites that a user is capable of logging in to once they are approved by a site admin.
        /// </summary>
        /// <param name="userName">User name for profile to modify.</param>
        /// <param name="availableSites">List of available sites to log in to.</param>
        public void SaveAvailableSites(string userName, List<ProfileSiteEntity> availableSites)
        {
            availableSites.ForEach(x =>
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "InsertProfileUserAssignedSites";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@LowerUserName", userName, System.Data.ParameterDirection.Input));
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", x.SiteCode, System.Data.ParameterDirection.Input));
                cmd.Parameters.Add(DAL.GetParamenter("@Approved", x.Approved, System.Data.ParameterDirection.Input));
                UpdateData(cmd);
            });
        }

        /// <summary>
        /// Deletes sites from a users available site list in the profile.
        /// </summary>
        /// <param name="userName">User name for profile to modify.</param>
        /// <param name="removeSites">List of sites to delete..</param>
        public void DeleteAvailableSite(String userName, List<ProfileSiteEntity> removeSites)
        {
            removeSites.ForEach(x => {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "DeleteProfile_UserAssignedSites";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@LowerUser", userName.ToLower()));
                cmd.Parameters.Add(DAL.GetParamenter("@AssignedSiteCode", x.SiteCode));
                DeleteData(cmd);
            });
        }
    }
}
