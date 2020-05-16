using System;
using System.Drawing;
using System.Drawing.Text;

namespace JR.Stand.Core.Utils
{
    /// <summary>
    /// 实用工具
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 加载字体
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FontFamily LoadFontFamily(String path)
        {
            /*
            FileStream fs = new FileStream(path,FileMode.Open);
            int fontSize = (int)fs.Length;
            byte[] fontData = new byte[fontSize];
            fs.Read(fontData, 0, fontSize);
            PrivateFontCollection _fonts = new PrivateFontCollection();
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            _fonts.AddMemoryFont(fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);
            return _fonts.Families[0];
             */
            PrivateFontCollection fc = new PrivateFontCollection();
            fc.AddFontFile(path);
            if(fc.Families.Length > 0)
            {
                return fc.Families[0];
            }
            return null;
           
        }
    }
}
