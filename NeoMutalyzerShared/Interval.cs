﻿using System;

namespace NeoMutalyzerShared
{
    public sealed class Interval : IEquatable<Interval>
    {
        public readonly int Start;
        public readonly int End;

        public Interval(int start, int end)
        {
            Start = start;
            End   = end;
        }

        public int Length => End - Start + 1;

        public bool Equals(Interval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Start == other.Start && End == other.End;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start * 397) ^ End;
            }
        }
    }
}