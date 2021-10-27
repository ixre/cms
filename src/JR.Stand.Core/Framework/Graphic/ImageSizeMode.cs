namespace JR.Stand.Core.Framework.Graphic
{
    /// <summary>
    /// 缩略图模式
    /// </summary>
    public enum ImageSizeMode
    {
        /// <summary>
        /// 固定尺寸，可能会变形
        /// </summary>
        CustomSize = 1,

        /// <summary>
        /// 裁剪
        /// </summary>
        Cut = 2,

        /// <summary>
        /// 根据宽度缩放
        /// </summary>
        SuitWidth = 3,

        /// <summary>
        /// 根据高度缩放
        /// </summary>
        SuitHeight = 4,

        /// <summary>
        /// 自动适配
        /// </summary>
        AutoSuit = 5,

        /// <summary>
        /// 自动填充
        /// </summary>
        FillFit = 6
    }
}