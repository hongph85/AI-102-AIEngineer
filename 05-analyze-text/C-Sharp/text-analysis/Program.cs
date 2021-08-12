using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

// Import namespaces
using Azure.Core;
using Azure.AI.TextAnalytics;
using Azure;
using System.Linq;

namespace text_analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
                string cogSvcKey = configuration["CognitiveServiceKey"];

                // Set console encoding to unicode
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;

                Uri uri = new Uri(cogSvcEndpoint);
                var credential = new AzureKeyCredential(cogSvcKey);
                // Create client using endpoint and key
                TextAnalyticsClient client = new TextAnalyticsClient(uri, credential);

                // Analyze each text file in the reviews folder
                var folderPath = Path.GetFullPath("./reviews");
                DirectoryInfo folder = new DirectoryInfo(folderPath);
                foreach (var file in folder.GetFiles("*.txt"))
                {
                    // Read the file contents
                    Console.WriteLine("\n-------------\n" + file.Name);
                    StreamReader sr = file.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine("\n" + text);

                    // Get language
                    DetectedLanguage lang = client.DetectLanguage(text);
                    Console.WriteLine($"language: {lang.Name}");

                    // Get sentiment
                    DocumentSentiment sentiments = client.AnalyzeSentiment(text);
                    Console.WriteLine($"Sentiments: {sentiments.Sentiment}");

                    // Get key phrases
                    KeyPhraseCollection keyphrase = client.ExtractKeyPhrases(text);
                    Console.WriteLine($"keyphrases: {string.Join(',', keyphrase.Select(x => x).ToArray())}");


                    // Get entities
                    CategorizedEntityCollection entities = client.RecognizeEntities(text);
                    Console.WriteLine($"Entities: {string.Join(',', entities.Select(x => x.Category + ":" + x.Text).ToArray())}");

                    // Get linked entities
                    LinkedEntityCollection linkedEntities = client.RecognizeLinkedEntities(text);
                    if (linkedEntities.Count > 0)
                    {
                        Console.WriteLine("\nLinks:");
                        foreach (LinkedEntity linkedEntity in linkedEntities)
                        {
                            Console.WriteLine($"\t{linkedEntity.Name} ({linkedEntity.Url})");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }
}
