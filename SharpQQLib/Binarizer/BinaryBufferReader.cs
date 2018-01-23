using System;
using System.Linq;
using System.Collections.Generic;
using SharpQQ.Utils;

namespace SharpQQ.Binarizer
{
    // This class was inspired by ByteBuffer
    public class BinaryBufferReader
    {
        byte[] _baseBuffer;

        public int Position { get; set; }

        public BinaryBufferReader(IEnumerable<byte> dat, int initPosition = 0)
        {
            this._baseBuffer = dat.ToArray();
            this.Position = initPosition;
        }

        private BinaryBufferReader() { }

        public BinaryBufferReader Clone()
        {
            var target = new BinaryBufferReader();
            target._baseBuffer = this._baseBuffer;
            target.Position = this.Position;
            return target;
        }

        public bool IsEndOfStream
        {
            get { return this.Position == this._baseBuffer.Length; }
        }

        public int RemainingLength
        {
            get { return this._baseBuffer.Length - this.Position; }
        }

        private void EnsureSufficient(int length)
        {
            if(this.RemainingLength < length)
                throw new BinarizerException("Not enough bytes to read.");
        }

        public byte PeekByte()
        {
            EnsureSufficient(1);
            return _baseBuffer[Position];
        }

        public byte ReadByte()
        {
            EnsureSufficient(1);
            byte result = _baseBuffer[Position];
            Position++;
            return result;
        }

        public IEnumerable<byte> PeekByteArray(int count)
        {
            EnsureSufficient(count);
            return _baseBuffer.Skip(Position).Take(count);
        }

        public IEnumerable<byte> ReadByteArray(int count)
        {
            EnsureSufficient(count);
            var result = _baseBuffer.Skip(Position).Take(count);
            Position += count;
            return result;
        }

        public short ReadInt16(Endianness e)
        {
            return MyBitConverter.GetInt16(this.ReadByteArray(2).ToArray(), e);
        }

        public ushort ReadUInt16(Endianness e)
        {
            return MyBitConverter.GetUInt16(this.ReadByteArray(2).ToArray(), e);
        }

        public int ReadInt32(Endianness e)
        {
            return MyBitConverter.GetInt32(this.ReadByteArray(4).ToArray(), e);
        }

        public uint ReadUInt32(Endianness e)
        {
            return MyBitConverter.GetUInt32(this.ReadByteArray(4).ToArray(), e);
        }

        public long ReadInt64(Endianness e)
        {
            return MyBitConverter.GetInt64(this.ReadByteArray(8).ToArray(), e);
        }

        public ulong ReadUInt64(Endianness e)
        {
            return MyBitConverter.GetUInt64(this.ReadByteArray(8).ToArray(), e);
        }
    }
}