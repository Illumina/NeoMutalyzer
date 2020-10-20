using System.Collections.Generic;
using System.Linq;
using RefSeq;

namespace NeoMutalyzerShared.Utilities
{
    public static class TranscriptRegionUtilities
    {
        public static string[] ToCodeLines(this TranscriptRegion[] transcriptRegions, bool onReverseStrand)
        {
            var regions = new List<TranscriptRegion>();
            regions.AddRange(transcriptRegions);
            
            // add introns
            for (var i = 1; i < transcriptRegions.Length; i++)
            {
                TranscriptRegion prevExon = transcriptRegions[i - 1];
                TranscriptRegion exon     = transcriptRegions[i];

                ushort id        = onReverseStrand ? exon.id : prevExon.id;
                int    cdnaStart = onReverseStrand ? exon.cdnaEnd : prevExon.cdnaEnd;
                int    cdnaEnd   = onReverseStrand ? prevExon.cdnaStart : exon.cdnaStart;

                var intron = new TranscriptRegion(TranscriptRegionType.Intron, id, prevExon.end + 1, exon.start - 1,
                    cdnaStart, cdnaEnd);
                regions.Add(intron);
            }
            
            // sort by genomic position
            IOrderedEnumerable<TranscriptRegion> sortedRegions = regions.OrderBy(x => x.start);
            
            // convert to code lines
            var codeLines = new List<string>();
            
            foreach (var region in sortedRegions)
            {
                var codeLine = $"new TranscriptRegion(TranscriptRegionType.{region.type}, {region.id}, {region.start}, {region.end}, {region.cdnaStart}, {region.cdnaEnd})";
                codeLines.Add(codeLine);
            }

            return codeLines.ToArray();
        }
    }
}