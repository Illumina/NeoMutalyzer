using System.Collections.Generic;
using System.Linq;
using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    public sealed class CodingSequence : IFeature
    {
        public int        Start      { get; }
        public int        End        { get; }
        public string     GeneSymbol { get; }
        public string     LocusTag   { get; }
        public string     Product    { get; }
        public string     ProteinId  { get; }
        public Interval[] Regions    { get; }
        public string     Note       { get; }

        private readonly string _geneId;
        private readonly int    _codonStart;
        public readonly  string Translation;

        public CodingSequence(Interval interval, string geneSymbol, string locusTag, string geneId, Interval[] regions,
            string note, int codonStart, string product, string proteinId, string translation)
        {
            Start       = interval.Start;
            End         = interval.End;
            GeneSymbol  = geneSymbol;
            LocusTag    = locusTag;
            _geneId     = geneId;
            Regions     = regions;
            Note        = note;
            _codonStart = codonStart;
            Product     = product;
            ProteinId   = proteinId;
            Translation = translation;
        }
        
        public override string ToString()
        {
            var intervals = new List<string>(Regions.Length);
            intervals.AddRange(Regions.Select(region => $"{region.Start}-{region.End}"));
            string regions = string.Join(", ", intervals);

            return
                $"CDS: {Start}-{End}, symbol: {GeneSymbol}, locus tag: {LocusTag}, gene ID: {_geneId}, regions: {regions}, note: {Note}, codon start: {_codonStart}, product: {Product}, protein ID: {ProteinId}, translation: {Translation.Length} aa";
        }

        // public Transcript ToTranscript(string bases)
        // {
        //     string codingSequence    = Regions.GetBases(bases);
        //     string aminoAcidSequence = AminoAcids.TranslateBases(codingSequence);
        //
        //     TranscriptRegion[] transcriptRegions = TranscriptRegion.Create(Regions);
        //
        //     return new Transcript(ProteinId, Start, End, BioType.CDS, codingSequence, aminoAcidSequence,
        //         transcriptRegions);
        // }
    }
}