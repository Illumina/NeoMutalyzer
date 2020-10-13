namespace NeoMutalyzerShared.GenBank
{
    public sealed class GenBankTranscript : IGenBankTranscript
    {
        public readonly string Id;
        public readonly string GeneSymbol;

        public string   CdnaSequence      { get; }
        public string   CdsSequence       { get; }
        public string   AminoAcidSequence { get; }
        public Interval CodingRegion      { get; }

        public GenBankTranscript(string id, string geneSymbol, string cdnaSequence, string cdsSequence,
            string aaSequence, Interval codingRegion)
        {
            Id                = id;
            GeneSymbol        = geneSymbol;
            CdnaSequence      = cdnaSequence;
            CdsSequence       = cdsSequence;
            AminoAcidSequence = aaSequence;
            CodingRegion      = codingRegion;
        }

        public string GetCdna(int start, int end)
        {
            if (start < 1 || end > CdnaSequence.Length) return null;
            return CdnaSequence.Substring(start - 1, Length(start, end));
        }

        public string GetCds(int start, int end)
        {
            if (CdsSequence == null || start < 1 || end > CdsSequence.Length) return null;
            return CdsSequence.Substring(start - 1, Length(start, end));
        }

        public string GetAminoAcids(int start, int end)
        {
            if (AminoAcidSequence == null || start < 1 || end > AminoAcidSequence.Length) return null;
            return AminoAcidSequence.Substring(start - 1, Length(start, end));
        }

        private static int Length(int start, int end) => end - start + 1;

        public override string ToString()
        {
            string codingRegion = CodingRegion == null ? "\t" : $"{CodingRegion.Start}\t{CodingRegion.End}";
            return $"{Id}\t{GeneSymbol}\t{CdnaSequence}\t{CdsSequence}\t{AminoAcidSequence}\t{codingRegion}";
        }
    }
}