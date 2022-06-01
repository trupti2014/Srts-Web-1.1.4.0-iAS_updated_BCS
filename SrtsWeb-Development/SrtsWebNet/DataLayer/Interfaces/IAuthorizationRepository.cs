using System;
using System.Data;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IAuthorizationRepository
    {
        DataTable GetAuthorizationsByUserName(string srtUserName);

        DataTable GetAuthorizationsByCAC_ID(string CAC_ID, string issuerName);

        DataTable UpdateAuthorizationCacInfoByUserName(string strUserName, string CAC_ID, string issuerName);

        DataTable InsertAuthorizationByUserName(string strUserName);

        DataTable UpdateAuthorizationSSOByUserName(string strUserName, string strSsoUserName);

        Boolean DeleteAuthorizationByUserName(string strUserName);
    }
}