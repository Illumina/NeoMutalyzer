using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Compression;
using NeoMutalyzer.Annotated;
using NeoMutalyzerShared;
using Newtonsoft.Json.Linq;
using ReferenceSequence;

namespace NeoMutalyzer.NirvanaJson
{
    public class NirvanaJsonParser : IDisposable
    {
        private readonly Dictionary<string, Chromosome> _refNameToChromosome;
        private readonly StreamReader                   _reader;
        
        private const string GeneSection = "],\"genes\":[";
        private const string EndSection  = "]}";
        private const string RefSeq      = "RefSeq";
        
        private readonly Regex _regex = new Regex("[^A-Z]", RegexOptions.Compiled);

        public NirvanaJsonParser(Stream stream, Dictionary<string, Chromosome> refNameToChromosome)
        {
            _refNameToChromosome = refNameToChromosome;
            _reader              = new StreamReader(new BlockGZipStream(stream, CompressionMode.Decompress));

            // skip the header line
            _reader.ReadLine();
        }
        
        public Position GetPosition()
        {
            string line = _reader.ReadLine();
            if (line == null || line == GeneSection || line == EndSection) return null;

            return ParsePosition(line);
        }

        private Position ParsePosition(string line)
        {
            dynamic position   = JObject.Parse(line.TrimEnd(','));

            string chromosome = position.chromosome;
            int    pos        = position.position;
            
            dynamic variants = position.variants;
            
            var annotatedVariants = new List<Variant>();
            
            foreach (dynamic variant in variants)
            {
                Variant annotatedVariant = ParseVariant(variant);
                annotatedVariants.Add(annotatedVariant);
            }
            
            return new Position(chromosome, pos, annotatedVariants.ToArray());
        }

        private Variant ParseVariant(dynamic variant)
        {
            string chr         = variant.chromosome;
            int    begin       = variant.begin;
            int    end         = variant.end;
            string refAllele   = variant.refAllele;
            string altAllele   = variant.altAllele;
            string vType       = variant.variantType;
            string hgvsGenomic = variant.hgvsg;
            
            Chromosome  chromosome  = ReferenceNameUtilities.GetChromosome(_refNameToChromosome, chr);
            string      vid         = CreateVid(chromosome.EnsemblName, begin, refAllele, altAllele);
            VariantType variantType = GetVariantType(vType);

            dynamic transcripts = variant.transcripts;
            
            var annotatedTranscripts = new List<Transcript>();

            if (transcripts == null)
                return new Variant(vid, chromosome, begin, end, refAllele, altAllele, variantType, hgvsGenomic,
                    annotatedTranscripts.ToArray(), variant.ToString());

            foreach (dynamic transcript in transcripts)
            {
                Transcript annotatedTranscript = ParseTranscript(transcript);
                if (annotatedTranscript != null) annotatedTranscripts.Add(annotatedTranscript);
            }
            
            return new Variant(vid, chromosome, begin, end, refAllele, altAllele, variantType, hgvsGenomic,
                annotatedTranscripts.ToArray(), variant.ToString());
        }

        private static string CreateVid(string chromosomeName, int start, string refAllele, string altAllele)
        {
            refAllele = RemoveHyphen(refAllele);
            altAllele = RemoveHyphen(altAllele);
            
            // add padding bases
            if (string.IsNullOrEmpty(refAllele) || string.IsNullOrEmpty(altAllele))
            {
                start--;
                const string paddingBase = "N";
                refAllele = paddingBase + refAllele;
                altAllele = paddingBase + altAllele;
            }

            return chromosomeName + '-' + start + '-' + refAllele + '-' + altAllele;
        }

        private static VariantType GetVariantType(string variantType)
        {
            return variantType switch
            {
                "deletion"  => VariantType.deletion,
                "indel"     => VariantType.indel,
                "insertion" => VariantType.insertion,
                "MNV"       => VariantType.MNV,
                "SNV"       => VariantType.SNV,
                _           => throw new InvalidDataException($"Found an unknown variant type: {variantType}")
            };
        }

        private Transcript ParseTranscript(dynamic transcript)
        {
            string source     = transcript.source;
            string hgvsCoding = transcript.hgvsc;

            if (source != RefSeq || string.IsNullOrEmpty(hgvsCoding)) return null;

            string id          = transcript.transcript;
            string hgvsProtein = transcript.hgvsp;
            string codons      = transcript.codons;
            string aminoAcids  = transcript.aminoAcids;
            string cdnaRange   = transcript.cdnaPos;
            string cdsRange    = transcript.cdsPos;
            string aaRange     = transcript.proteinPos;

            (string refAllele, string altAllele)         = GetAllelesFromCodons(codons);
            (string refAminoAcids, string altAminoAcids) = GetAllelesFromAminoAcids(aminoAcids);

            Interval cdnaPos      = GetIntervalFromRange(cdnaRange);
            Interval cdsPos       = GetIntervalFromRange(cdsRange);
            Interval aminoAcidPos = GetIntervalFromRange(aaRange);

            return new Transcript(id, refAllele, altAllele, refAminoAcids, altAminoAcids, cdnaPos, cdsPos, aminoAcidPos,
                hgvsCoding, hgvsProtein);
        }

        private static (string RefAminoAcids, string AltAminoAcids) GetAllelesFromAminoAcids(string aminoAcids)
        {
            if (aminoAcids == null) return (null, null);

            // VL/X
            string[] cols          = aminoAcids.Split('/');
            string   refAminoAcids = RemoveHyphen(cols[0]);
            string   altAminoAcids = cols.Length == 1 ? refAminoAcids : RemoveHyphen(cols[1]);

            return (refAminoAcids, altAminoAcids);
        }

        private static string RemoveHyphen(string aminoAcids) => aminoAcids == "-" ? null : aminoAcids;

        private (string RefAllele, string AltAllele) GetAllelesFromCodons(string codons)
        {
            if (codons == null) return (null, null);
            
            // gTACTt/gt
            string[] cols      = codons.Split('/');
            string   refAllele = _regex.Replace(cols[0], "");
            string   altAllele = _regex.Replace(cols[1], "");
            return (refAllele, altAllele);
        }

        private static Interval GetIntervalFromRange(string range)
        {
            if (range == null) return null;
            
            // 2307-2310
            string[] cols = range.Split('-');

            int begin = int.Parse(cols[0]);
            if (cols.Length == 1) return new Interval(begin, begin);

            int end = int.Parse(cols[1]);
            return new Interval(begin, end);
        }

        public void Dispose() => _reader.Dispose();
    }
}