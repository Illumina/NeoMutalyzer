namespace NeoMutalyzerShared.Statistics
{
    public class AccuracyStatistics
    {
        private readonly Subset TranscriptVariants          = new Subset();
        private readonly Subset Variants                    = new Subset();
        private readonly Subset Transcripts                 = new Subset();
        private readonly Subset Genes                       = new Subset();

        public void Add(string vid, string transcriptId, string geneSymbol, bool isBad)
        {
            string transcriptVariant = vid + '|' + transcriptId;
            TranscriptVariants.Add(transcriptVariant, isBad);
            Variants.Add(vid, isBad);
            Transcripts.Add(transcriptId, isBad);
            Genes.Add(geneSymbol, isBad);
        }

        public void Display()
        {
            TranscriptVariants.Display("Transcript-Variants");
            Variants.Display("Variants");
            Transcripts.Display("Transcripts");
            Genes.Display("Genes");
        }
    }
}