using System;

namespace Assets.Scripts
{
    public class LargerTadTrial : IGenericTrial
    {
        private string chrom;
        private int largerTad;
        private Formats format;

        public LargerTadTrial(string chrom, string largerTad, Formats format)
        {
            this.chrom = chrom;
            this.largerTad = Int32.Parse(largerTad);
            this.format = format;
        }

        public Formats StudyFormat
        {
            get { return format; }
        }

        public string Chrom { get { return chrom; } }

        public bool Correct(object subjectAnswer)
        {
            return (int)subjectAnswer == largerTad;
        }

        public string ToCSV()
        {
            return chrom + ", " +
                largerTad.ToString();
        }
    }
}