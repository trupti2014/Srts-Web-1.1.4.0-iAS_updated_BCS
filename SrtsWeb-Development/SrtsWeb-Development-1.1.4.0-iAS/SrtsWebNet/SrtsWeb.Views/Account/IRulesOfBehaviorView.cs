using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Account
{
    public interface IRulesOfBehaviorView
    {
        List<RuleOfBehavior> RulesOfBehaviorList { get; set; }
    }
}