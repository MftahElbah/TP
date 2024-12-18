using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods
{
    public static class PlatformFileHelper
    {
        public static string GetDownloadsPath(string fileName)
        {
            #if ANDROID
            var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            return Path.Combine(downloadsPath, fileName);
            #endif
        }
    }

}
