using System;

namespace NeoMutalyzer.Validation
{
    public sealed class ValidationResult
    {
        public bool HasHgvsCodingRefAlleleError;
        public bool HasHgvsCodingPositionError;
        public bool HasHgvsCodingBeforeCdsAndAfterCds;
        public bool HasHgvsProteinRefAlleleError;
        public bool HasHgvsProteinUnknownError;
        public bool HasHgvsProteinPositionError;
        public bool HasHgvsProteinAltAlleleError;
        public bool HasCdnaRefAlleleError;
        public bool HasCdsRefAlleleError;
        public bool HasInvalidCdnaPosition;
        public bool HasInvalidCdsPosition;
        public bool HasInvalidProteinPosition;
        public bool HasProteinRefAlleleError;

        public bool HasErrors => HasHgvsCodingRefAlleleError       ||
                                 HasHgvsCodingPositionError        ||
                                 HasHgvsCodingBeforeCdsAndAfterCds ||
                                 HasHgvsProteinRefAlleleError      ||
                                 HasHgvsProteinUnknownError        ||
                                 HasHgvsProteinPositionError       ||
                                 HasHgvsProteinAltAlleleError      ||
                                 HasCdnaRefAlleleError             ||
                                 HasCdsRefAlleleError              ||
                                 HasInvalidCdnaPosition            ||
                                 HasInvalidCdsPosition             ||
                                 HasInvalidProteinPosition         ||
                                 HasProteinRefAlleleError; 

        public void Reset()
        {
            HasHgvsCodingRefAlleleError       = false;
            HasHgvsCodingPositionError        = false;
            HasHgvsCodingBeforeCdsAndAfterCds = false;
            HasHgvsProteinRefAlleleError      = false;
            HasHgvsProteinUnknownError        = false;
            HasHgvsProteinPositionError       = false;
            HasHgvsProteinAltAlleleError      = false;
            HasCdnaRefAlleleError             = false;
            HasCdsRefAlleleError              = false;
            HasInvalidCdnaPosition            = false;
            HasInvalidCdsPosition             = false;
            HasInvalidProteinPosition         = false;
            HasProteinRefAlleleError          = false;
        }

        public void DumpErrors(string vid, string transcriptId, string hgvsCoding, string hgvsProtein,
            string transcriptJson)
        {
            Console.Write($"{vid}\t{transcriptId}\t{hgvsCoding}\t{hgvsProtein}\t");

            if (HasHgvsCodingRefAlleleError) Console.Write("HGVS c. RefAllele\t");
            if (HasHgvsCodingPositionError) Console.Write("HGVS c. Position\t");
            if (HasHgvsCodingBeforeCdsAndAfterCds) Console.Write("HGVS c. CDS range\t");
            if (HasHgvsProteinRefAlleleError) Console.Write("HGVS p. RefAllele\t");
            if (HasHgvsProteinUnknownError) Console.Write("HGVS p. Unknown\t");
            if (HasHgvsProteinPositionError) Console.Write("HGVS p. Position\t");
            if (HasHgvsProteinAltAlleleError) Console.Write("HGVS p. AltAllele\t");
            if (HasCdnaRefAlleleError) Console.Write("cDNA RefAllele\t");
            if (HasCdsRefAlleleError) Console.Write("CDS RefAllele\t");
            if (HasInvalidCdnaPosition) Console.Write("Invalid cDNA position\t");
            if (HasInvalidCdsPosition) Console.Write("Invalid CDS position\t");
            if (HasInvalidProteinPosition) Console.Write("Invalid protein position\t");
            if (HasProteinRefAlleleError) Console.Write("Protein RefAllele\t");
            Console.WriteLine(transcriptJson);
        }
    }
}