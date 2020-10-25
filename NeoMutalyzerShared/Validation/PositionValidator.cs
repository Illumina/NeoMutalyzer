namespace NeoMutalyzerShared.Validation
{
    public static class PositionValidator
    {
        public static void ValidateCdnaPosition(this ValidationResult result, RefSeq.ITranscript refseqTranscript,
            in Interval cdnaPos, in Interval cdsPos, string refAllele)
        {
            // we need a ref allele to validate the cDNA and CDS positions
            if (string.IsNullOrEmpty(refAllele)) return;

            if (cdnaPos.Start < 1 || cdnaPos.End > refseqTranscript.cdnaSequence.Length)
                result.HasInvalidCdnaPosition = true;
            
            if (cdsPos.Start < 1 || cdsPos.End > refseqTranscript.cdsSequence.Length)
                result.HasInvalidCdsPosition = true;
            
            string cdna = refseqTranscript.GetCdna(cdnaPos.Start, cdnaPos.End);
            string cds  = refseqTranscript.GetCds(cdsPos.Start, cdsPos.End);

            if (refAllele != cdna) result.HasCdnaRefAlleleError = true;
            if (refAllele != cds) result.HasCdsRefAlleleError   = true;
        }

        public static void ValidateAminoAcidPosition(this ValidationResult result, RefSeq.ITranscript refseqTranscript,
            in Interval aminoAcidPos, string refAminoAcids)
        {
            // we need a ref AA to validate the amino acid positions
            if (string.IsNullOrEmpty(refAminoAcids)) return;
            
            if (aminoAcidPos.Start < 1 || aminoAcidPos.End > refseqTranscript.aaSequence.Length)
                result.HasInvalidProteinPosition = true;

            string aa = refseqTranscript.GetAminoAcids(aminoAcidPos.Start, aminoAcidPos.End);
            if (refAminoAcids == aa) return;

            result.HasProteinRefAlleleError = true;
        }
    }
}