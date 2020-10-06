using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    public sealed class Gene : IFeature
    {
        public int        Start      { get; }
        public int        End        { get; }
        public string     GeneSymbol { get; }
        public string     LocusTag   { get; }
        public string     Product    { get; }
        public string     ProteinId  { get; }
        public Interval[] Regions    { get; }
        public string     Note       { get; }

        public Gene(Interval interval, string geneSymbol)
        {
            GeneSymbol = geneSymbol;
            Start      = interval.Start;
            End        = interval.End;
        }
    }
}