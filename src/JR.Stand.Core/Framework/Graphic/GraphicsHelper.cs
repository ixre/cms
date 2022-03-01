using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using JR.Stand.Core.Utils;

/*******************************************
* 文 件 名：ImageGraphicsUtility.cs
* 文件说明：
* 创 建 人：刘成文
* 创建日期：2012-9-29 11:53:40
********************************************/

namespace JR.Stand.Core.Framework.Graphic
{
    /// <summary>
    /// 绘图工具类
    /// </summary>
    public class GraphicsHelper
    {
        private static IDictionary<string, ImageCodecInfo> imageCoders;

        static GraphicsHelper()
        {
            imageCoders = new Dictionary<string, ImageCodecInfo>();
            ImageCodecInfo[] imgCodes = ImageCodecInfo.GetImageEncoders();
            string key = null;
            foreach (ImageCodecInfo code in imgCodes)
            {
                switch (code.MimeType)
                {
                    case "image/jpeg":
                        key = "Jpeg";
                        break;
                    case "image/png":
                        key = "Png";
                        break;
                    case "image/bmp":
                        key = "Bmp";
                        break;
                    default:
                        key = null;
                        break;
                }

                if (key != null)
                {
                    imageCoders.Add(key, code);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="resize"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Size GetSize(Size size, Size resize, ImageSizeMode mode)
        {
            //    int toWidth = resize.Width;
            //    int toHeight = resize.Height;

            //    int x = 0;
            //    int y = 0;
            //    int ow = size.Width;
            //    int oh = size.Height;

            Size newSize = new Size();
            newSize.Width = resize.Width;
            newSize.Height = resize.Height;


            switch (mode)
            {
                case ImageSizeMode.SuitWidth:
                    newSize.Height = size.Height*resize.Width/size.Width;
                    break;

                case ImageSizeMode.SuitHeight:
                    newSize.Width = size.Width*resize.Height/size.Height;
                    break;

                case ImageSizeMode.AutoSuit:
                    //根据宽度适配
                    if (size.Width > size.Height)
                    {
                        newSize.Height = size.Height*resize.Width/size.Width;
                    }
                    else //根据高度适配
                    {
                        newSize.Width = size.Width*resize.Height/size.Height;
                    }
                    break;

                //填充适应
                case ImageSizeMode.FillFit:
                    if ((double) size.Width/(double) size.Height > (double) resize.Width/(double) resize.Height)
                    {
                        newSize.Height = size.Height*resize.Width/size.Width;
                    }
                    else
                    {
                        newSize.Width = resize.Width*resize.Height/size.Height;
                    }

                    break;

                //裁剪
                case ImageSizeMode.Cut:
                    //裁剪宽
                    if ((double) size.Width/(double) size.Height > (double) resize.Width/(double) resize.Height)
                    {
                        newSize.Height = size.Height;
                        newSize.Width = size.Height*resize.Width/resize.Height;
                    }
                    else
                    {
                        newSize.Width = size.Width;
                        newSize.Height = size.Width*resize.Height/resize.Width;
                    }
                    break;

                default:
                case ImageSizeMode.CustomSize:
                    break;
            }

            return newSize;
        }

        /// <summary>
        /// 重新绘制指定尺寸的图片
        /// </summary>
        /// <param name="original">原图</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">缩略图模式</param>
        /// <param name="format">图像格式</param>
        /// <param name="compression"></param>
        /// <param name="graphicsHandler">对缩略图处理</param>
        /// <param name="imageQuality"></param>
        public static byte[] DrawBySize(
            Image original,
            ImageSizeMode mode,
            int width, int height,
            ImageFormat format,
            long imageQuality,
            long compression,
            ImageGraphicsHandler graphicsHandler)
        {
            int toWidth = width;
            int toHeight = height;

            int x = 0;
            int y = 0;
            int ow = original.Width; //原始宽度
            int oh = original.Height; //原始高度

            Size toSize = new Size(width, height); //要转换的图片尺寸
            Size imageSize = GetSize(original.Size, toSize, mode); //转换的实际尺寸


            //裁剪
            if (mode == ImageSizeMode.Cut)
            {
                if ((double) original.Width/(double) original.Height > (double) toSize.Width/(double) toSize.Height)
                {
                    oh = original.Height;
                    ow = original.Height*toWidth/toHeight;
                    y = 0;
                    x = (original.Width - ow)/2;
                }
                else
                {
                    ow = original.Width;
                    oh = original.Width*height/toWidth;
                    x = 0;
                    y = (original.Height - oh)/2;
                }
            }
            else
            {
                x = (toSize.Width - imageSize.Width)/2;
                y = (toSize.Height - imageSize.Height)/2;
            }

            //新建一个bmp图片
            Image bitmap;
            if (mode == ImageSizeMode.FillFit)
            {
                bitmap = new Bitmap(width, height);
            }
            else
            {
                bitmap = new Bitmap(toWidth, toHeight);
            }

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            if (mode == ImageSizeMode.FillFit)
            {
                g.Clear(Color.White);
            }

            //在指定位置并且按指定大小绘制原图片的指定部分

            g.DrawImage(original,
                new Rectangle(x, y, imageSize.Width, imageSize.Height),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);


            //对图片进行处理
            if (graphicsHandler != null)
            {
                graphicsHandler(bitmap);
                if (bitmap == null)
                {
                    throw new ArgumentException("不允许释放图片资源！");
                }
            }

            //保存到内存留
            MemoryStream ms = new MemoryStream();


            ImageCodecInfo imgCode = null;

            if (format != null)
            {
                string formatStr = format.ToString();
                imgCode = imageCoders.ContainsKey(formatStr) ? imageCoders[formatStr] : null;
            }

            if (imgCode != null)
            {
                EncoderParameters ep = new EncoderParameters(2);
                ep.Param[0] = new EncoderParameter(Encoder.Quality,
                    imageQuality > 100 || imageQuality <= 0 ? 100L : imageQuality);
                ep.Param[1] = new EncoderParameter(Encoder.Compression, compression < 1L ? 100L : compression);
                bitmap.Save(ms, imgCode, ep);
            }
            else
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
            }

            //释放资源
            g.Dispose();
            byte[] bytes = ms.ToArray();
            ms.Dispose();
            bitmap.Dispose();
            return bytes;
        }

        /// <summary>
        /// 重新绘制指定尺寸的图片
        /// </summary>
        /// <param name="original">原图</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">缩略图模式</param>
        /// <param name="graphicsHandler">对缩略图处理</param>
        public static byte[] DrawBySize(
            Image original,
            ImageSizeMode mode,
            int width, int height,
            ImageGraphicsHandler graphicsHandler)
        {
            return DrawBySize(original, mode, width, height, ImageFormat.Jpeg, 100L, 60L, graphicsHandler);
        }

        #region 生成缩略图

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="original">原图</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">缩略图模式</param>
        /// <param name="thumbHandler">对缩略图处理</param>
        public static MemoryStream MakeThumbnail(Image original, ImageSizeMode mode, int width, int height,
            ImageGraphicsHandler thumbHandler)
        {
            int toWidth = width;
            int toHeight = height;

            int x = 0;
            int y = 0;
            int ow = original.Width;
            int oh = original.Height;

            switch (mode)
            {
                case ImageSizeMode.SuitWidth:
                    toHeight = original.Height*width/original.Width;
                    break;

                case ImageSizeMode.SuitHeight:
                    toWidth = original.Width*height/original.Height;
                    break;

                case ImageSizeMode.AutoSuit:
                    //根据宽度适配
                    if (original.Width > original.Height)
                    {
                        toHeight = original.Height*width/original.Width;
                    }
                    else //根据高度适配
                    {
                        toWidth = original.Width*height/original.Height;
                    }
                    break;

                //填充适应
                case ImageSizeMode.FillFit:
                    if ((double) original.Width/(double) original.Height > (double) toWidth/(double) toHeight)
                    {
                        toWidth = width;
                        toHeight = original.Height*width/original.Width;
                    }
                    else
                    {
                        toHeight = height;
                        toWidth = original.Width*height/original.Height;
                    }

                    break;

                //裁剪
                case ImageSizeMode.Cut:
                    if ((double) original.Width/(double) original.Height > (double) toWidth/(double) toHeight)
                    {
                        oh = original.Height;
                        ow = original.Height*toWidth/toHeight;
                        y = 0;
                        x = (original.Width - ow)/2;
                    }
                    else
                    {
                        ow = original.Width;
                        oh = original.Width*height/toWidth;
                        x = 0;
                        y = (original.Height - oh)/2;
                    }
                    break;

                default:
                case ImageSizeMode.CustomSize:
                    break;
            }

            //新建一个bmp图片
            Image bitmap;
            if (mode == ImageSizeMode.FillFit)
            {
                bitmap = new Bitmap(width, height);
            }
            else
            {
                bitmap = new Bitmap(toWidth, toHeight);
            }

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            if (mode == ImageSizeMode.FillFit)
            {
                g.Clear(Color.White);
            }

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(original,
                new Rectangle((width - toWidth)/2, (height - toHeight)/2, toWidth, toHeight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);


            //对图片进行处理
            if (thumbHandler != null)
            {
                thumbHandler(bitmap);
                if (bitmap == null)
                {
                    throw new ArgumentException("不允许释放图片资源！");
                }
            }

            //保存到内存留
            MemoryStream ms = new MemoryStream();

            //图片质量参数
            EncoderParameters ep = new EncoderParameters(2);
            ep.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            ep.Param[1] = new EncoderParameter(Encoder.Compression, 60L);

            ImageCodecInfo imgCode = null;
            ImageCodecInfo[] imgCodes = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo code in imgCodes)
            {
                if (code.MimeType == "image/jpeg")
                {
                    imgCode = code;
                    break;
                }
            }
            if (imgCode != null)
            {
                bitmap.Save(ms, imgCode, ep);
            }
            else
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
            }

            //释放资源
            g.Dispose();
            bitmap.Dispose();

            return ms;
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        public static void SaveThumbnail(string originalImagePath, string thumbnailPath, int width, int height)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            //生成缩略图
            MakeThumbnail(originalImage, ImageSizeMode.Cut, width, height, (img) =>
            {
                //以jpg格式保存缩略图
                img.Save(thumbnailPath, ImageFormat.Jpeg);
            }).Dispose();

            //释放资源
            originalImage.Dispose();
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void SaveThumbnail(string originalImagePath, string thumbnailPath, ImageSizeMode mode, int width,
            int height)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            //生成缩略图
            MakeThumbnail(originalImage, mode, width, height, (img) =>
            {
                //以jpg格式保存缩略图
                img.Save(thumbnailPath, ImageFormat.Jpeg);
            }).Dispose();

            //释放资源
            originalImage.Dispose();
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImage">源图</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void SaveThumbnail(Image originalImage, string thumbnailPath, ImageSizeMode mode, int width,
            int height)
        {
            //生成缩略图
            MakeThumbnail(originalImage, mode, width, height, (img) =>
            {
                //以jpg格式保存缩略图
                img.Save(thumbnailPath, ImageFormat.Jpeg);
            }).Dispose();

            //释放资源
            originalImage.Dispose();
        }

        #endregion

        private void Thumbnal_Test()
        {
            int imgHeight = 480, imgWidth = 600;
            string file = "/1.jpg";
            string waterPath = "";
            string rootPath = EnvUtil.GetBaseDirectory();

            Bitmap img = new Bitmap(rootPath + file);
            int width, height;
            if (img.Width > img.Height)
            {
                width = imgWidth;
                height = imgHeight;
            }
            else
            {
                width = imgHeight;
                height = imgWidth;
            }

            byte[] data = DrawBySize(img, ImageSizeMode.CustomSize, width, height, ImageFormat.Jpeg, 90L,
                50L, null);
            img.Dispose();


            MemoryStream ms1 = new MemoryStream(data);
            img = new Bitmap(ms1);

            Image water = new Bitmap(waterPath);


            data = MakeWatermarkImage(
                img,
                water,
                WatermarkPosition.Middle
                );


            ms1.Dispose();
            img.Dispose();

            FileStream fs = File.OpenWrite(rootPath + "/1_1.jpg");
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(data);
            w.Flush();
            fs.Flush();
            fs.Dispose();
        }

        #region 生成水印

        /// <summary>
        /// 生成水印
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="water">水印</param>
        /// <param name="waterPos">水印位置</param>
        /// <param name="handler">处理图片</param>
        /// <returns>图片内存流</returns>
        public static byte[] MakeWatermarkImage(
            Image img,
            Image water,
            Point waterPos,
            ImageGraphicsHandler handler)
        {
            //尺寸检测
            if (water.Width > img.Width || water.Height > img.Height)
            {
                throw new ArgumentException("水印图片尺寸超过原图,无法生成水印！");
            }

            //绘制水印
            Graphics g = Graphics.FromImage(img);
            g.DrawImage(water, waterPos.X, waterPos.Y);

            if (handler != null)
            {
                handler(img);
                if (img == null)
                {
                    throw new ArgumentException("不允许释放图片资源！");
                }
            }

            //保存到内存流
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);

            //释放资源
            g.Dispose();

            byte[] bytes = ms.ToArray();
            ms.Dispose();
            water.Dispose();

            return bytes;
        }


        /// <summary>
        /// 生成水印
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="water">水印</param>
        /// <param name="pos">预置的水印位置</param>
        /// <param name="format">图像格式</param>
        /// <param name="handler">处理图片</param>
        /// <returns>图片内存流</returns>
        public static byte[] MakeWatermarkImage(
            Image img,
            Image water,
            WatermarkPosition pos,
            ImageGraphicsHandler handler)
        {
            int x;
            int y;

            int xOffset = 5; //宽度偏移量
            int yOffset = 5; //高度偏移量
            switch (pos)
            {
                case WatermarkPosition.Bottom:
                    //正下方
                    x = (img.Width - water.Width)/2;
                    y = img.Height - water.Height - yOffset;
                    break;
                case WatermarkPosition.LeftBottom:
                    //左下方
                    x = xOffset;
                    y = img.Height - water.Height - yOffset;
                    break;
                case WatermarkPosition.Right:
                    //右方
                    x = img.Width - water.Width - xOffset;
                    y = (img.Height - water.Height)/2;
                    break;
                case WatermarkPosition.Middle:
                    //正中
                    x = (img.Width - water.Width)/2;
                    y = (img.Height - water.Height)/2;
                    break;
                case WatermarkPosition.Left:
                    //左边
                    x = xOffset;
                    y = (img.Height - water.Height)/2;
                    break;
                case WatermarkPosition.RightTop:
                    //右上方
                    x = img.Width - water.Width - xOffset;
                    y = yOffset;
                    break;
                case WatermarkPosition.Top:
                    //正上方
                    x = (img.Width - water.Width)/2;
                    y = yOffset;
                    break;
                case WatermarkPosition.LeftTop:
                    //左上方
                    x = xOffset;
                    y = yOffset;
                    break;

                default:
                case WatermarkPosition.Default:
                case WatermarkPosition.RightBottom:
                    //右下角
                    x = img.Width - water.Width - xOffset;
                    y = img.Height - water.Height - yOffset;
                    break;
            }

            return MakeWatermarkImage(img, water, new Point(x, y), handler);
        }


        /// <summary>
        /// 生成水印
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="water">水印</param>
        /// <param name="pos">预置的水印位置</param>
        /// <returns>图片内存流</returns>
        public static byte[] MakeWatermarkImage(Image img, Image water, WatermarkPosition pos)
        {
            return MakeWatermarkImage(img, water, pos, null);
        }

        /// <summary>
        /// 生成水印并保存到文件
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        /// <param name="waterPath">水印路径</param>
        /// <param name="pos">水印位置</param>
        /// <param name="handler">图片处理</param>
        /// <returns></returns>
        public static byte[] MakeWatermarkImage(string imgPath, string waterPath, WatermarkPosition pos,
            ImageGraphicsHandler handler)
        {
            Image img = Image.FromFile(imgPath),
                water = Image.FromFile(waterPath);

            byte[] bytes = MakeWatermarkImage(img, water, pos, handler);
            img.Dispose();
            water.Dispose();
            return bytes;
        }

        /// <summary>
        /// 生成水印并保存到文件
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        /// <param name="waterPath">水印路径</param>
        /// <param name="pos">水印位置</param>
        /// <returns></returns>
        public static byte[] MakeWatermarkImage(string imgPath, string waterPath, WatermarkPosition pos)
        {
            return MakeWatermarkImage(imgPath, waterPath, pos, null);
        }

        #endregion

        #region 生成水印并保存文件

        /// <summary>
        /// 生成水印并保存到文件
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="water">水印</param>
        /// <param name="pos">预置的水印位置</param>
        public static void SaveWatermarkImage(Image img, Image water, string savePath, WatermarkPosition pos)
        {
            MakeWatermarkImage(img, water, pos, thumb => { thumb.Save(savePath, img.RawFormat); });

            img.Dispose();
        }

        /// <summary>
        /// 生成水印并保存到文件
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        /// <param name="waterPath">水印路径</param>
        /// <param name="savePath">保存路径</param>
        public static void SaveWatermarkImage(string imgPath, string waterPath, string savePath)
        {
            MakeWatermarkImage(imgPath, waterPath, WatermarkPosition.Default,
                img => { img.Save(savePath, img.RawFormat); });
        }

        /// <summary>
        /// 生成水印并保存到文件
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        /// <param name="waterPath">水印路径</param>
        /// <param name="savePath">保存路径</param>
        public static void SaveWatermarkImage(string imgPath, string waterPath, string savePath, WatermarkPosition pos)
        {
            MakeWatermarkImage(imgPath, waterPath, pos, img => { img.Save(savePath, img.RawFormat); });
        }

        #endregion

        #region 其他

        /// <summary>
        /// 自动上传文本中的图片
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="path">上传路径</param>
        /// <returns>上传结果信息</returns>
        //private static string AutoUpload(string content, string path)
        //{
        //    //自动保存远程图片
        //    WebClient client = new WebClient();
        //    //备用Reg:<img.*?src=([\"\'])(http:\/\/.+\.(jpg|gif|bmp|bnp))\1.*?>
        //    Regex reg = new Regex("IMG[^>]*?src\\s*=\\s*(?:\"(?<1>[^\"]*)\"|'(?<1>[^\']*)')", RegexOptions.IgnoreCase);
        //    MatchCollection m = reg.Matches(content);

        //    foreach (Match math in m)
        //    {
        //        string imgUrl = math.Groups[1].Value;

        //        //在原图片名称前加YYMMDD重名名并上传

        //        Regex regName = new Regex(@"\w+.(?:jpg|gif|bmp|png)", RegexOptions.IgnoreCase);

        //        string strNewImgName = DateTime.Now.ToShortDateString().Replace("-", "") + regName.Match(imgUrl).ToString();

        //        try
        //        {
        //            //保存图片
        //            client.DownloadFile(imgUrl, (path + strNewImgName));
        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {

        //        }
        //        client.Dispose();
        //    }
        //    return "远程图片保存成功，保存路径为ImgUpload/auto";
        //}

        #endregion
    }
}