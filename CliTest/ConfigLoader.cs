using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CliTest
{
    public static class ConfigLoader
    {
        public static async Task<DeviceIdentity> LoadDeviceIdentity(string fileName)
        {
            DeviceIdentity result;
            try
            {
                var val = await File.ReadAllTextAsync(fileName);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();
                result = deserializer.Deserialize<DeviceIdentity>(new StringReader(val));
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to read device identity file, generating a new one...");
                result = await DeviceIdentity.AskForInfo();
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();

                var text = serializer.Serialize(result);
                await File.WriteAllTextAsync(fileName, text);
                Console.WriteLine($"Device identity has been saved to {fileName}");
            }

            return result;
        }
    }
}