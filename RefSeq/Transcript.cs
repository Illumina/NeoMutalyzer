﻿using Newtonsoft.Json;
using ReferenceSequence;

namespace RefSeq
{
    public sealed class Transcript
    {
        public string id;
        public string geneSymbol;

        [JsonConverter(typeof(ChromosomeConverter))]
        public Chromosome chromosome;
        public int          start;
        public int          end;

        public bool onReverseStrand;

        public TranscriptRegion[] transcriptRegions;
        public ushort             numExons;
        public byte               startExonPhase;
        
        public CodingRegion codingRegion;
        public string       proteinId;
        
        public string cdnaSequence;
        public string cdsSequence;
        public string aaSeqence;

        // public Transcript(string id, Chromosome chromosome, int start, int end, string geneSymbol, bool onReverseStrand,
        //     ushort numExons, string cdnaSequence, string cdsSequence, string aaSeqence, byte startExonPhase,
        //     CodingRegion codingRegion, string proteinId)
        // {
        //     this.id              = id;
        //     this.chromosome      = chromosome;
        //     this.start           = start;
        //     this.end             = end;
        //     this.geneSymbol      = geneSymbol;
        //     this.onReverseStrand = onReverseStrand;
        //     this.numExons        = numExons;
        //     this.cdnaSequence    = cdnaSequence;
        //     this.cdsSequence     = cdsSequence;
        //     this.aaSeqence       = aaSeqence;
        //     this.startExonPhase  = startExonPhase;
        //     this.codingRegion    = codingRegion;
        //     this.proteinId       = proteinId;
        // }

        // GFF3 snapshot constructor
        public Transcript(string id, Chromosome chromosome, int start, int end, bool onReverseStrand, ushort numExons,
            TranscriptRegion[] transcriptRegions)
        {
            this.id                = id;
            this.chromosome        = chromosome;
            this.start             = start;
            this.end               = end;
            this.onReverseStrand   = onReverseStrand;
            this.numExons          = numExons;
            this.transcriptRegions = transcriptRegions;
        }
    }
}