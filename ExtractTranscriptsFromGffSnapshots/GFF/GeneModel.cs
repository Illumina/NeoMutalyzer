using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReferenceSequence;

namespace ExtractTranscriptsFromGffSnapshots.GFF
{
    public sealed class GeneModel : IEquatable<GeneModel>
    {
        public readonly Chromosome Chromosome;
        public readonly string     TranscriptId;
        public readonly List<Exon> Exons = new List<Exon>();

        public GeneModel(Chromosome chromosome, string transcriptId)
        {
            Chromosome   = chromosome;
            TranscriptId = transcriptId;
        }

        public bool Equals(GeneModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Exons.SequenceEqual(other.Exons);
        }

        public override int GetHashCode()
        {
            int h = 31 * Chromosome.Index;
            foreach (Exon transcriptRegion in Exons)
                h = 31 * h + transcriptRegion.GetHashCode();
            return h;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{TranscriptId} on {Chromosome.UcscName}");
            foreach (Exon transcriptRegion in Exons) sb.AppendLine(transcriptRegion.ToString());
            return sb.ToString();
        }
    }
}