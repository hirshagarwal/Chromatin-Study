using System;

namespace Assets.Scripts
{
    public class SegmentDistanceTrial : IGenericTrial
    {
        private string filenamethreedim;
        private string filenametwodim;
        private string redA;
        private string redB;
        private string blueA;
        private string blueB;
        private Formats studyFormat;
        private Boolean redIsShorter;

        public SegmentDistanceTrial(string chrom_num, string chrom_type, string redA, string redB, string blueA, string blueB, bool redIsShorter, Formats studyFormat)
        {
            filenamethreedim = "chr" + chrom_num + "_" + chrom_type;
            filenametwodim = chrom_num + "_formatted.bed." + chrom_type;
            this.redA = redA;
            this.redB = redB;
            this.blueA = blueA;
            this.blueB = blueB;
            this.redIsShorter = redIsShorter;
            this.studyFormat = studyFormat;
        }

        public string RedA
        {
            get
            {
                return redA;
            }
        }

        public string RedB
        {
            get
            {
                return redB;
            }
        }

        public string BlueA
        {
            get
            {
                return blueA;
            }
        }

        public string BlueB
        {
            get
            {
                return blueB;
            }
        }

        public Formats StudyFormat
        {
            get
            {
                return studyFormat;
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

        public Boolean Correct(object subjectChoseRed)
        {
            return (Boolean)subjectChoseRed == redIsShorter;
        }

        public String ToCSV()
        {
            return filenametwodim + ", " +
                redA + ", " +
                redB + ", " +
                blueA + ", " +
                blueB + ", " +
                redIsShorter.ToString();
        }
    }
}