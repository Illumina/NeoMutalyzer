using System;

namespace NeoMutalyzer.HgvsParsing
{
    public sealed class ProteinInterval : IEquatable<ProteinInterval>
    {
        public readonly ProteinPosition Start;
        public readonly ProteinPosition End;
        public readonly string          AltAA;

        public const string EmptyAA   = "?";
        public const string UnknownAA = "X";

        public ProteinInterval(ProteinPosition start, ProteinPosition end, string altAA)
        {
            Start = start;
            End   = end;
            AltAA = altAA;
        }

        public bool Equals(ProteinInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Start, other.Start) && Equals(End, other.End) && AltAA == other.AltAA;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Start                 != null ? Start.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (End   != null ? End.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AltAA != null ? AltAA.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}