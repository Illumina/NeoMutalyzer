using System;
using System.Collections.Generic;
using System.IO;
using IO;
using NeoMutalyzerShared.Annotated;
using NeoMutalyzerShared.GenBank;
using NeoMutalyzerShared.NirvanaJson;
using NeoMutalyzerShared.Utilities;
using NeoMutalyzerShared.Validation;
using ReferenceSequence;

namespace NeoMutalyzer
{
    internal static class NeoMutalyzerMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <RefSeq JSON path> <reference path> <Nirvana JSON path>");
                Environment.Exit(1);
            }

            string refseqJsonPath  = args[0];
            string referencePath   = args[1];
            string nirvanaJsonPath = args[2];
            
            var benchmark = new Benchmark();

            Console.Write("- loading reference sequences... ");
            ReferenceNameReader.Load(referencePath);
            Dictionary<string, Chromosome> refNameToChromosome = ReferenceNameReader.RefNameToChromosome;
            Console.WriteLine($"{refNameToChromosome.Count:N0} loaded.");
            
            Console.Write("- loading transcripts... ");
            Dictionary<string, RefSeq.ITranscript> idToTranscript = GenBankDataReader.Load(refseqJsonPath);
            Console.WriteLine($"{idToTranscript.Count:N0} loaded.");

            using var parser = new NirvanaJsonParser(FileUtilities.GetReadStream(nirvanaJsonPath), refNameToChromosome);
            
            while (true)
            {
                Position position = parser.GetPosition();
                if (position == null) break;
                
                TranscriptValidator.Validate(position, idToTranscript, geneId => false);
            }

            TranscriptValidator.DisplayStatistics();
            
            Console.WriteLine();
            Console.WriteLine($"  - elapsed time: {benchmark.GetElapsedTime()}");
            Console.WriteLine($"  - current RAM:  {MemoryUtilities.GetCurrentMemoryUsage()}");
            Console.WriteLine($"  - peak RAM:     {MemoryUtilities.GetPeakMemoryUsage()}\n");
        }
    }
}