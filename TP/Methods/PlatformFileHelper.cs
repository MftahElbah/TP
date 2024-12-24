using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
using AndroidX.Core.App;

namespace TP.Methods
{
    public static class PlatformFileHelper
    {
        /// <summary>
        /// Gets the full path for a file in the public Downloads directory.
        /// </summary>
        /// <param name="fileName">The name of the file to save.</param>
        /// <returns>The full file path.</returns>
        public static string GetPublicDownloadsPath(string fileName)
        {
#if ANDROID
            // Get the path to the public Downloads directory
            var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;

            if (string.IsNullOrEmpty(downloadsPath))
            {
                throw new InvalidOperationException("Unable to determine the public downloads directory path.");
            }

            return Path.Combine(downloadsPath, fileName);
#else
            throw new PlatformNotSupportedException("This method is only supported on Android.");
#endif
        }

        /// <summary>
        /// Downloads a file from a URL and saves it to the public Downloads directory.
        /// </summary>
        /// <param name="fileName">The name of the file to save.</param>
        /// <param name="fileUrl">The URL of the file to download.</param>
        /// <returns>The full path of the downloaded file.</returns>
        public static async Task<string> DownloadFileToDownloadsAsync(string fileName, string fileUrl)
        {
#if ANDROID
            // Check for permission
            if (!CheckWritePermission())
            {
                throw new UnauthorizedAccessException("Write permission to external storage is not granted.");
            }

            // Get the full path
            string filePath = GetPublicDownloadsPath(fileName);

            // Download and save the file
            using var httpClient = new HttpClient();
            var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
            File.WriteAllBytes(filePath, fileBytes);

            return filePath;
#else
            throw new PlatformNotSupportedException("This method is only supported on Android.");
#endif
        }

        /// <summary>
        /// Checks if the app has write permission for external storage.
        /// </summary>
        /// <returns>True if permission is granted, otherwise false.</returns>
        private static bool CheckWritePermission()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            return ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == Permission.Granted;
#else
            return false;
#endif
        }
    }
}
