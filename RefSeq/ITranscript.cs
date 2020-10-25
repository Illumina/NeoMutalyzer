namespace RefSeq
{
    public interface ITranscript
    {
        string       cdnaSequence { get; }
        string       cdsSequence  { get; }
        string       aaSequence   { get; }
        CodingRegion codingRegion { get; }
        string       geneSymbol   { get; }
        string       GetCdna(int start, int end);
        string       GetCds(int start, int end);
        string       GetAminoAcids(int start, int end);
    }
}