using System.Collections.Generic;
using System.IO;
using IO;

namespace NeoMutalyzerShared.IO
{
    public static class SimpleParser
    {
        public static HashSet<string> GetHashSet(string filePath)
        {
            using StreamReader reader  = FileUtilities.StreamReader(filePath);
            var                entries = new HashSet<string>();

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                entries.Add(line);
            }

            return entries;
        }
    }
}