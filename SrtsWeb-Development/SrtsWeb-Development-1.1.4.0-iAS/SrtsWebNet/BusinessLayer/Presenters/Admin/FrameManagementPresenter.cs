using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.Admin
{
    public sealed class FrameManagementPresenter
    {
        private IFrameManagement _view;

        public FrameManagementPresenter(IFrameManagement view)
        {
            _view = view;
        }

        public void InitView()
        {
            GetFrames();
            GetFrameItemData();
            GetFrameItemsFromFrameItemData();
            GetPriorities();
        }

        public void GetFrames()
        {
            var r = new FrameRepository();
            _view.FrameData = r.GetAllFrames();
        }
        public void GetFrameItemData()
        {
            var r = new FrameItemsRepository();
            _view.FrameItemInfo = r.GetFrameItems();
        }
        public void GetPriorities()
        {
            var r = new LookupRepository();
            _view.PriorityData = r.GetLookupsByType("OrderPriorityType").Select(x => new OrderPriorityEntity() { OrderPriorityText = x.Text, OrderPriorityValue = x.Value }).ToList();
        }
        public void GetFrameItemsFromFrameItemData()
        {
            if (_view.FrameItemInfo == null || _view.FrameItemInfo.Count.Equals(0)) return;
            // set individual item lists
            _view.TempleData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "temple").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            _view.BridgeData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "bridge").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            _view.ColorData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "color").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            _view.EyeSizeData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "eye").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            _view.LenseTypeData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "lens_type").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            _view.MaterialData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "material").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            _view.TintData = _view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "tint").Select(s => new { Key = s.Text, Value = s.Value }).Distinct().OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
        }

        public void SetSelectedFrameData()
        {
            var sf = _view.FrameCodeSelected;
            var fi = _view.FrameData.Where(x => x.FrameCode == sf).FirstOrDefault();
            if (fi == null) fi = new FrameEntity();
            _view.FrameInfo = fi;
        }
        public void SetSelectedFrameItemData()
        {
            var r = new FrameItemsRepository();
            var fil = r.GetFrameItemByFrameCode(_view.FrameCodeSelected);
            _view.BridgesSelected = fil.Where(x => x.TypeEntry == "BRIDGE").Select(x => x.Value).Distinct().ToList();
            _view.ColorsSelected = fil.Where(x => x.TypeEntry == "COLOR").Select(x => x.Value).Distinct().ToList();
            _view.EyeSizesSelected = fil.Where(x => x.TypeEntry == "EYE").Select(x => x.Value).Distinct().ToList();
            _view.LensTypesSelected = fil.Where(x => x.TypeEntry == "LENS_TYPE").Select(x => x.Value).Distinct().ToList();
            _view.MaterialsSelected = fil.Where(x => x.TypeEntry == "MATERIAL").Select(x => x.Value).Distinct().ToList();
            _view.TemplesSelected = fil.Where(x => x.TypeEntry == "TEMPLE").Select(x => x.Value).Distinct().ToList();
            _view.TintsSelected = fil.Where(x => x.TypeEntry == "TINT").Select(x => x.Value).Distinct().ToList();
        }
        public void SetGenderData()
        {
            var r = new FrameItemsRepository();
            var l = r.GetFrameItemEligibilityByFrameCode(_view.FrameCodeSelected);
            if (l.IsNullOrEmpty()) return;
            var g = l[0].ToString();
            if (String.IsNullOrEmpty(g)) return;
            if (g.Length < 2) return;
            _view.GenderSelected = g[0].ToString();
            //var t = r.GetFrameItemEligibilityByFrameCode(_view.FrameCodeSelected);
            //if (t == null || t.Rows.Count.Equals(0)) return;
            //var g = t.Rows[0].ToString();
            //if (String.IsNullOrEmpty(g)) return;
            //if (g.Length < 2) return;
            //_view.GenderSelected = g[0].ToString();
        }
        public void SetFrameEligibilities()
        {
            if (_view.FrameCodeSelected.ToLower().Equals("add"))
            {
                _view.Eligibilities = null;
                return;
            }

            var r = new FrameRepository();
            _view.Eligibilities = r.GetEligibilityByFrameCode(_view.FrameCodeSelected);
        }
        public void SetPriorityData()
        {
            var r = new FrameRepository();
            _view.PrioritiesSelected = r.GetFrameItemPrioritiesByFrameCode(_view.FrameCodeSelected).ConvertAll(x => x.Substring(1));
        }

        public Dictionary<BOSEntity, Dictionary<StatusEntity, List<RankEntity>>> BuildTableSource()
        {
            var r = new FrameRepository.EligibilityPartRepository();
            //var t = r.GetFrameEligibilityParts();

            var bsg = r.GetFrameEligibilityParts();

            var l = new LookupRepository();
            var bosStatGrades = new Dictionary<BOSEntity, Dictionary<StatusEntity, List<RankEntity>>>();

            var bosBase = l.GetLookupsByType("BOSType");
            var statusBase = l.GetLookupsByType("PatientStatusType");
            var gradeBase = l.GetLookupsByType("RankType");

            foreach (var bl in bsg)
            {
                var b = bosBase.Where(x => x.Value == bl.Key).Select(x => new BOSEntity() { BOSText = x.Text, BOSValue = x.Value }).FirstOrDefault();
                var statGrades = new Dictionary<StatusEntity, List<RankEntity>>();
                foreach (var sl in bl.Value)
                {
                    var s = statusBase.Where(x => x.Value == sl.Key).Select(a => new StatusEntity() { StatusText = a.Text, StatusValue = a.Value }).FirstOrDefault();

                    var g = new List<RankEntity>();
                    foreach (var gl in sl.Value)
                    {
                        g.AddRange(gradeBase.Where(x => x.Value == gl).Select(x => new RankEntity() { RankText = x.Text, RankValue = x.Value }).ToList());
                    }
                    statGrades.Add(s, g);
                }
                bosStatGrades.Add(b, statGrades);
            }

            return bosStatGrades;
        }

        public void InsertFrame()
        {
            var fr = new FrameRepository();
            var fe = _view.FrameInfo;
            fe.DateLastModified = DateTime.Now;
            fe.ModifiedBy = _view.mySession.MyUserID;
            fr.InsertFrame(fe);

            // Get the frame item codes and join then to the frame code
            var fie = new List<FrameItemEntity>();
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "bridge" && _view.BridgesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "color" && _view.ColorsSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "eye" && _view.EyeSizesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "lens_type" && _view.LensTypesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "material" && _view.MaterialsSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "temple" && _view.TemplesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "tint" && _view.TintsSelected.Contains(x.Value)));
            fie.ForEach(x => { x.ModifiedBy = _view.mySession.MyUserID; x.DateLastModified = DateTime.Now; });

            var p = new List<OrderPriorityEntity>();
            p = _view.PriorityData.Where(x => _view.PrioritiesSelected.Contains(x.OrderPriorityValue)).ToList();
            p.ForEach(x => x.OrderPriorityValue = String.Format("{0}{1}", _view.GenderSelected, x.OrderPriorityValue));

            //InsertFrameItemEligibilityAndUnion
            fr.InsertFrameItemsEligibilityAndUnion(fe.FrameCode, fie, p, _view.mySession.MyUserID);

            var e = _view.Eligibilities;
            fr.InsertFrameEligibilityDemographic(fe.FrameCode, e, _view.mySession.MyUserID);
        }
        public void UpdateFrame()
        {
            var fr = new FrameRepository();
            var fe = _view.FrameInfo;
            fe.DateLastModified = DateTime.Now;
            fe.ModifiedBy = _view.mySession.MyUserID;
            fr.UpdateFrame(fe);

            // Get the frame item codes and join then to the frame code
            var fie = new List<FrameItemEntity>();
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "bridge" && _view.BridgesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "color" && _view.ColorsSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "eye" && _view.EyeSizesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "lens_type" && _view.LensTypesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "material" && _view.MaterialsSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "temple" && _view.TemplesSelected.Contains(x.Value)));
            fie.AddRange(_view.FrameItemInfo.Where(x => x.TypeEntry.ToLower() == "tint" && _view.TintsSelected.Contains(x.Value)));
            fie.ForEach(x => { x.ModifiedBy = _view.mySession.MyUserID; x.DateLastModified = DateTime.Now; });

            var p = new List<OrderPriorityEntity>();
            p = _view.PriorityData.Where(x => _view.PrioritiesSelected.Contains(x.OrderPriorityValue)).ToList();
            p.ForEach(x => x.OrderPriorityValue = String.Format("{0}{1}", _view.GenderSelected, x.OrderPriorityValue));

            fr.InsertFrameItemsEligibilityAndUnion(fe.FrameCode, fie, p, _view.mySession.MyUserID);

            var e = _view.Eligibilities;
            fr.InsertFrameEligibilityDemographic(fe.FrameCode, e, _view.mySession.MyUserID);
        }
    }
}