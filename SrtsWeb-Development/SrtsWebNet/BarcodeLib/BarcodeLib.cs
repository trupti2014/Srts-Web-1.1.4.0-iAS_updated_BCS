using BarcodeLib.Symbologies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace BarcodeLib
{
    #region Enums

    public enum TYPE : int { UNSPECIFIED, UPCA, UPCE, UPC_SUPPLEMENTAL_2DIGIT, UPC_SUPPLEMENTAL_5DIGIT, EAN13, EAN8, Interleaved2of5, Standard2of5, Industrial2of5, CODE39, CODE39Extended, Codabar, PostNet, BOOKLAND, ISBN, JAN13, MSI_Mod10, MSI_2Mod10, MSI_Mod11, MSI_Mod11_Mod10, Modified_Plessey, CODE11, USD8, UCC12, UCC13, LOGMARS, CODE128, CODE128A, CODE128B, CODE128C, ITF14, CODE93, TELEPEN, FIM, PHARMACODE };

    public enum SaveTypes : int { JPG, BMP, PNG, GIF, TIFF, UNSPECIFIED };

    public enum AlignmentPositions : int { CENTER, LEFT, RIGHT };

    public enum LabelPositions : int { TOPLEFT, TOPCENTER, TOPRIGHT, BOTTOMLEFT, BOTTOMCENTER, BOTTOMRIGHT };

    #endregion Enums

    public sealed class Barcode : IDisposable
    {
        #region Variables

        private IBarcode ibarcode = new BarcodeLib.Symbologies.Blank();
        private string Raw_Data = "";
        private string Encoded_Value = "";
        private string _Country_Assigning_Manufacturer_Code = "N/A";
        private TYPE Encoded_Type = TYPE.UNSPECIFIED;
        private Image _Encoded_Image = null;
        private Color _ForeColor = Color.Black;
        private Color _BackColor = Color.White;
        private int _Width = 300;
        private int _Height = 150;
        private string _XML = "";
        private ImageFormat _ImageFormat = ImageFormat.Jpeg;
        private Font _LabelFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

        private LabelPositions _LabelPosition = LabelPositions.BOTTOMCENTER;

        private RotateFlipType _RotateFlipType = RotateFlipType.RotateNoneFlipNone;

        #endregion Variables

        #region Constructors

        public Barcode()
        {
        }

        public Barcode(string data)
        {
            this.Raw_Data = data;
        }

        public Barcode(string data, TYPE iType)
        {
            this.Raw_Data = data;
            this.Encoded_Type = iType;
        }

        #endregion Constructors

        #region Properties

        public string RawData
        {
            get { return Raw_Data; }
            set { Raw_Data = value; }
        }

        public string EncodedValue
        {
            get { return Encoded_Value; }
        }

        public string Country_Assigning_Manufacturer_Code
        {
            get { return _Country_Assigning_Manufacturer_Code; }
        }

        public TYPE EncodedType
        {
            set { Encoded_Type = value; }
            get { return Encoded_Type; }
        }

        public Image EncodedImage
        {
            get
            {
                return _Encoded_Image;
            }
        }

        public Color ForeColor
        {
            get { return this._ForeColor; }
            set { this._ForeColor = value; }
        }

        public Color BackColor
        {
            get { return this._BackColor; }
            set { this._BackColor = value; }
        }

        public Font LabelFont
        {
            get { return this._LabelFont; }
            set { this._LabelFont = value; }
        }

        public LabelPositions LabelPosition
        {
            get { return _LabelPosition; }
            set { _LabelPosition = value; }
        }

        public RotateFlipType RotateFlipType
        {
            get { return _RotateFlipType; }
            set { _RotateFlipType = value; }
        }

        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public bool IncludeLabel
        {
            get;
            set;
        }

        public double EncodingTime
        {
            get;
            set;
        }

        public string XML
        {
            get { return _XML; }
        }

        public ImageFormat ImageFormat
        {
            get { return _ImageFormat; }
            set { _ImageFormat = value; }
        }

        public List<string> Errors
        {
            get { return this.ibarcode.Errors; }
        }

        public AlignmentPositions Alignment
        {
            get;
            set;
        }

        public byte[] Encoded_Image_Bytes
        {
            get
            {
                if (_Encoded_Image == null)
                    return null;

                using (MemoryStream ms = new MemoryStream())
                {
                    _Encoded_Image.Save(ms, _ImageFormat);
                    return ms.ToArray();
                }
            }
        }

        public static Version Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }

        #endregion Properties

        #region Functions

        #region General Encode

        public Image Encode(TYPE iType, string StringToEncode, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            return Encode(iType, StringToEncode);
        }

        public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            return Encode(iType, StringToEncode, ForeColor, BackColor);
        }

        public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor)
        {
            this.BackColor = BackColor;
            this.ForeColor = ForeColor;
            return Encode(iType, StringToEncode);
        }

        public Image Encode(TYPE iType, string StringToEncode)
        {
            Raw_Data = StringToEncode;
            return Encode(iType);
        }

        internal Image Encode(TYPE iType)
        {
            Encoded_Type = iType;
            return Encode();
        }

        internal Image Encode()
        {
            ibarcode.Errors.Clear();

            DateTime dtStartTime = DateTime.Now;

            if (Raw_Data.Trim() == "")
                throw new Exception("EENCODE-1: Input data not allowed to be blank.");

            if (this.EncodedType == TYPE.UNSPECIFIED)
                throw new Exception("EENCODE-2: Symbology type not allowed to be unspecified.");

            this.Encoded_Value = "";
            this._Country_Assigning_Manufacturer_Code = "N/A";

            switch (this.Encoded_Type)
            {
                case TYPE.UCC12:
                case TYPE.UPCA:

                    ibarcode = new UPCA(Raw_Data);
                    break;

                case TYPE.UCC13:
                case TYPE.EAN13:

                    ibarcode = new EAN13(Raw_Data);
                    break;

                case TYPE.Interleaved2of5:

                    ibarcode = new Interleaved2of5(Raw_Data);
                    break;

                case TYPE.Industrial2of5:
                case TYPE.Standard2of5:

                    ibarcode = new Standard2of5(Raw_Data);
                    break;

                case TYPE.LOGMARS:
                case TYPE.CODE39:

                    ibarcode = new Code39(Raw_Data);
                    break;

                case TYPE.CODE39Extended:
                    ibarcode = new Code39(Raw_Data, true);
                    break;

                case TYPE.Codabar:

                    ibarcode = new Codabar(Raw_Data);
                    break;

                case TYPE.PostNet:

                    ibarcode = new Postnet(Raw_Data);
                    break;

                case TYPE.ISBN:
                case TYPE.BOOKLAND:

                    ibarcode = new ISBN(Raw_Data);
                    break;

                case TYPE.JAN13:

                    ibarcode = new JAN13(Raw_Data);
                    break;

                case TYPE.UPC_SUPPLEMENTAL_2DIGIT:

                    ibarcode = new UPCSupplement2(Raw_Data);
                    break;

                case TYPE.MSI_Mod10:
                case TYPE.MSI_2Mod10:
                case TYPE.MSI_Mod11:
                case TYPE.MSI_Mod11_Mod10:
                case TYPE.Modified_Plessey:

                    ibarcode = new MSI(Raw_Data, Encoded_Type);
                    break;

                case TYPE.UPC_SUPPLEMENTAL_5DIGIT:

                    ibarcode = new UPCSupplement5(Raw_Data);
                    break;

                case TYPE.UPCE:

                    ibarcode = new UPCE(Raw_Data);
                    break;

                case TYPE.EAN8:

                    ibarcode = new EAN8(Raw_Data);
                    break;

                case TYPE.USD8:
                case TYPE.CODE11:

                    ibarcode = new Code11(Raw_Data);
                    break;

                case TYPE.CODE128:

                    ibarcode = new Code128(Raw_Data);
                    break;

                case TYPE.CODE128A:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.A);
                    break;

                case TYPE.CODE128B:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.B);
                    break;

                case TYPE.CODE128C:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.C);
                    break;

                case TYPE.ITF14:
                    ibarcode = new ITF14(Raw_Data);
                    break;

                case TYPE.CODE93:
                    ibarcode = new Code93(Raw_Data);
                    break;

                case TYPE.TELEPEN:
                    ibarcode = new Telepen(Raw_Data);
                    break;

                case TYPE.FIM:
                    ibarcode = new FIM(Raw_Data);
                    break;

                case TYPE.PHARMACODE:
                    ibarcode = new Pharmacode(Raw_Data);
                    break;

                default: throw new Exception("EENCODE-2: Unsupported encoding type specified.");
            }

            this.Encoded_Value = ibarcode.Encoded_Value;
            this.Raw_Data = ibarcode.RawData;

            _Encoded_Image = (Image)Generate_Image();

            this.EncodedImage.RotateFlip(this.RotateFlipType);

            _XML = GetXML();

            this.EncodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            return EncodedImage;
        }

        #endregion General Encode

        #region Image Functions

        private Bitmap Generate_Image()
        {
            if (Encoded_Value == "") throw new Exception("EGENERATE_IMAGE-1: Must be encoded first.");
            Bitmap b = null;

            DateTime dtStartTime = DateTime.Now;

            switch (this.Encoded_Type)
            {
                case TYPE.ITF14:
                    {
                        b = new Bitmap(Width, Height);

                        int bearerwidth = (int)((b.Width) / 12.05);
                        int iquietzone = Convert.ToInt32(b.Width * 0.05);
                        int iBarWidth = (b.Width - (bearerwidth * 2) - (iquietzone * 2)) / Encoded_Value.Length;
                        int shiftAdjustment = ((b.Width - (bearerwidth * 2) - (iquietzone * 2)) % Encoded_Value.Length) / 2;

                        if (iBarWidth <= 0 || iquietzone <= 0)
                            throw new Exception("EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");

                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.Clear(BackColor);

                            using (Pen pen = new Pen(ForeColor, iBarWidth))
                            {
                                pen.Alignment = PenAlignment.Right;

                                while (pos < Encoded_Value.Length)
                                {
                                    if (Encoded_Value[pos] == '1')
                                        g.DrawLine(pen, new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height));

                                    pos++;
                                }

                                pen.Width = (float)b.Height / 8;
                                pen.Color = ForeColor;
                                pen.Alignment = PenAlignment.Center;
                                g.DrawLine(pen, new Point(0, 0), new Point(b.Width, 0));

                                g.DrawLine(pen, new Point(0, b.Height), new Point(b.Width, b.Height));

                                g.DrawLine(pen, new Point(0, 0), new Point(0, b.Height));

                                g.DrawLine(pen, new Point(b.Width, 0), new Point(b.Width, b.Height));
                            }
                        }

                        if (IncludeLabel)
                            Label_ITF14((Image)b);

                        break;
                    }

                default:
                    {
                        b = new Bitmap(Width, Height);
                        int iBarWidth = Width / Encoded_Value.Length;
                        int shiftAdjustment = 0;
                        int iBarWidthModifier = 1;

                        if (this.Encoded_Type == TYPE.PostNet)
                            iBarWidthModifier = 2;

                        switch (Alignment)
                        {
                            case AlignmentPositions.CENTER: shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;

                            case AlignmentPositions.LEFT: shiftAdjustment = 0;
                                break;

                            case AlignmentPositions.RIGHT: shiftAdjustment = (Width % Encoded_Value.Length);
                                break;

                            default: shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }

                        if (iBarWidth <= 0)
                            throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.Clear(BackColor);

                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (this.Encoded_Type == TYPE.PostNet)
                                        {
                                            if (Encoded_Value[pos] != '1')
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + 1, Height), new Point(pos * iBarWidth + shiftAdjustment + 1, Height / 2));

                                            g.DrawLine(backpen, new Point(pos * (iBarWidth * iBarWidthModifier) + shiftAdjustment + iBarWidth + 1, 0), new Point(pos * (iBarWidth * iBarWidthModifier) + shiftAdjustment + iBarWidth + 1, Height));
                                        }

                                        if (Encoded_Value[pos] == '1')
                                            g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + (int)(iBarWidth * 0.5), 0), new Point(pos * iBarWidth + shiftAdjustment + (int)(iBarWidth * 0.5), Height));

                                        pos++;
                                    }
                                }
                            }
                        }

                        if (IncludeLabel)
                        {
                            Label_Generic((Image)b);
                        }

                        break;
                    }
            }

            _Encoded_Image = (Image)b;

            this.EncodingTime += ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            return b;
        }

        public byte[] GetImageData(SaveTypes savetype)
        {
            byte[] imageData = null;

            try
            {
                if (_Encoded_Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SaveImage(ms, savetype);
                        imageData = ms.ToArray();
                        ms.Flush();
                        ms.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEDATA-1: Could not retrieve image data. " + ex.Message);
            }

            return imageData;
        }

        public void SaveImage(string Filename, SaveTypes FileType)
        {
            try
            {
                if (_Encoded_Image != null)
                {
                    System.Drawing.Imaging.ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveTypes.BMP: imageformat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case SaveTypes.GIF: imageformat = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case SaveTypes.JPG: imageformat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case SaveTypes.PNG: imageformat = System.Drawing.Imaging.ImageFormat.Png; break;
                        case SaveTypes.TIFF: imageformat = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }

                    ((Bitmap)_Encoded_Image).Save(Filename, imageformat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESAVEIMAGE-1: Could not save image.\n\n=======================\n\n" + ex.Message);
            }
        }

        public void SaveImage(Stream stream, SaveTypes FileType)
        {
            try
            {
                if (_Encoded_Image != null)
                {
                    System.Drawing.Imaging.ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveTypes.BMP: imageformat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case SaveTypes.GIF: imageformat = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case SaveTypes.JPG: imageformat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case SaveTypes.PNG: imageformat = System.Drawing.Imaging.ImageFormat.Png; break;
                        case SaveTypes.TIFF: imageformat = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }

                    ((Bitmap)_Encoded_Image).Save(stream, imageformat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESAVEIMAGE-2: Could not save image.\n\n=======================\n\n" + ex.Message);
            }
        }

        public void GetSizeOfImage(ref double Width, ref double Height, bool Metric)
        {
            Width = 0;
            Height = 0;
            if (this.EncodedImage != null && this.EncodedImage.Width > 0 && this.EncodedImage.Height > 0)
            {
                double MillimetersPerInch = 25.4;
                using (Graphics g = Graphics.FromImage(this.EncodedImage))
                {
                    Width = this.EncodedImage.Width / g.DpiX;
                    Height = this.EncodedImage.Height / g.DpiY;

                    if (Metric)
                    {
                        Width *= MillimetersPerInch;
                        Height *= MillimetersPerInch;
                    }
                }
            }
        }

        #endregion Image Functions

        #region Label Generation

        private Image Label_ITF14(Image img)
        {
            try
            {
                System.Drawing.Font font = this.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, img.Height - (font.Height - 2), img.Width, font.Height));

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    g.DrawString(this.RawData, font, new SolidBrush(this.ForeColor), (float)(img.Width / 2), img.Height - font.Height + 1, f);

                    Pen pen = new Pen(ForeColor, (float)img.Height / 16);
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawLine(pen, new Point(0, img.Height - font.Height - 2), new Point(img.Width, img.Height - font.Height - 2));

                    g.Save();
                }

                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_ITF14-1: " + ex.Message);
            }
        }

        private Image Label_Generic(Image img)
        {
            try
            {
                System.Drawing.Font font = this.LabelFont;
                int LabelX = 0;
                int LabelY = 0;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Near;
                    f.LineAlignment = StringAlignment.Near;

                    switch (LabelPosition)
                    {
                        case LabelPositions.BOTTOMCENTER:
                            LabelX = img.Width / 2;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Center;
                            break;

                        case LabelPositions.BOTTOMLEFT:
                            LabelX = 0;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Near;
                            break;

                        case LabelPositions.BOTTOMRIGHT:
                            LabelX = img.Width;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Far;
                            break;

                        case LabelPositions.TOPCENTER:
                            LabelX = img.Width / 2;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Center;
                            break;

                        case LabelPositions.TOPLEFT:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Near;
                            break;

                        case LabelPositions.TOPRIGHT:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Far;
                            break;
                    }

                    g.FillRectangle(new SolidBrush(this.BackColor), new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height));

                    g.DrawString(this.RawData, font, new SolidBrush(this.ForeColor), new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height), f);

                    g.Save();
                }

                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_GENERIC-1: " + ex.Message);
            }
        }

        private Image Label_UPCA(Image img)
        {
            try
            {
                int iBarWidth = Width / Encoded_Value.Length;
                int shiftAdjustment = 0;

                switch (Alignment)
                {
                    case AlignmentPositions.CENTER: shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                        break;

                    case AlignmentPositions.LEFT: shiftAdjustment = 0;
                        break;

                    case AlignmentPositions.RIGHT: shiftAdjustment = (Width % Encoded_Value.Length);
                        break;

                    default: shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                        break;
                }

                System.Drawing.Font font = new System.Drawing.Font("OCR A Extended", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0))); ;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    RectangleF rect = new RectangleF((iBarWidth * 3) + shiftAdjustment, this.Height - (int)(this.Height * 0.1), (iBarWidth * 43), (int)(this.Height * 0.1));
                    g.FillRectangle(new SolidBrush(Color.Yellow), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(this.RawData.Substring(1, 5), font, new SolidBrush(this.ForeColor), rect.X, rect.Y);

                    g.Save();
                }

                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_UPCA-1: " + ex.Message);
            }
        }

        #endregion Label Generation

        #endregion Functions

        #region Misc

        private string GetXML()
        {
            if (EncodedValue == "")
                throw new Exception("EGETXML-1: Could not retrieve XML due to the barcode not being encoded first.  Please call Encode first.");
            else
            {
                try
                {
                    using (BarcodeXML xml = new BarcodeXML())
                    {
                        BarcodeXML.BarcodeRow row = xml.Barcode.NewBarcodeRow();
                        row.Type = EncodedType.ToString();
                        row.RawData = RawData;
                        row.EncodedValue = EncodedValue;
                        row.EncodingTime = EncodingTime;
                        row.IncludeLabel = IncludeLabel;
                        row.Forecolor = ColorTranslator.ToHtml(ForeColor);
                        row.Backcolor = ColorTranslator.ToHtml(BackColor);
                        row.CountryAssigningManufacturingCode = Country_Assigning_Manufacturer_Code;
                        row.ImageWidth = Width;
                        row.ImageHeight = Height;
                        row.RotateFlipType = this.RotateFlipType;
                        row.LabelPosition = (int)this.LabelPosition;
                        row.LabelFont = this.LabelFont.ToString();
                        row.ImageFormat = this.ImageFormat.ToString();
                        row.Alignment = (int)this.Alignment;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            EncodedImage.Save(ms, ImageFormat);
                            row.Image = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None);
                        }

                        xml.Barcode.AddBarcodeRow(row);

                        StringWriter sw = new StringWriter();
                        xml.WriteXml(sw, XmlWriteMode.WriteSchema);
                        return sw.ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("EGETXML-2: " + ex.Message);
                }
            }
        }

        public static Image GetImageFromXML(BarcodeXML internalXML)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(internalXML.Barcode[0].Image)))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
            }
        }

        #endregion Misc

        #region Static Methods

        public static Image DoEncode(TYPE iType, string Data)
        {
            using (Barcode b = new Barcode())
            {
                return b.Encode(iType, Data);
            }
        }

        public static Image DoEncode(TYPE iType, string Data, ref string XML)
        {
            using (Barcode b = new Barcode())
            {
                Image i = b.Encode(iType, Data);
                XML = b.XML;
                return i;
            }
        }

        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data);
            }
        }

        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, int Width, int Height)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, Width, Height);
            }
        }

        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, DrawColor, BackColor);
            }
        }

        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor, int Width, int Height)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
            }
        }

        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor, int Width, int Height, ref string XML)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                Image i = b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
                XML = b.XML;
                return i;
            }
        }

        #endregion Static Methods

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                this._LabelFont.Dispose();
                this._Encoded_Image.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("EDISPOSE-1: " + ex.Message);
            }
        }

        #endregion IDisposable Members
    }
}