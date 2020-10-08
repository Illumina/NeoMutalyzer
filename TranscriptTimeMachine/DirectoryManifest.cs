using System.Collections.Generic;
using System.IO;
using NeoMutalyzerShared;

namespace TranscriptTimeMachine
{
    public static class DirectoryManifest
    {
        public static List<string> Load(string filePath)
        {
            using StreamReader reader = FileUtilities.StreamReader(filePath);

            var directories = new List<string>();

            while (true)
            {
                string directory = reader.ReadLine();
                if (directory == null) break;
                directories.Add(directory);
            }

            return directories;
        }
    }
}