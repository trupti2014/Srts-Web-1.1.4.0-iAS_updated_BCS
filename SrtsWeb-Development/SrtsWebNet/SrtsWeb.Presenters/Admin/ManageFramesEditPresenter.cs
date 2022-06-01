using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class ManageFramesEditPresenter
    {
        private IManageFramesEditView _view;

        private FrameImageRepository frameImageRepository;
        private FrameFamilyRepository frameFamilyRepository;

        public ManageFramesEditPresenter(IManageFramesEditView view)
        {
            _view = view;
        }

        #region GET FUNCTIONS

        /// <summary>
        /// Get Frame Image Records
        /// </summary>
        /// <param name="Frame"></param>
        /// <param name="Family"></param>
        /// <returns></returns>
        public bool GetAllFrames(String Frame, String Family)
        {
            var repository = new FrameImageRepository();
            var records = repository.GetFrameImage(Frame, Family);

            if (records == null || records.Count.Equals(0))
            {
                return false;
            }
            else
            {
                this._view.FrameImageRecords = records;
                return true;
            }
        }
        
        /// <summary>
        /// Get a list of Frame Families
        /// </summary>
        public void GetFrameFamily()
        {
            frameFamilyRepository = new FrameFamilyRepository();
            this._view.FrameFamilyList = frameFamilyRepository.GetFrameFamily();
        }

        /// <summary>
        /// Get Frame by FrameFamilyID
        /// </summary>
        /// <param name="FamilyID"></param>
        public void GetFramesByFrameFamily(int FamilyID)
        {
            frameImageRepository = new FrameImageRepository();
            this._view.FrameList = frameImageRepository.GetFramesByFrameFamily(FamilyID);
        }

        #endregion

        #region UPDATE FUNCTION

        /// <summary>
        /// Update Frame Image by ID
        /// </summary>
        /// <param name="ImageEntity"></param>
        /// <returns></returns>
        public Boolean UpdateFrameImage(FrameImageEntity ImageEntity)
        {
            frameImageRepository = new FrameImageRepository();

            return frameImageRepository.UpdateFrameImage(ImageEntity);

        }

        #endregion

    }
}
