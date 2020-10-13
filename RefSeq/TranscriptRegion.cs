using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RefSeq
{
    public sealed class TranscriptRegion : IEquatable<TranscriptRegion>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly TranscriptRegionType type;
        public readonly ushort               id;
        public readonly int                  start;
        public readonly int                  end;
        public readonly int                  cdnaStart;
        public readonly int                  cdnaEnd;

        public TranscriptRegion(TranscriptRegionType type, ushort id, int start, int end, int cdnaStart, int cdnaEnd)
        {
            this.type      = type;
            this.id        = id;
            this.start     = start;
            this.end       = end;
            this.cdnaStart = cdnaStart;
            this.cdnaEnd   = cdnaEnd;
        }

        public bool Equals(TranscriptRegion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return type      == other.type      && id      == other.id && start == other.start && end == other.end &&
                   cdnaStart == other.cdnaStart && cdnaEnd == other.cdnaEnd;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) type;
                hashCode = (hashCode * 397) ^ id.GetHashCode();
                hashCode = (hashCode * 397) ^ start;
                hashCode = (hashCode * 397) ^ end;
                hashCode = (hashCode * 397) ^ cdnaStart;
                hashCode = (hashCode * 397) ^ cdnaEnd;
                return hashCode;
            }
        }

        public override string ToString() => $"{type}\t{id}\t{start:N0}-{end:N0}\t{cdnaStart:N0}-{cdnaEnd:N0}";
    }
}