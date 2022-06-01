using SrtsWeb.Entities;

namespace SrtsWeb.Views.JSpecs
{
    public interface IJSpecsLoginView
    {
        JSpecsSession userInfo { get; set; }
        string ErrorMessage { set; }
        string ClinicCode { get; set; }
    }
}
