namespace NeoMutalyzerShared.GenBank
{
    public interface IGenBankTranscript
    {
        string   CdnaSequence      { get; }
        string   CdsSequence       { get; }
        string   AminoAcidSequence { get; }
        Interval CodingRegion      { get; }
        string   GetCdna(int start, int end);
        string   GetCds(int start, int end);
        string   GetAminoAcids(int start, int end);
    }
}