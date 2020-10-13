using System;

namespace ExtractTranscriptsFromGffSnapshots.GFF
{
    public sealed class Exon : IEquatable<Exon>
    {
        public readonly int  start;
        public readonly int  end;
        public readonly int  cdnaStart;
        public readonly int  cdnaEnd;
        public readonly bool onReverseStrand;

        public Exon(int start, int end, int cdnaStart, int cdnaEnd, bool onReverseStrand)
        {
            this.start           = start;
            this.end             = end;
            this.cdnaStart       = cdnaStart;
            this.cdnaEnd         = cdnaEnd;
            this.onReverseStrand = onReverseStrand;
        }

        public bool Equals(Exon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return start   == other.start   && end             == other.end && cdnaStart == other.cdnaStart &&
                   cdnaEnd == other.cdnaEnd && onReverseStrand == other.onReverseStrand;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = start;
                hashCode = (hashCode * 397) ^ end;
                hashCode = (hashCode * 397) ^ cdnaStart;
                hashCode = (hashCode * 397) ^ cdnaEnd;
                hashCode = (hashCode * 397) ^ onReverseStrand.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            char strand = onReverseStrand ? '-' : '+';
            return $"{start:N0}-{end:N0}\t{cdnaStart:N0}-{cdnaEnd:N0}\t{strand}";
        }
    }
}