using System.Collections.ObjectModel;

namespace SharpQQ.Binarizer
{
    public abstract class BinaryPacket
    {
        public abstract void ParseFrom(BinaryBufferReader reader);

        public abstract void WriteTo(BinaryBufferWriter writer);

        public void ParseFrom(byte[] dat)
        {
            this.ParseFrom(new BinaryBufferReader(dat));
        }

        public byte[] GetBinary()
        {
            BinaryBufferWriter writer = new BinaryBufferWriter();
            this.WriteTo(writer);
            return writer.GetContent();
        }
    }
}