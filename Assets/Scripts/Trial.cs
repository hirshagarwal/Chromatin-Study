using System;

namespace Assets.Scripts
{
    public class Trial
    {
        private int subjectID;
        private int row;
        private Formats format;
        private Tasks task;
        private int trialNumber;
        private Boolean training;
        private int data;

        public Trial(String s)
        {
            String[] trialConditions = s.Split(',');
            subjectID = Int32.Parse(trialConditions[0]);
            row = Int32.Parse(trialConditions[1]);
            trialNumber = Int32.Parse(trialConditions[4]);
            data = Int32.Parse(trialConditions[6]);

            if (Formats.HoloLens.ToString() == trialConditions[2])
            {
                format = Formats.HoloLens;
            }
            else if (Formats.Projection.ToString() == trialConditions[2])
            {
                format = Formats.Projection;
            }
            else if (Formats.Heatmap.ToString() == trialConditions[2])
            {
                format = Formats.Heatmap;
            }
            else
            {
                throw new Exception("Unhandled task format: " + trialConditions[2]);
            }

            if (Tasks.PointDistance.ToString() == trialConditions[3])
            {
                task = Tasks.PointDistance;
            }
            else
            {
                throw new Exception("Unhandled task: " + trialConditions[3]);
            }

            if ("T" == trialConditions[5])
            {
                training = true;
            }
            else if ("R" == trialConditions[5])
            {
                training = false;
            }
            else
            {
                throw new Exception("Unhandled training type: " + trialConditions[5]);
            }
        }

        public int Row
        {
            get
            {
                return row;
            }
        }

        public Formats Format
        {
            get
            {
                return format;
            }
        }

        public Tasks Task
        {
            get
            {
                return task;
            }
        }

        public int TrialNumber
        {
            get
            {
                return trialNumber;
            }
        }

        public bool Training
        {
            get
            {
                return training;
            }
        }

        public int Data
        {
            get
            {
                return data;
            }
        }

        public int SubjectID
        {
            get
            {
                return subjectID;
            }

            set
            {
                subjectID = value;
            }
        }
    }
}