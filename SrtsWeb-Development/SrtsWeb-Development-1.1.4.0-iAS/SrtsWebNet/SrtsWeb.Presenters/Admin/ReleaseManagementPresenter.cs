using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SrtsWeb.Presenters.Admin
{
    public class ReleaseManagementPresenter : IDisposable
    {
        private IReleaseManagementView v;
        private Boolean disposed = false;
        public ReleaseManagementPresenter(IReleaseManagementView view)
        {
            this.v = view;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if (v != null)
                    v = null;
            }
            disposed = true;
        }

        public class UserGuidesPresenter : IDisposable
        {
            private Boolean disposed = false;
            private IReleaseManagementUserGuidesView v;
            private UserGuidesRepository r;

            public UserGuidesPresenter()
            {
                
            }

            public UserGuidesPresenter(IReleaseManagementUserGuidesView view)
            {
                this.v = view;
            }

            public void InitView()
            {
                FillUserGuides();
            }

            public void FillUserGuides()
            {
                r = new UserGuidesRepository();
                v.UserGuideData = r.GetAllUserGuides();
            }

            public static List<ReleaseManagementUserGuideEntity> GetAllGuides()
            {
                var r = new UserGuidesRepository();
                return r.GetAllUserGuides();
            }

            public ReleaseManagementUserGuideEntity GetUserGuide(string userGuideName)
            {
                r = new UserGuidesRepository();
                return r.GetUserGuideByName(userGuideName);

            }

            public bool InsertUpdateUserGuide(ReleaseManagementUserGuideEntity uge)
            {
                r = new UserGuidesRepository();
                return r.InsertUpdateUserGuide(uge);
            }

            public void DeleteUserGuide(string userGuideName)
            {
                r = new UserGuidesRepository();
                r.DeleteUserGuide(userGuideName);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (disposed) return;
                if (disposing)
                {
                    if (v != null)
                        v = null;
                }
                disposed = true;
            }
        }



    }
}