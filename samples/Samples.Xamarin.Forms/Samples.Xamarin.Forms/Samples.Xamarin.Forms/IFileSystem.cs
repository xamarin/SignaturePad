using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Xam.Forms
{
    public interface IFileSystem
    {
        string Save(string fileName, Stream stream);
    }
}
