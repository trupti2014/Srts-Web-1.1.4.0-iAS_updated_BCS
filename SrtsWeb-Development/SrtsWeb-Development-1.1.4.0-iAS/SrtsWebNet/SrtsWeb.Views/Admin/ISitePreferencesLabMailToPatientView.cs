using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISitePreferencesLabMailToPatientView
    {
        SRTSSession mySession { get; }

        IEnumerable<SiteCodeEntity> Clinics { get; set; }

        IEnumerable<SitePrefLabMTPEntity> DisabledClinics { get; set; }

        IEnumerable<SitePrefLabMTPEntity> EnabledClinics { get; set; }

        SitePrefLabMTPEntity SitePrefLabMTPEntity { get; set; }

        List<EmailMessageEntity> EmailNotification { get; set; }
    }
}

