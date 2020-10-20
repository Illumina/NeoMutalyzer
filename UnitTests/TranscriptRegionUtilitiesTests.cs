using NeoMutalyzerShared.Utilities;
using RefSeq;
using Xunit;

namespace UnitTests
{
    public class TranscriptRegionUtilitiesTests
    {
        [Fact]
        public void GetTranscriptRegionsCode_Forward()
        {
            // NR_046018.2
            var transcriptRegions = new[]
            {
                new TranscriptRegion(TranscriptRegionType.Exon, 1, 11874, 12227, 1,   354),
                new TranscriptRegion(TranscriptRegionType.Exon, 2, 12613, 12721, 355, 463),
                new TranscriptRegion(TranscriptRegionType.Exon, 3, 13221, 14409, 464, 1652)
            };

            var expectedResults = new[]
            {
                "new TranscriptRegion(TranscriptRegionType.Exon, 1, 11874, 12227, 1, 354)",
                "new TranscriptRegion(TranscriptRegionType.Intron, 1, 12228, 12612, 354, 355)",
                "new TranscriptRegion(TranscriptRegionType.Exon, 2, 12613, 12721, 355, 463)",
                "new TranscriptRegion(TranscriptRegionType.Intron, 2, 12722, 13220, 463, 464)",
                "new TranscriptRegion(TranscriptRegionType.Exon, 3, 13221, 14409, 464, 1652)"
            };

            string[] actualResults = transcriptRegions.ToCodeLines(false);
            Assert.Equal(expectedResults, actualResults);
        }
        
        [Fact]
        public void GetTranscriptRegionsCode_Reverse()
        {
            // NR_026818.1
            var transcriptRegions = new[]
            {
                new TranscriptRegion(TranscriptRegionType.Exon, 3, 34611, 35174, 567, 1130),
                new TranscriptRegion(TranscriptRegionType.Exon, 2, 35277, 35481, 362, 566),
                new TranscriptRegion(TranscriptRegionType.Exon, 1, 35721, 36081, 1,   361)
            };

            var expectedResults = new[]
            {
                "new TranscriptRegion(TranscriptRegionType.Exon, 3, 34611, 35174, 567, 1130)",
                "new TranscriptRegion(TranscriptRegionType.Intron, 2, 35175, 35276, 566, 567)",
                "new TranscriptRegion(TranscriptRegionType.Exon, 2, 35277, 35481, 362, 566)",
                "new TranscriptRegion(TranscriptRegionType.Intron, 1, 35482, 35720, 361, 362)",
                "new TranscriptRegion(TranscriptRegionType.Exon, 1, 35721, 36081, 1, 361)"
            };

            string[] actualResults = transcriptRegions.ToCodeLines(true);
            Assert.Equal(expectedResults,    actualResults);
        }
    }
}