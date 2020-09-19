using System;
using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    public class GenBankData
    {
        public Interval   Interval;
        public Interval[] Regions;

        public DateTime CollectionDate;
        public int      CodonStart;

        public string Country;
        public string Function;
        public string GeneId;
        public string GeneSymbol;
        public string Host;
        public string Isolate;
        public string LocusTag;
        public string MoleculeType;
        public string Note;
        public string Organism;
        public string Product;
        public string ProteinId;
        public string TaxonomyId;
        public string Translation;
    }
}