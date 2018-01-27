using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CliTest
{
    public static class Utils
    {
        public static readonly Random GlobalRandom = new Random();

        public static async Task<string> Prompt(string prompt)
        {
            Console.Write(prompt);
            return await Task.Run(() => Console.ReadLine());
        }

        public static async Task<string> PromptUntil(
            string prompt,
            Func<string, bool> check,
            string invalidHint = "Invalid value provided."
        )
        {
            while (true)
            {
                var val = await Prompt(prompt);
                if (check(val))
                    return val;
                else
                    Console.WriteLine(invalidHint);
            }
        }

        // Code from here: https://www.codeproject.com/Tips/302157/Mobile-IMEI-Validation
        public static bool ValidateIMEI(string imeiStr)
        {
            const int len = 15;
            if (imeiStr.Length != len)
                return false;
            else
            {
                var imeiNum = new int[len];
                for (int innlop = 0; innlop < len; innlop++)
                {
                    try
                    {
                        imeiNum[innlop] = int.Parse(imeiStr.Substring(innlop, 1));
                    }
                    catch
                    {
                        return false;
                    }

                    if (innlop % 2 != 0)
                        imeiNum[innlop] *= 2;
                    while (imeiNum[innlop] > 9)
                        imeiNum[innlop] = (imeiNum[innlop] % 10) + (imeiNum[innlop] / 10);
                }

                int sum = imeiNum.Sum();
                return sum % 10 == 0;
            }
        }

        public static bool ValidateIMSI(string val)
        {
            return long.TryParse(val, out _) && (val.Length == 15 || val.Length == 14);
        }

        public static bool ValidateAndroidVersion(string val)
        {
            return new Regex(@"^\d+\.\d\.\d$").IsMatch(val);
        }
    }
}