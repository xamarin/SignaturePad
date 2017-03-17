using System;
using System.IO;
using Windows.UI.Xaml;

namespace Samples.Xam.Forms
{
    internal class FileSystem : IFileSystem
    {
        public string Save(string fileName, System.IO.Stream stream)
        {
            var applicationData = Windows.Storage.ApplicationData.Current;
            var localFolder = applicationData.LocalFolder.Path;
            var filePath = Path.Combine(localFolder, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var str = File.OpenWrite(filePath))
            using (stream)
            {
                stream.CopyTo(str);
            }

            return filePath;
        }
    }
}