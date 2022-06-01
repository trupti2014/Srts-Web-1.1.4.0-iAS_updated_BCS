using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.DataLayer.Repositories
{
    public interface INostraRepository
    {
        DataTable GetNostraFileData(String _siteCode);

        Boolean SetNostraCsvData(List<NostraFileEntity> entities);
    }
}