using System.Collections.Generic;
using System.IO;
using NeoMutalyzerShared;

namespace ReferenceSequence
{
    public static class ReferenceNameReader
    {
        public static readonly Dictionary<string, Chromosome> RefNameToChromosome  = new Dictionary<string, Chromosome>();
        public static readonly Dictionary<ushort, Chromosome> RefIndexToChromosome = new Dictionary<ushort, Chromosome>();

        public static void Load(string filePath)
        {
            using var reader = FileUtilities.ExtendedBinaryReader(filePath);

            CheckHeaderVersion(reader);

            int numRefSeqs = GetNumRefSeqs(reader);
            AddChromosomes(reader, numRefSeqs);
        }

        private static void CheckHeaderVersion(ExtendedBinaryReader reader)
        {
            string headerTag     = reader.ReadString();
            int    headerVersion = reader.ReadInt32();

            if (headerTag     != ReferenceSequenceCommon.HeaderTag ||
                headerVersion != ReferenceSequenceCommon.HeaderVersion)
            {
                throw new InvalidDataException(
                    $"The header identifiers do not match the expected values: Obs: {headerTag} {headerVersion} vs Exp: {ReferenceSequenceCommon.HeaderTag} {ReferenceSequenceCommon.HeaderVersion}");
            }
        }

        private static int GetNumRefSeqs(ExtendedBinaryReader reader)
        {
            // skip genome assembly and patch level
            reader.ReadByte();
            reader.ReadByte();

            return reader.ReadOptInt32();
        }

        private static void AddChromosomes(ExtendedBinaryReader reader, int numRefSeqs)
        {
            for (var i = 0; i < numRefSeqs; i++)
            {
                Chromosome chromosome = Chromosome.Read(reader);
                AddReferenceName(chromosome);
            }
        }

        private static void AddReferenceName(Chromosome chromosome)
        {
            if (!string.IsNullOrEmpty(chromosome.UcscName))         RefNameToChromosome[chromosome.UcscName]         = chromosome;
            if (!string.IsNullOrEmpty(chromosome.EnsemblName))      RefNameToChromosome[chromosome.EnsemblName]      = chromosome;
            if (!string.IsNullOrEmpty(chromosome.RefSeqAccession))  RefNameToChromosome[chromosome.RefSeqAccession]  = chromosome;
            if (!string.IsNullOrEmpty(chromosome.GenBankAccession)) RefNameToChromosome[chromosome.GenBankAccession] = chromosome;
            RefIndexToChromosome[chromosome.Index] = chromosome;
        }
    }
}