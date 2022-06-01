using SrtsWeb.Entities;

namespace SrtsWeb.DataLayer.Repositories
{
    public interface ICustomErrorRepository
    {
        void InsertError(CustomErrorEntity _error);
    }
}