using SrtsWeb.DataLayer.Repositories;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public class ProfileServices
    {
        public List<String> GetAspnetUserNamesByIndividualId(Int32 IndividualId)
        {
            var r = new MembershipRepository();
            return r.GetAspnetUserNamesByIndividualId(IndividualId);
        }
    }
}