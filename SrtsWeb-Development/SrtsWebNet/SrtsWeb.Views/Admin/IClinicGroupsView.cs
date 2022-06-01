using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface IClinicGroupsView
    {
        SRTSSession mySession { get; }
        
        //List<SitePrefClinicGroupsEntity> ClinicGroupsList { get; set; }

        //SitePrefClinicGroupsEntity ClinicGroupEntity { get; set; }

        //String ClinicSite { get; set; }
        String GroupName { get; set; }
        String GroupDesc { get; set; }
        Boolean IsActive { get; set; }
    }
}
