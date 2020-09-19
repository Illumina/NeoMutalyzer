﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using NeoMutalyzerShared;

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

        public Dictionary<string, Transcript> GetIdToTranscript()
        {
            var idToTranscript = new Dictionary<string, Transcript>();
            
            while (true)
            {
                Transcript transcript = ParseTranscript();
                if (transcript == null) break;

                idToTranscript[transcript.Id] = transcript;
            }

            return idToTranscript;
        }

        private Transcript ParseTranscript()
        {
            string         id  = GetVersion();
            if (id == null) return null;
            
            CodingSequence cds = null;

            FindFeatures();

            while (!ParsingUtilities.FoundOrigin(_readerData.CurrentLine))
            {
                IFeature feature = GetNextFeature();

                var codingSequence = feature as CodingSequence;
                if (codingSequence == null) continue;
                cds = codingSequence;
            }

            string cdnaSequence = ParseSequence();
            return new Transcript(id, cdnaSequence, cds?.Start, cds?.End, cds?.Translation);
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
                GeneFeatureTag           => SkipFeature(),
                JSegmentFeatureTag       => SkipFeature(),
                MatPeptideFeatureTag     => SkipFeature(),
                MiscBindingFeatureTag    => SkipFeature(),
                MiscDifferenceFeatureTag => SkipFeature(),
                MiscFeatureFeatureTag    => SkipFeature(),
                MiscRnaFeatureTag        => SkipFeature(),
                MiscStructureTag         => SkipFeature(),
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
            var cds = new CodingSequence(data.Interval, data.GeneSymbol, data.LocusTag, data.GeneId, data.Regions,
                data.Note, data.CodonStart, data.Product, data.ProteinId, data.Translation + '*');
            // _currentCds = cds;
            return cds;
        }

        public void Dispose() => _readerData.Dispose();
    }
}