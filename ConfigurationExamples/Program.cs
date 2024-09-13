using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace ConfigurationExamples
{
    internal class Program
    {
        public static void Main(string[] args) {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // In Microsoft.Extensions.Configuration.FileExtensions
                .AddJsonFile("appsettings.json") // In Microsoft.Extensions.Configuration.Json
                .AddEnvironmentVariables() // In Microsoft.Extensions.Configuration.EnvironmentVaiables
                .AddCommandLine(args) // In Microsoft.Extensions.Configuration.CommandLine
                .Build();

            ListFromConfigurationExample(configuration);
            DictionaryFromConfigurationExample(configuration);
            ObjectFromConfigurationExample(configuration);
            Console.Write("\nDone, press [enter]...");
            Console.ReadLine();
        }

        static void ListFromConfigurationExample(IConfigurationRoot configuration) {
            Console.WriteLine($"\n{nameof(ListFromConfigurationExample)}");
            var values = configuration.GetSection("ListValue").Get<List<string>>(); // In Microsoft.Extensions.Configuration.Binder
            for (var i = 0; i < values.Count; ++i)
                Console.WriteLine($"[{i}]: {values[i]}");
        }

        static void DictionaryFromConfigurationExample(IConfigurationRoot configuration) {
            Console.WriteLine($"\n{nameof(DictionaryFromConfigurationExample)}");
            var values = configuration.GetSection("DictionaryValue").Get<Dictionary<string, string>>(); // In Microsoft.Extensions.Configuration.Binder
            foreach (var key in values.Keys)
                Console.WriteLine($"{key}: {values[key]}");
        }

        static void ObjectFromConfigurationExample(IConfigurationRoot configuration) {
            Console.WriteLine($"\n{nameof(ObjectFromConfigurationExample)}");
            var value = configuration.GetSection("AppSettings").Get<AppSettings>(); // In Microsoft.Extensions.Configuration.Binder
            Console.WriteLine($"{nameof(AppSettings.UploadTimeoutSeconds)}: {value.UploadTimeoutSeconds}");
            Console.WriteLine($"{nameof(AppSettings.DefaultLogLevel)}: {value.DefaultLogLevel}");
            Console.WriteLine($"{nameof(AppSettings.SupportEmailAddresses)}: {string.Join(", ", value.SupportEmailAddresses)}");
        }
    }
}
