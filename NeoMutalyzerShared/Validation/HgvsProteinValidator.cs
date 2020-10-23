﻿using NeoMutalyzerShared.GenBank;
using NeoMutalyzerShared.HgvsParsing;

namespace NeoMutalyzerShared.Validation
{
    public static class HgvsProteinValidator
    {
        public static void ValidateHgvsProtein(this ValidationResult result, IGenBankTranscript genBankTranscript,
            string hgvsProtein)
        {
            if (hgvsProtein == null) return;

            ProteinInterval interval = HgvsProteinParser.Parse(hgvsProtein);
            if (interval == null) return;

            // evaluate each position independently. For the most part, HGVS p. lists the first and last ref AA
            result.ValidateProteinPosition(interval.Start, genBankTranscript);
            if (interval.End != null) result.ValidateProteinPosition(interval.End, genBankTranscript);

            // look for an invalid alt allele
            if (interval.AltAA == ProteinInterval.UnknownAA) result.HasHgvsProteinAltAlleleError = true;
            if (hgvsProtein.Contains("_?_")) result.HasHgvsProteinUnknownError                   = true;
            
            // check the right rotated protein position
            // result.ValidateRightRotation(expectedRightProteinPos, interval.Start.Position, interval.End.Position);
        }

        private static void ValidateProteinPosition(this ValidationResult result, ProteinPosition position,
            IGenBankTranscript genBankTranscript)
        {
            string aa = genBankTranscript.GetAminoAcids(position.Position, position.Position);
            if (string.IsNullOrEmpty(aa)) return;

            if (position.RefAA != aa[0]) result.HasHgvsProteinRefAlleleError  = true;
        }

        // private static void ValidateRightRotation(this ValidationResult result, Interval rightAminoAcidPos,
        //     in int start, in int end)
        // {
        //     if (rightAminoAcidPos == null) return;
        //
        //     // need to make sure this wasn't an HGVS entry that was converted to a substitution
        //     bool diffRightCoords = rightAminoAcidPos.Start != rightAminoAcidPos.End;
        //     bool diffCoords      = start                   != end;
        //     if (diffCoords != diffRightCoords) return;
        //     
        //     
        //
        //     var hgvsInterval = new Interval(start, end);
        //
        //     if (!hgvsInterval.Equals(rightAminoAcidPos)) result.HasHgvsProteinPositionError = true;
        // }
    }
}