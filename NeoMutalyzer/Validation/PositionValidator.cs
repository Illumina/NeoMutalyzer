using NeoMutalyzerShared;

namespace NeoMutalyzer.Validation
{
    public static class PositionValidator
    {
        public static void ValidateCdnaPosition(this ValidationResult result, IGenBankTranscript genBankTranscript,
            in Interval cdnaPos, in Interval cdsPos, string refAllele)
        {
            // we need a ref allele to validate the cDNA and CDS positions
            if (string.IsNullOrEmpty(refAllele)) return;

            if (cdnaPos.Start < 1 || cdnaPos.End > genBankTranscript.CdnaSequence.Length)
                result.HasInvalidCdnaPosition = true;
            
            if (cdsPos.Start < 1 || cdsPos.End > genBankTranscript.CdsSequence.Length)
                result.HasInvalidCdsPosition = true;
            
            string cdna = genBankTranscript.GetCdna(cdnaPos.Start, cdnaPos.End);
            string cds  = genBankTranscript.GetCds(cdsPos.Start, cdsPos.End);

            if (refAllele != cdna) result.HasCdnaRefAlleleError = true;
            if (refAllele != cds) result.HasCdsRefAlleleError   = true;
        }

        public static void ValidateAminoAcidPosition(this ValidationResult result, IGenBankTranscript genBankTranscript,
            in Interval aminoAcidPos, string refAminoAcids)
        {
            // we need a ref AA to validate the amino acid positions
            if (string.IsNullOrEmpty(refAminoAcids)) return;
            
            if (aminoAcidPos.Start < 1 || aminoAcidPos.End > genBankTranscript.AminoAcidSequence.Length)
                result.HasInvalidProteinPosition = true;

            string aa = genBankTranscript.GetAminoAcids(aminoAcidPos.Start, aminoAcidPos.End);
            if (refAminoAcids == aa) return;

            result.HasProteinRefAlleleError = true;
        }
    }
}