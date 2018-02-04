using System;

namespace Assets.Scripts
{
    public class AttributeUnderstandingTrial : IGenericTrial
    {
        private string chromosome;
        private string start;
        private string end;
        private string question;
        private Boolean answerIsRed;

        internal AttributeUnderstandingTrial(string chromosome, string start, string end, string question, bool answerIsRed)
        {
            this.chromosome = chromosome;
            this.start = start;
            this.end = end;
            this.question = question;
            this.answerIsRed = answerIsRed;
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