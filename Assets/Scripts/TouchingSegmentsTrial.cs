using System;

namespace Assets.Scripts
{
    public class TouchingSegmentsTrial : IGenericTrial
    {
        private string chrom;
        private int count;
        private float split;
        private Formats studyFormat;
        private bool moreOfRed;

        public TouchingSegmentsTrial(string chrom, string count, string split, Formats studyFormat)
        {
            this.chrom = chrom;
            this.count = Int32.Parse(count);
            this.split = float.Parse(split);
            this.studyFormat = studyFormat;
            moreOfRed = (this.split > 0.5f);
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

        public float Split
        {
            get
            {
                return split;
            }
        }

        public Formats StudyFormat
        {
            get
            {
                return studyFormat;
            }
        }

        public bool Correct(object subjectChoseRed)
        {
            return (Boolean)subjectChoseRed == moreOfRed;
        }

        public string ToCSV()
        {
            return chrom + ", " +
                split.ToString() + ", " +
                moreOfRed.ToString();
        }
    }
}