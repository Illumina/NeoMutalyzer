using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtractGenBank.GenBank;
using IO;
using Newtonsoft.Json;

namespace ExtractGenBank
{
    internal static class ExtractGenBankMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <input GenBank path> <output data path>");
                Environment.Exit(1);
            }

            string inputGenBankPath = args[0];
            string outputDataPath   = args[1];

            if (!outputDataPath.EndsWith(".gz")) outputDataPath += ".gz";

            Console.Write("- parsing GenBank file... ");
            List<RefSeq.Transcript> transcripts = LoadTranscripts(inputGenBankPath);
            Console.WriteLine($"{transcripts.Count} transcripts loaded.");

            Console.Write($"- writing transcripts to {Path.GetFileName(outputDataPath)}... ");
            WriteTranscripts(outputDataPath, transcripts);
            Console.WriteLine("finished.");
        }

        private static void WriteTranscripts(string filePath, List<RefSeq.Transcript> transcripts)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling    = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting           = Formatting.Indented
            };

            using StreamWriter writer = FileUtilities.GzipWriter(filePath);
            serializer.Serialize(writer,
                transcripts.OrderBy(x => x.id.Length).ThenBy(x => x.id));
        }

        private static List<RefSeq.Transcript> LoadTranscripts(string filePath)
        {
            using var reader = new GenBankReader(FileUtilities.GetReadStream(filePath));
            return reader.GetTranscripts();
        }
    }
}