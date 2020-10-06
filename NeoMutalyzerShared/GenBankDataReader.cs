using System.Collections.Generic;
using System.IO;

namespace NeoMutalyzerShared
{
    public static class GenBankDataReader
    {
        public static Dictionary<string, GenBankTranscript> Load(string filePath)
        {
            var idToTranscript = new Dictionary<string, GenBankTranscript>();

            using StreamReader reader = FileUtilities.GzipReader(filePath);

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                string[] cols = line.Split('\t');

                string id           = cols[0];
                string geneSymbol   = cols[1];
                string cdnaSequence = cols[2];
                string cdsSequence  = cols[3];
                string aaSequence   = cols[4];
                string cdsStart     = cols[5];
                string cdsEnd       = cols[6];

                if (string.IsNullOrEmpty(cdnaSequence)) cdnaSequence = null;
                if (string.IsNullOrEmpty(cdsSequence)) cdsSequence   = null;
                if (string.IsNullOrEmpty(aaSequence)) aaSequence     = null;

                Interval codingRegion = null;
                if (!string.IsNullOrEmpty(cdsStart))
                    codingRegion = new Interval(int.Parse(cdsStart), int.Parse(cdsEnd));

                idToTranscript[id] =
                    new GenBankTranscript(id, geneSymbol, cdnaSequence, cdsSequence, aaSequence, codingRegion);
            }

            return idToTranscript;
        }
    }
}