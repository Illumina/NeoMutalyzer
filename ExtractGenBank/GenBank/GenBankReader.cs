using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ExtractGenBank.GenBank
{
    public sealed class GenBankReader : IDisposable
    {
        private readonly ReaderData _readerData;
        private readonly GenBankFeatureParser _parser;

        private const int SequenceTagLength = 10;
        private const int HeaderTagLength   = 12;

        private const string FeaturesTag = "FEATURES";
        private const string VersionTag  = "VERSION";

        private const string CRegionFeatureTag        = "C_region";
        private const string CdsFeatureTag            = "CDS";
        private const string ExonFeatureTag           = "exon";
        private const string GeneFeatureTag           = "gene";
        private const string JSegmentFeatureTag       = "J_segment";
        private const string MatPeptideFeatureTag     = "mat_peptide";
        private const string MiscBindingFeatureTag    = "misc_binding";
        private const string MiscDifferenceFeatureTag = "misc_difference";
        private const string MiscFeatureFeatureTag    = "misc_feature";
        private const string MiscRnaFeatureTag        = "misc_RNA";
        private const string MiscStructureTag         = "misc_structure";
        private const string ModifiedBaseTag          = "modified_base";
        private const string NcRnaFeatureTag          = "ncRNA";
        private const string PolyASiteFeatureTag      = "polyA_site";
        private const string PrecursorRnaFeatureTag   = "precursor_RNA";
        private const string PrimerBindFeatureTag     = "primer_bind";
        private const string PrimTranscriptFeatureTag = "prim_transcript";
        private const string ProProteinFeatureTag     = "proprotein";
        private const string RegulatoryFeatureTag     = "regulatory";
        private const string RepeatRegionFeatureTag   = "repeat_region";
        private const string RrnaFeatureTag           = "rRNA";
        private const string SigPeptideFeatureTag     = "sig_peptide";
        private const string StemLoopFeatureTag       = "stem_loop";
        private const string StsFeatureTag            = "STS";
        private const string SourceFeatureTag         = "source";
        private const string TransitPeptideFeatureTag = "transit_peptide";
        private const string UnsureFeatureTag         = "unsure";
        private const string VariationFeatureTag      = "variation";
        
        public GenBankReader(Stream stream, bool leaveOpen = false)
        {
            var reader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress), Encoding.Default, true,
                1024, leaveOpen);
            _readerData = new ReaderData(reader);
            _parser     = new GenBankFeatureParser(_readerData);
        }

        public List<RefSeq.Transcript> GetTranscripts()
        {
            var transcripts = new List<RefSeq.Transcript>();
            
            while (true)
            {
                RefSeq.Transcript transcript = ParseTranscript();
                if (transcript == null) break;
                
                transcripts.Add(transcript);
            }

            return transcripts;
        }

        private RefSeq.Transcript ParseTranscript()
        {
            string         id  = GetVersion();
            if (id == null) return null;
            
            CodingSequence cds  = null;
            Gene           gene = null;

            FindFeatures();

            while (!ParsingUtilities.FoundOrigin(_readerData.CurrentLine))
            {
                IFeature feature = GetNextFeature();

                switch (feature)
                {
                    case CodingSequence cdsFeature:
                        cds = cdsFeature;
                        break;
                    case Gene geneFeature:
                        gene = geneFeature;
                        break;
                }
            }

            if (gene == null) throw new InvalidDataException($"Unable to find the gene feature for {id}");

            string cdnaSequence = ParseSequence();

            string aaSeqence      = cds?.Translation;
            string proteinId      = cds?.ProteinId;
            byte   startExonPhase = cds == null ? (byte) 0 : (byte) (cds.CodonStart - 1);

            RefSeq.CodingRegion codingRegion = null;
            string              cdsSequence  = null;

            if (cds != null)
            {
                codingRegion = new RefSeq.CodingRegion(-1, -1, cds.Start, cds.End);
                cdsSequence  = cdnaSequence.Substring(cds.Start - 1, cds.End - cds.Start + 1);
            }

            return new RefSeq.Transcript(id, proteinId, gene.GeneSymbol, cdnaSequence, cdsSequence, aaSeqence,
                codingRegion, startExonPhase);
        }

        private void FindFeatures()
        {
            while (true)
            {
                _readerData.GetNextLine();
                if (_readerData.CurrentLine == null) return;
                if (_readerData.CurrentLine.StartsWith(FeaturesTag)) break;
            }

            _readerData.GetNextLine();
        }

        private string GetVersion()
        {
            while (true)
            {
                _readerData.GetNextLine();
                if (_readerData.CurrentLine == null) return null;
                if (_readerData.CurrentLine.StartsWith(VersionTag)) break;
            }

            return _readerData.CurrentLine.Substring(HeaderTagLength);
        }

        private string ParseSequence()
        {
            var sb = new StringBuilder();
            
            while (true)
            {
                _readerData.GetNextLine();
                if (_readerData.CurrentLine == null || ParsingUtilities.FoundEnd(_readerData.CurrentLine)) break;

                string line = _readerData.CurrentLine.Substring(SequenceTagLength).Replace(" ", "").ToUpper();
                sb.Append(line);
            }

            return sb.ToString();
        }

        private IFeature GetNextFeature()
        {
            string label = ParsingUtilities.GetLabel(_readerData.CurrentLine);
            if (string.IsNullOrEmpty(label))
                throw new InvalidDataException($"Found an empty state label: {_readerData.CurrentLine}");

            return label switch
            {
                CdsFeatureTag            => GetCds(),
                CRegionFeatureTag        => SkipFeature(),
                ExonFeatureTag           => SkipFeature(),
                GeneFeatureTag           => GetGene(),
                JSegmentFeatureTag       => SkipFeature(),
                MatPeptideFeatureTag     => SkipFeature(),
                MiscBindingFeatureTag    => SkipFeature(),
                MiscDifferenceFeatureTag => SkipFeature(),
                MiscFeatureFeatureTag    => SkipFeature(),
                MiscRnaFeatureTag        => SkipFeature(),
                MiscStructureTag         => SkipFeature(),
                ModifiedBaseTag          => SkipFeature(),
                NcRnaFeatureTag          => SkipFeature(),
                PolyASiteFeatureTag      => SkipFeature(),
                PrecursorRnaFeatureTag   => SkipFeature(),
                PrimerBindFeatureTag     => SkipFeature(),
                PrimTranscriptFeatureTag => SkipFeature(),
                ProProteinFeatureTag     => SkipFeature(),
                RegulatoryFeatureTag     => SkipFeature(),
                RepeatRegionFeatureTag   => SkipFeature(),
                RrnaFeatureTag           => SkipFeature(),
                SigPeptideFeatureTag     => SkipFeature(),
                SourceFeatureTag         => SkipFeature(),
                StemLoopFeatureTag       => SkipFeature(),
                StsFeatureTag            => SkipFeature(),
                TransitPeptideFeatureTag => SkipFeature(),
                UnsureFeatureTag         => SkipFeature(),
                VariationFeatureTag      => SkipFeature(),
                _                        => throw new NotSupportedException($"Found an unsupported state: {label}")
            };
        }

        private IFeature SkipFeature()
        {
            _parser.GetFeature();
            return null;
        }

        private IFeature GetCds()
        {
            GenBankData data = _parser.GetFeature();
            return new CodingSequence(data.Interval, data.GeneSymbol, data.LocusTag, data.GeneId, data.Regions,
                data.Note, data.CodonStart, data.Product, data.ProteinId, data.Translation + '*');
        }

        private IFeature GetGene()
        {
            GenBankData data = _parser.GetFeature();
            return new Gene(data.Interval, data.GeneSymbol);
        }

        public void Dispose() => _readerData.Dispose();
    }
}
