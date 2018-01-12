namespace SharpQQ.Binarizer
{
    public static class BinaryConvertibleExtensions
    {
        public static void ParseFrom(this IBinaryConvertible conv, byte[] dat)
        {
            conv.ParseFrom(new BinaryBufferReader(dat));
        }

        public static byte[] GetBinary(this IBinaryConvertible conv)
        {
            var writer = new BinaryBufferWriter();
            conv.WriteTo(writer);
            return writer.GetContent();
        }
    }
}