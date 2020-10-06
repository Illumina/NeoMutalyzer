using System;

namespace NeoMutalyzer.HgvsParsing
{
    public class ProteinPosition : IEquatable<ProteinPosition>
    {
        public readonly int  Position;
        public readonly char RefAA;

        public ProteinPosition(int position, char refAA)
        {
            Position = position;
            RefAA    = refAA;
        }

        public bool Equals(ProteinPosition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Position == other.Position && RefAA == other.RefAA;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position * 397) ^ RefAA.GetHashCode();
            }
        }
    }
}