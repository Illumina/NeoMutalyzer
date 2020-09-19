using System;
using System.Collections.Generic;
using System.IO;

namespace GenBankDownloader
{
    internal static class GenBankDownloaderMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <input RefSeq ID path> <output GenBank directory>");
                Environment.Exit(1);
            }

            string inputRefSeqIdPath = args[0];
            string outputGenBankDir  = args[1];

            List<RemoteFile> filesToDownload = GetFilesToDownload(inputRefSeqIdPath, outputGenBankDir);
            DownloadGenBankEntries(filesToDownload);
        }

        private static void DownloadGenBankEntries(List<RemoteFile> filesToDownload)
        {
            var client = new Client("eutils.ncbi.nlm.nih.gov");
            GenBankDirectory.DownloadFiles(client, filesToDownload);
        }

        private static List<RemoteFile> GetFilesToDownload(string filePath, string outputDir)
        {
            const int     numIdsPerFile = 250;
            Queue<string> refSeqIds     = GetRefSeqIds(filePath);

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
                var    fileName = $"batch_{batch:000}.gb";
                batch++;

                string localPath  = Path.Combine(outputDir, fileName);
                var    remotePath = $"/entrez/eutils/efetch.fcgi?db=nuccore&id={batchIds}&rettype=gb&retmode=text";

                var file = new RemoteFile(remotePath, localPath, fileName);
                filesToDownload.Add(file);
            }

            return filesToDownload;
        }

        private static Queue<string> GetRefSeqIds(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream);

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