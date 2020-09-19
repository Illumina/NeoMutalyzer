using System;
using System.IO;
using System.Text.RegularExpressions;
using NeoMutalyzerShared;

namespace NeoMutalyzer
{
    public static class TranscriptValidator
    {
        public static void Validate(Transcript transcript, AnnotatedTranscript annotatedTranscript)
        {
            ValidateMapping(transcript,           annotatedTranscript.CdnaPos,   annotatedTranscript.CdsPos,
                annotatedTranscript.AminoAcidPos, annotatedTranscript.RefAllele, annotatedTranscript.RefAminoAcids);

            Interval expectedRightCdsPos = VariantRotator.Right(annotatedTranscript.CdsPos,
                annotatedTranscript.RefAllele, annotatedTranscript.AltAllele, annotatedTranscript.Type,
                transcript.CdsSequence).ShiftedPosition;
            
            ValidateHgvsCoding(transcript, annotatedTranscript.HgvsCoding, expectedRightCdsPos);
            ValidateHgvsProtein(transcript, annotatedTranscript.HgvsProtein);
        }

        private static void ValidateHgvsProtein(Transcript transcript, string hgvsProtein)
        {
            // NP_000305.3:p.(Thr319Ter)
            var regex = new Regex(@"[^:]+:p.\((\D+)(\d+)([^\)]+)\)", RegexOptions.Compiled);

            Match match = regex.Match(hgvsProtein);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS p. regex to: {hgvsProtein}");

            string refAminoAcids3 = match.Groups[1].Value;
            int    aaStart        = int.Parse(match.Groups[2].Value);
            string altAminoAcids3 = match.Groups[3].Value;

            string refAminoAcids = AminoAcids.GetIupacCode(refAminoAcids3);
            string altAminoAcids = AminoAcids.GetIupacCode(altAminoAcids3);
            
            var    aaPos         = new Interval(aaStart, aaStart);
            string aa            = transcript.GetAminoAcids(aaPos);
            
            if (refAminoAcids != aa)
                Console.WriteLine(
                    $"Found mismatch between the ref allele ({refAminoAcids}) and the HGVS p. ref allele ({aa})");
            
            // check the right rotation
            // var result = VariantRotator.Right(aaPos, refAminoAcids, altAminoAcids, variantType, refSequence);
        }

        private static void ValidateHgvsCoding(Transcript transcript, string hgvsCoding, Interval expectedRightCdsPos)
        {
            // NM_000314.6:c.956_959delACTT
            var regex = new Regex(@"[^:]+:c.(\d+)_(\d+)del([ACGT]+)", RegexOptions.Compiled);

            Match match = regex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            int    cdsStart  = int.Parse(match.Groups[1].Value);
            int    cdsEnd    = int.Parse(match.Groups[2].Value);
            string refAllele = match.Groups[3].Value;

            var    cdsPos = new Interval(cdsStart, cdsEnd);
            string cds    = transcript.GetCds(cdsPos);
            
            if (refAllele != cds)
                Console.WriteLine(
                    $"Found mismatch between the ref allele ({refAllele}) and the HGVS c. ref ({cds})");
            
            // check the expected position
            if (!cdsPos.Equals(expectedRightCdsPos))
            {
                Console.WriteLine($"Expected the CDS position to be ({expectedRightCdsPos.Start}, {expectedRightCdsPos.End}), but found it here: ({cdsStart}, {cdsEnd})");
            }
        }

        private static void ValidateMapping(Transcript transcript, in Interval cdnaPos, in Interval cdsPos,
            in Interval aminoAcidPos, string refAllele, string refAminoAcids)
        {
            string cdna = transcript.GetCdna(cdnaPos);
            string cds  = transcript.GetCds(cdsPos);
            string aa   = transcript.GetAminoAcids(aminoAcidPos);

            if (refAllele != cdna)
                throw new InvalidDataException(
                    $"Found mismatch between the ref allele ({refAllele}) and the cDNA ref allele ({cdna})");
            
            if (refAllele != cds)
                throw new InvalidDataException(
                    $"Found mismatch between the ref allele ({refAllele}) and the CDS ref allele ({cds})");
            
            if (refAminoAcids != aa)
                throw new InvalidDataException(
                    $"Found mismatch between the ref allele ({refAminoAcids}) and the AA ref allele ({aa})");
            
                        
            Console.WriteLine($"DEBUG: refAllele from cDNA pos: {cdna} - refAllele: {refAllele}");
            Console.WriteLine($"DEBUG: refAllele from CDS pos: {cds} - refAllele: {refAllele}");
            Console.WriteLine($"DEBUG: refAllele from AA pos: {aa} - refAminoAcids: {refAminoAcids}");
        }
    }
}