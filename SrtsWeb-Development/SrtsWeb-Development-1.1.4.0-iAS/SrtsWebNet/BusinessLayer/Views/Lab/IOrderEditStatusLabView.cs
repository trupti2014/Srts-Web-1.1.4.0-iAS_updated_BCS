using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Lab
{
    public interface IOrderEditStatusLabView
    {
        SRTSSession mySession { get; set; }

        DataTable LookupCache { get; set; }

        DataTable PriorityData { get; set; }

        List<SiteCodeEntity> LabData { get; set; }

        int AddressID { get; set; }

        int Pairs { get; set; }

        int Cases { get; set; }

        string Address1 { get; set; }

        string Address2 { get; set; }

        string AddressType { get; set; }

        string Bridge { get; set; }

        string City { get; set; }

        string Country { get; set; }

        string Color { get; set; }

        string Eye { get; set; }

        string Frame { get; set; }

        string Lens { get; set; }

        string Material { get; set; }

        string ODSegHeight { get; set; }

        string OSSegHeight { get; set; }

        string AddressState { get; set; }

        string Temple { get; set; }

        string Tint { get; set; }

        string ZipCode { get; set; }

        string PrioritySelected { get; set; }

        string ODSphere { get; set; }

        string ODCylinder { get; set; }

        string ODAxis { get; set; }

        string ODHPrism { get; set; }

        string ODHBase { get; set; }

        string ODVPrism { get; set; }

        string ODVBase { get; set; }

        string ODAdd { get; set; }

        string OSSphere { get; set; }

        string OSCylinder { get; set; }

        string OSAxis { get; set; }

        string OSHPrism { get; set; }

        string OSHBase { get; set; }

        string OSVPrism { get; set; }

        string OSVBase { get; set; }

        string OSAdd { get; set; }

        string PDOD { get; set; }

        string PDOS { get; set; }

        string PDTotal { get; set; }

        string PDODNear { get; set; }

        string PDOSNear { get; set; }

        string PDTotalNear { get; set; }

        string OHComment { get; set; }

        string LabSelected { get; set; }

        string Location { get; set; }

        string FOCDate { get; set; }

        string Message { get; set; }

        string StatusSelected { get; set; }

        string JustificationInfo { get; set; }

        bool RequiresJustification { get; set; }

        bool IsMultiVision { get; set; }
    }
}