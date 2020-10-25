using System;
using System.Collections.Generic;
using NeoMutalyzerShared.Annotated;
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

        public static void Validate(Position position, Dictionary<string, RefSeq.ITranscript> idToTranscript,
            Func<string, bool> FilterGene)
        {
            var result = new ValidationResult();
            
            foreach (Variant variant in position.Variants)
            {
                foreach (Transcript transcript in variant.Transcripts)
                {
                    if (transcript.Id.StartsWith("X") || FilterGene(transcript.GeneId)) continue;
                    
                    if (!idToTranscript.TryGetValue(transcript.Id, out RefSeq.ITranscript refseqTranscript))
                    {
                        Console.WriteLine($"ERROR: Unable to find the following transcript: {transcript.Id}");
                        Environment.Exit(1);
                    }

                    result.Reset();
                    ValidateTranscript(refseqTranscript, transcript, variant.Type, result);
                    
                    if (result.HasErrors)
                    {
                        Statistics.Add(variant.VID, transcript.Id, refseqTranscript.geneSymbol, true);
                        if (transcript.IsCanonical) CanonicalStatistics.Add(variant.VID, transcript.Id, refseqTranscript.geneSymbol, true);
                        result.DumpErrors(variant.VID, transcript.Id, refseqTranscript.geneSymbol, transcript.HgvsCoding, transcript.HgvsProtein,
                            transcript.Json);
                    }

                    Statistics.Add(variant.VID, transcript.Id, refseqTranscript.geneSymbol, false);
                    if (transcript.IsCanonical) CanonicalStatistics.Add(variant.VID, transcript.Id, refseqTranscript.geneSymbol, false);
                }
            }
        }

        private static void ValidateTranscript(RefSeq.ITranscript refseqTranscript, Transcript transcript,
            VariantType variantType, ValidationResult result)
        {
            result.ValidateCdnaPosition(refseqTranscript, transcript.CdnaPos, transcript.CdsPos, transcript.RefAllele);
            result.ValidateAminoAcidPosition(refseqTranscript, transcript.AminoAcidPos, transcript.RefAminoAcids);

            Interval expectedRightCdsPos = VariantRotator.Right(transcript.CdsPos, transcript.RefAllele,
                transcript.AltAllele, variantType, refseqTranscript.cdsSequence).ShiftedPosition;

            bool potentialCdsTruncation = transcript.CdsPos != null && transcript.CdsPos.Start == 1;

            result.ValidateHgvsCoding(refseqTranscript, transcript.HgvsCoding, expectedRightCdsPos, variantType,
                transcript.OverlapsIntronAndExon, transcript.IsSpliceVariant, potentialCdsTruncation);
            
            result.ValidateHgvsProtein(refseqTranscript, transcript.HgvsProtein);
        }
    }
}