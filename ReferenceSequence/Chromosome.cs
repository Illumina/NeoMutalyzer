using IO;

namespace ReferenceSequence
{
    public sealed class Chromosome
    {
        public readonly string UcscName;
        public readonly string EnsemblName;
        public readonly string RefSeqAccession;
        public readonly string GenBankAccession;
        public readonly ushort Index;

        public Chromosome(string ucscName, string ensemblName, string refSeqAccession, string genBankAccession,
            ushort index)
        {
            UcscName         = ucscName;
            EnsemblName      = ensemblName;
            RefSeqAccession  = refSeqAccession;
            GenBankAccession = genBankAccession;
            Index            = index;
        }

        public static Chromosome Read(ExtendedBinaryReader reader)
        {
            string ucscName         = reader.ReadAsciiString();
            string ensemblName      = reader.ReadAsciiString();
            string refseqAccession  = reader.ReadAsciiString();
            string genBankAccession = reader.ReadAsciiString();

            reader.ReadOptInt32(); // skip length
            ushort refIndex = reader.ReadOptUInt16();

            return new Chromosome(ucscName, ensemblName, refseqAccession, genBankAccession, refIndex);
        }

        public override string ToString() => UcscName;
    }
}