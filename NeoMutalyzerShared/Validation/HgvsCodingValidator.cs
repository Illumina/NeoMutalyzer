using NeoMutalyzerShared.Annotated;
using NeoMutalyzerShared.GenBank;
using NeoMutalyzerShared.HgvsParsing;

namespace NeoMutalyzerShared.Validation
{
    public static class HgvsCodingValidator
    {
        public static void ValidateHgvsCoding(this ValidationResult result, IGenBankTranscript genBankTranscript,
            string hgvsCoding, Interval expectedRightCdsPos, VariantType variantType, bool overlapsIntronAndExon,
            bool isSpliceVariant)
        {
            if (variantType == VariantType.insertion) return;
            
            (CodingInterval hgvsInterval, string hgvsRef, string hgvsAlt, bool isCoding) =
                HgvsCodingParser.Parse(hgvsCoding);

            // we can't do much for intronic positions
            if (hgvsInterval.Start.Offset != 0 || hgvsInterval.End.Offset != 0) return;

            // if our variant is beyond the coding end or has a negative start position, convert to cDNA
            if (isCoding && IsCdnaSwitchNeeded(hgvsInterval))
            {
                if (HasConflictingInfo(hgvsInterval))
                {
                    result.HasHgvsCodingBeforeCdsAndAfterCds = true;
                    return;
                }                
                
                SwitchToCdnaSequence(ref hgvsInterval, genBankTranscript.CodingRegion);
                isCoding = false;
            }

            // we need to use the cDNA sequence if we encounter HGVS n. notation
            string bases = isCoding
                ? genBankTranscript.GetCds(hgvsInterval.Start.Position, hgvsInterval.End.Position)
                : genBankTranscript.GetCdna(hgvsInterval.Start.Position, hgvsInterval.End.Position);

            bool isSilentVariant = string.IsNullOrEmpty(hgvsRef) && string.IsNullOrEmpty(hgvsAlt);
            bool isDelOrDelIns   = hgvsCoding.Contains("delins") || hgvsCoding.Contains("del");
            
            if (!isSilentVariant && !isDelOrDelIns && hgvsRef != bases) result.HasHgvsCodingRefAlleleError = true;

            // check the expected position. We need to take into account that some CDS need to be rotated outside.
            if (!overlapsIntronAndExon && !isSpliceVariant && expectedRightCdsPos != null && isCoding &&
                !hgvsInterval.Equals(expectedRightCdsPos)) result.HasHgvsCodingPositionError = true;
        }

        private static bool HasConflictingInfo(CodingInterval interval) =>
            HasConflictingInfo(interval.Start) || HasConflictingInfo(interval.End);
        
        private static bool HasConflictingInfo(PositionOffset po) => po.BeyondCodingEnd && po.Position < 1;

        private static bool IsCdnaSwitchNeeded(CodingInterval interval) =>
            IsCdnaSwitchNeeded(interval.Start) || IsCdnaSwitchNeeded(interval.End);

        private static bool IsCdnaSwitchNeeded(PositionOffset po) => po.BeyondCodingEnd || po.Position < 1;

        private static void SwitchToCdnaSequence(ref CodingInterval hgvsInterval, Interval codingRegion)
        {
            PositionOffset start = hgvsInterval.Start.ConvertToCdna(codingRegion);
            PositionOffset end   = hgvsInterval.End.ConvertToCdna(codingRegion);
            hgvsInterval = new CodingInterval(start, end);
        }
    }
}