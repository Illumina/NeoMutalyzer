namespace NeoMutalyzerShared
{
    public sealed class Transcript
    {
        public readonly string Id;
        public readonly string CdnaSequence;
        public readonly string CdsSequence;
        public readonly string AminoAcidSequence;

        public Transcript(string id, string cdnaSequence, int? cdsStart, int? cdsEnd, string aaSequence)
        {
            Id                = id;
            CdnaSequence      = cdnaSequence;
            AminoAcidSequence = aaSequence;

            if (cdsStart == null || cdsEnd == null) return;

            int cdsLength = cdsEnd.Value - cdsStart.Value + 1;

            CdsSequence = cdnaSequence.Substring(cdsStart.Value - 1, cdsLength);
        }
        
        public Transcript(string id, string cdnaSequence, string cdsSequence, string aaSequence)
        {
            Id                = id;
            CdnaSequence      = cdnaSequence;
            CdsSequence       = cdsSequence;
            AminoAcidSequence = aaSequence;
        }

        public string GetCdna(in Interval interval) => CdnaSequence.Substring(interval.Start - 1, interval.Length);
        public string GetCds(in Interval interval)  => CdsSequence.Substring(interval.Start  - 1, interval.Length);

        public string GetAminoAcids(in Interval interval) =>
            AminoAcidSequence.Substring(interval.Start - 1, interval.Length);

        public override string ToString() => $"{Id}\t{CdnaSequence}\t{CdsSequence}\t{AminoAcidSequence}";
    }
}