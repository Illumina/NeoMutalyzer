using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoMutalyzerShared;

namespace NeoMutalyzer
{
    internal static class NeoMutalyzerMain
    {
        private static void Main()
        {
            const string transcriptDataPath = @"D:\Projects\NeoMutalyzer\Data\Cache26_transcripts.tsv.gz";

            Console.Write("- loading transcripts... ");
            Dictionary<string, Transcript> idToTranscript = LoadTranscripts(transcriptDataPath);
            Console.WriteLine($"{idToTranscript.Count:N0} loaded.");

            Transcript transcript = idToTranscript["NM_000314.6"];
            
            var annotated_NM_000314_6 = new AnnotatedTranscript(
                "NM_000314.6",
                VariantType.deletion,
                "TACT",
                "",
                "VL",
                "X",
                new Interval(1981, 1984),
                new Interval(950,  953),
                new Interval(317,  318),
                "NM_000314.6:c.956_959delACTT",
                "NP_000305.3:p.(Thr319Ter)");

            TranscriptValidator.Validate(transcript, annotated_NM_000314_6);
        }

        private static Dictionary<string, Transcript> LoadTranscripts(string filePath)
        {
            var idToTranscript = new Dictionary<string, Transcript>();

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

                idToTranscript[id] = new Transcript(id, cdnaSequence, cdsSequence, aaSequence);
            }

            return idToTranscript;
        }
    }
}