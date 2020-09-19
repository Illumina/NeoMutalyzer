using NeoMutalyzer.Annotated;
using NeoMutalyzer.Validation;
using NeoMutalyzerShared;
using Xunit;

namespace UnitTests
{
    public sealed class VariantRotatorTests
    {
        [Fact]
        public void Right_Deletion_LeftEnd_CompleteRotate()
        {
            const string refSequence = "ATGATGATGATGATG";
            var          position    = new Interval(1, 3);
            const string refAllele   = "ATG";
            const string altAllele   = "";

            (Interval shiftedPosition, string shiftedRef, string shiftedAlt, bool hasShifted) =
                VariantRotator.Right(position, refAllele, altAllele, VariantType.deletion, refSequence);

            Assert.True(hasShifted);
            Assert.Equal(new Interval(13, 15), shiftedPosition);
            Assert.Equal("ATG",                shiftedRef);
            Assert.Equal("",                   shiftedAlt);
        }
        
        [Fact]
        public void Right_Deletion_LeftEnd_IncompleteRotate()
        {
            const string refSequence = "ATGATGATGATGA";
            var          position    = new Interval(1, 3);
            const string refAllele   = "ATG";
            const string altAllele   = "";

            (Interval shiftedPosition, string shiftedRef, string shiftedAlt, bool hasShifted) =
                VariantRotator.Right(position, refAllele, altAllele, VariantType.deletion, refSequence);

            Assert.True(hasShifted);
            Assert.Equal(new Interval(11, 13), shiftedPosition);
            Assert.Equal("TGA",                shiftedRef);
            Assert.Equal("",                   shiftedAlt);
        }
        
        [Fact]
        public void Right_Deletion_RightEnd_Identity()
        {
            const string refSequence = "ATGATGATGATGA";
            var          position    = new Interval(11, 13);
            const string refAllele   = "TGA";
            const string altAllele   = "";

            (Interval shiftedPosition, string shiftedRef, string shiftedAlt, bool hasShifted) =
                VariantRotator.Right(position, refAllele, altAllele, VariantType.deletion, refSequence);

            Assert.False(hasShifted);
            Assert.Equal(position,  shiftedPosition);
            Assert.Equal(refAllele, shiftedRef);
            Assert.Equal(altAllele, shiftedAlt);
        }
        
        [Fact]
        public void Right_Insertion_LeftEnd_CompleteRotate()
        {
            const string refSequence = "CCCATGATGATGATG";
            var          position    = new Interval(4, 3);
            const string refAllele   = "";
            const string altAllele   = "ATG";

            (Interval shiftedPosition, string shiftedRef, string shiftedAlt, bool hasShifted) =
                VariantRotator.Right(position, refAllele, altAllele, VariantType.insertion, refSequence);

            Assert.True(hasShifted);
            Assert.Equal(new Interval(16, 15), shiftedPosition);
            Assert.Equal("",                   shiftedRef);
            Assert.Equal("ATG",                shiftedAlt);
        }
        
        [Fact]
        public void Right_Insertion_LeftEnd_IncompleteRotate()
        {
            const string refSequence = "CCCATGATGATGA";
            var          position    = new Interval(4, 3);
            const string refAllele   = "";
            const string altAllele   = "ATG";

            (Interval shiftedPosition, string shiftedRef, string shiftedAlt, bool hasShifted) =
                VariantRotator.Right(position, refAllele, altAllele, VariantType.insertion, refSequence);

            Assert.True(hasShifted);
            Assert.Equal(new Interval(14, 13), shiftedPosition);
            Assert.Equal("",                   shiftedRef);
            Assert.Equal("TGA",                shiftedAlt);
        }
        
        [Fact]
        public void Right_Insertion_RightEnd_Identity()
        {
            const string refSequence = "CCCATGATGATGA";
            var          position    = new Interval(14, 13);
            const string refAllele   = "";
            const string altAllele   = "TGA";

            (Interval shiftedPosition, string shiftedRef, string shiftedAlt, bool hasShifted) =
                VariantRotator.Right(position, refAllele, altAllele, VariantType.insertion, refSequence);

            Assert.False(hasShifted);
            Assert.Equal(position,  shiftedPosition);
            Assert.Equal(refAllele, shiftedRef);
            Assert.Equal(altAllele, shiftedAlt);
        }
    }
}