namespace RefSeq
{
    public sealed class CodingRegion
    {
        public readonly int start;
        public readonly int end;
        public readonly int cdnaStart;
        public readonly int cdnaEnd;

        public CodingRegion(int start, int end, int cdnaStart, int cdnaEnd)
        {
            this.start     = start;
            this.end       = end;
            this.cdnaStart = cdnaStart;
            this.cdnaEnd   = cdnaEnd;
        }
    }
}