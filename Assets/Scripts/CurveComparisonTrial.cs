using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal class CurveComparisonTrial : IGenericTrial
    {
        private string referenceChromosome;
        private string blueChromosome;
        private string redChromosome;
        private Boolean redIsCloser;

        public CurveComparisonTrial(string referenceChromosome, string blueChromosome, string redChromosome, bool redIsCloser)
        {
            this.referenceChromosome = referenceChromosome;
            this.blueChromosome = blueChromosome;
            this.redChromosome = redChromosome;
            this.redIsCloser = redIsCloser;
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
