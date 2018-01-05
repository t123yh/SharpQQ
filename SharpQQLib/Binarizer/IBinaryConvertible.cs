using System.Collections.ObjectModel;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer
{
    public abstract class BinaryPacket
    {
        public abstract void ParseFrom(BinaryBufferReader reader, CodingContext decodeContext = null);

        public abstract void WriteTo(BinaryBufferWriter writer, CodingContext encodeContext = null);

        public void ParseFrom(byte[] dat, CodingContext context = null)
        {
            this.ParseFrom(new BinaryBufferReader(dat), context);
        }

        public byte[] GetBinary(CodingContext context = null)
        {
            BinaryBufferWriter writer = new BinaryBufferWriter();
            this.WriteTo(writer, context);
            return writer.GetContent();
        }
    }
}