using System;
using NeoMutalyzerShared.Annotated;

namespace NeoMutalyzerShared.Validation
{
    public static class VariantRotator
    {
        public static (Interval ShiftedPosition, string ShiftedRef, string ShiftedAlt, bool HasShifted) Right(
            Interval pos, string refAllele, string altAllele, VariantType variantType, string refSequence)
        {
            bool bothAllelesNull = refAllele   == null                 && altAllele   == null;
            bool notInsOrDel     = variantType != VariantType.deletion && variantType != VariantType.insertion;
            
            if (refSequence == null || pos == null || bothAllelesNull || notInsOrDel)
                return (pos, refAllele, altAllele, false);

            if(!pos.Overlaps(1, refSequence.Length)) return (pos, refAllele, altAllele, false);

            ReadOnlySpan<char> refSpan = refSequence.AsSpan();

            string             rotatingBases = GetRotatingBases(refAllele, altAllele, variantType);
            ReadOnlySpan<char> combinedSpan;
            
            combinedSpan = GetCombinedSequence(pos, refSpan, rotatingBases);
            
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
            
            ReadOnlySpan<char> downstreamSpan;

            try
            {
                downstreamSpan = GetDownstreamBases(pos, refSpan);
                if (downstreamSpan == ReadOnlySpan<char>.Empty) return rotatingBases;
            }
            catch (Exception e)
            {
                Console.WriteLine($"interval: {pos.Start}-{pos.End}, refSpan lenght:{refSpan.Length}");
                Console.WriteLine(e);
                throw;
            }
            int numCombinedBases = numRotatingBases + downstreamSpan.Length;
            var combinedBases    = new char[numCombinedBases];

            for (var i = 0; i < rotatingBases.Length; i++) combinedBases[i]                     = rotatingSpan[i];
            for (var i = 0; i < downstreamSpan.Length; i++) combinedBases[numRotatingBases + i] = downstreamSpan[i];

            return combinedBases;
        }

        private static ReadOnlySpan<char> GetDownstreamBases(Interval pos, ReadOnlySpan<char> refSpan) =>
            pos.End < 0 || pos.End >= refSpan.Length ? ReadOnlySpan<char>.Empty : refSpan.Slice(pos.End, refSpan.Length - pos.End);
        
        private static string GetRotatingBases(string refAllele, string altAllele, VariantType variantType) =>
            variantType == VariantType.insertion ? altAllele : refAllele;
    }
}