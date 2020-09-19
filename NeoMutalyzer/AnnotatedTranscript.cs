using NeoMutalyzerShared;

namespace NeoMutalyzer
{
    public sealed class AnnotatedTranscript
    {
        public readonly string      Id;
        public readonly VariantType Type;
        public readonly string      RefAllele;
        public readonly string      AltAllele;
        public readonly string      RefAminoAcids;
        public readonly string      AltAminoAcids;
        public readonly Interval    CdnaPos;
        public readonly Interval    CdsPos;
        public readonly Interval    AminoAcidPos;
        public readonly string      HgvsCoding;
        public readonly string      HgvsProtein;

        public AnnotatedTranscript(string id, VariantType type, string refAllele, string altAllele,
            string refAminoAcids, string altAminoAcids, Interval cdnaPos, Interval cdsPos, Interval aminoAcidPos,
            string hgvsCoding, string hgvsProtein)
        {
            Id            = id;
            Type          = type;
            RefAllele     = refAllele;
            AltAllele     = altAllele;
            RefAminoAcids = refAminoAcids;
            AltAminoAcids = altAminoAcids;
            CdnaPos       = cdnaPos;
            CdsPos        = cdsPos;
            AminoAcidPos  = aminoAcidPos;
            HgvsCoding    = hgvsCoding;
            HgvsProtein   = hgvsProtein;
        }
    }
}