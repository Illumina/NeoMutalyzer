using Moq;
using NeoMutalyzerShared;
using NeoMutalyzerShared.Annotated;
using NeoMutalyzerShared.GenBank;
using NeoMutalyzerShared.Validation;
using Xunit;

namespace UnitTests.Validation
{
    public class HgvsCodingValidatorTests
    {
        [Fact]
        public void ValidateHgvsCoding_NM_000314_6_956_959delACTT_IncorrectPosition()
        {
            var result       = new ValidationResult();
            var rightCdsPos  = new Interval(955, 958);
            var codingRegion = new Interval(81,  2126);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCds(956, 959)).Returns("CTTT");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_000314.6:c.956_959delACTT", rightCdsPos,
                VariantType.indel, false);

            Assert.True(result.HasErrors);
            Assert.True(result.HasHgvsCodingPositionError);
            Assert.True(result.HasHgvsCodingRefAlleleError);
        }
        
        [Fact]
        public void ValidateHgvsCoding_NM_000314_6_956_959del_OnlyPositionError()
        {
            var result       = new ValidationResult();
            var rightCdsPos  = new Interval(955, 958);
            var codingRegion = new Interval(81,  2126);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCds(956, 959)).Returns("CTTT");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_000314.6:c.956_959del", rightCdsPos,
                VariantType.indel, false);

            Assert.True(result.HasErrors);
            Assert.True(result.HasHgvsCodingPositionError);
            Assert.False(result.HasHgvsCodingRefAlleleError);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_152486_2_PastCodingRegion_Correct()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(81, 2126);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCdna(2133, 2133)).Returns("C");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_152486.2:c.*7C>T", null, VariantType.SNV, false);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_199454_2_PastCodingRegion_Correct()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(83, 3856);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCdna(3852, 3857)).Returns("TCTGAC");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_199454.2:c.3770_*1delTCTGAC", null, VariantType.deletion, false);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_006983_1_PastCodingRegion_Correct()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(39, 1211);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCdna(1211, 1212)).Returns("AG");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_006983.1:c.1173_*1delAG", null, VariantType.deletion, false);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_032129_2_BeforeCodingRegion_Correct()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(36, 1871);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCdna(1, 1)).Returns("G");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_032129.2:c.-35G>C", null, VariantType.deletion, false);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_001310156_1_PastEnd_IncorrectPosition()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(190, 1569);

            var genbankTranscript =
                new GenBankTranscript("NM_001310156.1", null, new string('N', 2155), null, null, codingRegion);

            result.ValidateHgvsCoding(genbankTranscript, "NM_001310156.1:c.*595A>G", null, VariantType.deletion, false);

            Assert.True(result.HasErrors);
            Assert.True(result.HasHgvsCodingRefAlleleError);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_024813_2_PastCodingRegion_BeforeCodingRegion_InvalidCombo()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(110, 1948);

            var genbankTranscript =
                new GenBankTranscript("NM_024813.2", null, new string('N', 3112), null, null, codingRegion);

            result.ValidateHgvsCoding(genbankTranscript, "NM_024813.2:c.*-2898A>G", null, VariantType.deletion, false);

            Assert.True(result.HasErrors);
            Assert.True(result.HasHgvsCodingBeforeCdsAndAfterCds);
        }
        
        [Fact]
        public void ValidateHgvsCoding_NM_001293228_1_OverlapsIntronAndExon_Correct()
        {
            var result       = new ValidationResult();
            var rightCdsPos  = new Interval(791, 797);
            var codingRegion = new Interval(150, 488);

            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCds(801, 809)).Returns("TGATGAAGA");

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_001293228.1:c.801_809delTGATGAAGA", rightCdsPos,
                VariantType.deletion, true);

            Assert.False(result.HasErrors);
        }
    }
}