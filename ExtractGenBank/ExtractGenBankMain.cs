using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ExtractGenBank.GenBank;
using NeoMutalyzerShared;

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
            Dictionary<string, Transcript> idToTranscript = LoadTranscripts(inputGenBankPath);
            Console.WriteLine($"{idToTranscript.Count} transcripts loaded.");

            Console.Write($"- writing transcripts to {Path.GetFileName(outputDataPath)}... ");
            WriteTranscripts(idToTranscript, outputDataPath);
            Console.WriteLine("finished.");
        }

        private static void WriteTranscripts(Dictionary<string, Transcript> idToTranscript, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(new GZipStream(stream, CompressionLevel.Optimal));

            foreach ((_, Transcript transcript) in idToTranscript.OrderBy(x => x.Key))
            {
                writer.WriteLine(transcript);
            }
        }

        private static Dictionary<string, Transcript> LoadTranscripts(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new GenBankReader(stream);
            return reader.GetIdToTranscript();
        }
    }
}