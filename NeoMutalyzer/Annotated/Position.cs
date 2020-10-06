namespace NeoMutalyzer.Annotated
{
    public sealed class Position
    {
        public readonly string    Chromosome;
        public readonly int       Pos;
        public readonly Variant[] Variants;

        public Position(string chromosome, int position, Variant[] variants)
        {
            Chromosome = chromosome;
            Pos        = position;
            Variants   = variants;
        }
    }
}