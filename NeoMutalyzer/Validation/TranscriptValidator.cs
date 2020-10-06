using System;
using System.Collections.Generic;
using NeoMutalyzer.Annotated;
using NeoMutalyzer.Statistics;
using NeoMutalyzerShared;

namespace NeoMutalyzer.Validation
{
    public static class TranscriptValidator
    {
        public static readonly AccuracyStatistics Statistics = new AccuracyStatistics();
        
        public static void Validate(Position position, Dictionary<string, GenBankTranscript> idToTranscript)
        {
            var result = new ValidationResult();
            
            foreach (Variant variant in position.Variants)
            {
                foreach (Transcript transcript in variant.Transcripts)
                {
                    if (transcript.Id.StartsWith("X")) continue;
                    
                    if (!idToTranscript.TryGetValue(transcript.Id, out GenBankTranscript gbTranscript))
                    {
                        Console.WriteLine($"ERROR: Unable to find the following transcript: {transcript.Id}");
                        Environment.Exit(1);
                    }

                    result.Reset();
                    ValidateTranscript(gbTranscript, transcript, variant.Type, result);
                    
                    if (result.HasErrors)
                    {
                        Statistics.Add(variant.VID, transcript.Id, gbTranscript.GeneSymbol, true);
                        result.DumpErrors(variant.VID, transcript.Id, transcript.HgvsCoding, transcript.HgvsProtein);
                    }

                    Statistics.Add(variant.VID, transcript.Id, gbTranscript.GeneSymbol, false);
                }
            }
        }

        private static void ValidateTranscript(IGenBankTranscript gbTranscript, Transcript transcript,
            VariantType variantType, ValidationResult result)
        {
            result.ValidateCdnaPosition(gbTranscript, transcript.CdnaPos, transcript.CdsPos, transcript.RefAllele);
            result.ValidateAminoAcidPosition(gbTranscript, transcript.AminoAcidPos, transcript.RefAminoAcids);

            Interval expectedRightCdsPos = VariantRotator.Right(transcript.CdsPos, transcript.RefAllele,
                transcript.AltAllele, variantType, gbTranscript.CdsSequence).ShiftedPosition;
            
            // Interval expectedRightProteinPos = VariantRotator.Right(transcript.AminoAcidPos, transcript.RefAminoAcids,
            //     transcript.AltAminoAcids, variantType, gbTranscript.AminoAcidSequence).ShiftedPosition;
            
            result.ValidateHgvsCoding(gbTranscript, transcript.HgvsCoding, expectedRightCdsPos, variantType);
            result.ValidateHgvsProtein(gbTranscript, transcript.HgvsProtein);
        }
    }
}