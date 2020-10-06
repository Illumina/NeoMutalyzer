using System.Text.RegularExpressions;

namespace NeoMutalyzer.HgvsParsing
{
    public static class HgvsRange
    {
        private static readonly Regex intronOffsetRegex = new Regex(@"^(-?\d+)([-|+]\d+)?$", RegexOptions.Compiled);

        public static CodingInterval Parse(string s)
        {
            int            underlinePos = s.IndexOf('_');
            PositionOffset start;

            if (underlinePos == -1)
            {
                start = ParseIntronOffset(s);
                return new CodingInterval(start, start);
            }

            string startString = s.Substring(0, underlinePos);
            string endString   = s.Substring(underlinePos + 1);

            start = ParseIntronOffset(startString);
            PositionOffset end = ParseIntronOffset(endString);

            return new CodingInterval(start, end);
        }

        public static PositionOffset ParseIntronOffset(string s)
        {
            var beyondCodingEnd = false;

            if (s.Contains("*"))
            {
                s               = s.Replace("*", "");
                beyondCodingEnd = true;
            }
            
            Match match = intronOffsetRegex.Match(s);

            string positionString = match.Groups[1].Value;
            string offsetString   = match.Groups[2].Value;

            int position = int.Parse(positionString);
            int offset   = offsetString.Length > 0 ? int.Parse(offsetString) : 0;

            return new PositionOffset(position, offset, beyondCodingEnd);
        }
    }
}