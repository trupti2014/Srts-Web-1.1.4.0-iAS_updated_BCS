using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Patients
{
    public interface IBmtView
    {
        SRTSSession mySession { get; set; }

        List<BmtEntity> BasicMilitaryTrainee { get; set; }
    }
}