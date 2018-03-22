using System;

namespace Assets.Scripts
{
    public class LargerTadTrial : IGenericTrial
    {
        private string filenamethreedim;
        private string filenametwodim;
        private int largerTad;
        private Formats format;

        public LargerTadTrial(string chrom_num, string chrom_type, string largerTad, Formats format)
        {
            filenamethreedim = "chr" + chrom_num + "_" + chrom_type;
            filenametwodim = chrom_num + "_formatted.bed." + chrom_type;
            this.largerTad = Int32.Parse(largerTad);
            this.format = format;
        }

        public Formats StudyFormat
        {
            get { return format; }
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

        public bool Correct(object subjectAnswer)
        {
            return (int)subjectAnswer == largerTad;
        }

        public string ToCSV()
        {
            return filenametwodim + ", " +
                largerTad.ToString();
        }
    }
}