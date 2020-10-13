using System;
using System.Collections.Generic;
using ExtractTranscriptsFromGffSnapshots.GFF;

namespace ExtractTranscriptsFromGffSnapshots
{
    public static class TranscriptAnalyzer
    {
        public static void Analyze(Dictionary<string, Transcript> idToTranscript)
        {
            int numTotalTranscripts                  = idToTranscript.Count;
            var numTranscriptsFound                  = 0;
            var numTranscriptsWithMultipleGeneModels = 0;
            
            foreach (Transcript transcript in idToTranscript.Values)
            {
                int numGffFiles = transcript.GffPathToFile.Count;
                if (numGffFiles == 0)
                {
                    Console.WriteLine($"  - {transcript.Id} did not show up in any GFF file.");
                    continue;
                }

                numTranscriptsFound++;

                var geneModels = new HashSet<GeneModel>();
                var hasChrX    = false;
                var hasChrY    = false;

                foreach (GffFile gffFile in transcript.GffPathToFile.Values)
                {
                    foreach (GeneModel geneModel in gffFile.UuidToGeneModel.Values)
                    {
                        switch (geneModel.Chromosome.UcscName)
                        {
                            case "chrX":
                                hasChrX = true;
                                break;
                            case "chrY":
                                hasChrY = true;
                                break;
                        }

                        geneModels.Add(geneModel);
                    }
                }

                if (geneModels.Count > 1 && !(hasChrX && hasChrY))
                {
                    Console.WriteLine($"  - {transcript.Id} has {geneModels.Count} gene models");
                    foreach(GeneModel geneModel in geneModels) Console.WriteLine(geneModel);

                    numTranscriptsWithMultipleGeneModels++;
                }
            }

            double percentFound            = numTranscriptsFound / (double)numTotalTranscripts * 100.0;
            int    numTranscriptsRemaining = numTotalTranscripts - numTranscriptsFound;
            
            Console.WriteLine($"  - {numTranscriptsFound:N0} found / {numTotalTranscripts:N0} total ({percentFound:0.000}%)");
            Console.WriteLine($"  - {numTranscriptsRemaining:N0} remaining");
            Console.WriteLine($"  - {numTranscriptsWithMultipleGeneModels:N0} transcripts with more than 1 gene model");
        }
    }
}