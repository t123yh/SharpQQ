using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SharpQQ.Utils;

namespace SharpQQ.Binarizer
{
    public class BinaryBufferWriter
    {
        List<byte[]> _buffers = new List<byte[]>();

        public BinaryBufferWriter()
        { 
        }

        public void WriteByte(byte b)
        {
            _buffers.Add(new byte[1] { b });
        }

        public void WriteByteArray(byte[] buf)
        {
            _buffers.Add(buf);
        }

        public void WriteUInt16(ushort val, Endianness e)
        {
            WriteByteArray(MyBitConverter.GetBytesFromUInt16(val, e));
        }

        public void WriteInt16(short val, Endianness e)
        {
            WriteByteArray(MyBitConverter.GetBytesFromInt16(val, e));
        }

        public void WriteUInt32(uint val, Endianness e)
        {
            WriteByteArray(MyBitConverter.GetBytesFromUInt32(val, e));
        }

        public void WriteInt32(int val, Endianness e)
        {
            WriteByteArray(MyBitConverter.GetBytesFromInt32(val, e));
        }

        public void WriteUInt64(ulong val, Endianness e)
        {
            WriteByteArray(MyBitConverter.GetBytesFromUInt64(val, e));
        }

        public void WriteInt64(long val, Endianness e)
        {
            WriteByteArray(MyBitConverter.GetBytesFromInt64(val, e));
        }

        public int ContentLength
        {
            get
            {
                return _buffers.Select(x => x.Length).Sum();
            }
        }

        public byte[] GetContent()
        {
            byte[] buf = new byte[this.ContentLength];
            int pos = 0;
            foreach (var x in this._buffers)
            {
                Array.Copy(x, 0, buf, pos, x.Length);
                pos += x.Length;
            }
            return buf;
        }
    }
}