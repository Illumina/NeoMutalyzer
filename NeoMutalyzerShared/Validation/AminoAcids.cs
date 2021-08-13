using System;
using System.Collections.Generic;
using System.Text;

namespace NeoMutalyzerShared.Validation
{
    public static class AminoAcids
    {
        private static readonly Dictionary<string, char> ThreeToSingleAminoAcids = new Dictionary<string, char>
        {
            {"Ala", 'A'},
            {"Cys", 'C'},
            {"Asp", 'D'},
            {"Glu", 'E'},
            {"Phe", 'F'},
            {"Gly", 'G'},
            {"His", 'H'},
            {"Ile", 'I'},
            {"Lys", 'K'},
            {"Leu", 'L'},
            {"Met", 'M'},
            {"Asn", 'N'},
            {"Pro", 'P'},
            {"Gln", 'Q'},
            {"Arg", 'R'},
            {"Ser", 'S'},
            {"Thr", 'T'},
            {"Val", 'V'},
            {"Trp", 'W'},
            {"Tyr", 'Y'},
            {"Glx", 'Z'},
            {"Ter", '*'},
            {"_?_", '?'},
            {"Xaa", 'X'}
        };
        
        public static string GetIupacCode(string aminoAcids3)
        {
            if (string.IsNullOrEmpty(aminoAcids3)) return "";
            if (aminoAcids3.Length == 1) return aminoAcids3;
            
            int                aminoAcidStringLen = aminoAcids3.Length;
            ReadOnlySpan<char> charSpan           = aminoAcids3.AsSpan();

            if (aminoAcidStringLen == 3) return ConvertThreeLetterToIupacCode(ref charSpan).ToString();

            var sb = new StringBuilder();

            while (charSpan.Length >= 3)
            {
                sb.Append(ConvertThreeLetterToIupacCode(ref charSpan));
            }

            return sb.ToString();
        }

        private static char ConvertThreeLetterToIupacCode(ref ReadOnlySpan<char> charSpan)
        {
            var aminoAcid = charSpan.Slice(0, 3).ToString();
            charSpan = charSpan.Slice(3);

            if (!ThreeToSingleAminoAcids.TryGetValue(aminoAcid, out char iupacCode))
            {
                throw new NotSupportedException(
                    $"Unable to convert the following abbreviation to an IUPAC code: {aminoAcid}");
            }

            return iupacCode;
        }
    }
}