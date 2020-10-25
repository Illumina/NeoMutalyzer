using NeoMutalyzerShared.HgvsParsing;
using RefSeq;

namespace NeoMutalyzerShared.Validation
{
    public static class HgvsProteinValidator
    {
        public static void ValidateHgvsProtein(this ValidationResult result, ITranscript refseqTranscript,
            string hgvsProtein)
        {
            if (hgvsProtein == null) return;

            ProteinInterval interval = HgvsProteinParser.Parse(hgvsProtein);
            if (interval == null) return;

            // evaluate each position independently. For the most part, HGVS p. lists the first and last ref AA
            result.ValidateProteinPosition(interval.Start, refseqTranscript);
            if (interval.End != null) result.ValidateProteinPosition(interval.End, refseqTranscript);

            // look for an invalid alt allele
            if (interval.AltAA == ProteinInterval.UnknownAA) result.HasHgvsProteinAltAlleleError = true;
            if (hgvsProtein.Contains("_?_")) result.HasHgvsProteinUnknownError                   = true;
        }

        private static void ValidateProteinPosition(this ValidationResult result, ProteinPosition position,
            ITranscript refseqTranscript)
        {
            string aa = refseqTranscript.GetAminoAcids(position.Position, position.Position);
            if (string.IsNullOrEmpty(aa)) return;

            if (position.RefAA != aa[0]) result.HasHgvsProteinRefAlleleError  = true;
        }
    }
}