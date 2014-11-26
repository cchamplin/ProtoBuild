using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup.Elements.Interfaces
{
    public interface IDatagram
    {
        void SetDataPosition(int index, int position, int length);
        void ReadDataStream(Stream stream);
        IEnumerable<Stream> GetDataStreams(Action<int> setIndex);
    }
}
