using System;
using System.Collections.Generic;
using System.IO;
using IO;
using NeoMutalyzerShared.Annotated;
using NeoMutalyzerShared.GenBank;
using NeoMutalyzerShared.IO;
using NeoMutalyzerShared.NirvanaJson;
using NeoMutalyzerShared.Utilities;
using NeoMutalyzerShared.Validation;
using ReferenceSequence;

namespace NeoMutalyzerGeneFilter
{
    internal static class NeoMutalyzerGeneFilterMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <GenBank data path> <reference path> <Nirvana JSON path> <gene ID path>");
                Environment.Exit(1);
            }

            string transcriptDataPath = args[0];
            string referencePath      = args[1];
            string nirvanaJsonPath    = args[2];
            string geneIdPath         = args[3];
            
            var benchmark = new Benchmark();

            Console.Write("- loading reference sequences... ");
            ReferenceNameReader.Load(referencePath);
            Dictionary<string, Chromosome> refNameToChromosome = ReferenceNameReader.RefNameToChromosome;
            Console.WriteLine($"{refNameToChromosome.Count:N0} loaded.");
            
            Console.Write("- loading transcripts... ");
            Dictionary<string, GenBankTranscript> idToTranscript = GenBankDataReader.Load(transcriptDataPath);
            Console.WriteLine($"{idToTranscript.Count:N0} loaded.");
            
            Console.Write("- loading Entrez gene IDs... ");
            HashSet<string> entrezGeneIds = SimpleParser.GetHashSet(geneIdPath);
            Console.WriteLine($"{entrezGeneIds.Count:N0} loaded.");

            using var parser = new NirvanaJsonParser(FileUtilities.GetReadStream(nirvanaJsonPath), refNameToChromosome);
            
            while (true)
            {
                Position position = parser.GetPosition();
                if (position == null) break;

                TranscriptValidator.Validate(position, idToTranscript, geneId => !entrezGeneIds.Contains(geneId));
            }

            TranscriptValidator.DisplayStatistics();
            
            Console.WriteLine();
            Console.WriteLine($"  - elapsed time: {benchmark.GetElapsedTime()}");
            Console.WriteLine($"  - current RAM:  {MemoryUtilities.GetCurrentMemoryUsage()}");
            Console.WriteLine($"  - peak RAM:     {MemoryUtilities.GetPeakMemoryUsage()}\n");
        }
    }
}