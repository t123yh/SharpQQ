using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace SharpQQ.Utils
{
    public static class TcpClientExtensions
    {
        public static async Task WritePacketAsync(this TcpClient client, byte[] dat)
        {
            byte[] lenBytes = MyBitConverter.GetBytesFromInt32(dat.Length, Endianness.Big);
            await client.GetStream().WriteAsync(lenBytes, 0, lenBytes.Length);
            await client.GetStream().WriteAsync(dat, 0, dat.Length);
        }

        public static async Task<byte[]> ReadPacketAsync(this TcpClient client)
        {
            byte[] lenBytes = new byte[4];
            await client.GetStream().ReadAsync(lenBytes, 0, lenBytes.Length);
            int len = MyBitConverter.GetInt32(lenBytes, Endianness.Big);
            byte[] data = new byte[len];
            int pos = 0;
            do
            {
                int read = await client.GetStream().ReadAsync(data, pos, len - pos);
                pos += read;
            } while (pos < len);

            return data;
        }
    }
}