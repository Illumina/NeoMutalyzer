using System;
using System.Collections.Generic;
using NeoMutalyzerShared.Annotated;
using NeoMutalyzerShared.GenBank;
using NeoMutalyzerShared.Statistics;

namespace NeoMutalyzerShared.Validation
{
    public static class TranscriptValidator
    {
        private static readonly AccuracyStatistics Statistics          = new AccuracyStatistics();
        private static readonly AccuracyStatistics CanonicalStatistics = new AccuracyStatistics();

        public static void DisplayStatistics()
        {
            Console.WriteLine();
            Console.WriteLine("All transcripts:");
            Console.WriteLine("================");
            Statistics.Display();
            
            Console.WriteLine();
            Console.WriteLine("Canonical transcripts:");
            Console.WriteLine("======================");
            CanonicalStatistics.Display();
        }

        public static void Validate(Position position, Dictionary<string, GenBankTranscript> idToTranscript,
            Func<string, bool> FilterGene)
        {
            var result = new ValidationResult();
            
            foreach (Variant variant in position.Variants)
            {
                foreach (Transcript transcript in variant.Transcripts)
                {
                    if (transcript.Id.StartsWith("X") || FilterGene(transcript.GeneId)) continue;
                    
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
                        if (transcript.IsCanonical) CanonicalStatistics.Add(variant.VID, transcript.Id, gbTranscript.GeneSymbol, true);
                        result.DumpErrors(variant.VID, transcript.Id, gbTranscript.GeneSymbol, transcript.HgvsCoding, transcript.HgvsProtein,
                            transcript.Json);
                    }

                    Statistics.Add(variant.VID, transcript.Id, gbTranscript.GeneSymbol, false);
                    if (transcript.IsCanonical) CanonicalStatistics.Add(variant.VID, transcript.Id, gbTranscript.GeneSymbol, false);
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
            
            result.ValidateHgvsCoding(gbTranscript, transcript.HgvsCoding, expectedRightCdsPos, variantType, transcript.OverlapsIntronAndExon);
            result.ValidateHgvsProtein(gbTranscript, transcript.HgvsProtein);
        }
    }
}