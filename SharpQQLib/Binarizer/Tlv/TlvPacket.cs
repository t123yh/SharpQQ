using System;
using System.Linq;
using System.Reflection;
using SharpQQ.Utils;

namespace SharpQQ.Binarizer.Tlv
{
    // TlvPacketCollection doesn't use this class to perform decode / encode tasks, because we can only
    // figure out what target type (such as Tlv_T1) is when we are decoding the PacketCollection.
    // Only when we are trying to`TlvPacketCollection.Get` them can we know the concrete type.
    // TlvPacket<T> should only be used when target type is known, thus unfit for the TlvPacketCollection.
    // Use `Find Usages` function of your IDE to discover use cases of these two classes.
    public class TlvPacket<T> : IBinaryConvertible where T : IBinaryConvertible, new()
    {
        public static implicit operator TlvPacket<T>(T src)
        {
            return new TlvPacket<T>(src);
        }

        public static implicit operator T(TlvPacket<T> src)
        {
            return src.Content;
        }
        
        public T Content { get; set; }
        
        public TlvPacket()
        {
            this.Content = new T();
        }

        public TlvPacket(T data)
        {
            this.Content = data;
        }

        public static short Tag => typeof(T).GetCustomAttribute<TlvPacketContent>().Tag;

        public void ParseFrom(BinaryBufferReader reader)
        {
            short dataTag = reader.ReadInt16(Endianness.Big);
            if (dataTag != Tag)
                throw new Exception($"Packet tag mismatch: expected {Tag}, got {dataTag}.");
            
            var length = reader.ReadInt16(Endianness.Big);
            var content = reader.ReadByteArray(length).ToArray();
            this.Content.ParseFrom(content);
        }

        public void WriteTo(BinaryBufferWriter writer)
        {
            var content = Content.GetBinary();
            if (content.Length > short.MaxValue)
                throw new Exception("TlvPacket content too long!");
            
            writer.WriteInt16(Tag, Endianness.Big);
            writer.WriteInt16((short)content.Length, Endianness.Big);
            writer.WriteByteArray(content);
        }
    }
}