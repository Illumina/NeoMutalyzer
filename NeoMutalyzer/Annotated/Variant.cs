using ReferenceSequence;

namespace NeoMutalyzer.Annotated
{
    public sealed class Variant
    {
        public readonly Chromosome   Chromosome;
        public readonly int          Begin;
        public readonly int          End;
        public readonly string       RefAllele;
        public readonly string       AltAllele;
        public readonly VariantType  Type;
        public readonly string       HgvsGenomic;
        public readonly Transcript[] Transcripts;

        public Variant(Chromosome chromosome, int begin, int end, string refAllele, string altAllele, VariantType type,
            string hgvsGenomic, Transcript[] transcripts)
        {
            Chromosome  = chromosome;
            Begin       = begin;
            End         = end;
            RefAllele   = refAllele;
            AltAllele   = altAllele;
            Type        = type;
            HgvsGenomic = hgvsGenomic;
            Transcripts = transcripts;
        }
    }
}