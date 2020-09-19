using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoMutalyzer.Annotated;
using NeoMutalyzer.NirvanaJson;
using NeoMutalyzer.Validation;
using NeoMutalyzerShared;
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
                Console.WriteLine($"{programName} <GenBank data path> <reference path> <Nirvana JSON path>");
                Environment.Exit(1);
            }

            string transcriptDataPath = args[0];
            string referencePath      = args[1];
            string nirvanaJsonPath    = args[2];

            Console.Write("- loading reference sequences... ");
            ReferenceNameReader.Load(referencePath);
            Dictionary<string, Chromosome> refNameToChromosome = ReferenceNameReader.RefNameToChromosome;
            Console.WriteLine($"{refNameToChromosome.Count:N0} loaded.");
            
            Console.Write("- loading transcripts... ");
            Dictionary<string, GenBankTranscript> idToTranscript = LoadTranscripts(transcriptDataPath);
            Console.WriteLine($"{idToTranscript.Count:N0} loaded.");

            using var stream = new FileStream(nirvanaJsonPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var parser = new NirvanaJsonParser(stream, refNameToChromosome);
            
            while (true)
            {
                Position position = parser.GetPosition();
                if (position == null) break;
             
                // TranscriptValidator.Validate(position, idToTranscript);
            }

            // Transcript transcript = idToTranscript["NM_000314.6"];
            //
            // var annotated_NM_000314_6 = new Annotated.Transcript(
            //     "NM_000314.6",
            //     VariantType.deletion,
            //     "TACT",
            //     "",
            //     "VL",
            //     "X",
            //     new Interval(1981, 1984),
            //     new Interval(950,  953),
            //     new Interval(317,  318),
            //     "NM_000314.6:c.956_959delACTT",
            //     "NP_000305.3:p.(Thr319Ter)");
            //
            // TranscriptValidator.Validate(transcript, annotated_NM_000314_6);
        }

        private static Dictionary<string, GenBankTranscript> LoadTranscripts(string filePath)
        {
            var idToTranscript = new Dictionary<string, GenBankTranscript>();

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress));

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                string[] cols = line.Split('\t');

                string id           = cols[0];
                string cdnaSequence = cols[1];
                string cdsSequence  = cols[2];
                string aaSequence   = cols[3];

                idToTranscript[id] = new GenBankTranscript(id, cdnaSequence, cdsSequence, aaSequence);
            }

            return idToTranscript;
        }
    }
}