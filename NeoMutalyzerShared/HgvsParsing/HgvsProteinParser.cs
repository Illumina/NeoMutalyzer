using System;
using System.IO;
using System.Text.RegularExpressions;
using NeoMutalyzerShared.Validation;

namespace NeoMutalyzerShared.HgvsParsing
{
    public static class HgvsProteinParser
    {
        private const string prefix       = @"^[^:]+:p\.\(";
        private const string suffix       = @"\)$";
        private const string aaAbbrev     = @"([a-zA-Z_\?]+)?";
        private const string position     = @"(\d+)";
        private const string remaining    = @"(.*?)";
        private const string silentPrefix = @"\(p\.\(";
        private const string silentSuffix = @"\)\)$";

        // NP_001005484.1:p.(Leu160SerfsTer7)
        private static readonly Regex _frameshiftRegex =
            new Regex($"{prefix}{aaAbbrev}{position}{aaAbbrev}fs.*?{suffix}", RegexOptions.Compiled);
        
        // NP_000305.3:p.(Thr319Ter)
        private static readonly Regex _substitutionRegex =
            new Regex($"{prefix}{aaAbbrev}{position}{aaAbbrev}{suffix}", RegexOptions.Compiled);

        // NP_938073.1:p.(Pro29_Pro35del)
        private static readonly Regex _deletionRegex =
            new Regex($"{prefix}{aaAbbrev}{position}(?:_{aaAbbrev}{position})?del{suffix}", RegexOptions.Compiled);

        // NP_001153656.1:p.(Glu210_Leu211delinsAspIle)
        private static readonly Regex _delInsRegex =
            new Regex($"{prefix}{aaAbbrev}{position}(?:_{aaAbbrev}{position})?delins{remaining}{suffix}", RegexOptions.Compiled);

        // NP_001278295.1:p.(Thr371_Ala372insAspMet)
        private static readonly Regex _insertionRegex =
            new Regex($"{prefix}{aaAbbrev}{position}_{aaAbbrev}{position}ins{remaining}{suffix}", RegexOptions.Compiled);

        // NP_542172.2:p.(Ter330TrpextTer73)
        private static readonly Regex _extensionRegex =
            new Regex($"{prefix}{aaAbbrev}{position}{aaAbbrev}extTer{remaining}{suffix}", RegexOptions.Compiled);

        // NP_112199.2:p.(Ala6_Ala8dup)
        private static readonly Regex _duplicationRegex =
            new Regex($"{prefix}{aaAbbrev}{position}(?:_{aaAbbrev}{position})?dup{suffix}", RegexOptions.Compiled);

        // NP_001284534.1:p.(Met1_?4)
        private static readonly Regex _unknownRegex =
            new Regex($"{prefix}{aaAbbrev}{position}_{aaAbbrev}{position}{suffix}", RegexOptions.Compiled);

        // NM_001005484.1:c.180A>G(p.(Ser60=))
        // NM_032129.2:c.1575_1576delCCinsAT(p.(AlaLeu525=))
        private static readonly Regex _silentRegex =
            new Regex($"{silentPrefix}{aaAbbrev}{position}={silentSuffix}", RegexOptions.Compiled);

        public static ProteinInterval Parse(string hgvsProtein)
        {
            if (hgvsProtein == null) return null;

            bool hasDel = hgvsProtein.Contains("del");
            bool hasIns = hgvsProtein.Contains("ins");

            if (hgvsProtein.Contains(":c.")) return ParseSilent(hgvsProtein);
            if (hasIns && hasDel) return ParseDelIns(hgvsProtein);
            if (hasIns) return ParseInsertion(hgvsProtein);
            if (hasDel) return ParseDeletion(hgvsProtein);
            if (hgvsProtein.Contains("fs")) return ParseFrameshift(hgvsProtein);
            if (hgvsProtein.Contains("extTer")) return ParseExtension(hgvsProtein);
            if (hgvsProtein.Contains("dup")) return ParseDuplication(hgvsProtein);
            if (hgvsProtein.EndsWith(":p.?") || hgvsProtein.EndsWith(":p.0?")) return null;
            if (IsUnknown(hgvsProtein)) return ParseUnknown(hgvsProtein); // TODO: this should be deprecated
            return ParseSubstitution(hgvsProtein);
        }

        // TODO: this should be deprecated
        private static bool IsUnknown(string hgvsProtein)
        {
            int pDotIndex = hgvsProtein.IndexOf(":p.", StringComparison.Ordinal);
            if (pDotIndex == -1) throw new InvalidDataException($"Could not find p. string in {hgvsProtein}");

            string pDotString = hgvsProtein.Substring(pDotIndex + 1);
            return pDotString.Contains("_");
        }

        private static ProteinInterval ParseInsertion(string hgvsProtein)
        {
            Match match = _insertionRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // single position if insertion starts with Ter, two positions otherwise
            string          altAllele = AminoAcids.GetIupacCode(match.Groups[5].Value);
            ProteinPosition start     = Convert(match.Groups[2].Value, match.Groups[1].Value);
            ProteinPosition end       = Convert(match.Groups[4].Value, match.Groups[3].Value);

            return new ProteinInterval(start, end, altAllele);
        }

        private static ProteinInterval ParseDuplication(string hgvsProtein)
        {
            Match match = _duplicationRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // single or double position, first and last AA shown
            bool            hasEnd = match.Groups[4].Success;
            ProteinPosition start  = Convert(match.Groups[2].Value, match.Groups[1].Value);
            ProteinPosition end    = hasEnd ? Convert(match.Groups[4].Value, match.Groups[3].Value) : null;

            // unknown alt AA since this can be a large range (only first and last ref AA is used in HGVS)
            return new ProteinInterval(start, end, ProteinInterval.EmptyAA);
        }

        private static ProteinInterval ParseExtension(string hgvsProtein)
        {
            Match match = _extensionRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // always single position ending in extTer
            string          altAllele = AminoAcids.GetIupacCode(match.Groups[3].Value);
            ProteinPosition start     = Convert(match.Groups[2].Value, match.Groups[1].Value);
            return new ProteinInterval(start, null, altAllele);
        }

        private static ProteinInterval ParseDeletion(string hgvsProtein)
        {
            Match match = _deletionRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // single or double position. first and last AA shown
            bool            hasEnd = match.Groups[4].Success;
            ProteinPosition start  = Convert(match.Groups[2].Value, match.Groups[1].Value);
            ProteinPosition end    = hasEnd ? Convert(match.Groups[4].Value, match.Groups[3].Value) : null;

            return new ProteinInterval(start, end, null);
        }

        private static ProteinInterval ParseDelIns(string hgvsProtein)
        {
            Match match = _delInsRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // has first and last ref AA, all alt AA
            bool   hasEnd    = match.Groups[4].Success;
            string altAllele = AminoAcids.GetIupacCode(match.Groups[5].Value);

            ProteinPosition start = Convert(match.Groups[2].Value, match.Groups[1].Value);
            ProteinPosition end   = hasEnd ? Convert(match.Groups[4].Value, match.Groups[3].Value) : null;

            return new ProteinInterval(start, end, altAllele);
        }

        private static ProteinInterval ParseFrameshift(string hgvsProtein)
        {
            Match match = _frameshiftRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // always single position
            // ends with fsTer or Ter if alt AA starts with Ter
            string          altAllele = AminoAcids.GetIupacCode(match.Groups[3].Value);
            ProteinPosition start     = Convert(match.Groups[2].Value, match.Groups[1].Value);
            return new ProteinInterval(start, null, altAllele);
        }

        private static ProteinInterval ParseSubstitution(string hgvsProtein)
        {
            Match match = _substitutionRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // always single position. If start lost, alt AA will be ?
            string          altAllele = AminoAcids.GetIupacCode(match.Groups[3].Value);
            ProteinPosition start     = Convert(match.Groups[2].Value, match.Groups[1].Value);
            return new ProteinInterval(start, null, altAllele);
        }

        private static ProteinInterval ParseSilent(string hgvsProtein)
        {
            Match match = _silentRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // always single position. Ter if stop retained, ref AA otherwise
            ProteinPosition start = Convert(match.Groups[2].Value, match.Groups[1].Value);
            return new ProteinInterval(start, null, start.RefAA.ToString());
        }
        
        // TODO: this should be deprecated since this is not an actual HGVS entry
        private static ProteinInterval ParseUnknown(string hgvsProtein)
        {
            Match match = _unknownRegex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            // always double position since single position looks like substitution
            string          altAllele = AminoAcids.GetIupacCode(match.Groups[3].Value);
            ProteinPosition start     = Convert(match.Groups[2].Value, match.Groups[1].Value);
            ProteinPosition end       = Convert(match.Groups[4].Value, ProteinInterval.UnknownAA);

            return new ProteinInterval(start, end, altAllele);
        }

        private static ProteinPosition Convert(string posString, string refAllele3)
        {
            if (string.IsNullOrEmpty(refAllele3)) refAllele3 = ProteinInterval.EmptyAA;

            int    pos       = int.Parse(posString);
            string refAllele = AminoAcids.GetIupacCode(refAllele3);
            return new ProteinPosition(pos, refAllele[0]);
        }
    }
}