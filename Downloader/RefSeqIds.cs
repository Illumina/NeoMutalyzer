using System.Collections.Generic;
using System.IO;
using NeoMutalyzerShared;

namespace Downloader
{
    public static class RefSeqIds
    {
        public static Queue<string> Load(string filePath)
        {
            using StreamReader reader = FileUtilities.StreamReader(filePath);

            var refSeqIds = new Queue<string>();

            while (true)
            {
                string refSeqId = reader.ReadLine();
                if (refSeqId == null) break;
                refSeqIds.Enqueue(refSeqId);
            }

            return refSeqIds;
        }
    }
}