using SharpQQ.Utils;

namespace SharpQQ
{
    public class Constants
    {
        public const int AppId = 537051018;
        
        public const string PacketName = "com.tencent.tim";

        public const string AppVersion = "1.1.5.9114";

        public const string AndroidVersion = "7.1.2";

        public const string Operator = "中国电信";

        public const string OS = "android";
        
        public static readonly byte[] QQServerKey = BinaryUtils.HexToBin("3046301006072A8648CE3D020106052B8104001F03320004928D8850673088B343264E0C6BACB8496D697799F37211DEB25BB73906CB089FEA9639B4E0260498B51A992D50813DA8");
    }
}