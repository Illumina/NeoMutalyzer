namespace NeoMutalyzerShared.Annotated
{
    public sealed class Transcript
    {
        public readonly string   Id;
        public readonly string   GeneId;
        public readonly string   RefAllele;
        public readonly string   AltAllele;
        public readonly string   RefAminoAcids;
        public readonly string   AltAminoAcids;
        public readonly Interval CdnaPos;
        public readonly Interval CdsPos;
        public readonly Interval AminoAcidPos;
        public readonly string   HgvsCoding;
        public readonly string   HgvsProtein;
        public readonly bool     OverlapsIntronAndExon;
        public readonly bool     IsCanonical;
        public readonly string   Json;

        public Transcript(string id, string geneId, string refAllele, string altAllele, string refAminoAcids,
            string altAminoAcids, Interval cdnaPos, Interval cdsPos, Interval aminoAcidPos, string hgvsCoding,
            string hgvsProtein, bool overlapsIntronAndExon, bool isCanonical, string json)
        {
            Id                    = id;
            GeneId                = geneId;
            RefAllele             = refAllele;
            AltAllele             = altAllele;
            RefAminoAcids         = refAminoAcids;
            AltAminoAcids         = altAminoAcids;
            CdnaPos               = cdnaPos;
            CdsPos                = cdsPos;
            AminoAcidPos          = aminoAcidPos;
            HgvsCoding            = hgvsCoding;
            HgvsProtein           = hgvsProtein;
            OverlapsIntronAndExon = overlapsIntronAndExon;
            IsCanonical           = isCanonical;
            Json                  = json;
        }
    }
}