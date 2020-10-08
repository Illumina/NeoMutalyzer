using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoMutalyzerShared;
using ReferenceSequence;

namespace TranscriptTimeMachine
{
    internal static class TranscriptTimeMachineMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <RefSeq ID path> <directory manifest path> <output path> <reference path>");
                Environment.Exit(1);
            }

            string refSeqIdPath          = args[0];
            string directoryManifestPath = args[1];
            string outputPath            = args[2];
            string referencePath         = args[3];

            if (!outputPath.EndsWith(".gz")) outputPath += ".gz";
            
            Console.Write("- loading reference sequences... ");
            ReferenceNameReader.Load(referencePath);
            Dictionary<string, Chromosome> refNameToChromosome = ReferenceNameReader.RefNameToChromosome;
            Console.WriteLine($"{refNameToChromosome.Count:N0} loaded.");

            Console.Write("- parsing RefSeq transcript file... ");
            HashSet<string> remainingRefSeqIds = LoadTranscriptIds(refSeqIdPath);
            Console.WriteLine($"{remainingRefSeqIds.Count:N0} transcript IDs loaded.");

            Console.Write("- parsing directory manifest... ");
            List<string> directories = DirectoryManifest.Load(directoryManifestPath);
            Console.WriteLine($"{directories.Count:N0} directories loaded.");

            string prefix = Path.GetDirectoryName(directoryManifestPath);

            Dictionary<string, TranscriptGeneModel> keyToGeneModels =
                GffCrawler.GetKeyToGeneModels(directories, prefix, remainingRefSeqIds, refNameToChromosome);

            DisplayRemainingRefSeqIds(remainingRefSeqIds);
            
            Console.Write("- writing gene models... ");
            WriteGeneModels(outputPath, keyToGeneModels.Values);
            Console.WriteLine("finished.");
        }

        private static void DisplayRemainingRefSeqIds(HashSet<string> remainingRefSeqIds)
        {
            if (remainingRefSeqIds.Count == 0) return;

            Console.WriteLine("- remaining RefSeq transcript IDs:");
            foreach (string transcriptId in remainingRefSeqIds) Console.WriteLine($"  - {transcriptId}");
        }

        private static void WriteGeneModels(string filePath, ICollection<TranscriptGeneModel> transcriptGeneModels)
        {
            using StreamWriter writer = FileUtilities.GzipWriter(filePath);

            foreach (TranscriptGeneModel transcriptGeneModel in transcriptGeneModels
                .OrderBy(x => x.Chromosome.Index)
                .ThenBy(x => x.Start)
                .ThenBy(x => x.End))
            {
                writer.Write($"{transcriptGeneModel.Id}\t{transcriptGeneModel.Chromosome.UcscName}\t{transcriptGeneModel.Exons.Count}");

                foreach (Exon exon in transcriptGeneModel.Exons)
                {
                    writer.Write($"\t{exon.Id}\t{exon.Start}\t{exon.End}\t{exon.CdnaStart}\t{exon.CdnaEnd}");
                }

                writer.WriteLine();
            }
        }

        private static HashSet<string> LoadTranscriptIds(string filePath)
        {
            using StreamReader reader = FileUtilities.StreamReader(filePath);

            var remainingRefSeqIds = new HashSet<string>();

            while (true)
            {
                string refSeqId = reader.ReadLine();
                if (refSeqId == null) break;
                remainingRefSeqIds.Add(refSeqId);
            }

            return remainingRefSeqIds;
        }
    }
}