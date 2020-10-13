using System;
using System.Collections.Generic;
using System.IO;
using Downloader;

namespace GffDownloader
{
    internal static class GffDownloaderMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <input RefSeq ID path> <output directory>");
                Environment.Exit(1);
            }

            string inputRefSeqIdPath = args[0];
            string outputDir         = args[1];

            List<RemoteFile> filesToDownload = GetFilesToDownload(inputRefSeqIdPath, outputDir);
            DownloadGffEntries(filesToDownload);
        }

        private static void DownloadGffEntries(List<RemoteFile> filesToDownload)
        {
            var client = new Client("www.ncbi.nlm.nih.gov");
            OutputDirectory.DownloadFiles(client, filesToDownload);
        }

        private static List<RemoteFile> GetFilesToDownload(string filePath, string outputDir)
        {
            const int     numIdsPerFile = 250;
            Queue<string> refSeqIds     = RefSeqIds.Load(filePath);

            var batchRefSeqIds  = new List<string>(numIdsPerFile);
            var filesToDownload = new List<RemoteFile>();
            var batch           = 1;

            while (refSeqIds.Count > 0)
            {
                batchRefSeqIds.Clear();

                for (var i = 0; i < numIdsPerFile; i++)
                {
                    if (refSeqIds.Count == 0) break;
                    string refSeqId = refSeqIds.Dequeue();
                    batchRefSeqIds.Add(refSeqId);
                }

                string batchIds = string.Join(',', batchRefSeqIds);
                var    fileName = $"batch_{batch:000}.gff3";
                batch++;

                string localPath  = Path.Combine(outputDir, fileName);
                var    remotePath = $"/sviewer/viewer.cgi?db=nuccore&report=gff3&id={batchIds}";

                var file = new RemoteFile(remotePath, localPath, fileName);
                filesToDownload.Add(file);
            }

            return filesToDownload;
        }
    }
}