using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IUserGuidesRepository
    {
        bool InsertUpdateUserGuide(ReleaseManagementUserGuideEntity param);

        bool DeleteUserGuide(string guidename);

        List<ReleaseManagementUserGuideEntity> GetAllUserGuides();
    }
}
