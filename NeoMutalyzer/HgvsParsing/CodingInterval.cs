using System;
using NeoMutalyzerShared;

namespace NeoMutalyzer.HgvsParsing
{
    public class CodingInterval : IEquatable<CodingInterval>
    {
        public readonly PositionOffset Start;
        public readonly PositionOffset End;

        public CodingInterval(PositionOffset start, PositionOffset end)
        {
            Start = start;
            End   = end;
        }

        public bool Equals(CodingInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public bool Equals(Interval other) => Start.Position == other.Start && End.Position == other.End;

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }
    }
}