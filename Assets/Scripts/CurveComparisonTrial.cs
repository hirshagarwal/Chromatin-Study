using System;

namespace Assets.Scripts
{
    public class CurveComparisonTrial : IGenericTrial
    {
        private string referenceChromosome;
        private string blueChromosome;
        private string redChromosome;
        private Formats studyFormat;
        private Boolean redIsCloser;

        public CurveComparisonTrial(string referenceChromosome, string blueChromosome, string redChromosome, bool redIsCloser, Formats studyFormat)
        {
            this.referenceChromosome = referenceChromosome;
            this.blueChromosome = blueChromosome;
            this.redChromosome = redChromosome;
            this.redIsCloser = redIsCloser;
            this.studyFormat = studyFormat;
        }

        public string ReferenceChromosome
        {
            get
            {
                return referenceChromosome;
            }
        }

        public string BlueChromosome
        {
            get
            {
                return blueChromosome;
            }
        }

        public string RedChromosome
        {
            get
            {
                return redChromosome;
            }
        }

        public bool RedIsCloser
        {
            get
            {
                return redIsCloser;
            }
        }

        public Formats StudyFormat
        {
            get
            {
                return studyFormat;
            }
        }

        public Boolean Correct(object subjectChoseRed)
        {
            return (Boolean)subjectChoseRed == redIsCloser;
        }

        public String ToCSV()
        {
            return referenceChromosome.ToString() + ", " +
                redChromosome + ", " +
                blueChromosome + ", " +
                redIsCloser.ToString();
        }
    }
}