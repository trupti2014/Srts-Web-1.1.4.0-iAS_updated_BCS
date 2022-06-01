using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class ManageFramePresenter
    {

        private IManageFramesView _view;
        private FrameRepository frameRepository;
        private FrameImageRepository frameImageRepository;
        private FrameFamilyRepository frameFamilyRepository;
        private FrameItemsRepository frameItemsRepository;
        private FrameItemsDefaultsRepository itemsDefaultsRepository;

        public ManageFramePresenter(IManageFramesView view)
        {
            _view = view;
        }

        public void GetFrameFamily()
        {
            frameFamilyRepository = new FrameFamilyRepository();
            this._view.FrameFamilyList = frameFamilyRepository.GetFrameFamily();
        }

        public void GetFrameItemsList()
        {
            frameItemsRepository = new FrameItemsRepository();
            var l = frameItemsRepository.GetFrameItemByFrameCode(this._view.Frame);

            this._view.ColorList = l.Where(x => x.TypeEntry.ToLower() == "color").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            this._view.EyeList = l.Where(x => x.TypeEntry.ToLower() == "eye").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            this._view.BridgeList = l.Where(x => x.TypeEntry.ToLower() == "bridge").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            this._view.TempleList = l.Where(x => x.TypeEntry.ToLower() == "temple").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            
        }

        public void GetGlobalDefaultList()
        {
            itemsDefaultsRepository = new FrameItemsDefaultsRepository();
            this._view.FrameItemDefault = itemsDefaultsRepository.GetFrameItemsDefaults(this._view.Frame);
        }

        public void GetImageAngle()
        {
            var lookupRepository = new LookupRepository();
            _view.ImageAngleList = lookupRepository.GetLookupsByType("ImgAngle").Select(x => new FrameImageImageAngleEntity() { ImageAngleText = x.Text, ImageAngleValue = x.Value }).ToList();
        }

        public void GetFramesByFrameFamily(int FamilyID)
        {
            frameImageRepository = new FrameImageRepository();
            this._view.FrameList = frameImageRepository.GetFramesByFrameFamily(FamilyID);
        }

        public Boolean InsertUpdateFrameImage(FrameImageEntity ImageEntity)
        {
            frameImageRepository = new FrameImageRepository();

            return frameImageRepository.InsertUpdateFrameImage(ImageEntity);

        }

        public void SetDefaultsForFrame()
        {
            //var p = this._view.FrameImageItem.Clone();
            var entity = new FrameImageEntity();

            // BRIDGE
            this._view.Bridge = !Convert.ToString(entity.BridgeSize).IsNullOrEmpty() ? Convert.ToString(entity.BridgeSize) : !this._view.FrameItemDefault.DefaultBridge.IsNullOrEmpty() ? this._view.FrameItemDefault.DefaultBridge : "G";

            // COLOR - Global BLK
            this._view.Color = !entity.Color.IsNullOrEmpty() ? entity.Color : this._view.ColorList.Any(x => x.Value == "BLK") ? "BLK" : "G";

            // EYE
            this._view.Eye = !Convert.ToString(entity.EyeSize).IsNullOrEmpty() ? Convert.ToString(entity.EyeSize) : !this._view.FrameItemDefault.DefaultEyeSize.IsNullOrEmpty() ? this._view.FrameItemDefault.DefaultEyeSize : "G";

            // TEMPLE
            this._view.Temple = !entity.Temple.IsNullOrEmpty() ? entity.Temple : !this._view.FrameItemDefault.DefaultTemple.IsNullOrEmpty() ? this._view.FrameItemDefault.DefaultTemple : "G";


        }


    }
}
