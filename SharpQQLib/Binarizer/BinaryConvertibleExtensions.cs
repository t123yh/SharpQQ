namespace SharpQQ.Binarizer
{
    public static class BinaryConvertibleExtensions
    {
        public static void ParseFrom(this IBinaryConvertible conv, byte[] dat, bool allowRemainingData = false)
        {
            var reader = new BinaryBufferReader(dat);
            conv.ParseFrom(reader);

            if (!allowRemainingData && reader.RemainingLength != 0)
            {
                throw new BinarizerException("The binary structure have redundant data. This usually means data corruption.", conv.GetType().FullName);
            }
        }

        public static byte[] GetBinary(this IBinaryConvertible conv)
        {
            var writer = new BinaryBufferWriter();
            conv.WriteTo(writer);
            return writer.GetContent();
        }
    }
}