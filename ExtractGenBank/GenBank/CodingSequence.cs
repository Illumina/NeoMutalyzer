using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    public sealed class CodingSequence : IFeature
    {
        public int        Start      { get; }
        public int        End        { get; }
        public string     GeneSymbol { get; }
        public string     LocusTag   { get; }
        public string     Product    { get; }
        public string     ProteinId  { get; }
        public Interval[] Regions    { get; }
        public string     Note       { get; }
        public int        CodonStart { get; }

        private readonly string _geneId;
        public readonly  string Translation;

        public CodingSequence(Interval interval, string geneSymbol, string locusTag, string geneId, Interval[] regions,
            string note, int codonStart, string product, string proteinId, string translation)
        {
            Start       = interval.Start;
            End         = interval.End;
            GeneSymbol  = geneSymbol;
            LocusTag    = locusTag;
            _geneId     = geneId;
            Regions     = regions;
            Note        = note;
            CodonStart  = codonStart;
            Product     = product;
            ProteinId   = proteinId;
            Translation = translation;
        }
    }
}