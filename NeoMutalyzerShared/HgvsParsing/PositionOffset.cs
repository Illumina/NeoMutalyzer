﻿using System;

namespace NeoMutalyzerShared.HgvsParsing
{
    public readonly struct PositionOffset : IEquatable<PositionOffset>
    {
        public readonly int  Position;
        public readonly int  Offset;
        public readonly bool BeyondCodingEnd;

        public PositionOffset(int position, int offset = 0, bool beyondCodingEnd = false)
        {
            Position        = position;
            Offset          = offset;
            BeyondCodingEnd = beyondCodingEnd;
        }
        
        public bool Equals(PositionOffset other) => Position == other.Position && Offset == other.Offset && BeyondCodingEnd == other.BeyondCodingEnd;

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position;
                hashCode = (hashCode * 397) ^ Offset;
                hashCode = (hashCode * 397) ^ BeyondCodingEnd.GetHashCode();
                return hashCode;
            }
        }

        public PositionOffset ConvertToCdna(RefSeq.CodingRegion codingRegion)
        {
            if (BeyondCodingEnd) return new PositionOffset(Position + codingRegion.cdnaEnd, Offset);

            int offset = codingRegion.cdnaStart - 1;
            
            // correction since we can never have a Position equal to zero
            if (Position < 1) offset++;

            return new PositionOffset(Position + offset, Offset);
        }
    }
}