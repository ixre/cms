using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
            using (var image = Image.Load(originalImagePath))
            {
                image.Mutate(x => x.Resize(width, 0));
                image.Save(thumbnailPath);
            }
        }
    }
}