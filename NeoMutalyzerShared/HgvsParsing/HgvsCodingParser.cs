using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NeoMutalyzerShared.HgvsParsing
{
    public static class HgvsCodingParser
    {
        private static readonly Regex _duplicationRegex =
            new Regex(@"^[^:]+:(c|n)\.(.*?)dup(.*)$", RegexOptions.Compiled);

        private static readonly Regex _insertionRegex =
            new Regex(@"^[^:]+:(c|n)\.(.*?)ins(.*)$", RegexOptions.Compiled);
        
        private static readonly Regex _inversionRegex =
            new Regex(@"^[^:]+:(c|n)\.(.*?)inv(.*)$", RegexOptions.Compiled);

        private static readonly Regex _substitutionRegex =
            new Regex(@"^[^:]+:(c|n)\.([^ACGT]+?)([ACGT]+)>([ACGT]+)$", RegexOptions.Compiled);

        private static readonly Regex _deletionRegex =
            new Regex(@"^[^:]+:(c|n)\.(.*?)del(.*)$", RegexOptions.Compiled);
        
        private static readonly Regex _delInsRegex =
            new Regex(@"^[^:]+:(c|n)\.(.*?)del(.*?)ins(.*)$", RegexOptions.Compiled);
        
        private static readonly Regex _silentRegex =
            new Regex(@"^[^:]+:(c|n)\.(.*?)=$", RegexOptions.Compiled);

        public static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) Parse(string hgvsCoding)
        {
            bool hasDel = hgvsCoding.Contains("del");
            bool hasIns = hgvsCoding.Contains("ins");

            if (hgvsCoding.Contains(">")) return ParseSubstitution(hgvsCoding);
            if (hasDel && hasIns) return ParseDelIns(hgvsCoding);
            if (hasDel) return ParseDeletion(hgvsCoding);
            if (hasIns) return ParseInsertion(hgvsCoding);
            if (hgvsCoding.Contains("dup")) return ParseDuplication(hgvsCoding);
            if (hgvsCoding.Contains("inv")) return ParseInversion(hgvsCoding);
            if (hgvsCoding.Contains("=")) return ParseSilent(hgvsCoding);
            throw new NotImplementedException($"Unable to parse the HGVS coding string: {hgvsCoding}");
        }

        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseDuplication(
            string hgvsCoding)
        {
            Match match = _duplicationRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // for a duplication we really want the reference position so that it can be verified
            // single or double position. Always has dup
            bool           isCoding  = match.Groups[1].Value == "c";
            CodingInterval cdsPos    = HgvsRange.Parse(match.Groups[2].Value);
            string         refAllele = match.Groups[3].Value;
            string         altAllele = refAllele + refAllele;

            return (cdsPos, refAllele, altAllele, isCoding);
        }

        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseInsertion(string hgvsCoding)
        {
            Match match = _insertionRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // single or double position. Always has ins
            bool   isCoding  = match.Groups[1].Value == "c";
            string range     = match.Groups[2].Value;
            string altAllele = match.Groups[3].Value;

            CodingInterval cdsPos = HgvsRange.Parse(range);
            return (cdsPos, "", altAllele, isCoding);
        }

        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseSubstitution(
            string hgvsCoding)
        {
            Match match = _substitutionRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // always single position and has >
            bool           isCoding  = match.Groups[1].Value == "c";
            PositionOffset cdsStart  = HgvsRange.ParseIntronOffset(match.Groups[2].Value);
            string         refAllele = match.Groups[3].Value;
            string         altAllele = match.Groups[4].Value;

            var cdsPos = new CodingInterval(cdsStart, cdsStart);
            return (cdsPos, refAllele, altAllele, isCoding);
        }
        
        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseSilent(
            string hgvsCoding)
        {
            Match match = _silentRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // always single position and has =
            bool           isCoding  = match.Groups[1].Value == "c";
            PositionOffset cdsStart  = HgvsRange.ParseIntronOffset(match.Groups[2].Value);

            var cdsPos = new CodingInterval(cdsStart, cdsStart);
            return (cdsPos, null, null, isCoding);
        }

        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseDeletion(string hgvsCoding)
        {
            Match match = _deletionRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // single or double position. Always has del
            bool   isCoding  = match.Groups[1].Value == "c";
            string range     = match.Groups[2].Value;
            string refAllele = match.Groups[3].Value;

            CodingInterval cdsPos = HgvsRange.Parse(range);
            return (cdsPos, refAllele, "", isCoding);
        }

        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseInversion(string hgvsCoding)
        {
            Match match = _inversionRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // single or double position. Always has inv
            bool   isCoding  = match.Groups[1].Value == "c";
            string range     = match.Groups[2].Value;
            string refAllele = match.Groups[3].Value;

            CodingInterval cdsPos = HgvsRange.Parse(range);
            return (cdsPos, refAllele, "", isCoding);
        }
        
        private static (CodingInterval CdsPos, string RefAllele, string AltAllele, bool IsCoding) ParseDelIns(string hgvsCoding)
        {
            Match match = _delInsRegex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            // single or double position. should actually be delins?
            bool   isCoding  = match.Groups[1].Value == "c";
            string range     = match.Groups[2].Value;
            string refAllele = match.Groups[3].Value;
            string altAllele = match.Groups[4].Value;

            CodingInterval cdsPos = HgvsRange.Parse(range);
            return (cdsPos, refAllele, altAllele, isCoding);
        }
    }
}