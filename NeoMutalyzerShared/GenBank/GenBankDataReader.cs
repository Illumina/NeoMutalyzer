using System.Collections.Generic;
using IO;
using Newtonsoft.Json;

namespace NeoMutalyzerShared.GenBank
{
    public static class GenBankDataReader
    {
        public static Dictionary<string, RefSeq.ITranscript> Load(string filePath)
        {
            var       idToTranscript = new Dictionary<string, RefSeq.ITranscript>();
            using var reader         = new JsonTextReader(FileUtilities.GzipReader(filePath));
            
            var serializer = new JsonSerializer
            {
                NullValueHandling    = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting           = Formatting.Indented
            };

            var transcripts = serializer.Deserialize<List<RefSeq.Transcript>>(reader);
            foreach (RefSeq.Transcript transcript in transcripts) idToTranscript[transcript.id] = transcript;

            return idToTranscript;
        }
    }
}