using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader
{
    public static class ParallelUtilities
    {
        private const int NumThreads = 5;

        public static void ParallelExecute(this List<RemoteFile> files, Func<RemoteFile, bool> clientFunc,
            Action<RemoteFile, Func<RemoteFile, bool>, CancellationTokenSource, string> httpAction,
            string finishedMessage, string exceptionMessage)
        {
            var tasks     = new Task[files.Count];
            var maxThread = new SemaphoreSlim(NumThreads);

            var               tokenSource       = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            try
            {
                for (var i = 0; i < files.Count; i++)
                {
                    maxThread.Wait(cancellationToken);

                    RemoteFile file = files[i];
                    tasks[i] = Task.Factory
                        .StartNew(() => httpAction(file, clientFunc, tokenSource, exceptionMessage), TaskCreationOptions.LongRunning)
                        .ContinueWith(task => maxThread.Release(), cancellationToken);

                    if (cancellationToken.IsCancellationRequested) break;
                }

                Task.WaitAll(tasks);
                Console.WriteLine($"  - {finishedMessage}.\n");
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException($"Unable to {exceptionMessage}. Please verify network connection.");
            }
        }
    }
}