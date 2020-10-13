using System.Collections.Generic;

namespace ExtractTranscriptsFromGffSnapshots.GFF
{
    public class Transcript
    {
        public readonly string                      Id;
        public readonly Dictionary<string, GffFile> GffPathToFile = new Dictionary<string, GffFile>();

        public Transcript(string id) => Id = id;
    }
}