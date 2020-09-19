using System;
using System.Collections.Generic;
using System.Threading;

namespace GenBankDownloader
{
    public static class GenBankDirectory
    {
        public static void DownloadFiles(IClient client, List<RemoteFile> files) =>
            files.ParallelExecute(client.DownloadFile, Retry, "finished", "download the file");

        private static void Retry(RemoteFile file, Func<RemoteFile, bool> clientFunc,
            CancellationTokenSource tokenSource, string exceptionMessage)
        {
            var       numAttempts = 0;
            const int maxAttempts = 5;

            while (true)
            {
                numAttempts++;

                if (numAttempts == maxAttempts)
                {
                    Console.WriteLine($"  - Unable to {exceptionMessage} for {file.Description} after {maxAttempts} attempts.");
                    tokenSource.Cancel();
                    break;
                }

                bool success = clientFunc(file);
                if (success) break;
            }
        }
    }
}