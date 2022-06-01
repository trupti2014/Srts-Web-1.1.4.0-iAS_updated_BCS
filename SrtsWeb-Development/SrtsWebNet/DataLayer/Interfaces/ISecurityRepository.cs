using System.Data;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ISecurityRepository
    {
        DataTable ValidateUser(string CACSubject);

        DataTable ValidateUser(string Username, string Password);
    }
}