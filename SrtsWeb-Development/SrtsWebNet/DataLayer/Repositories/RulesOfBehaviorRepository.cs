using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to do release note operations.
    /// </summary>
    public class RulesOfBehaviorRepository : RepositoryBase<RuleOfBehavior>, IRulesOfBehaviorRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public RulesOfBehaviorRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets the rules of behavior acceptance date.
        /// </summary>
        /// <param name="UserName">User name to search with.</param>
        /// <returns>Rules of behavior acceptance date</returns>
        public String GetRulesOfBehaviorDate (string UserName)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "GetRulesOfBehaviorDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@UserName", UserName.ToLower()));
            var o = GetObject(cmd);
            return o.IsNull() ? String.Empty : o.ToString();
        }

        /// <summary>
        /// Adds rules of behavior date parameter to the database.
        /// </summary>
        /// <param name="username">Username to add date for.</param>
        /// <param name="useracceptancedate">User acceptance date</param>
        /// <returns>Success/failure of insert.</returns>
        public bool InsertUpdateRulesOfBehaviorDate(string username, DateTime useracceptancedate)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "IURulesOfBehavior";
            cmd.Parameters.Add(this.DAL.GetParamenter("@UserName", username));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ROBDate", useracceptancedate));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1);
        }
                

        /// <summary>
        /// Gets the list of rules of behavior.
        /// </summary>
        /// <returns></returns>
        public List<RuleOfBehavior> GetRulesOfBehavior()
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRulesOfBehavior";

            return GetRecords(cmd).ToList();
        }

        protected override RuleOfBehavior FillRecord(IDataReader dr)
        {
            var n = new RuleOfBehavior()
            {
                ID = dr.ToInt32("ID"),
                RuleBehavior = dr.AsString("RuleOfBehavior")
            };

            return n;
        }
    }
}