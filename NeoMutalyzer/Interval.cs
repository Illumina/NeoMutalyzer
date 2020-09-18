using System;

namespace NeoMutalyzer
{
    public readonly struct Interval : IEquatable<Interval>
    {
        public readonly int Start;
        public readonly int End;

        public Interval(int start, int end)
        {
            Start = start;
            End   = end;
        }

        public int Length => End - Start + 1;

        public bool Equals(Interval other) => Start == other.Start && End == other.End;

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start * 397) ^ End;
            }
        }
    }
}