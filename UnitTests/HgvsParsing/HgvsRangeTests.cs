using NeoMutalyzer.HgvsParsing;
using Xunit;

namespace UnitTests.HgvsParsing
{
    public sealed class HgvsRangeTests
    {
        [Theory]
        [InlineData("123_456", 123, 456)]
        [InlineData("789",     789, 789)]
        public void Parse(string range, int expectedStart, int expectedEnd)
        {
            CodingInterval observedPos = HgvsRange.Parse(range);
            Assert.Equal(expectedStart, observedPos.Start.Position);
            Assert.Equal(expectedEnd,   observedPos.End.Position);
        }

        [Theory]
        [InlineData("21+33",  21,  33)]
        [InlineData("-21+33", -21, 33)]
        [InlineData("21-33",  21,  -33)]
        [InlineData("-21-33", -21, -33)]
        public void ParseIntronOffset(string range, int expectedPosition, int expectedOffset)
        {
            PositionOffset observed = HgvsRange.ParseIntronOffset(range);
            Assert.Equal(expectedPosition, observed.Position);
            Assert.Equal(expectedOffset,   observed.Offset);
        }
    }
}