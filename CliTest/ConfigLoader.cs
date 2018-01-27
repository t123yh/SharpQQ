using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CliTest
{
    public static class ConfigLoader
    {
        public static async Task<T> LoadConfig<T>(string fileName, Func<Task<T>> generateNew)
        {
            T result;
            try
            {
                var val = await File.ReadAllTextAsync(fileName);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();
                result = deserializer.Deserialize<T>(new StringReader(val));
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to read config file for {typeof(T).Name}, generating a new one...");
                result = await generateNew();
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(new PascalCaseNamingConvention())
                    .Build();

                var text = serializer.Serialize(result);
                await File.WriteAllTextAsync(fileName, text);
                Console.WriteLine($"Config has been saved to {fileName}");
            }

            return result;
        }
    }
}