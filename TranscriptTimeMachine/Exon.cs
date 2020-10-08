namespace TranscriptTimeMachine
{
    public sealed class Exon
    {
        public readonly int Start;
        public readonly int End;
        public readonly int CdnaStart;
        public readonly int CdnaEnd;

        public ushort Id;

        public Exon(int start, int end, int cdnaStart, int cdnaEnd)
        {
            Start     = start;
            End       = end;
            CdnaStart = cdnaStart;
            CdnaEnd   = cdnaEnd;
        }
    }
}