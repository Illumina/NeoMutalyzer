﻿using NeoMutalyzer.Annotated;
using NeoMutalyzer.HgvsParsing;
using NeoMutalyzerShared;

namespace NeoMutalyzer.Validation
{
    public static class HgvsCodingValidator
    {
        public static void ValidateHgvsCoding(this ValidationResult result, IGenBankTranscript genBankTranscript,
            string hgvsCoding, Interval expectedRightCdsPos, VariantType variantType)
        {
            if (variantType == VariantType.insertion) return;
            
            (CodingInterval hgvsInterval, string hgvsRef, string _, bool isCoding) =
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

            if (hgvsRef != bases) result.HasHgvsCodingRefAlleleError = true;

            // check the expected position. We need to take into account that some CDS need to be rotated outside.
            if (expectedRightCdsPos != null && isCoding && !hgvsInterval.Equals(expectedRightCdsPos))
                result.HasHgvsCodingPositionError = true;
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