using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;
using SrtsWeb.BusinessLayer.Views.Shared;

namespace SrtsWeb.BusinessLayer.Views.Patients
{
    public interface IBmtView : IPersonnelMilitaryData, IAddress
    {
        List<String> ColumnList { get; set; }
        String SelectedAddressType { get; set; }
        List<BmtEntity> BasicMilitaryTrainee { get; set; }
    }
}
