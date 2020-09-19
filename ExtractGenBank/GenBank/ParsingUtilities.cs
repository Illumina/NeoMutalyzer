using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NeoMutalyzerShared;

namespace ExtractGenBank.GenBank
{
    internal static class ParsingUtilities
    {
        private const char   Quote               = '\"';
        private const int    FeatureColumnLength = 21;
        private const string OriginTag           = "ORIGIN";
        private const string Join                = "join";
        private const string Order               = "order";
        private const string DoubleSlash         = "//";

        internal static DateTime GetDate(string s) => DateTime.ParseExact(s, "MMM-yyyy", CultureInfo.InvariantCulture);

        internal static (string Key, string Value) GetKeyValuePair(ReaderData readerData)
        {
            string info     = GetInfo(readerData.CurrentLine).TrimStart('/');
            int    equalPos = info.IndexOf('=');
            
            if (equalPos == -1) return (info, null);

            string key   = info.Substring(0, equalPos);
            string value = info.Substring(equalPos + 1);

            bool insertSpaces = key != GenBankFeatureParser.TranslationKey;

            if (value.StartsWith(Quote))
            {
                return value.EndsWith(Quote)
                    ? (key, value.TrimStart(Quote).TrimEnd(Quote))
                    : (key, GetMultiLineString(readerData, value, insertSpaces));
            }

            return (key, value);
        }

        private static  string GetInfo(string line)     => line.Substring(FeatureColumnLength);
        internal static bool   FoundOrigin(string line) => line.StartsWith(OriginTag);
        internal static bool   FoundEnd(string line)    => line.StartsWith(DoubleSlash);

        private static string GetMultiLineString(ReaderData readerData, string value, bool insertSpaces)
        {
            var sb = new StringBuilder();
            sb.Append(value.TrimStart(Quote));
            if (insertSpaces) sb.Append(' ');

            while (true)
            {
                readerData.GetNextLine();
                if (readerData.CurrentLine == null) break;

                string info     = GetInfo(readerData.CurrentLine);
                bool   hasQuote = info.EndsWith(Quote);
                sb.Append(info.TrimEnd(Quote));
                if (hasQuote) break;
                if (insertSpaces) sb.Append(' ');
            }

            return sb.ToString();
        }

        internal static string GetValueFromColon(string colonString, string expectedKey)
        {
            string[] cols = colonString.Split(':');
            if (cols.Length != 2)
                throw new InvalidDataException($"Expected two columns in the key-value pair: {colonString}");

            string key = cols[0];
            if (key != expectedKey) throw new InvalidDataException($"Expected the key to be {expectedKey}: {key}");
            return cols[1];
        }

        internal static bool HasLabel(string line)
        {
            string label = GetLabel(line);
            return !string.IsNullOrEmpty(label);
        }

        internal static string GetLabel(string line) => line.Substring(0, FeatureColumnLength).Trim();

        internal static Interval[] GetRegions(string line)
        {
            string info = GetInfo(line);
            if (!info.StartsWith(Join) && !info.StartsWith(Order)) return new[] {GetInterval(line)};

            int parenthesisIndex = info.IndexOf('(');

            string[] intervals    = info.Substring(parenthesisIndex + 1).TrimEnd(')').Split(',');
            var      intervalList = new List<Interval>(intervals.Length);
            intervalList.AddRange(intervals.Select(ConvertRangeToInterval));
            return intervalList.ToArray();
        }

        private static Interval GetInterval(string line) => ConvertRangeToInterval(GetInfo(line));

        private static Interval ConvertRangeToInterval(string s)
        {
            string[] coordinates = s.Replace("<", "").Replace(">", "").Split("..");
            int      start;

            if (coordinates.Length == 1)
            {
                start = int.Parse(coordinates[0]);
                return new Interval(start, start);
            }

            if (coordinates.Length != 2)
                throw new InvalidDataException($"Expected two coordinates in the interval: {s}");

            start = int.Parse(coordinates[0]);
            int end = int.Parse(coordinates[1]);
            return new Interval(start, end);
        }
    }
}