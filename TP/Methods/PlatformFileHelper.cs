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
            // Scoped storage: Use app-specific download directory
            var downloadsPath = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;

            // If app-specific path is null, fall back to shared Downloads directory
            if (string.IsNullOrEmpty(downloadsPath))
            {
                downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;
            }

            if (string.IsNullOrEmpty(downloadsPath))
            {
                throw new InvalidOperationException("Unable to determine the downloads directory path.");
            }

            return Path.Combine(downloadsPath, fileName);
#else
        throw new PlatformNotSupportedException("This method is only supported on Android.");
#endif
        }
    }


}
