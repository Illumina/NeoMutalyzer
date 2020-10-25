using System.Collections.Generic;
using System.Linq;

namespace ExtractGenBank.GenBank
{
    public static class ExonExtensions
    {
        public static RefSeq.TranscriptRegion[] ToTranscriptRegions(this List<Exon> exons)
        {
            if (exons.Count == 0) return null;
            
            var    regions  = new List<RefSeq.TranscriptRegion>();
            ushort regionId = 1;

            foreach (Exon exon in exons.OrderBy(x => x.Start))
            {
                var region = new RefSeq.TranscriptRegion(RefSeq.TranscriptRegionType.Exon, regionId, -1, -1, exon.Start,
                    exon.End);
                regions.Add(region);
                regionId++;
            }

            return regions.ToArray();
        }
    }
}