using System;
using System.Collections.Generic;
using System.IO;
using ExtractTranscriptsFromGffSnapshots.GFF;
using IO;
using ReferenceSequence;

namespace ExtractTranscriptsFromGffSnapshots
{
    public static class GffCrawler
    {
        public static void ParseTranscripts(string searchPath, Dictionary<string, Transcript> idToTranscript,
            Dictionary<string, Chromosome> refNameToChromosome)
        {
            foreach (string directory in Directory.GetDirectories(searchPath))
            {
                ParseDirectory(directory, idToTranscript, refNameToChromosome);
            }
        }

        private static void ParseDirectory(string directoryPath, Dictionary<string, Transcript> idToTranscript,
            Dictionary<string, Chromosome> refNameToChromosome)
        {
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (!IsParsingSupported(filePath)) continue;
                Console.WriteLine($"- parsing {filePath}");

                bool onlyAddEmpty = filePath.Contains("72505");
                
                ParseGff(filePath, idToTranscript, refNameToChromosome, onlyAddEmpty);
            }
        }

        private static bool IsParsingSupported(string filePath) => filePath.EndsWith(".gff.gz")  ||
                                                                   filePath.EndsWith(".gff3.gz") ||
                                                                   filePath.EndsWith(".gff3");

        private static void ParseGff(string filePath, Dictionary<string, Transcript> idToTranscript,
            Dictionary<string, Chromosome> refNameToChromosome, bool onlyAddEmpty)
        {
            using StreamReader reader = filePath.EndsWith(".gz")
                ? FileUtilities.GzipReader(filePath)
                : FileUtilities.StreamReader(filePath);

            var numExonsAdded       = 0;
            var numTranscriptsAdded = 0;

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

                int    start           = int.Parse(cols[3]);
                int    end             = int.Parse(cols[4]);
                bool   onReverseStrand = cols[6] == "-";
                string info            = cols[8];

                string[] infoCols = info.Split(';');
                (string target, string gffId) = GetTargetAndGffId(infoCols);

                if (gffId  == null) throw new InvalidDataException($"Found a null GFF ID in [{line}]");
                if (target == null) throw new InvalidDataException($"Found a null target in [{line}]");

                (string transcriptId, int cdnaStart, int cdnaEnd) = GetCdnaCoordinates(target);

                if (!transcriptId.StartsWith("NM_") &&
                    !transcriptId.StartsWith("NR_") &&
                    !transcriptId.StartsWith("XM_") &&
                    !transcriptId.StartsWith("XR_")) continue;

                // create new transcript entry
                if (!idToTranscript.TryGetValue(transcriptId, out Transcript transcript))
                {
                    transcript                   = new Transcript(transcriptId);
                    idToTranscript[transcriptId] = transcript;
                }

                bool needGffEntry = !transcript.GffPathToFile.TryGetValue(filePath, out GffFile gffFile);

                // if we already have information from another GFF, skip this entry
                if (needGffEntry && transcript.GffPathToFile.Count > 0 && onlyAddEmpty) continue;

                if (needGffEntry)
                {
                    gffFile                            = new GffFile();
                    transcript.GffPathToFile[filePath] = gffFile;
                    numTranscriptsAdded++;
                }

                // create new UUID entry
                if (!gffFile.UuidToGeneModel.TryGetValue(gffId, out GeneModel geneModel))
                {
                    geneModel                      = new GeneModel(chromosome, transcriptId);
                    gffFile.UuidToGeneModel[gffId] = geneModel;
                }

                // add the exon
                geneModel.Exons.Add(new Exon(start, end, cdnaStart, cdnaEnd, onReverseStrand));
                numExonsAdded++;
            }

            Console.WriteLine($"  - {numTranscriptsAdded:N0} transcripts added.");
            Console.WriteLine($"  - {numExonsAdded:N0} exons added.");
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
    }
}