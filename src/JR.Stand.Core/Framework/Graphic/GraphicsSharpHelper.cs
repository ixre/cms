
using SkiaSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace JR.Stand.Core.Framework.Graphic
{
    /// <summary>
    ///     绘图工具类
    /// </summary>
    public static partial class GraphicsHelper
    {
        /// <summary>
        ///     生成缩略图(V2开源版本)
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        public static void SaveThumbnailV2(string originalImagePath, string thumbnailPath, int width, int height)
        {
            //todo: SixLabors only support .net4.7.2+
            //using (var image = Image.Load(originalImagePath))
            //{
            //    image.Mutate(x => x.Resize(width, 0));
            //    image.Save(thumbnailPath);
            //}
        }

        /// <summary>
        ///     生成缩略图(V3开源版本)
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        private static void SaveThumbnailV4(string originalImagePath, string thumbnailPath, int width, int height)
        {
            const int quality = 100; //质量为[SKFilterQuality.Medium]结果的100%
             var input = File.OpenRead(originalImagePath);
            using (var inputStream = new SKManagedStream(input))
            {
                var original = SKBitmap.Decode(inputStream);
                input.Close();
                if (original.Width <= width && original.Height <= height)//如果宽度和高度都小于缩率图值，则直接复制文件
                {
                    File.Copy(originalImagePath, thumbnailPath);
                }
                using (var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
                {
                    if (resized != null)
                    {
                         var image = SKImage.FromBitmap(resized);
                         var output = File.OpenWrite(thumbnailPath);
                        image.Encode(SKEncodedImageFormat.Png, quality).SaveTo(output);
                        input.Close();
                        output.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="source">原文件位置</param>
        /// <param name="target">生成目标文件位置</param>
        /// <param name="maxWidth">最大宽度，根据此宽度计算是否需要缩放，计算新高度</param>
        /// <param name="quality">图片质量，范围0-100</param>
        public static void SaveThumbnailV3(string source, string target, decimal maxWidth, int quality)
        {
            using (var file = File.OpenRead(source))
            using (var fileStream = new SKManagedStream(file))
                using (var bitmap = SKBitmap.Decode(fileStream))
                {
                    var width = (decimal)bitmap.Width;
                    var height = (decimal)bitmap.Height;
                    var newWidth = width;
                    var newHeight = height;
                    if (width > maxWidth)
                    {
                        newWidth = maxWidth;
                        newHeight = height / width * maxWidth;
                    }
                    using (var resized = bitmap.Resize(new SKImageInfo((int)newWidth, (int)newHeight), SKFilterQuality.Medium))
                    {
                        if (resized != null)
                        {
                            using (var image = SKImage.FromBitmap(resized))
                            using (var writeStream = File.OpenWrite(target))
                            {
                                image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(writeStream);
                            }
                        }
                    }
                }
            }
    }
}