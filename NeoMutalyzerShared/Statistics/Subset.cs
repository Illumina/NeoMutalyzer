using System;
using System.Collections.Generic;

namespace NeoMutalyzerShared.Statistics
{
    public class Subset
    {
        private readonly HashSet<string> all = new HashSet<string>();
        private readonly HashSet<string> bad = new HashSet<string>();

        public void Add(string s, bool isBad)
        {
            HashSet<string> hashSet = isBad ? bad : all;
            hashSet.Add(s);
        }

        public void Display(string description)
        {
            double percentBad = bad.Count / (double) all.Count * 100.0;
            string prefix     = description + ':';
            Console.WriteLine($"{prefix,-20} bad: {bad.Count:N0} / {all.Count:N0} ({percentBad:0.000}%)");
        }
    }
}