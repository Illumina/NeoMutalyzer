using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    public sealed class Exon : IFeature
    {
        public int Start { get; }
        public int End   { get; }

        // not used
        public string     GeneSymbol { get; }
        public string     LocusTag   { get; }
        public string     Product    { get; }
        public string     ProteinId  { get; }
        public Interval[] Regions    { get; }
        public string     Note       { get; }

        public Exon(Interval interval)
        {
            Start = interval.Start;
            End   = interval.End;
        }
    }
}