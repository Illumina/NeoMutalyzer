using System;
using System.Collections.Generic;
using System.Text;

namespace NeoMutalyzer
{
    public static class AminoAcids
    {
        private const string StopCodon = "*";

        private static readonly Dictionary<string, char> _aminoAcidLookupTable;

        // converts single letter amino acid ambiguity codes to three
        // letter abbreviations
        private static readonly Dictionary<char, string> SingleToThreeAminoAcids = new Dictionary<char, string>
        {
            {'A', "Ala"},
            {'B', "Asx"},
            {'C', "Cys"},
            {'D', "Asp"},
            {'E', "Glu"},
            {'F', "Phe"},
            {'G', "Gly"},
            {'H', "His"},
            {'I', "Ile"},
            {'K', "Lys"},
            {'L', "Leu"},
            {'M', "Met"},
            {'N', "Asn"},
            {'P', "Pro"},
            {'Q', "Gln"},
            {'R', "Arg"},
            {'S', "Ser"},
            {'T', "Thr"},
            {'V', "Val"},
            {'W', "Trp"},
            {'Y', "Tyr"},
            {'Z', "Glx"},
            {'X', "Ter"}, // Ter now recommended in HGVS
            {'*', "Ter"},
            {'U', "Sec"},
            {'O', "Pyl"},
            {'J', "Xle"},
            {'?', "_?_"} //deletion at the end of incomplete transcript results in unknown change
        };

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
            {"Ter", '*'}
        };

        static AminoAcids()
        {
            _aminoAcidLookupTable = new Dictionary<string, char>
            {
                // 2nd base: T
                {"TTT", 'F'},
                {"TTC", 'F'},
                {"TTA", 'L'},
                {"TTG", 'L'},
                {"CTT", 'L'},
                {"CTC", 'L'},
                {"CTA", 'L'},
                {"CTG", 'L'},
                {"ATT", 'I'},
                {"ATC", 'I'},
                {"ATA", 'I'},
                {"ATG", 'M'},
                {"GTT", 'V'},
                {"GTC", 'V'},
                {"GTA", 'V'},
                {"GTG", 'V'},

                // 2nd base: C
                {"TCT", 'S'},
                {"TCC", 'S'},
                {"TCA", 'S'},
                {"TCG", 'S'},
                {"CCT", 'P'},
                {"CCC", 'P'},
                {"CCA", 'P'},
                {"CCG", 'P'},
                {"ACT", 'T'},
                {"ACC", 'T'},
                {"ACA", 'T'},
                {"ACG", 'T'},
                {"GCT", 'A'},
                {"GCC", 'A'},
                {"GCA", 'A'},
                {"GCG", 'A'},

                // 2nd base: A
                {"TAT", 'Y'},
                {"TAC", 'Y'},
                {"TAA", '*'},
                {"TAG", '*'},
                {"CAT", 'H'},
                {"CAC", 'H'},
                {"CAA", 'Q'},
                {"CAG", 'Q'},
                {"AAT", 'N'},
                {"AAC", 'N'},
                {"AAA", 'K'},
                {"AAG", 'K'},
                {"GAT", 'D'},
                {"GAC", 'D'},
                {"GAA", 'E'},
                {"GAG", 'E'},

                // 2nd base: G
                {"TGT", 'C'},
                {"TGC", 'C'},
                {"TGA", '*'},
                {"TGG", 'W'},
                {"CGT", 'R'},
                {"CGC", 'R'},
                {"CGA", 'R'},
                {"CGG", 'R'},
                {"AGT", 'S'},
                {"AGC", 'S'},
                {"AGA", 'R'},
                {"AGG", 'R'},
                {"GGT", 'G'},
                {"GGC", 'G'},
                {"GGA", 'G'},
                {"GGG", 'G'}
            };
        }

        private static string AddUnknownAminoAcid(string aminoAcids) =>
            aminoAcids == StopCodon ? aminoAcids : aminoAcids + 'X';

        /// <summary>
        /// converts a DNA triplet to the appropriate amino acid abbreviation
        /// </summary>
        public static string ConvertAminoAcidToAbbreviation(char aminoAcid)
        {
            if (!SingleToThreeAminoAcids.TryGetValue(aminoAcid, out string abbreviation))
            {
                throw new NotSupportedException(
                    $"Unable to convert the following string to an amino acid abbreviation: {aminoAcid}");
            }

            return abbreviation;
        }

        /// <summary>
        /// converts a DNA triplet to the appropriate amino acid abbreviation
        /// The default conversion is human chromosomes. The second parameter also allows the user to specify other codon conversions like mitochondria, etc.
        /// </summary>
        private static char ConvertTripletToAminoAcid(string triplet)
        {
            string upperTriplet = triplet.ToUpper();
            return _aminoAcidLookupTable.TryGetValue(upperTriplet, out char aminoAcid) ? aminoAcid : 'X';
        }


        public static string GetIupacCode(string aminoAcids3)
        {
            if (string.IsNullOrEmpty(aminoAcids3)) return "";

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

        /// <summary>
        /// returns a string of single-letter amino acids translated from a string of bases. 
        /// The bases must already be grouped by triplets (i.e. len must be a multiple of 3)
        /// </summary>
        public static string TranslateBases(string bases, bool forceNonTriplet)
        {
            // sanity check: handle the empty case
            if (bases == null) return null;

            int numAminoAcids = bases.Length / 3;

            // check if we have a non triplet case
            bool nonTriplet = !forceNonTriplet && numAminoAcids * 3 != bases.Length;

            // special case: single amino acid
            string aminoAcidString;
            if (numAminoAcids == 1)
            {
                aminoAcidString =
                    ConvertTripletToAminoAcid(bases.Substring(0, 3))
                        .ToString();
                return nonTriplet ? AddUnknownAminoAcid(aminoAcidString) : aminoAcidString;
            }

            // multiple amino acid case
            var aminoAcids = new char[numAminoAcids];
            for (var i = 0; i < numAminoAcids; i++)
            {
                aminoAcids[i] = ConvertTripletToAminoAcid(bases.Substring(i * 3, 3));
            }

            aminoAcidString = new string(aminoAcids);
            return nonTriplet ? AddUnknownAminoAcid(aminoAcidString) : aminoAcidString;
        }
    }
}