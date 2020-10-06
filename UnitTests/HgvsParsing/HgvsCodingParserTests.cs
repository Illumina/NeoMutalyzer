using NeoMutalyzer.HgvsParsing;
using Xunit;

namespace UnitTests.HgvsParsing
{
    public sealed class HgvsCodingParserTests
    {
        [Theory]
        [InlineData("NM_001005221.2:n.29dupC", 29, 0, 29, 0, "C", "CC", false)]
        [InlineData("NM_152486.2:c.306-1811_306-1784dupTGCTCCGTTACAGGTGGGCAGGGGAGGC", 306, -1811, 306, -1784,
            "TGCTCCGTTACAGGTGGGCAGGGGAGGC", "TGCTCCGTTACAGGTGGGCAGGGGAGGCTGCTCCGTTACAGGTGGGCAGGGGAGGC", true)]
        public void Parse_Duplication(string hgvsCoding, int expectedStart, int expectedStartOffset, int expectedEnd,
            int expectedEndOffset, string expectedRef, string expectedAlt, bool expectedCoding)
        {
            (CodingInterval actualPos, string actualRef, string actualAlt, bool actualCoding) =
                HgvsCodingParser.Parse(hgvsCoding);
            Assert.Equal(expectedStart,       actualPos.Start.Position);
            Assert.Equal(expectedStartOffset, actualPos.Start.Offset);
            Assert.Equal(expectedEnd,         actualPos.End.Position);
            Assert.Equal(expectedEndOffset,   actualPos.End.Offset);
            Assert.Equal(expectedRef,         actualRef);
            Assert.Equal(expectedAlt,         actualAlt);
            Assert.Equal(expectedCoding,      actualCoding);
        }

        [Theory]
        [InlineData("NM_001005484.1:c.477_478insT", 477, 478, "", "T", true)]
        [InlineData("NM_001005484.1:n.477_478insT", 477, 478, "", "T", false)]
        public void Parse_Insertion(string hgvsCoding, int expectedStart, int expectedEnd, string expectedRef,
            string expectedAlt, bool expectedCoding)
        {
            (CodingInterval actualPos, string actualRef, string actualAlt, bool actualCoding) =
                HgvsCodingParser.Parse(hgvsCoding);
            Assert.Equal(expectedStart,  actualPos.Start.Position);
            Assert.Equal(expectedEnd,    actualPos.End.Position);
            Assert.Equal(expectedRef,    actualRef);
            Assert.Equal(expectedAlt,    actualAlt);
            Assert.Equal(expectedCoding, actualCoding);
        }

        [Theory]
        [InlineData("NM_001005484.1:c.134A>C", 134,  0,  134,  0,  "A", "C", true)]
        [InlineData("NR_039983.2:n.5215C>T",   5215, 0,  5215, 0,  "C", "T", false)]
        [InlineData("NM_152486.2:c.-28G>A",    -28,  0,  -28,  0,  "G", "A", true)]
        [InlineData("NM_152486.2:c.-21+33C>T", -21,  33, -21,  33, "C", "T", true)]
        [InlineData("NM_152486.2:c.1A>G",      1,    0,  1,    0,  "A", "G", true)]
        [InlineData("NM_152486.2:c.*7C>T",     7,    0,  7,    0,  "C", "T", true)]
        public void Parse_Substitution(string hgvsCoding, int expectedStart, int expectedStartOffset, int expectedEnd,
            int expectedEndOffset, string expectedRef, string expectedAlt, bool expectedCoding)
        {
            (CodingInterval actualPos, string actualRef, string actualAlt, bool actualCoding) =
                HgvsCodingParser.Parse(hgvsCoding);
            Assert.Equal(expectedStart,       actualPos.Start.Position);
            Assert.Equal(expectedStartOffset, actualPos.Start.Offset);
            Assert.Equal(expectedEnd,         actualPos.End.Position);
            Assert.Equal(expectedEndOffset,   actualPos.End.Offset);
            Assert.Equal(expectedRef,         actualRef);
            Assert.Equal(expectedAlt,         actualAlt);
            Assert.Equal(expectedCoding,      actualCoding);
        }

        [Theory]
        [InlineData("NM_000314.6:c.956_959delACTT", 956,  959,  "ACTT", "", true)]
        [InlineData("NR_039983.2:n.3602delC",       3602, 3602, "C",    "", false)]
        public void Parse_Deletion(string hgvsCoding, int expectedStart, int expectedEnd, string expectedRef,
            string expectedAlt, bool expectedCoding)
        {
            (CodingInterval actualPos, string actualRef, string actualAlt, bool actualCoding) =
                HgvsCodingParser.Parse(hgvsCoding);
            Assert.Equal(expectedStart,  actualPos.Start.Position);
            Assert.Equal(expectedEnd,    actualPos.End.Position);
            Assert.Equal(expectedRef,    actualRef);
            Assert.Equal(expectedAlt,    actualAlt);
            Assert.Equal(expectedCoding, actualCoding);
        }

        [Theory]
        [InlineData("NM_198317.2:c.585_586delGGinsTT", 585, 586, "GG", "TT", true)]
        [InlineData("NM_198317.2:n.585_586delGGinsTT", 585, 586, "GG", "TT", false)]
        public void Parse_DelIns(string hgvsCoding, int expectedStart, int expectedEnd, string expectedRef,
            string expectedAlt, bool expectedCoding)
        {
            (CodingInterval actualPos, string actualRef, string actualAlt, bool actualCoding) =
                HgvsCodingParser.Parse(hgvsCoding);
            Assert.Equal(expectedStart,  actualPos.Start.Position);
            Assert.Equal(expectedEnd,    actualPos.End.Position);
            Assert.Equal(expectedRef,    actualRef);
            Assert.Equal(expectedAlt,    actualAlt);
            Assert.Equal(expectedCoding, actualCoding);
        }

        [Theory]
        [InlineData("NM_000314.6:c.956_959invACTT", 956,  959,  "ACTT", "", true)]
        [InlineData("NR_039983.2:n.3602invC",       3602, 3602, "C",    "", false)]
        public void Parse_Inversion(string hgvsCoding, int expectedStart, int expectedEnd, string expectedRef,
            string expectedAlt, bool expectedCoding)
        {
            (CodingInterval actualPos, string actualRef, string actualAlt, bool actualCoding) =
                HgvsCodingParser.Parse(hgvsCoding);
            Assert.Equal(expectedStart,  actualPos.Start.Position);
            Assert.Equal(expectedEnd,    actualPos.End.Position);
            Assert.Equal(expectedRef,    actualRef);
            Assert.Equal(expectedAlt,    actualAlt);
            Assert.Equal(expectedCoding, actualCoding);
        }
    }
}