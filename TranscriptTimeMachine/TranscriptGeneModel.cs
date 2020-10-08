using System.Collections.Generic;
using System.Linq;
using ReferenceSequence;

namespace TranscriptTimeMachine
{
    public sealed class TranscriptGeneModel
    {
        public readonly string     Id;
        public readonly Chromosome Chromosome;

        public int Start => Exons[0].Start;
        public int End   => Exons[^1].End;

        public List<Exon> Exons = new List<Exon>();

        public TranscriptGeneModel(string id, Chromosome chromosome)
        {
            Id         = id;
            Chromosome = chromosome;
        }

        public string Key => Chromosome.EnsemblName + '|' + Id + '|' + Exons[0].Start;

        public void AddIdsAndSortExons()
        {
            // add exon IDs when sorted by cDNA position
            ushort exonId = 1;

            foreach (Exon exon in Exons.OrderBy(x => x.CdnaStart)) exon.Id = exonId++;

            // sort the exons by genomic position
            Exons = Exons.OrderBy(x => x.Start).ToList();
        }
    }
}