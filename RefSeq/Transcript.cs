using Newtonsoft.Json;
using ReferenceSequence;

namespace RefSeq
{
    public sealed class Transcript : ITranscript
    {
        public string id;
        public string geneSymbol { get; }

        [JsonConverter(typeof(ChromosomeConverter))]
        public Chromosome chromosome;
        public int          start;
        public int          end;

        public bool onReverseStrand;

        public TranscriptRegion[] transcriptRegions;
        public ushort             numExons;
        public byte               startExonPhase;
        
        public CodingRegion codingRegion { get; }
        public string       proteinId;

        public string[] transcriptRegionsCode;

        public string cdnaSequence { get; }
        public string cdsSequence  { get; }
        public string aaSequence   { get; }
        
        // JSON constructor
        [JsonConstructor]
        public Transcript(string id, string proteinId, string geneSymbol, Chromosome chromosome, int start, int end,
            bool onReverseStrand, TranscriptRegion[] transcriptRegions, ushort numExons, byte startExonPhase,
            CodingRegion codingRegion, string[] transcriptRegionsCode, string cdnaSequence, string cdsSequence,
            string aaSequence)
        {
            this.id                    = id;
            this.proteinId             = proteinId;
            this.geneSymbol            = geneSymbol;
            this.chromosome            = chromosome;
            this.start                 = start;
            this.end                   = end;
            this.onReverseStrand       = onReverseStrand;
            this.transcriptRegions     = transcriptRegions;
            this.numExons              = numExons;
            this.startExonPhase        = startExonPhase;
            this.codingRegion          = codingRegion;
            this.transcriptRegionsCode = transcriptRegionsCode;
            this.cdnaSequence          = cdnaSequence;
            this.cdsSequence           = cdsSequence;
            this.aaSequence            = aaSequence;
        }

        // GFF3 snapshot constructor
        public Transcript(string id, Chromosome chromosome, int start, int end, bool onReverseStrand, ushort numExons,
            TranscriptRegion[] transcriptRegions, string[] transcriptRegionsCode)
        {
            this.id                    = id;
            this.chromosome            = chromosome;
            this.start                 = start;
            this.end                   = end;
            this.onReverseStrand       = onReverseStrand;
            this.numExons              = numExons;
            this.transcriptRegions     = transcriptRegions;
            this.transcriptRegionsCode = transcriptRegionsCode;
        }

        // GenBank (transcript entry) constructor
        public Transcript(string id, string proteinId, string geneSymbol, string cdnaSequence, string cdsSequence,
            string aaSequence, CodingRegion codingRegion, byte startExonPhase, TranscriptRegion[] transcriptRegions)
        {
            this.id                = id;
            this.proteinId         = proteinId;
            this.geneSymbol        = geneSymbol;
            this.cdnaSequence      = cdnaSequence;
            this.cdsSequence       = cdsSequence;
            this.aaSequence        = aaSequence;
            this.codingRegion      = codingRegion;
            this.startExonPhase    = startExonPhase;
            this.transcriptRegions = transcriptRegions;
        }        
        
        public string GetCdna(int start, int end)
        {
            if (start < 1 || end > cdnaSequence.Length) return null;
            return cdnaSequence.Substring(start - 1, Length(start, end));
        }

        public string GetCds(int start, int end)
        {
            if (cdsSequence == null || start < 1 || end > cdsSequence.Length) return null;
            return cdsSequence.Substring(start - 1, Length(start, end));
        }

        public string GetAminoAcids(int start, int end)
        {
            if (aaSequence == null || start < 1 || end > aaSequence.Length) return null;
            return aaSequence.Substring(start - 1, Length(start, end));
        }

        private static int Length(int start, int end) => end - start + 1;
    }
}