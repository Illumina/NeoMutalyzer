using System;
using System.Collections.Generic;
using System.IO;
using NeoMutalyzerShared;
using ReferenceSequence;

namespace TranscriptTimeMachine
{
    public static class GffCrawler
    {
        public static Dictionary<string, TranscriptGeneModel> GetKeyToGeneModels(List<string> directories,
            string prefix, HashSet<string> remainingRefSeqIds, Dictionary<string, Chromosome> refNameToChromosome)
        {
            var keyToGeneModels = new Dictionary<string, TranscriptGeneModel>();
            int totalRefSeqIds  = remainingRefSeqIds.Count;

            foreach (string directory in directories)
            {
                string directoryPath = Path.Combine(prefix, directory);

                foreach (string filePath in Directory.GetFiles(directoryPath))
                {
                    if (!IsParsingSupported(filePath)) continue;
                    Console.WriteLine($"- parsing {filePath}");
                    ParseGff(filePath, remainingRefSeqIds, keyToGeneModels, refNameToChromosome);
                    ShowProgress(remainingRefSeqIds.Count, totalRefSeqIds);
                }
            }

            return keyToGeneModels;
        }

        private static bool IsParsingSupported(string filePath) => filePath.EndsWith(".gff.gz")  ||
                                                                   filePath.EndsWith(".gff3.gz") ||
                                                                   filePath.EndsWith(".gff3");

        private static void ParseGff(string filePath, HashSet<string> remainingRefSeqIds,
            Dictionary<string, TranscriptGeneModel> keyToGeneModels, Dictionary<string, Chromosome> refNameToChromosome)
        {
            using StreamReader reader = filePath.EndsWith(".gz")
                ? FileUtilities.GzipReader(filePath)
                : FileUtilities.StreamReader(filePath);

            var gffIdToGeneModels = new Dictionary<string, TranscriptGeneModel>();
            var numTargetsFound   = 0;

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                if (line.StartsWith("#")) continue;

                string[] cols = line.Split('\t');
                string   type = cols[2];
                if (type != "cDNA_match" && type != "match") continue;

                string     accession  = cols[0];
                Chromosome chromosome = ReferenceNameUtilities.GetChromosome(refNameToChromosome, accession);
                if (chromosome == null || chromosome.Index > 23) continue;

                int    start = int.Parse(cols[3]);
                int    end   = int.Parse(cols[4]);
                string info  = cols[8];

                string[] infoCols = info.Split(';');
                (string target, string gffId) = GetTargetAndGffId(infoCols);

                if (gffId  == null) throw new InvalidDataException($"Found a null GFF ID in [{line}]");
                if (target == null) throw new InvalidDataException($"Found a null target in [{line}]");

                // handle duplicate of NM_001282321.1 on chr1 @ 16999498
                if (gffId == "898ad271-9913-4c37-b742-b67b33770b2c") continue;

                numTargetsFound++;

                (string transcriptId, int cdnaStart, int cdnaEnd) = GetCdnaCoordinates(target);
                if (!remainingRefSeqIds.Contains(transcriptId)) continue;

                if (!gffIdToGeneModels.TryGetValue(gffId, out TranscriptGeneModel transcriptGeneModel))
                {
                    transcriptGeneModel      = new TranscriptGeneModel(transcriptId, chromosome);
                    gffIdToGeneModels[gffId] = transcriptGeneModel;
                }

                transcriptGeneModel.Exons.Add(new Exon(start, end, cdnaStart, cdnaEnd));
            }

            Console.WriteLine($"  - {gffIdToGeneModels.Count:N0} transcripts loaded.");

            CheckForDuplicates(gffIdToGeneModels, remainingRefSeqIds);
            AddToGeneModels(gffIdToGeneModels.Values, keyToGeneModels);
            
            Console.WriteLine($"  - {numTargetsFound:N0} targets found.");

            if (numTargetsFound == 0) throw new InvalidDataException($"Did not find any targets in {filePath}");
        }

        private static void AddToGeneModels(ICollection<TranscriptGeneModel> transcriptGeneModels,
            Dictionary<string, TranscriptGeneModel> keyToGeneModels)
        {
            foreach (TranscriptGeneModel transcriptGeneModel in transcriptGeneModels)
            {
                string key = transcriptGeneModel.Key;
                
                if (keyToGeneModels.ContainsKey(key))
                    throw new InvalidDataException(
                        $"Key ({transcriptGeneModel.Key}) already exists in keyToGeneModels");

                transcriptGeneModel.AddIdsAndSortExons();
                keyToGeneModels[key] = transcriptGeneModel;
            }
        }

        private static void CheckForDuplicates(Dictionary<string, TranscriptGeneModel> gffIdToGeneModels,
            HashSet<string> remainingRefSeqIds)
        {
            var transcriptKeyToIds = new Dictionary<string, HashSet<string>>();

            foreach ((string id, TranscriptGeneModel transcriptGeneModel) in gffIdToGeneModels)
            {
                if (!transcriptKeyToIds.TryGetValue(transcriptGeneModel.Key, out HashSet<string> ids))
                {
                    ids                                         = new HashSet<string>();
                    transcriptKeyToIds[transcriptGeneModel.Key] = ids;
                }

                ids.Add(id);
                remainingRefSeqIds.Remove(transcriptGeneModel.Id);
            }

            var foundError = false;

            foreach ((string transcriptKey, HashSet<string> ids) in transcriptKeyToIds)
            {
                if (ids.Count == 1) continue;
                Console.WriteLine($"  - transcript key: {transcriptKey}, count: {ids.Count}: {string.Join(", ", ids)}");
                foundError = true;
            }

            if (foundError) throw new InvalidDataException("Found a duplicate transcript in the GFF.");
        }

        private static (string TranscriptId, int CdnaStart, int CdnaEnd) GetCdnaCoordinates(string target)
        {
            string[] targetCols = target.Split(' ');

            string transcriptId = targetCols[0];
            int    cdnaStart    = int.Parse(targetCols[1]);
            int    cdnaEnd      = int.Parse(targetCols[2]);

            return (transcriptId, cdnaStart, cdnaEnd);
        }

        private static (string Target, string Id) GetTargetAndGffId(string[] infoCols)
        {
            string target = null;
            string id     = null;

            foreach (string col in infoCols)
            {
                if (col.StartsWith("ID=")) id         = col.Substring(3);
                if (col.StartsWith("Target=")) target = col.Substring(7);
            }

            return (target, id);
        }

        private static void ShowProgress(int numRemainingTranscriptIds, int totalRefSeqIds)
        {
            double percentRemaining = numRemainingTranscriptIds / (double) totalRefSeqIds * 100.0;
            Console.WriteLine($"  - remaining transcripts: {numRemainingTranscriptIds:N0} / {totalRefSeqIds:N0} ({percentRemaining:0.00}%)");
        }
    }
}