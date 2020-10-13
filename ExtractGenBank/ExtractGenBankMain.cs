using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtractGenBank.GenBank;
using IO;
using NeoMutalyzerShared.GenBank;

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
            Dictionary<string, GenBankTranscript> idToTranscript = LoadTranscripts(inputGenBankPath);
            Console.WriteLine($"{idToTranscript.Count} transcripts loaded.");

            Console.Write($"- writing transcripts to {Path.GetFileName(outputDataPath)}... ");
            WriteTranscripts(idToTranscript, outputDataPath);
            Console.WriteLine("finished.");
        }

        private static void WriteTranscripts(Dictionary<string, GenBankTranscript> idToTranscript, string filePath)
        {
            using StreamWriter writer = FileUtilities.GzipWriter(filePath);

            foreach ((_, GenBankTranscript transcript) in idToTranscript.OrderBy(x => x.Key))
            {
                writer.WriteLine(transcript);
            }
        }

        private static Dictionary<string, GenBankTranscript> LoadTranscripts(string filePath)
        {
            using var reader = new GenBankReader(FileUtilities.GetReadStream(filePath));
            return reader.GetIdToTranscript();
        }
    }
}