using System.Text;
using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    public static class RegionExtensions
    {
        public static string GetBases(this Interval[] regions, string sequence)
        {
            var sb = new StringBuilder();

            foreach (Interval region in regions)
            {
                sb.Append(sequence.Substring(region.Start - 1, region.End - region.Start + 1));
            }

            return sb.ToString();
        }
    }
}