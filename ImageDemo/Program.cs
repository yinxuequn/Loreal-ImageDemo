using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace ImageDemo
{
    class Program
    {
        private static int _rowHeight;
        private static Font _font = new Font("宋体", 12);
        private static SolidBrush _sbrush = new SolidBrush(Color.Black);
        private static int _currentHeight = 0;
        private static int _tempLeftHeight = 0;
        private static int _tempRightHeight = 0;
        private static int _headWidth = 80;//标题区域的宽带
        private static int _containWidth = 240;//内容区域的宽带
        private static ImgModel _imgModel;
        private static StringFormat _stringFormat;
        private static int _imageWidth;
        private static int _lineHeight = 5;//行间距离
        private static string _baseDirectory;
        private static string _dateTimeDisplayFormat = @"yyyy/MM/dd hh:mm:ss";
        private static string _destImageExtension;

        static void Main(string[] args)
        {
            _baseDirectory = Environment.CurrentDirectory;
            _imgModel = CreateImgModel();

            Image destImg = ReadDestImage();
            InitParameters(destImg.Width);
            //绘制文字图片
            System.Drawing.Bitmap fontBmp = DrawFontImage();

            System.Drawing.Bitmap blankBmp = new Bitmap(destImg.Width, _currentHeight + _lineHeight + destImg.Height);

            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(blankBmp);
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //合并图片
            //放置文字图片
            graph.DrawImage(fontBmp, 0, 0, fontBmp.Width,fontBmp.Height);
            //DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
            graph.DrawImage(destImg, 0, _currentHeight + _lineHeight, destImg.Width, destImg.Height);
            //保存处理后的图片
            SaveDestImag(blankBmp);
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="imgWidth"></param>
        private static void InitParameters(int imgWidth)
        {
            _imageWidth = imgWidth;
            _font = new Font("宋体", 12);
            _sbrush = new SolidBrush(Color.Black);
            _headWidth = 80;//标题区域的宽度
            _containWidth = imgWidth / 2 - _headWidth;//内容区域的宽度
            _lineHeight = 5;//行间距离
            _currentHeight = 0;
            _stringFormat = new StringFormat();
            _stringFormat.Alignment = StringAlignment.Near;
            _stringFormat.LineAlignment = StringAlignment.Center;
        }

        /// <summary>
        /// 读取图片
        /// </summary>
        /// <returns></returns>
        private static Image ReadDestImage()
        {
            var imagePath = System.IO.Path.Combine(_baseDirectory, "..", "..", @"Image\login.jpg");
            _destImageExtension=System.IO.Path.GetExtension(imagePath);
            return new Bitmap(imagePath);
        }
        
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="destImag"></param>
        private static void SaveDestImag(Image destImag)
        {
            string newFileName = System.IO.Path.Combine(_baseDirectory, "..", "..", @"Image\login" + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + _destImageExtension);
            destImag.Save(newFileName);
        }

        #region 绘制文字图片

        private static Bitmap DrawFontImage()
        {
            System.Drawing.Bitmap fontBmp = new Bitmap(_imageWidth, 1000);

            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(fontBmp);
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度  
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, fontBmp.Width, fontBmp.Height);
            _rowHeight = (int)(Math.Ceiling(graph.MeasureString(@"测试\", _font).Height));

            DrawName(graph);
            DrawCode(graph);
            DrawArea(graph);
            DrawProduct(graph);
            DrawDisplay(graph);
            DrawRecordingDateTime(graph);
            DrawUploadDateTime(graph);

            return fontBmp;
        }

        /// <summary>
        /// 门店名称
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawName(Graphics graphic)
        {
            graphic.DrawString("门店名称:", _font, _sbrush, new Rectangle(0, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            var rowcount = DrawStringWrap(graphic, _font, _sbrush, _imgModel.Name, new Rectangle(_headWidth, _lineHeight, 640 - _headWidth, _rowHeight), _currentHeight);

            _currentHeight += rowcount * _rowHeight;
        }

        /// <summary>
        /// 门店编号
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawCode(Graphics graphic)
        {
            graphic.DrawString("门店编号:", _font, _sbrush, new Rectangle(0, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            graphic.DrawString(_imgModel.Code, _font, _sbrush, new Rectangle(_headWidth, _currentHeight + _lineHeight, _containWidth, _rowHeight), _stringFormat);
        }

        /// <summary>
        /// 拍摄区域
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawArea(Graphics graphic)
        {
            graphic.DrawString("拍摄区域:", _font, _sbrush, new Rectangle(_headWidth + _containWidth, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            _tempLeftHeight = DrawMultiLineString(graphic, _headWidth + _containWidth + _headWidth, _imgModel.Area);

            _currentHeight = _tempLeftHeight;
        }

        /// <summary>
        /// 拍摄产品
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawProduct(Graphics graphic)
        {
            graphic.DrawString("拍摄产品:", _font, _sbrush, new Rectangle(0, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            _tempLeftHeight = DrawMultiLineString(graphic, _headWidth, _imgModel.Product);
        }

        /// <summary>
        /// 陈列形式
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawDisplay(Graphics graphic)
        {
            graphic.DrawString("陈列形式:", _font, _sbrush, new Rectangle(_headWidth + _containWidth, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            _tempRightHeight = DrawMultiLineString(graphic, _headWidth + _containWidth + _headWidth, _imgModel.Display);
            _currentHeight = _tempLeftHeight > _tempRightHeight ? _tempLeftHeight : _tempRightHeight;

        }

        /// <summary>
        /// 拍摄时间
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawRecordingDateTime(Graphics graphic)
        {
            graphic.DrawString("拍摄时间:", _font, _sbrush, new Rectangle(0, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            graphic.DrawString(_imgModel.RecordingDateTime.ToString(_dateTimeDisplayFormat), _font, _sbrush, new Rectangle(_headWidth, _currentHeight + _lineHeight, _containWidth, _rowHeight), _stringFormat);
        }

        /// <summary>
        /// 上传时间
        /// </summary>
        /// <param name="graphic"></param>
        private static void DrawUploadDateTime(Graphics graphic)
        {
            graphic.DrawString("上传时间:", _font, _sbrush, new Rectangle(_headWidth + _containWidth, _currentHeight + _lineHeight, _headWidth, _rowHeight), _stringFormat);
            graphic.DrawString(_imgModel.UploadDateTime.ToString(_dateTimeDisplayFormat), _font, _sbrush, new Rectangle(_headWidth + _containWidth + _headWidth, _currentHeight + _lineHeight, _containWidth, _rowHeight), _stringFormat);
            _currentHeight += _rowHeight;
        }

        private static int DrawMultiLineString(Graphics graphic, int startY, string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var rowcount = DrawStringWrap(graphic, _font, _sbrush, str, new Rectangle(startY, _currentHeight + _lineHeight, _containWidth, _rowHeight), _currentHeight);
                var height = _currentHeight + rowcount * _rowHeight;
                return height;

            }
            else
            {
                return _currentHeight + _rowHeight;
            }

        }

        /// <summary>
        /// 绘制文本自动换行（超出截断）
        /// </summary>
        /// <param name=\"graphic\">绘图图面</param>
        /// <param name=\"font\">字体</param>
        /// <param name=\"text\">文本</param>
        /// <param name=\"recangle\">绘制范围</param>
        private static int DrawStringWrap(Graphics graphic, Font font, SolidBrush sbrush, string text, Rectangle recangle, int startY)
        {
            List<string> textRows = GetStringRows(graphic, font, text, recangle.Width);

            for (int i = 0; i < textRows.Count; i++)
            {
                Rectangle fontRectanle = new Rectangle(recangle.Left, startY + _lineHeight + _rowHeight * i, recangle.Width, _rowHeight);
                graphic.DrawString(textRows[i], font, sbrush, fontRectanle, _stringFormat);
            }

            return textRows.Count;
        }

        /// <summary>
        /// 将文本分行
        /// </summary>
        /// <param name=\"graphic\">绘图图面</param>
        /// <param name=\"font\">字体</param>
        /// <param name=\"text\">文本</param>
        /// <param name=\"width\">行宽</param>
        /// <returns></returns>
        private static List<string> GetStringRows(Graphics graphic, Font font, string text, int width)
        {
            int RowBeginIndex = 0;
            int rowEndIndex = 0;
            int textLength = text.Length;
            List<string> textRows = new List<string>();

            for (int index = 0; index < textLength; index++)
            {
                rowEndIndex = index;

                if (index == textLength - 1)
                {
                    textRows.Add(text.Substring(RowBeginIndex));
                }
                else if (rowEndIndex + 1 < text.Length && text.Substring(rowEndIndex, 2) == @"\\r\\n\")
                {
                    textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                    rowEndIndex = index += 2;
                    RowBeginIndex = rowEndIndex;
                }
                else if (graphic.MeasureString(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex + 1), font).Width > width)
                {
                    textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                    RowBeginIndex = rowEndIndex;
                }
            }

            return textRows;
        }

        #endregion

        public class ImgModel
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Area { get; set; }
            public string Product { get; set; }
            public string Display { get; set; }
            public DateTime RecordingDateTime { get; set; }
            public DateTime UploadDateTime { get; set; }
        }


        private static ImgModel CreateImgModel()
        {
            return new ImgModel()
            {
                Name = "测试化妆品门店2",
                Code = "test2",
                Area = "",
                Product = "柜台照片",
                Display = "欧莱雅柜台",
                RecordingDateTime = DateTime.Now,
                UploadDateTime = DateTime.Now,
            };
        }
    }
}
