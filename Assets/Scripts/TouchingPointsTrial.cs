using System;

namespace Assets.Scripts
{
    public class TouchingPointsTrial : IGenericTrial
    {
        private string chrom;
        private int count;
        private Formats studyFormat;

        public TouchingPointsTrial(string chrom, string count, Formats studyFormat)
        {
            this.chrom = chrom;
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