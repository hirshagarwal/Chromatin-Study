using System;

namespace Assets.Scripts
{
    public class TripleTrial : IGenericTrial
    {
        private string chrom;
        private int count;
        private int skip;
        private Formats studyFormat;

        public TripleTrial(string chrom, string skip, string count, Formats studyFormat)
        {
            this.chrom = chrom;
            this.skip = Int32.Parse(skip);
            this.count = Int32.Parse(count);
            this.studyFormat = studyFormat;
        }

        public Formats StudyFormat { get { return studyFormat; } }

        public string Chrom { get { return chrom; } }

        public int Count { get { return count; } }

        public int Skip
        {
            get
            {
                return skip;
            }
        }

        public bool Correct(object lessThanTwo)
        {
            return (Boolean)lessThanTwo == (count < 2);
        }

        public string ToCSV()
        {
            return chrom + ", " +
                count.ToString();
        }
    }
}