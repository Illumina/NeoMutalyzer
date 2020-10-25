using NeoMutalyzerShared;
using NeoMutalyzerShared.Validation;
using Xunit;

namespace UnitTests.Validation
{
    public class PositionValidatorTests
    {
        [Fact]
        public void ValidateCdnaPosition_NM_001346067_1_InvalidCdsPosition()
        {
            var result       = new ValidationResult();
            var cdnaPos      = new Interval(605, 605);
            var cdsPos       = new Interval(225, 607);
            var cdnaSequence = new string('N', 1000);
            var cdsSequence  = new string('N', 450);

            var refseqTranscript = new RefSeq.Transcript("NM_001346067.1", null, null, cdnaSequence, cdsSequence, null,
                null, 0, null);

            result.ValidateCdnaPosition(refseqTranscript, cdnaPos, cdsPos, "AAA");

            Assert.True(result.HasErrors);
            Assert.True(result.HasInvalidCdsPosition);
        }

        [Fact]
        public void ValidateAminoAcidPosition_NM_001346067_1_InvalidProteinPosition()
        {
            var result            = new ValidationResult();
            var aminoAcidPos      = new Interval(203, 203);
            var aminoAcidSequence = new string('N', 149);

            var refseqTranscript = new RefSeq.Transcript("NM_001346067.1", null, null, null, null, aminoAcidSequence,
                null, 0, null);

            result.ValidateAminoAcidPosition(refseqTranscript, aminoAcidPos, "K");

            Assert.True(result.HasErrors);
            Assert.True(result.HasInvalidProteinPosition);
        }
    }
}