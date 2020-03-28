/*
 * VerifyCode 验证码
 * Copyright 2010 OPSoft ,All right reseved!
 * Newmin(ops.cc)  @  2010/11/18
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using JR.Stand.Core.Utils;

namespace JR.Stand.Core.Framework.Web.UI
{
    /// <summary>
    /// 验证码组成字符选项
    /// </summary>
    public enum VerifyWordOptions
    {
        /// <summary>
        /// 全数字
        /// </summary>
        Number,

        /// <summary>
        /// 全字母
        /// </summary>
        Letter,

        /// <summary>
        /// 字母和数字
        /// </summary>
        LetterAndNumber,

        /// <summary>
        /// 中文字符
        /// </summary>
        Chinese
    }

    /// <summary>
    /// 验证码生成器
    /// </summary>
    public class VerifyCodeGenerator
    {
        private delegate bool TestCondition(int number, int[] array);
        private static FontFamily _letterFont;
        private static readonly object Locker = new object();

        private const int Ns = 48; //数字开始
        private const int Ne = 57; //数字结束
        private const int UlS = 65; //大写字母开始
        private static int UlE { get; } = 90;
        private const int LlS = 97; //小写字母开始
        private const int LlE = 122; //小写字母结束
        private const int WordLength = 62;

        private static readonly int[] WordArray = new int[62];

        private bool _allowRepeat = true;

        static VerifyCodeGenerator()
        {
            //初始化，将0-9,A-Z,a-z添加到数组中去
            for (int i = 0; i < 10; i++)
            {
                WordArray[i] = Ns + i;
            }
            for (int i = 0; i < 26; i++)
            {
                WordArray[36 + i] = UlS + i;
                WordArray[10 + i] = LlS + i;
            }
        }

        /// <summary>
        /// 是否允许重复出现
        /// </summary>
        public bool AllowRepeat
        {
            get { return _allowRepeat; }
            set { _allowRepeat = value; }
        }


        /*
        /// <summary>
        /// 验证是否与当前验证码输入一致(区分大小写)
        /// </summary>
        /// <param name="verifyString"></param>
        /// <returns></returns>
        public static bool Verify(string verifyString)
        {
            return Verify(verifyString, false);
        }

        
        /// <summary>
        /// 验证是否与当前验证码输入一致
        /// </summary>
        /// <param name="verifyString"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool Verify(string verifyString, bool ignoreCase)
        {
            string verifyCode = HttpContext.Current.Session["current_verifycode"] as string;
            if (String.IsNullOrEmpty(verifyString)) return false;
            return String.Compare(verifyString, verifyCode, ignoreCase) == 0;
        }
         */

            /// <summary>
            /// 设置字体
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
        public static FontFamily SetFontFamily(String path)
        {
            _letterFont = Util.LoadFontFamily(path);
            return _letterFont;
        }

        private static FontFamily loadDefaultFont()
        {
            FontFamily fontFamily;
            try
            {
                fontFamily = FontFamily.GenericSansSerif;
            }
            catch
            {
                if (FontFamily.Families.Length != 0)
                {
                    fontFamily = FontFamily.Families[0];
                }
                else
                {
                    throw new Exception("计算机上找不到字体!");
                }
            }
            return fontFamily;
        }


        /// <summary>
        /// 获取字体
        /// </summary>
        /// <returns></returns>
        public static FontFamily GetFontFamily()
        {
            if (_letterFont == null)
            {
                lock (Locker)
                {
                    _letterFont = loadDefaultFont();
                }
            }
            return _letterFont;
        }

        /// <summary>
        /// 获取默认字体
        /// </summary>
        /// <returns></returns>
        private Font GetDefaultFont()
        {
            return new Font(GetFontFamily(), 14, FontStyle.Regular, GraphicsUnit.Pixel);
        }
        
        /// <summary>
        /// 显示验证码图片
        /// </summary>
        public byte[] GetGraphicImage(char[] code, bool simpleMode,int height)
        {
            return GraphicDrawImage(code,  simpleMode, GetDefaultFont(),height);
        }

        /// <summary>
        /// 显示验证码图片
        /// </summary>
        public byte[] GraphicDrawImage(char[] code, bool simpleMode, Font font, int imageHeight)
        {
            return DrawingImage(code, simpleMode, font, imageHeight);
        }

        public char[] GenerateVerifyWords(int number, VerifyWordOptions opt)
        {
            int[] verifyWords = new int[number];
            Random rd = new Random();

            TestCondition test;
            int _tempInt;


            switch (opt)
            {
                //纯数字
                case VerifyWordOptions.Number:

                    test = (i, array) =>
                    {
                        if (i == 0) return false;
                        else if (i < Ns || i > Ne) return false;
                        else if (!AllowRepeat && Array.Exists(array, a => a == i)) return false;
                        return true;
                    };

                    for (int i = 0; i < number; i++)
                    {
                        while (verifyWords[i] == 0)
                        {
                            _tempInt = WordArray[rd.Next(WordLength)];
                            if (test(_tempInt, verifyWords))
                            {
                                verifyWords[i] = _tempInt;
                            }
                        }
                    }

                    break;


                //纯字母
                case VerifyWordOptions.Letter:

                    test = (i, array) =>
                    {
                        if (i == 0) return false;
                        else if (i < UlS || i > LlE || (i > UlE && i < LlS)) return false;
                        else if (!AllowRepeat && Array.Exists(array, a => a == i)) return false;
                        return true;
                    };

                    for (int i = 0; i < number; i++)
                    {
                        while (verifyWords[i] == 0)
                        {
                            _tempInt = WordArray[rd.Next(WordLength)];
                            if (test(_tempInt, verifyWords))
                            {
                                verifyWords[i] = _tempInt;
                            }
                        }
                    }

                    break;


                //字母和数字
                case VerifyWordOptions.LetterAndNumber:

                    test = (i, array) =>
                    {
                        if (i == 0) return false;
                        else if (!Array.Exists(WordArray, a => a == i)) return false;
                        else if (!AllowRepeat && Array.Exists(array, a => a == i)) return false;
                        return true;
                    };

                    for (int i = 0; i < number; i++)
                    {
                        while (verifyWords[i] == 0)
                        {
                            _tempInt = WordArray[rd.Next(WordLength)];
                            if (test(_tempInt, verifyWords))
                            {
                                verifyWords[i] = _tempInt;
                            }
                        }
                    }

                    break;
            }

            //转换成字母
            char[] charArray = new char[verifyWords.Length];
            for (int i=0;i<verifyWords.Length; i++)
            {
                charArray[i] = (char) verifyWords[i];
            }
            //context.Session["current_verifycode"] = sb.ToString();
            return charArray;
        }

        /// <summary>
        /// 绘图
        /// </summary>
        /// <param name="charNumberArray"></param>
        /// <param name="simpleMode"></param>
        /// <param name="font"></param>
        /// <param name="imageHeight"></param>
        private byte[] DrawingImage(char[] charNumberArray, bool simpleMode, Font font, int imageHeight)
        {
            float fontSize = font.Size;
            int height = imageHeight;
            const int offset = 5;

            Bitmap img = new Bitmap(charNumberArray.Length*(int) fontSize + offset, height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);

            //生成随机生成器   
            Random rd = new Random();

            //画图片的干扰线   
            for (int i = 0; i < 25; i++)
            {
                int x1 = rd.Next(img.Width);
                int x2 = rd.Next(img.Width);
                int y1 = rd.Next(img.Height);
                int y2 = rd.Next(img.Height);
                g.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), x1, y1, x2, y2);
            }

            FontFamily ffamily = font.FontFamily;
            //try
            //{
            //    ffamily = FontFamily.GenericSerif;
            //}
            //catch
            //{
            //    FontFamily[] ffs = FontFamily.Families;
            //    if (ffs.Length > 1) ffamily = ffs[0];
            //    else
            //    {
            //        throw new Exception("系统中未找到任何字体!");
            //    }
            //}

            Brush[] brushes = {
                new SolidBrush(Color.Green),
                new SolidBrush(Color.Blue),
                new SolidBrush(Color.Red),
                new SolidBrush(Color.Black)
                //new SolidBrush(Color.Orange)
            };


            for (int i = 0; i < charNumberArray.Length; i++)
            {
                g.DrawString(charNumberArray[i].ToString(), font, brushes[rd.Next(brushes.Length)],
                    new PointF(offset + i*(fontSize - 1), (height - fontSize)/2));
            }

            if (!simpleMode)
            {
                //弯曲图片
                img = TwistImage(img, true, 2, 1);

                //画图片的前景干扰点   
                for (int i = 0; i < 100; i++)
                {
                    int x = rd.Next(img.Width);
                    int y = rd.Next(img.Height);
                    img.SetPixel(x, y, Color.FromArgb(235, 235, 235));
                }
            }


            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            font.Dispose();
            g.Dispose();
            img.Dispose();

            byte[] data = stream.ToArray();
            stream.Dispose();

            return data;
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片（http://www.51aspx.com/CV/VerifyColorTwistCode/）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        private Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            // const double PI = 3.1415926535897932384626433832795;

            const double PI2 = 6.283185307179586476925286766559;

            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double) destBmp.Height : (double) destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2*(double) j)/dBaseAxisLen : (PI2*(double) i)/dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int) (dy*dMultValue) : i;
                    nOldY = bXDir ? j : j + (int) (dy*dMultValue);

                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                        && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }
    }
}