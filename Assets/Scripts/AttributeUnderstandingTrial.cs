using System;

namespace Assets.Scripts
{
    public class AttributeUnderstandingTrial : IGenericTrial
    {
        private string filenamethreedim;
        private string filenametwodim;
        private string start;
        private string end;
        private string question;
        private Formats studyFormat;
        private Boolean answerIsRed;

        internal AttributeUnderstandingTrial(string chrom_num, string chrom_type, string start, string end, string question, bool answerIsRed, Formats studyFormat)
        {
            filenamethreedim = "chr" + chrom_num + "_" + chrom_type;
            filenametwodim = chrom_num + "_formatted.bed." + chrom_type;
            this.start = start;
            this.end = end;
            this.question = question;
            this.answerIsRed = answerIsRed;
            this.studyFormat = studyFormat;
        }

        public string Start
        {
            get
            {
                return start;
            }
        }

        public string End
        {
            get
            {
                return end;
            }
        }

        public string Question
        {
            get
            {
                return question;
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

        public bool Correct(object subjectAnswer)
        {
            return (Boolean)subjectAnswer == answerIsRed;
        }

        public string ToCSV()
        {
            return filenametwodim + ", " +
                start + ", " +
                end + ", " +
                answerIsRed.ToString();
        }
    }
}