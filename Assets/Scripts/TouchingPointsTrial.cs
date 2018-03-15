using System;

namespace Assets.Scripts
{
    public class TouchingPointsTrial : IGenericTrial
    {
        private string chrom;
        private int skip;
        private int count;
        private Formats studyFormat;

        public TouchingPointsTrial(string chrom, string skip, string count, Formats studyFormat)
        {
            this.chrom = chrom;
            this.skip = Int32.Parse(skip);
            this.count = Int32.Parse(count);
            this.studyFormat = studyFormat;
        }

        public string Chrom
        {
            get
            {
                return chrom;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public Formats StudyFormat
        {
            get
            {
                return studyFormat;
            }
        }

        public int Skip
        {
            get
            {
                return skip;
            }
        }

        public bool Correct(object lessThanTen)
        {
            return (Boolean)lessThanTen == (count < 10);
        }

        public string ToCSV()
        {
            return chrom + ", " +
                count.ToString();
        }
    }
}