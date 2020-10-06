using NeoMutalyzerShared;

namespace ReferenceSequence
{
    public sealed class Chromosome
    {
        public readonly string UcscName;
        public readonly string EnsemblName;
        public readonly ushort Index;

        public Chromosome(string ucscName, string ensemblName, ushort index)
        {
            UcscName    = ucscName;
            EnsemblName = ensemblName;
            Index       = index;
        }
        
        public static Chromosome Read(ExtendedBinaryReader reader)
        {
            string ucscName    = reader.ReadAsciiString();
            string ensemblName = reader.ReadAsciiString();
            
            // skip accessions and length
            reader.ReadAsciiString();
            reader.ReadAsciiString();
            reader.ReadOptInt32();

            ushort refIndex = reader.ReadOptUInt16();

            return new Chromosome(ucscName, ensemblName, refIndex);
        }

        public override string ToString() => UcscName;
    }
}