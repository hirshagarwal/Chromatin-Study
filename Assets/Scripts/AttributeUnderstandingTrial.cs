using System;

namespace Assets.Scripts
{
    public class AttributeUnderstandingTrial : IGenericTrial
    {
        private string chromosome;
        private string start;
        private string end;
        private string question;
        private Formats studyFormat;
        private Boolean answerIsRed;

        internal AttributeUnderstandingTrial(string chromosome, string start, string end, string question, bool answerIsRed, Formats studyFormat)
        {
            this.chromosome = chromosome;
            this.start = start;
            this.end = end;
            this.question = question;
            this.answerIsRed = answerIsRed;
            this.studyFormat = studyFormat;
        }

        public string Chromosome
        {
            get
            {
                return chromosome;
            }
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

        public bool Correct(object subjectAnswer)
        {
            return (Boolean)subjectAnswer == answerIsRed;
        }

        public string ToCSV()
        {
            return chromosome + ", " +
                start + ", " +
                end + ", " +
                answerIsRed.ToString();
        }
    }
}