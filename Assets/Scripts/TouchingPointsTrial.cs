using System;

namespace Assets.Scripts
{
    public class TouchingPointsTrial : IGenericTrial
    {
        private string filenamethreedim;
        private string filenametwodim;
        private int skip;
        private int count;
        private Formats studyFormat;
        public int[] startsRed = new int[2];
        public int[] startsBlue = new int[2];
        public string correctPair;

        public TouchingPointsTrial(string chrom_num, string chrom_type, string skip, string count, Formats studyFormat,
            string startsRed1,
            string startsRed2,
            string startsBlue1,
            string startsBlue2,
            string correctPair
)
        {
            filenamethreedim = "chr" + chrom_num + "_" + chrom_type;
            filenametwodim = chrom_num + "_formatted.bed." + chrom_type;
            this.skip = Int32.Parse(skip);
            this.count = Int32.Parse(count);
            this.studyFormat = studyFormat;
            this.startsRed[0] = int.Parse(startsRed1);
            this.startsRed[1] = int.Parse(startsRed2);
            this.startsBlue[0] = int.Parse(startsBlue1);
            this.startsBlue[1] = int.Parse(startsBlue2);
            this.correctPair = correctPair;
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

        public string Filenamethreedim
        {
            get
            {
                return filenamethreedim;
            }
        }

        public string Filenametwodim
        {
            get
            {
                return filenametwodim;
            }
        }

        public bool Correct(object lessThanTen)
        {
            return (int)lessThanTen == count;
        }

        public string ToCSV()
        {
            return filenametwodim + ", " +
                count.ToString();
        }
    }
}