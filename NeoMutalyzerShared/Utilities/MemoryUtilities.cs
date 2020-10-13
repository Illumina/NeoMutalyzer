using System.Diagnostics;

namespace NeoMutalyzerShared.Utilities
{
    public static class MemoryUtilities
    {
        private const long NumBytesInGB = 1_073_741_824;
        private const long NumBytesInMB = 1_048_576;
        private const long NumBytesInKB = 1_024;

        private static readonly Process CurrentProcess = Process.GetCurrentProcess();

        public static string GetPeakMemoryUsage()    => ToHumanReadable(CurrentProcess.PeakWorkingSet64);
        public static string GetCurrentMemoryUsage() => ToHumanReadable(CurrentProcess.WorkingSet64);

        private static string ToHumanReadable(long numBytes)
        {
            if (numBytes > NumBytesInGB)
            {
                double gigaBytes = numBytes / (double) NumBytesInGB;
                return $"{gigaBytes:0.000} GB";
            }

            if (numBytes > NumBytesInMB)
            {
                double megaBytes = numBytes / (double) NumBytesInMB;
                return $"{megaBytes:0.0} MB";
            }

            if (numBytes > NumBytesInKB)
            {
                double kiloBytes = numBytes / (double) NumBytesInKB;
                return $"{kiloBytes:0.0} KB";
            }

            return $"{numBytes} B";
        }
    }
}