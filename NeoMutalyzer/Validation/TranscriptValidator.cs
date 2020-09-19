using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NeoMutalyzer.Annotated;
using NeoMutalyzerShared;

namespace NeoMutalyzer.Validation
{
    public static class TranscriptValidator
    {
        public static void Validate(Position position, Dictionary<string, GenBankTranscript> idToTranscript)
        {
            foreach (Variant variant in position.Variants)
            {
                foreach (Transcript transcript in variant.Transcripts)
                {
                    if (!idToTranscript.TryGetValue(transcript.Id, out GenBankTranscript gbTranscript))
                    {
                        Console.WriteLine($"ERROR: Unable to find the following transcript: {transcript.Id}");
                        Environment.Exit(1);
                    }
                    
                    ValidateTranscript(gbTranscript, transcript, variant.Type);
                }
            }
        }

        private static void ValidateTranscript(GenBankTranscript gbTranscript, Transcript transcript, VariantType variantType)
        {
            ValidateMapping(gbTranscript, transcript.CdnaPos, transcript.CdsPos, transcript.AminoAcidPos,
                transcript.RefAllele,     transcript.RefAminoAcids);

            Interval expectedRightCdsPos = VariantRotator.Right(transcript.CdsPos, transcript.RefAllele,
                transcript.AltAllele, variantType, gbTranscript.CdsSequence).ShiftedPosition;

            ValidateHgvsCoding(gbTranscript, transcript.HgvsCoding, expectedRightCdsPos);
            ValidateHgvsProtein(gbTranscript, transcript.HgvsProtein);
        }

        private static void ValidateHgvsProtein(GenBankTranscript genBankTranscript, string hgvsProtein)
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
            string aa            = genBankTranscript.GetAminoAcids(aaPos);
            
            if (refAminoAcids != aa)
                Console.WriteLine(
                    $"Found mismatch between the ref allele ({refAminoAcids}) and the HGVS p. ref allele ({aa})");
            
            // check the right rotation
            // var result = VariantRotator.Right(aaPos, refAminoAcids, altAminoAcids, variantType, refSequence);
        }

        private static void ValidateHgvsCoding(GenBankTranscript genBankTranscript, string hgvsCoding, Interval expectedRightCdsPos)
        {
            // NM_000314.6:c.956_959delACTT
            var regex = new Regex(@"[^:]+:c.(\d+)_(\d+)del([ACGT]+)", RegexOptions.Compiled);

            Match match = regex.Match(hgvsCoding);
            if (!match.Success) throw new InvalidDataException($"Unable to apply HGVS c. regex to: {hgvsCoding}");

            int    cdsStart  = int.Parse(match.Groups[1].Value);
            int    cdsEnd    = int.Parse(match.Groups[2].Value);
            string refAllele = match.Groups[3].Value;

            var    cdsPos = new Interval(cdsStart, cdsEnd);
            string cds    = genBankTranscript.GetCds(cdsPos);
            
            if (refAllele != cds)
                Console.WriteLine(
                    $"Found mismatch between the ref allele ({refAllele}) and the HGVS c. ref ({cds})");
            
            // check the expected position
            if (!cdsPos.Equals(expectedRightCdsPos))
            {
                Console.WriteLine($"Expected the CDS position to be ({expectedRightCdsPos.Start}, {expectedRightCdsPos.End}), but found it here: ({cdsStart}, {cdsEnd})");
            }
        }

        private static void ValidateMapping(GenBankTranscript genBankTranscript, in Interval cdnaPos, in Interval cdsPos,
            in Interval aminoAcidPos, string refAllele, string refAminoAcids)
        {
            string cdna = genBankTranscript.GetCdna(cdnaPos);
            string cds  = genBankTranscript.GetCds(cdsPos);
            string aa   = genBankTranscript.GetAminoAcids(aminoAcidPos);

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