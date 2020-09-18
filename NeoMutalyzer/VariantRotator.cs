using System;

namespace NeoMutalyzer
{
    public static class VariantRotator
    {
        public static (Interval ShiftedPosition, string ShiftedRef, string ShiftedAlt, bool HasShifted) Right(
            Interval pos, string refAllele, string altAllele, VariantType variantType, string refSequence)
        {
            if (variantType != VariantType.deletion && variantType != VariantType.insertion)
                return (pos, refAllele, altAllele, false);

            ReadOnlySpan<char> refSpan = refSequence.AsSpan();

            string             rotatingBases = GetRotatingBases(refAllele, altAllele, variantType);
            ReadOnlySpan<char> combinedSpan  = GetCombinedSequence(pos, refSpan, rotatingBases);

            int shiftOffset;
            var hasShifted = false;

            int numRotatingBases = rotatingBases.Length;
            int maxCombinedBases = combinedSpan.Length - numRotatingBases;

            for (shiftOffset = 0; shiftOffset < maxCombinedBases; shiftOffset++)
            {
                if (combinedSpan[shiftOffset] != combinedSpan[shiftOffset + numRotatingBases]) break;
                hasShifted = true;
            }

            if (!hasShifted) return (pos, refAllele, altAllele, false);
            
            var    shiftedPos = new Interval(pos.Start + shiftOffset, pos.End + shiftOffset);
            string shiftedRef = refAllele;
            string shiftedAlt = altAllele;

            var rotatedSequence = combinedSpan.Slice(shiftOffset, numRotatingBases).ToString(); 
            
            if (variantType == VariantType.insertion) shiftedAlt = rotatedSequence;
            else shiftedRef                                      = rotatedSequence;

            return (shiftedPos, shiftedRef, shiftedAlt, true);
        }

        private static ReadOnlySpan<char> GetCombinedSequence(in Interval pos, in ReadOnlySpan<char> refSpan,
            string rotatingBases)
        {
            ReadOnlySpan<char> rotatingSpan     = rotatingBases.AsSpan();
            int                numRotatingBases = rotatingSpan.Length;

            ReadOnlySpan<char> downstreamSpan = GetDownstreamBases(pos, refSpan);

            int numCombinedBases = numRotatingBases + downstreamSpan.Length;
            var combinedBases    = new char[numCombinedBases];

            for (var i = 0; i < rotatingBases.Length; i++) combinedBases[i]                     = rotatingSpan[i];
            for (var i = 0; i < downstreamSpan.Length; i++) combinedBases[numRotatingBases + i] = downstreamSpan[i];

            return combinedBases;
        }

        private static ReadOnlySpan<char> GetDownstreamBases(Interval pos, ReadOnlySpan<char> refSpan) =>
            refSpan.Slice(pos.End, refSpan.Length - pos.End);

        private static string GetRotatingBases(string refAllele, string altAllele, VariantType variantType) =>
            variantType == VariantType.insertion ? altAllele : refAllele;
    }
}