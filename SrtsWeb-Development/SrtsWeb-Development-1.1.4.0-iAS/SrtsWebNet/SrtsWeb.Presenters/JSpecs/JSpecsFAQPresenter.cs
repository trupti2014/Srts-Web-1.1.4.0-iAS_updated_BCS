using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.JSpecs;
using System.Linq;

namespace SrtsWeb.Presenters.JSpecs
{
    public sealed class JSpecsFAQPresenter
    {
        private IJSpecsFAQView _view;

        public JSpecsFAQPresenter(IJSpecsFAQView view)
        {
            _view = view;
        }

        public void InitView()
        {

        }
    }
}
