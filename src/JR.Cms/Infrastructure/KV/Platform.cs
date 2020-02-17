using System;

namespace JR.Cms.Infrastructure.KV
{
    internal static class Platform
    {
        private static bool runAtLinux;
        private static bool isDetected;
        public const int LimitCharLength = 50;

        public static bool RunAtLinux()
        {
            if (!isDetected)
            {
                Int32 platFormID = (Int32)Environment.OSVersion.Platform;
                runAtLinux = platFormID == 4 || platFormID == 6 || platFormID == 128;
                isDetected = true;
            }
            return runAtLinux;
        }
    }
}
