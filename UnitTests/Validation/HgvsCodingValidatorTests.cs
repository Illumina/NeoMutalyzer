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
                VariantType.indel, false, false, false);

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
                VariantType.indel, false, false, false);

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

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_152486.2:c.*7C>T", null, VariantType.SNV, false, false, false);

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

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_199454.2:c.3770_*1delTCTGAC", null, VariantType.deletion, false, false, false);

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

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_006983.1:c.1173_*1delAG", null, VariantType.deletion, false, false, false);

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

            result.ValidateHgvsCoding(mockTranscript.Object, "NM_032129.2:c.-35G>C", null, VariantType.deletion, false, false, false);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidateHgvsCoding_NM_001310156_1_PastEnd_IncorrectPosition()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(190, 1569);

            var genbankTranscript =
                new GenBankTranscript("NM_001310156.1", null, new string('N', 2155), null, null, codingRegion);

            result.ValidateHgvsCoding(genbankTranscript, "NM_001310156.1:c.*595A>G", null, VariantType.deletion, false, false, false);

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

            result.ValidateHgvsCoding(genbankTranscript, "NM_024813.2:c.*-2898A>G", null, VariantType.deletion, false, false, false);

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
                VariantType.deletion, true, false, false);

            Assert.False(result.HasErrors);
        }
        
        [Fact]
        public void ValidateHgvsCoding_NM_001203248_1_NearExonBoundary_Correct()
        {
            var result       = new ValidationResult();
            var rightCdsPos  = new Interval(702, 702);
            var codingRegion = new Interval(194, 2407);
        
            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCds(701, 701)).Returns("A");
        
            result.ValidateHgvsCoding(mockTranscript.Object, "NM_001203248.1:c.701del", rightCdsPos,
                VariantType.deletion, false, true, false);
        
            Assert.False(result.HasErrors);
        }
        
        [Fact]
        public void ValidateHgvsCoding_NR_136542_1_NonConsecutivePositions_IncorrectPosition()
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(-1, -1);
        
            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
        
            result.ValidateHgvsCoding(mockTranscript.Object, "NR_136542.1:n.3433_3435insC", null,
                VariantType.insertion, false, false, false);
        
            Assert.True(result.HasErrors);
            Assert.True(result.HasHgvsCodingInsPositionError);
        }
        
        [Fact]
        public void ValidateHgvsCoding_NM_000551_3_TruncatedCoveredCdsStart_BeforeRightAlignment_Correct()
        {
            var result       = new ValidationResult();
            var rightCdsPos  = new Interval(1, 14);
            var codingRegion = new Interval(215, 855);
        
            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.SetupGet(x => x.CodingRegion).Returns(codingRegion);
            mockTranscript.Setup(x => x.GetCds(1, 17)).Returns("ATGCCCCGGAGGGCGGA");
        
            result.ValidateHgvsCoding(mockTranscript.Object, "NM_000551.3:c.1_17del", rightCdsPos,
                VariantType.deletion, false, false, true);
        
            Assert.False(result.HasErrors);
        }

        [Theory]
        [InlineData("NM_021960.4:c.*960_*961insTAGA", true)]
        [InlineData("NM_021960.4:c.*961_*962insAGAT", true)]
        [InlineData("NM_021960.4:c.*962_*963insGATA", true)]
        [InlineData("NM_021960.4:c.*963_*964insATAG", true)]
        [InlineData("NM_021960.4:c.*964_*965insTAGA", true)]
        [InlineData("NM_021960.4:c.*965_*966insAGAT", true)]
        [InlineData("NM_021960.4:c.*966_*967insGATA", true)]
        [InlineData("NM_021960.4:c.*967_*968insATAG", true)]
        [InlineData("NM_021960.4:c.*967_*968insACAG", false)]
        public void ValidateHgvsCoding_NM_021960_4_DetectDuplication(string hgvsCoding, bool expectedIsDupe)
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(209, 1261);

            string cdnaSequence = new string('N', 2196) + "TTATCTGATTTTGGTAAGTATTCCTTAGATAGGTTTTTCTTTGAAAACCT";
            var gbTranscript =
                new GenBankTranscript("NM_021960.4", "NM_021960.4", cdnaSequence, null, null, codingRegion);

            result.ValidateHgvsCoding(gbTranscript, hgvsCoding, null, VariantType.insertion, false, false, false);

            if (expectedIsDupe)
            {
                Assert.True(result.HasErrors);
                Assert.True(result.HasHgvsCodingInsToDupError);
            }
            else
            {
                Assert.False(result.HasErrors);
            }
        }
        
        [Theory]
        [InlineData("NM_021960.4:c.*961_*964dup", false)]
        [InlineData("NM_021960.4:c.*962_*965dup", false)]
        [InlineData("NM_021960.4:c.*963_*966dup", false)]
        [InlineData("NM_021960.4:c.*964_*967dup", true)]
        [InlineData("NM_021960.4:c.*965_*968dup", false)]
        [InlineData("NM_021960.4:c.*966_*969dup", true)]
        public void ValidateHgvsCoding_NM_021960_4_CheckDuplicationPosition(string hgvsCoding, bool expectedPosition)
        {
            var result       = new ValidationResult();
            var codingRegion = new Interval(209, 1261);

            const string cdnaSequence =
                "GCGCAACCCTCCGGAAGCTGCCGCCCCTTTCCCCTTTTATGGGAATACTTTTTTTAAAAAAAAAGAGTTCGCTGGCGCCACCCCGTAGGACTGGCCGCCCTAAAACCGTGATAAAGGAGCTGCTCGCCACTTCTCACTTCCGCTTCCTTCCAGTAAGGAGTCGGGGTCTTCCCCAGTTTTCTCAGCCAGGCGGCGGCGGCGACTGGCAATGTTTGGCCTCAAAAGAAACGCGGTAATCGGACTCAACCTCTACTGTGGGGGGGCCGGCTTGGGGGCCGGCAGCGGCGGCGCCACCCGCCCGGGAGGGCGACTTTTGGCTACGGAGAAGGAGGCCTCGGCCCGGCGAGAGATAGGGGGAGGGGAGGCCGGCGCGGTGATTGGCGGAAGCGCCGGCGCAAGCCCCCCGTCCACCCTCACGCCAGACTCCCGGAGGGTCGCGCGGCCGCCGCCCATTGGCGCCGAGGTCCCCGACGTCACCGCGACCCCCGCGAGGCTGCTTTTCTTCGCGCCCACCCGCCGCGCGGCGCCGCTTGAGGAGATGGAAGCCCCGGCCGCTGACGCCATCATGTCGCCCGAAGAGGAGCTGGACGGGTACGAGCCGGAGCCTCTCGGGAAGCGGCCGGCTGTCCTGCCGCTGCTGGAGTTGGTCGGGGAATCTGGTAATAACACCAGTACGGACGGGTCACTACCCTCGACGCCGCCGCCAGCAGAGGAGGAGGAGGACGAGTTGTACCGGCAGTCGCTGGAGATTATCTCTCGGTACCTTCGGGAGCAGGCCACCGGCGCCAAGGACACAAAGCCAATGGGCAGGTCTGGGGCCACCAGCAGGAAGGCGCTGGAGACCTTACGACGGGTTGGGGATGGCGTGCAGCGCAACCACGAGACGGCCTTCCAAGGCATGCTTCGGAAACTGGACATCAAAAACGAAGACGATGTGAAATCGTTGTCTCGAGTGATGATCCATGTTTTCAGCGACGGCGTAACAAACTGGGGCAGGATTGTGACTCTCATTTCTTTTGGTGCCTTTGTGGCTAAACACTTGAAGACCATAAACCAAGAAAGCTGCATCGAACCATTAGCAGAAAGTATCACAGACGTTCTCGTAAGGACAAAACGGGACTGGCTAGTTAAACAAAGAGGCTGGGATGGGTTTGTGGAGTTCTTCCATGTAGAGGACCTAGAAGGTGGCATCAGGAATGTGCTGCTGGCTTTTGCAGGTGTTGCTGGAGTAGGAGCTGGTTTGGCATATCTAATAAGATAGCCTTACTGTAAGTGCAATAGTTGACTTTTAACCAACCACCACCACCACCAAAACCAGTTTATGCAGTTGGACTCCAAGCTGTAACTTCCTAGAGTTGCACCCTAGCAACCTAGCCAGAAAAGCAAGTGGCAAGAGGATTATGGCTAACAAGAATAAATACATGGGAAGAGTGCTCCCCATTGATTGAAGAGTCACTGTCTGAAAGAAGCAAAGTTCAGTTTCAGCAACAAACAAACTTTGTTTGGGAAGCTATGGAGGAGGACTTTTAGATTTAGTGAAGATGGTAGGGTGGAAAGACTTAATTTCCTTGTTGAGAACAGGAAAGTGGCCAGTAGCCAGGCAAGTCATAGAATTGATTACCCGCCGAATTCATTAATTTACTGTAGTGTTAAGAGAAGCACTAAGAATGCCAGTGACCTGTGTAAAAGTTACAAGTAATAGAACTATGACTGTAAGCCTCAGTACTGTACAAGGGAAGCTTTTCCTCTCTCTAATTAGCTTTCCCAGTATACTTCTTAGAAAGTCCAAGTGTTCAGGACTTTTATACCTGTTATACTTTGGCTTGGTTTCCATGATTCTTACTTTATTAGCCTAGTTTATCACCAATAATACTTGACGGAAGGCTCAGTAATTAGTTATGAATATGGATATCCTCAATTCTTAAGACAGCTTGTAAATGTATTTGTAAAAATTGTATATATTTTTACAGAAAGTCTATTTCTTTGAAACGAAGGAAGTATCGAATTTACATTAGTTTTTTTCATACCCTTTTGAACTTTGCAACTTCCGTAATTAGGAACCTGTTTCTTACAGCTTTTCTATGCTAAACTTTGTTCTGTTCAGTTCTAGAGTGTATACAGAACGAATTGATGTGTAACTGTATGCAGACTGGTTGTAGTGGAACAAATCTGATAACTATGCAGGTTTAAATTTTCTTATCTGATTTTGGTAAGTATTCCTTAGATAGGTTTTTCTTTGAAAACCTGGGATTGAGAGGTTGATGAATGGAAATTCTTTCACTTCATTATATGCAAGTTTTCAATAATTAGGTCTAAGTGGAGTTTTAAGGTTACTGATGACTTACAAATAATGGGCTCTGATTGGGCAATACTCATTTGAGTTCCTTCCATTTGACCTAATTTAACTGGTGAAATTTAAAGTGAATTCATGGGCTCATCTTTAAAGCTTTTACTAAAAGATTTTCAGCTGAATGGAACTCATTAGCTGTGTGCATATAAAAAGATCACATCAGGTGGATGGAGAGACATTTGATCCCTTGTTTGCTTAATAAATTATAAAATGATGGCTTGGAAAAGCAGGCTAGTCTAACCATGGTGCTATTATTAGGCTTGCTTGTTACACACACAGGTCTAAGCCTAGTATGTCAATAAAGCAAATACTTACTGTTTTGTTTCTATTAATGATTCCCAAACCTTGTTGCAAGTTTTTGCATTGGCATCTTTGGATTTCAGTCTTGATGTTTGTTCTATCAGACTTAACCTTTTATTTCCTGTCCTTCCTTGAAATTGCTGATTGTTCTGCTCCCTCTACAGATATTTATATCAATTCCTACAGCTTTCCCCTGCCATCCCTGAACTCTTTCTAGCCCTTTTAGATTTTGGCACTGTGAAACCCCTGCTGGAAACCTGAGTGACCCTCCCTCCCCACCAAGAGTCCACAGACCTTTCATCTTTCACGAACTTGATCCTGTTAGCAGGTGGTAATACCATGGGTGCTGTGACACTAACAGTCATTGAGAGGTGGGAGGAAGTCCCTTTTCCTTGGACTGGTATCTTTTCAACTATTGTTTTATCCTGTCTTTGGGGGCAATGTGTCAAAAGTCCCCTCAGGAATTTTCAGAGGAAAGAACATTTTATGAGGCTTTCTCTAAAGTTTCCTTTGTATAGGAGTATGCTCACTTAAATTTACAGAAAGAGGTGAGCTGTGTTAAACCTCAGAGTTTAAAAGCTACTGATAAACTGAAGAAAGTGTCTATATTGGAACTAGGGTCATTTGAAAGCTTCAGTCTCGGAACATGACCTTTAGTCTGTGGACTCCATTTAAAAATAGGTATGAATAAGATGACTAAGAATGTAATGGGGAAGAACTGCCCTGCCTGCCCATCTCAGAGCCATAAGGTCATCTTTGCTAGAGCTATTTTTACCTATGTATTTATCGTTCTTGATCATAAGCCGCTTATTTATATCATGTATCTCTAAGGACCTAAAAGCACTTTATGTAGTTTTTAATTAATCTTAAGATCTGGTTACGGTAACTAAAAAAGCCTGTCTGCCAAATCCAGTGGAAACAAGTGCATAGATGTGAATTGGTTTTTAGGGGCCCCACTTCCCAATTCATTAGGTATGACTGTGGAAATACAGACAAGGATCTTAGTTGATATTTTGGGCTTGGGGCAGTGAGGGCTTAGGACACCCCAAGTGGTTTGGGAAAGGAGGAGGGGAGTGGTGGGTTTATAGGGGGAGGAGGAGGCAGGTGGTCTAAGTGCTGACTGGCTACGTAGTTCGGGCAAATCCTCCAAAAGGGAAAGGGAGGATTTGCTTAGAAGGATGGCGCTCCCAGTGACTACTTTTTGACTTCTGTTTGTCTTACGCTTCTCTCAGGGAAAAACATGCAGTCCTCTAGTGTTTCATGTACATTCTGTGGGGGGTGAACACCTTGGTTCTGGTTAAACAGCTGTACTTTTGATAGCTGTGCCAGGAAGGGTTAGGACCAACTACAAATTAATGTTGGTTGTCAAATGTAGTGTGTTTCCCTAACTTTCTGTTTTTCCTGAGAAAAAAAAATAAATCTTTTATTCAAATACAGGGAAAAAAAAAAAAAAAAAA";
            var gbTranscript =
                new GenBankTranscript("NM_021960.4", "NM_021960.4", cdnaSequence, null, null, codingRegion);

            result.ValidateHgvsCoding(gbTranscript, hgvsCoding, null, VariantType.insertion, false, false, false);

            if (expectedPosition)
            {
                Assert.False(result.HasErrors);
            }
            else
            {
                Assert.True(result.HasErrors);
                Assert.True(result.HasHgvsCodingDupPositionError);
            }
        }
    }
}