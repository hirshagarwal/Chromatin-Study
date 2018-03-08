using System;

namespace Assets.Scripts
{
    public class PointDistanceTrial : IGenericTrial
    {
        private string chromosome;
        private string redA;
        private string redB;
        private string blueA;
        private string blueB;
        private Formats studyFormat;
        private Boolean redIsShorter;

        public PointDistanceTrial(string chromosome, string redA, string redB, string blueA, string blueB, bool redIsShorter, Formats studyFormat)
        {
            this.chromosome = chromosome;
            this.redA = redA;
            this.redB = redB;
            this.blueA = blueA;
            this.blueB = blueB;
            this.redIsShorter = redIsShorter;
            this.studyFormat = studyFormat;
        }

        public string Chromosome
        {
            get
            {
                return chromosome;
            }
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

        public Boolean Correct(object subjectChoseRed)
        {
            return (Boolean)subjectChoseRed == redIsShorter;
        }

        public String ToCSV()
        {
            return chromosome + ", " +
                redA + ", " +
                redB + ", " +
                blueA + ", " +
                blueB + ", " +
                redIsShorter.ToString();
        }
    }
}