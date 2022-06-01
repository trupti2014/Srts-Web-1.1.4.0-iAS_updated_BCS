using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ITheaterCodeRepository
    {
        List<TheaterLocationCodeEntity> GetActiveTheaterCodes();
    }
}