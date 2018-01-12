using System;
using System.Diagnostics;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class MsfNegotiationConvertible : StructuredBinaryConvertible
    {
        private const int DefaultVersion = 20140601;
        private const string DefaultMsfString = "MSF";
        
        [IntegerField(1)]
        public int Version { get; set; } = DefaultVersion;

        [IntegerField(2)]
        public int _AlwaysZero1 { get; set; } = 0;

        [VariableLengthByteArrayField(3, PrependLengthType.Int8, PrependLengthTransform.NoTransform)]
        public string MsfString { get; set; } = DefaultMsfString;

        [IntegerField(4)]
        public byte _AlwaysZero2 { get; set; } = 0;

        [IntegerField(5)]
        public int _AlwaysZero3 { get; set; } = 0;

        public override void Check()
        {
            if (Version != DefaultVersion || MsfString != DefaultMsfString)
            {
                throw new Exception($"Malformed MSF negotiation packet; maybe not an MSF server.");
            }
        }
    }
}