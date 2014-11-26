using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup
{
    public static class StreamExtensions
    {
        public static int CopyStream(this Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int bytesCopied = 0;
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                bytesCopied += len;
                output.Write(buffer, 0, len);
            }
            return bytesCopied;
        }
    }
}
