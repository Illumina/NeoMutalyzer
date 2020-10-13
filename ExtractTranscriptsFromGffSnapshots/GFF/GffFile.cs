using System.Collections.Generic;

namespace ExtractTranscriptsFromGffSnapshots.GFF
{
    public sealed class GffFile
    {
        public readonly Dictionary<string, GeneModel> UuidToGeneModel = new Dictionary<string, GeneModel>();
    }
}