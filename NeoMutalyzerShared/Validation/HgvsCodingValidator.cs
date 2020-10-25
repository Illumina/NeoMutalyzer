using NeoMutalyzerShared.Annotated;
using NeoMutalyzerShared.HgvsParsing;

namespace NeoMutalyzerShared.Validation
{
    public static class HgvsCodingValidator
    {
        public static void ValidateHgvsCoding(this ValidationResult result, RefSeq.ITranscript refseqTranscript,
            string hgvsCoding, Interval expectedRightCdsPos, VariantType variantType, bool overlapsIntronAndExon,
            bool isSpliceVariant, bool potentialCdsTruncation)
        {
            if (variantType == VariantType.insertion)
            {
                if (hgvsCoding.Contains("ins")) result.ValidateHgvsCodingInsertion(hgvsCoding, refseqTranscript);
                if (hgvsCoding.Contains("dup")) result.ValidateHgvsCodingDuplication(hgvsCoding, refseqTranscript);
                return;
            }
            
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
                
                SwitchToCdnaSequence(ref hgvsInterval, refseqTranscript.codingRegion);
                isCoding = false;
            }

            // we need to use the cDNA sequence if we encounter HGVS n. notation
            string bases = isCoding
                ? refseqTranscript.GetCds(hgvsInterval.Start.Position, hgvsInterval.End.Position)
                : refseqTranscript.GetCdna(hgvsInterval.Start.Position, hgvsInterval.End.Position);

            bool isRefAlleleEmpty = string.IsNullOrEmpty(hgvsRef);
            bool isSilentVariant  = isRefAlleleEmpty && string.IsNullOrEmpty(hgvsAlt);
            bool isDelOrDelIns    = (hgvsCoding.Contains("delins") || hgvsCoding.Contains("del")) && isRefAlleleEmpty;
            
            if (!isSilentVariant && !isDelOrDelIns && hgvsRef != bases) result.HasHgvsCodingRefAlleleError = true;

            // check the expected position. We need to take into account that some CDS need to be rotated outside.
            if (!overlapsIntronAndExon  && !isSpliceVariant && expectedRightCdsPos != null && isCoding &&
                !potentialCdsTruncation && !hgvsInterval.Equals(expectedRightCdsPos)) 
                result.HasHgvsCodingPositionError = true;
        }

        private static void ValidateHgvsCodingInsertion(this ValidationResult result, string hgvsCoding,
            RefSeq.ITranscript refseqTranscript)
        {
            (CodingInterval hgvsInterval, _, string hgvsAlt, bool isCoding) = HgvsCodingParser.Parse(hgvsCoding);

            // we can't do much for intronic positions
            if (hgvsInterval.Start.Offset != 0 || hgvsInterval.End.Offset != 0) return;

            if (isCoding) SwitchToCdnaSequence(ref hgvsInterval, refseqTranscript.codingRegion);

            // check the bases before or after to see if this is a duplication
            (string basesBeforeIns, string basesAfterIns) = GetBasesBeforeAndAfterIns(hgvsInterval, refseqTranscript, hgvsAlt.Length);
            if (basesBeforeIns == hgvsAlt || basesAfterIns == hgvsAlt) result.HasHgvsCodingInsToDupError = true;

            // check that insertion coordinates are consecutive
            int diff = hgvsInterval.End.Position - hgvsInterval.Start.Position;
            if (diff != 1) result.HasHgvsCodingInsPositionError = true;
        }

        private static void ValidateHgvsCodingDuplication(this ValidationResult result, string hgvsCoding,
            RefSeq.ITranscript refseqTranscript)
        {
            (CodingInterval hgvsInterval, _, _, bool isCoding) = HgvsCodingParser.Parse(hgvsCoding);

            // we can't do much for intronic positions
            if (hgvsInterval.Start.Offset != 0 || hgvsInterval.End.Offset != 0) return;

            if (isCoding) SwitchToCdnaSequence(ref hgvsInterval, refseqTranscript.codingRegion);

            var    dupInterval = new Interval(hgvsInterval.Start.Position, hgvsInterval.End.Position);
            string hgvsAlt     = refseqTranscript.GetCdna(dupInterval.Start, dupInterval.End);

            Interval rightDupInterval = VariantRotator.Right(dupInterval, "", hgvsAlt, VariantType.insertion, 
                refseqTranscript.cdnaSequence).ShiftedPosition;

            if (dupInterval.Start != rightDupInterval.Start) result.HasHgvsCodingDupPositionError = true;
        }

        private static (string BeforeBases, string AfterBases) GetBasesBeforeAndAfterIns(CodingInterval hgvsInterval,
            RefSeq.ITranscript refseqTranscript, int numBases)
        {
            int beforeStart = hgvsInterval.Start.Position - numBases + 1;
            int beforeEnd   = hgvsInterval.Start.Position;
            int afterStart  = hgvsInterval.End.Position;
            int afterEnd    = hgvsInterval.End.Position + numBases - 1;

            string before = refseqTranscript.GetCdna(beforeStart, beforeEnd);
            string after  = refseqTranscript.GetCdna(afterStart,  afterEnd);

            return (before, after);
        }

        private static bool HasConflictingInfo(CodingInterval interval) =>
            HasConflictingInfo(interval.Start) || HasConflictingInfo(interval.End);
        
        private static bool HasConflictingInfo(PositionOffset po) => po.BeyondCodingEnd && po.Position < 1;

        private static bool IsCdnaSwitchNeeded(CodingInterval interval) =>
            IsCdnaSwitchNeeded(interval.Start) || IsCdnaSwitchNeeded(interval.End);

        private static bool IsCdnaSwitchNeeded(PositionOffset po) => po.BeyondCodingEnd || po.Position < 1;

        private static void SwitchToCdnaSequence(ref CodingInterval hgvsInterval, RefSeq.CodingRegion codingRegion)
        {
            PositionOffset start = hgvsInterval.Start.ConvertToCdna(codingRegion);
            PositionOffset end   = hgvsInterval.End.ConvertToCdna(codingRegion);
            hgvsInterval = new CodingInterval(start, end);
        }
    }
}