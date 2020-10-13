using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtractTranscriptsFromGffSnapshots.GFF;
using NeoMutalyzerShared;
using Newtonsoft.Json;
using ReferenceSequence;
using RefSeq;

namespace ExtractTranscriptsFromGffSnapshots
{
    internal static class ExtractTranscriptMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <RefSeq IDs path> <reference path> <root search path> <output path>");
                Environment.Exit(1);
            }

            string refSeqIdPath   = args[0];
            string referencePath  = args[1];
            string rootSearchPath = args[2];
            string outputPath     = args[3];
            
            // if (!outputPath.EndsWith(".gz")) outputPath += ".gz";
            
            Console.Write("- loading reference sequences... ");
            ReferenceNameReader.Load(referencePath);
            Dictionary<string, Chromosome> refNameToChromosome = ReferenceNameReader.RefNameToChromosome;
            Console.WriteLine($"{refNameToChromosome.Count:N0} loaded.");

            Console.Write("- parsing RefSeq transcript file... ");
            Dictionary<string, GFF.Transcript> idToTranscript = LoadRefSeqIds(refSeqIdPath);
            Console.WriteLine($"{idToTranscript.Count:N0} transcript IDs loaded.");

            GffCrawler.ParseTranscripts(rootSearchPath, idToTranscript, refNameToChromosome);

            TranscriptAnalyzer.Analyze(idToTranscript);

            List<RefSeq.Transcript> transcripts = CreateRefSeqTranscripts(idToTranscript);
            
            Console.Write("- writing transcripts... ");
            WriteTranscripts(outputPath, transcripts);
            Console.WriteLine("finished.");
        }

        private static void WriteTranscripts(string filePath, List<RefSeq.Transcript> transcripts)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting        = Formatting.Indented
            };

            using StreamWriter writer = FileUtilities.StreamWriter(filePath);
            serializer.Serialize(writer,
                transcripts.OrderBy(x => x.chromosome.Index).ThenBy(x => x.start).ThenBy(x => x.end));
        }

        private static List<RefSeq.Transcript> CreateRefSeqTranscripts(Dictionary<string, GFF.Transcript> idToTranscript)
        {
            var transcripts = new List<RefSeq.Transcript>();

            foreach (GFF.Transcript gffTranscript in idToTranscript.Values)
            {
                foreach (GffFile gffFile in gffTranscript.GffPathToFile.Values)
                {
                    foreach (GeneModel geneModel in gffFile.UuidToGeneModel.Values)
                    {
                        RefSeq.Transcript transcript = ConvertToRefSeqTranscript(geneModel);
                        transcripts.Add(transcript);
                    }
                }
            }

            return transcripts;
        }

        private static RefSeq.Transcript ConvertToRefSeqTranscript(GeneModel geneModel)
        {
            TranscriptRegion[] transcriptRegions = GetTranscriptRegions(geneModel.Exons);

            int  start           = transcriptRegions[0].start;
            int  end             = transcriptRegions[^1].end;
            bool onReverseStrand = geneModel.Exons[0].onReverseStrand;
            var  numExons        = (ushort) geneModel.Exons.Count;

            return new RefSeq.Transcript(geneModel.TranscriptId, geneModel.Chromosome, start, end, onReverseStrand,
                numExons, transcriptRegions);
        }

        private static TranscriptRegion[] GetTranscriptRegions(List<Exon> exons)
        {
            var    transcriptRegions = new List<TranscriptRegion>(exons.Count);
            ushort exonIndex         = 0;

            foreach (Exon exon in exons.OrderBy(x => x.cdnaStart))
            {
                var transcriptRegion = new TranscriptRegion(TranscriptRegionType.Exon, exonIndex, exon.start, exon.end,
                    exon.cdnaStart, exon.cdnaEnd);
                transcriptRegions.Add(transcriptRegion);
                exonIndex++;
            }

            return transcriptRegions.OrderBy(x => x.start).ToArray();
        }

        private static Dictionary<string, GFF.Transcript> LoadRefSeqIds(string filePath)
        {
            using StreamReader reader = FileUtilities.StreamReader(filePath);

            var idToTranscript = new Dictionary<string, GFF.Transcript>();

            while (true)
            {
                string refSeqId = reader.ReadLine();
                if (refSeqId == null) break;
                idToTranscript[refSeqId] = new GFF.Transcript(refSeqId);
            }

            return idToTranscript;
        }
    }
}