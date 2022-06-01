using SrtsWeb.BusinessLayer.Abstract;
using System.Drawing;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public sealed class CreateBarCodes : ICreateBarCodes
    {
        public Bitmap CreateBarCode(string data)
        {
            Bitmap barCode = new Bitmap(1, 1);

            Font Wasp39M = new Font("Wasp 39 M", 60,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point);

            Graphics oGraphics = Graphics.FromImage(barCode);

            SizeF dataSize = oGraphics.MeasureString(data, Wasp39M);

            barCode = new Bitmap(barCode, dataSize.ToSize());

            oGraphics.Clear(System.Drawing.Color.White);

            oGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            oGraphics.DrawString(data, Wasp39M, new SolidBrush(System.Drawing.Color.Black), 0, 0);

            oGraphics.Flush();

            Wasp39M.Dispose();
            oGraphics.Dispose();

            return barCode;
        }
    }
}