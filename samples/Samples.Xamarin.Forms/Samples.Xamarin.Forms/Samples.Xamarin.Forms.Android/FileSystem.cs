using System;
using System.IO;

namespace Samples.Xam.Forms
{
    internal class FileSystem : IFileSystem
    {
        public string Save(string fileName, System.IO.Stream stream)
        {
            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(appPath, fileName);
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