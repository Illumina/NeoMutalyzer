using Moq;
using NeoMutalyzer.Validation;
using NeoMutalyzerShared;
using Xunit;

namespace UnitTests.Validation
{
    public class HgvsProteinValidatorTests
    {
        [Fact]
        public void ValidateHgvsProtein_NP_001137434_1_InvalidAltAminoAcid()
        {
            // NP_001137434.1:p.(Val701_?_fsTer?)
            // Mutalyzer: NM_001143962.1(CAPN8_v001):c.2101G>A
            //            NM_001143962.1(CAPN8_i001):p.(Val701Met)
            var result         = new ValidationResult();
            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.Setup(x => x.GetAminoAcids(701, 701)).Returns("V");

            result.ValidateHgvsProtein(mockTranscript.Object, "NP_001137434.1:p.(Val701_?_fsTer?)");

            Assert.True(result.HasErrors);
            Assert.True(result.HasHgvsProteinAltAlleleError);
        }
        
        [Fact]
        public void ValidateHgvsProtein_NP_115505_2_Silent_MultipleRefAA_Correct()
        {
            // 
            //
            //
            var result         = new ValidationResult();
            var mockTranscript = new Mock<IGenBankTranscript>();
            mockTranscript.Setup(x => x.GetAminoAcids(525, 525)).Returns("A");
            
            // p.(AlaLeu525=) means that position 525 will be A

            result.ValidateHgvsProtein(mockTranscript.Object, "NM_032129.2:c.1575_1576delCCinsAT(p.(AlaLeu525=))");

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidateHgvsProtein_NP_008964_3_Extension_Correct()
        {
            // NP_008964.3:p.(Ter197LysextTer4)
            // Mutalyzer: NM_007033.4:c.589T>A
            //            NM_007033.4(RER1_i001):p.(*197Lysext*4)
            var result = new ValidationResult();

            const string aaSequence =
                "MSEGDSVGESVHGKPSVVYRFFTRLGQIYQSWLDKSTPYTAVRWVVTLGLSFVYMIRVYLLQGWYIVTYALGIYHLNLFIAFLSPKVDPSLMEDSDDGPSLPTKQNEEFRPFIRRLPEFKFWHAATKGILVAMVCTFFDAFNVPVFWPILVMYFIMLFCITMKRQIKHMIKYRYIPFTHGKRRYRGKEDAGKAFAS*";
            var genbankTranscript = new GenBankTranscript(null, null, null, null, aaSequence, null);

            // var mockTranscript = new Mock<IGenBankTranscript>();
            // mockTranscript.Setup(x => x.GetAminoAcids(197, 197)).Returns("*");

            // 1-2334561-T-A   NM_007033.4     NM_007033.4:c.589T>A            Invalid protein position        Protein RefAllele
            result.ValidateHgvsProtein(genbankTranscript, "NP_008964.3:p.(Ter197LysextTer4)");

            Assert.False(result.HasErrors);
        }
    }
}