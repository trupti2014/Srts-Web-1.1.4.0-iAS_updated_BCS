using System.Drawing;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IGenBarCodes
    {
        Image GenerateBarCode(string data);
    }
}