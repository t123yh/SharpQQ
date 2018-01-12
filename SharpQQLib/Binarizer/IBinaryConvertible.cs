using System.Collections.ObjectModel;

namespace SharpQQ.Binarizer
{
    public interface IBinaryConvertible
    {
        void ParseFrom(BinaryBufferReader reader);

        void WriteTo(BinaryBufferWriter writer);
    }
}