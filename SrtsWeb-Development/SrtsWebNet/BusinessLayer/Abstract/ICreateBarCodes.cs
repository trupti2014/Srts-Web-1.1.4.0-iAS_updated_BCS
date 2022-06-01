using System.Drawing;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface ICreateBarCodes
    {
        Bitmap CreateBarCode(string data);
    }
}