using NeoMutalyzerShared.HgvsParsing;
using Xunit;

namespace UnitTests.HgvsParsing
{
    public class HgvsProteinParserTests
    {
        [Theory]
        [InlineData("NM_001005484.1:c.180A>G(p.(Ser60=))",               "S", 60)]
        [InlineData("NM_032129.2:c.1575_1576delCCinsAT(p.(AlaLeu525=))", "A", 525)]
        [InlineData("NM_032129.2:c.1575_1576delCCinsAT(p.(Ter525=))",    "*", 525)]
        [InlineData("NM_173348.1:c.-2_1dupGGA(p.(1=))",                  "?", 1)]
        public void Parse_Silent(string hgvsProtein, string expectedStartRef, int expectedStart)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Equal(expectedStartRef, actual.AltAA);
        }

        [Theory]
        [InlineData("NP_689699.2:p.(Glu663del)", "E", 663)]
        public void Parse_Deletion_Single(string hgvsProtein, string expectedStartRef, int expectedStart)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Null(actual.AltAA);
        }

        [Theory]
        [InlineData("NP_938073.1:p.(Pro29_Pro35del)", "P", 29, "P", 35)]
        public void Parse_Deletion_Double(string hgvsProtein, string expectedStartRef, int start, string expectedEndRef,
            int end)
        {
            var expectedStart = new ProteinPosition(start, expectedStartRef[0]);
            var expectedEnd   = new ProteinPosition(end,   expectedEndRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expectedStart, actual.Start);
            Assert.Equal(expectedEnd,   actual.End);
            Assert.Null(actual.AltAA);
        }

        [Theory]
        [InlineData("NP_001278295.1:p.(Thr371_Ala372insAspMet)", "T", 371, "A", 372, "DM")]
        public void Parse_Insertion(string hgvsProtein, string expectedStartRef, int start, string expectedEndRef,
            int end, string expectedAlt)
        {
            var expectedStart = new ProteinPosition(start, expectedStartRef[0]);
            var expectedEnd   = new ProteinPosition(end,   expectedEndRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expectedStart, actual.Start);
            Assert.Equal(expectedEnd,   actual.End);
            Assert.Equal(expectedAlt,   actual.AltAA);
        }

        [Theory]
        [InlineData("NP_001005484.1:p.(Leu160SerfsTer7)", "L", 160, "S")]
        [InlineData("NP_683699.1:p.(Pro244LeufsTer?)",    "P", 244, "L")]
        [InlineData("NP_683699.1:p.(Pro244Ter)",          "P", 244, "*")]
        [InlineData("NP_001137434.1:p.(Val701_?_fsTer?)", "V", 701, "?")]
        public void Parse_Frameshift(string hgvsProtein, string expectedStartRef, int expectedStart, string expectedAlt)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Equal(expectedAlt, actual.AltAA);
        }

        [Theory]
        [InlineData("NP_000305.3:p.(Thr319Ter)", "T", 319, "*")]
        [InlineData("NP_000305.3:p.(Thr1?)",     "T", 1,   "?")] // start lost
        public void Parse_Substitution(string hgvsProtein, string expectedStartRef, int expectedStart,
            string expectedAlt)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Equal(expectedAlt, actual.AltAA);
        }

        [Theory]
        [InlineData("NP_001284534.1:p.(Met1_?4)", "M", 1, "?", 4)]
        public void Parse_Unknown(string hgvsProtein, string expectedStartRef, int start, string expectedAlt, int end)
        {
            var expectedStart = new ProteinPosition(start, expectedStartRef[0]);
            var expectedEnd   = new ProteinPosition(end,   ProteinInterval.UnknownAA[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expectedStart, actual.Start);
            Assert.Equal(expectedEnd,   actual.End);
            Assert.Equal(expectedAlt,   actual.AltAA);
        }

        [Theory]
        [InlineData("NP_001153656.1:p.(Glu210delinsAsp)", "E", 210, "D")]
        public void Parse_DelIns_Single(string hgvsProtein, string expectedStartRef, int expectedStart,
            string expectedAlt)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Equal(expectedAlt, actual.AltAA);
        }

        [Theory]
        [InlineData("NP_001153656.1:p.(Glu210_Leu211delinsAspIle)", "E", 210, "L", 211, "DI")]
        public void Parse_DelIns_Double(string hgvsProtein, string expectedStartRef, int start, string expectedEndRef,
            int end, string expectedAlt)
        {
            var expectedStart = new ProteinPosition(start, expectedStartRef[0]);
            var expectedEnd   = new ProteinPosition(end,   expectedEndRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expectedStart, actual.Start);
            Assert.Equal(expectedEnd,   actual.End);
            Assert.Equal(expectedAlt,   actual.AltAA);
        }

        [Theory]
        [InlineData("NP_542172.2:p.(Ter330TrpextTer73)", "*", 330, "W")]
        [InlineData("NP_542172.2:p.(Ter330TrpextTer?)",  "*", 330, "W")]
        public void Parse_Extension(string hgvsProtein, string expectedStartRef, int expectedStart,
            string expectedAlt)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Equal(expectedAlt,   actual.AltAA);
        }

        [Theory]
        [InlineData("NP_001185924.1:p.(Gly414dup)", "G", 414)]
        public void Parse_Duplication_Single(string hgvsProtein, string expectedStartRef, int expectedStart)
        {
            var expected = new ProteinPosition(expectedStart, expectedStartRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expected, actual.Start);
            Assert.Null(actual.End);
            Assert.Equal(ProteinInterval.EmptyAA, actual.AltAA);
        }
        
        [Theory]
        [InlineData("NP_112199.2:p.(Ala6_Ala8dup)", "A", 6, "A", 8)]
        public void Parse_Duplication_Double(string hgvsProtein, string expectedStartRef, int start, string expectedEndRef,
            int end)
        {
            var expectedStart = new ProteinPosition(start, expectedStartRef[0]);
            var expectedEnd   = new ProteinPosition(end,   expectedEndRef[0]);

            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Equal(expectedStart,           actual.Start);
            Assert.Equal(expectedEnd,             actual.End);
            Assert.Equal(ProteinInterval.EmptyAA, actual.AltAA);
        }

        [Theory]
        [InlineData("NP_112199.2:p.?")]
        public void Parse_StartLost(string hgvsProtein)
        {
            ProteinInterval actual = HgvsProteinParser.Parse(hgvsProtein);
            Assert.Null(actual);
        }
    }
}