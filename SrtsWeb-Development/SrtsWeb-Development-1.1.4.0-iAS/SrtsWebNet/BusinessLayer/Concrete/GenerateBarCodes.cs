using BarcodeLib;
using SrtsWeb.BusinessLayer.Abstract;
using System;
using System.Drawing;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used to generate barcode images.
    /// </summary>
    public sealed class GenerateBarCodes : IGenBarCodes
    {
        private IBarcodeLib b;

        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="barcodeLib">Instance of BarcodeLib.dll</param>
        public GenerateBarCodes(IBarcodeLib barcodeLib)
        {
            this.b = barcodeLib;
        }

        /// <summary>
        /// Creates and returns a Code39 barcode image.
        /// </summary>
        /// <param name="data">Base 64 formatted string of barcode data.</param>
        /// <returns>Barcode image.</returns>
        public Image GenerateBarCode(string data)
        {
            Image barcodeImage = null;
            if (data != null)
            {
                string strData = data;
                int imageHeight = Convert.ToInt32(50);
                int imageWidth = Convert.ToInt32(300);
                string Forecolor = "";
                string Backcolor = "";

                Bitmap barCode = new Bitmap(50, 300);

                TYPE type = TYPE.UNSPECIFIED;
                type = TYPE.CODE39Extended;

                try
                {
                    if (type != TYPE.UNSPECIFIED)
                    {
                        if (Forecolor.Trim() == "" && Forecolor.Trim().Length != 6)
                            Forecolor = "000000";
                        if (Backcolor.Trim() == "" && Backcolor.Trim().Length != 6)
                            Backcolor = "FFFFFF";

                        barcodeImage = b.Encode(type, strData.Trim(), ColorTranslator.FromHtml("#" + Forecolor), ColorTranslator.FromHtml("#" + Backcolor), imageWidth, imageHeight);
                    }
                }
                catch { }
                barCode.Dispose();
            }
            return barcodeImage;
        }
    }
}