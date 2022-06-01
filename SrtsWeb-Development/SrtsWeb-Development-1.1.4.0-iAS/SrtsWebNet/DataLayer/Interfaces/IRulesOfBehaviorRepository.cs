using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IRulesOfBehaviorRepository
    {
        List<RuleOfBehavior> GetRulesOfBehavior();
    }
}